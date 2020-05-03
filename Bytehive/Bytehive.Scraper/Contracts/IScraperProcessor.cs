using Bytehive.Data.Models;
using Bytehive.Scraper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperProcessor
    {
        Task<bool> ProcessScrapeRequest();

        Task<List<Dictionary<string, string>>> ProcessScrapeType(ScrapeType type, ScrapeSettings settings);

        Task<bool> ProcessExportType(ExportType exportType, List<Dictionary<string, string>> results, ScrapeRequest currentRequest);

        Task<List<Dictionary<string, string>>> ProcessListDetails(ScrapeSettings settings);

        Task<List<Dictionary<string, string>>> ProcessLists(ScrapeSettings settings);

        Task<List<Dictionary<string, string>>> ProcessDetails(ScrapeSettings settings);

        Task<Dictionary<string, string>> ProcessDetailPage(string url, List<FieldMapping> fieldMappings);

        Task<List<Dictionary<string, string>>> ProcessListPage(string url, List<FieldMapping> fieldMappings);

        Task<List<string>> ProcessUrl(string url, string fieldMapping);

        Task<bool> ProcessJsonExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest);

        Task<bool> ProcessXmlExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest);

        Task<bool> ProcessCsvExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest);

        Task<bool> ProcessTxtExport(List<Dictionary<string, string>> entries, ScrapeRequest scrapeRequest);
    }
}
