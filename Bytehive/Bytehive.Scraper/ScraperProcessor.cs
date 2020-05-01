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

namespace Bytehive.Scraper
{
    public class ScraperProcessor : IScraperProcessor
    {
        private IAzureBlobStorageProvider azureBlobStorage;
        private IScraperClient scraperClient;
        private IScraperParser scraperParser;
        private IScrapeRequestsService scrapeRequestsService;

        public ScraperProcessor(IAzureBlobStorageProvider azureBlobStorage,
            IScraperClient scraperClient,
            IScraperParser scraperParser,
            IScrapeRequestsService scrapeRequestsService)
        {
            this.azureBlobStorage = azureBlobStorage;
            this.scraperClient = scraperClient;
            this.scraperParser = scraperParser;
            this.scrapeRequestsService = scrapeRequestsService;
        }

        public async Task<bool> ProcessScrapeRequest()
        {
            var requests = await this.scrapeRequestsService.GetScrapeRequests<ScrapeRequest>();
            var currentRequest = requests.Where(r => r.Status == ScrapeRequestStatus.Pending).OrderBy(r => r.CreationDate).FirstOrDefault();

            if(currentRequest != null)
            {
                //currentRequest.Status = ScrapeRequestStatus.Started;
                //await this.scrapeRequestsService.Update(currentRequest);
            }

            return true;
        }

        public async Task<bool> ProcessDetails(ScrapeSettings settings)
        {
            var taskList = new List<Task<Dictionary<string, string>>>();
            foreach (var detailUrl in settings.DetailUrls)
            {
                var task = ProcessPage(detailUrl, settings.FieldMappings);
                taskList.Add(task);
            }

            var results = await Task.WhenAll(taskList).ConfigureAwait(false);

            foreach (var result in results)
            {
            }

            return true;
        }

        public async Task<Dictionary<string, string>> ProcessPage(string url, List<FieldMapping> fieldMappings)
        {
            Dictionary<string, string> outputObject = new Dictionary<string, string>();

            // TODO: SET TO TRUE AS PROXY
            var response = await this.scraperClient.GetAsync(url, false);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var htmlNode = this.scraperParser.GetNodeFromHtml(content);

                foreach (var fieldMapping in fieldMappings)
                {
                    try
                    {
                        var node = htmlNode.QuerySelector(fieldMapping.FieldMarkup);
                        
                        if(!outputObject.ContainsKey(fieldMapping.FieldName))
                        {
                            outputObject[fieldMapping.FieldName] = HttpUtility.HtmlDecode(node.InnerText.Trim());
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
    }
}
