using Bytehive.Scraper.Contracts;
using Bytehive.Scraper.Models;
using Bytehive.Storage;
using Fizzler.Systems.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bytehive.Scraper
{
    public class ScraperProcessor : IScraperProcessor
    {
        private IAzureBlobStorageProvider azureBlobStorage;
        private IScraperClient scraperClient;
        private IScraperParser scraperParser;

        public ScraperProcessor(IAzureBlobStorageProvider azureBlobStorage,
            IScraperClient scraperClient,
            IScraperParser scraperParser)
        {
            this.azureBlobStorage = azureBlobStorage;
            this.scraperClient = scraperClient;
            this.scraperParser = scraperParser;
        }

        public async Task<bool> ProcessDetails(ScrapeSettings settings)
        {
            var taskList = new List<Task<Dictionary<string, string>>>();
            foreach (var detailUrl in settings.DetailUrls)
            {
                var task = ProcessRequest(detailUrl, settings.FieldMappings);
                taskList.Add(task);
            }

            var results = await Task.WhenAll(taskList).ConfigureAwait(false);

            foreach (var result in results)
            {
            }

            return true;
        }

        public async Task<Dictionary<string, string>> ProcessRequest(string url, List<FieldMapping> fieldMappings)
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
