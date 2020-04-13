using Bytehive.Scraper.AppConfig;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bytehive.Scraper.Contracts
{
    public interface IScraperClient
    {
        Task<HttpResponseMessage> GetAsync(string url, bool useProxy = false);
    }
}
