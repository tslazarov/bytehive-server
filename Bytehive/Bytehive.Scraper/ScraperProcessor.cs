using Bytehive.Data.Models;
using Bytehive.Scraper.Contracts;
using Bytehive.Scraper.Models;
using Bytehive.Services.Contracts.Services;
using Bytehive.Storage;
using Fizzler.Systems.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Bytehive.Scraper
{
    public class ScraperProcessor : IScraperProcessor
    {
        private IAzureBlobStorageProvider azureBlobStorage;
        private IScraperClient scraperClient;
        private IScraperParser scraperParser;
        private IScraperFileHelper scraperFileHelper;
        private IScrapeRequestsService scrapeRequestsService;

        public ScraperProcessor(IAzureBlobStorageProvider azureBlobStorage,
            IScraperClient scraperClient,
            IScraperParser scraperParser,
            IScraperFileHelper scraperFileHelper,
            IScrapeRequestsService scrapeRequestsService)
        {
            this.azureBlobStorage = azureBlobStorage;
            this.scraperClient = scraperClient;
            this.scraperParser = scraperParser;
            this.scraperFileHelper = scraperFileHelper;
            this.scrapeRequestsService = scrapeRequestsService;
        }

        public async Task<bool> ProcessScrapeRequest()
        {
            var requests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequest>();
            var currentRequest = requests.Where(r => r.Status == ScrapeRequestStatus.Pending).OrderBy(r => r.CreationDate).FirstOrDefault();

            if(currentRequest != null)
            {
                try
                {
                    currentRequest.Status = ScrapeRequestStatus.Started;
                    await this.scrapeRequestsService.Update(currentRequest);

                    var scrapeSettings = JsonConvert.DeserializeObject<ScrapeSettings>(currentRequest.Data);
                    var results = await ProcessScrapeType(currentRequest.ScrapeType, scrapeSettings);
                    var exported = await ProcessExportType(currentRequest.ExportType, results, currentRequest);

                    if(exported)
                    {
                        currentRequest.Status = ScrapeRequestStatus.Completed;
                        currentRequest.FileName = this.GetFileName(currentRequest.Id, currentRequest.ExportType);
                        currentRequest.ExpirationDate = DateTime.UtcNow.AddMonths(2);
                        await this.scrapeRequestsService.Update(currentRequest);
                    }
                }
                catch(Exception ex)
                {
                    // TODO: Log exception
                    currentRequest.Status = ScrapeRequestStatus.Failed;
                    await this.scrapeRequestsService.Update(currentRequest);
                }
            }

            return true;
        }

        public async Task<List<Dictionary<string, string>>> ProcessScrapeType(ScrapeType type, ScrapeSettings settings)
        {
            var results = new List<Dictionary<string, string>>();

            switch (type)
            {
                case ScrapeType.ListDetail:
                    results = await this.ProcessListDetails(settings);
                    break;
                case ScrapeType.List:
                    break;
                case ScrapeType.Detail:
                    results = await this.ProcessDetails(settings);
                    break;
                default:
                    break;
            }

            return results;
        }

        public async Task<bool> ProcessExportType(ExportType exportType, List<Dictionary<string, string>> results, ScrapeRequest currentRequest)
        {
            bool isProcessed = false;

            switch (exportType)
            {
                case ExportType.Json:
                    isProcessed = await this.ProcessJsonExport(results, currentRequest);
                    break;
                case ExportType.Xml:
                    isProcessed = await this.ProcessXmlExport(results, currentRequest);
                    break;
                case ExportType.Csv:
                    isProcessed = await this.ProcessCsvExport(results, currentRequest);
                    break;
                case ExportType.Txt:
                    isProcessed = await this.ProcessTxtExport(results, currentRequest);
                    break;
                default:
                    break;
            }

            return isProcessed;
        }

        public async Task<List<Dictionary<string, string>>> ProcessListDetails(ScrapeSettings settings)
        {
            var listTaskList = new List<Task<List<string>>>();
            var detailsTaskList = new List<Task<Dictionary<string, string>>>();

            if (settings.StartPage != null && settings.EndPage != null)
            {
                for (int i = settings.StartPage.Value; i <= settings.EndPage.Value; i++)
                {
                    string url = Regex.Replace(settings.ListUrl, "{{page}}", i.ToString());
                    var task = ProcessUrl(url, settings.DetailMarkup);
                    listTaskList.Add(task);
                }
            }
            else
            {
                var task = ProcessUrl(settings.ListUrl, settings.DetailMarkup);
                listTaskList.Add(task);
            }

            var detailUrls = await Task.WhenAll(listTaskList).ConfigureAwait(false);

            foreach (var detailUrl in detailUrls.SelectMany(l => l).Distinct().ToList())
            {
                var task = ProcessDetailPage(detailUrl, settings.FieldMappings);
                detailsTaskList.Add(task);
            }

            var results = await Task.WhenAll(detailsTaskList).ConfigureAwait(false);

            return results.ToList();
        }


        public async Task<List<Dictionary<string, string>>> ProcessDetails(ScrapeSettings settings)
        {
            var detailsTaskList = new List<Task<Dictionary<string, string>>>();
            foreach (var detailUrl in settings.DetailUrls)
            {
                var task = ProcessDetailPage(detailUrl, settings.FieldMappings);
                detailsTaskList.Add(task);
            }

            var results = await Task.WhenAll(detailsTaskList).ConfigureAwait(false);

            return results.ToList();
        }

        public async Task<Dictionary<string, string>> ProcessDetailPage(string url, List<FieldMapping> fieldMappings)
        {
            Dictionary<string, string> outputObject = new Dictionary<string, string>();

            // TODO: SET TO TRUE AS PROXY
            var response = await this.scraperClient.GetAsync(url, false);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var html = new HtmlDocument();
                html.LoadHtml(content);

                var htmlNode = html.DocumentNode;

                foreach (var fieldMapping in fieldMappings)
                {
                    try
                    {
                        var node = htmlNode.QuerySelector(fieldMapping.FieldMarkup);
                        
                        if(!outputObject.ContainsKey(fieldMapping.FieldName))
                        {
                            outputObject[fieldMapping.FieldName] = Regex.Replace(Regex.Replace(HttpUtility.HtmlDecode(node.InnerText.Trim()), @"\r\n?|\n", ""), @"\s+", " ");
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: Log errors
                    }
                }
            }

            return outputObject;
        }

        public async Task<List<string>> ProcessUrl(string url, string fieldMapping)
        {
            var host = new Uri(url).Host;

            List<string> detailUrls = new List<string>();
            // TODO: SET TO TRUE AS PROXY
            var response = await this.scraperClient.GetAsync(url, false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var html = new HtmlDocument();
                html.LoadHtml(content);

                var htmlNode = html.DocumentNode;

                var nodes = htmlNode.QuerySelectorAll(fieldMapping);

                foreach (var node in nodes)
                {
                    if (node.Attributes["href"] != null)
                    {
                        if (!node.Attributes["href"].Value.StartsWith("http"))
                        {
                            var baseUri = new Uri(host);
                            var href = node.Attributes["href"].Value;
                            var relativeUrl = href.StartsWith("~/") ? href.Substring(2, href.Length - 2) : href.StartsWith("/") ? href.Substring(1, href.Length - 1) : href;

                            node.Attributes["href"].Value = new Uri(baseUri, relativeUrl).AbsoluteUri;
                        }

                        var detailUrl = node.Attributes["href"].Value;
                        detailUrls.Add(detailUrl);
                    }
                }
            }

            return detailUrls;
        }

        public async Task<bool> ProcessJsonExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest)
        {
            var fileName = string.Format("results-{0}.json", scrapeRequest.Id);

            var json = JsonConvert.SerializeObject(entries, Formatting.Indented);

            var fileStream = this.scraperFileHelper.GenerateStreamFromString(json);
            var blobContent = await this.azureBlobStorage.UploadBlob("scrapefiles", fileName, ".json", fileStream);

            return blobContent != null;
        }

        public async Task<bool> ProcessXmlExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest)
        {
            var fileName = string.Format("results-{0}.xml", scrapeRequest.Id);

            var xml = this.scraperFileHelper.SerializeToXml(entries);

            var fileStream = this.scraperFileHelper.GenerateStreamFromString(xml);
            var blobContent = await this.azureBlobStorage.UploadBlob("scrapefiles", fileName, ".xml", fileStream);

            return blobContent != null;
        }

        public async Task<bool> ProcessCsvExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest)
        {
            var fileName = string.Format("results-{0}.csv", scrapeRequest.Id);

            var txt = this.scraperFileHelper.SerializeToTxt(entries);

            var fileStream = this.scraperFileHelper.GenerateStreamFromString(txt);
            var blobContent = await this.azureBlobStorage.UploadBlob("scrapefiles", fileName, ".csv", fileStream);

            return blobContent != null;
        }

        public async Task<bool> ProcessTxtExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest)
        {
            var fileName = string.Format("results-{0}.txt", scrapeRequest.Id);

            var txt = this.scraperFileHelper.SerializeToTxt(entries);

            var fileStream = this.scraperFileHelper.GenerateStreamFromString(txt);
            var blobContent = await this.azureBlobStorage.UploadBlob("scrapefiles", fileName, ".txt", fileStream);

            return blobContent != null;
        }

        private string GetFileName(Guid id, ExportType exportType)
        {
            string fileName = string.Empty;

            switch (exportType)
            {
                case ExportType.Json:
                    fileName = string.Format("results-{0}.json", id);
                    break;
                case ExportType.Xml:
                    fileName = string.Format("results-{0}.xml", id);
                    break;
                case ExportType.Csv:
                    fileName = string.Format("results-{0}.csv", id);
                    break;
                case ExportType.Txt:
                    fileName = string.Format("results-{0}.txt", id);
                    break;
                default:
                    break;
            }

            return fileName;
        }
    }
}
