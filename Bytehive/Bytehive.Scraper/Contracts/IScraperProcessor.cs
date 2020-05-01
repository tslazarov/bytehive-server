using Bytehive.Scraper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperProcessor
    {
        Task<bool> ProcessScrapeRequest();

        Task<bool> ProcessDetails(ScrapeSettings settings);

        Task<Dictionary<string, string>> ProcessPage(string url, List<FieldMapping> fieldMappings);
    }
}
