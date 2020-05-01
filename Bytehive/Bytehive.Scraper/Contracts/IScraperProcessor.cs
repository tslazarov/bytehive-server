using Bytehive.Scraper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperProcessor
    {
        Task<bool> ProcessDetails(ScrapeSettings settings);

        Task<Dictionary<string, string>> ProcessRequest(string url, List<FieldMapping> fieldMappings);
    }
}
