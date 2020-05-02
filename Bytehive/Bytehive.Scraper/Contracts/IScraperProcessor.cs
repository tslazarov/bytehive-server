﻿using Bytehive.Scraper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperProcessor
    {
        Task<bool> ProcessScrapeRequest();

        Task<List<Dictionary<string, string>>> ProcessDetails(ScrapeSettings settings);

        Task<Dictionary<string, string>> ProcessDetailPage(string url, List<FieldMapping> fieldMappings);
    }
}
