using Bytehive.Scraper.AppConfig;
using Bytehive.Scraper.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bytehive.Scraper
{
    public class ScraperClient : IScraperClient
    {
        private IAppConfiguration appConfiguration;

        public ScraperClient(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, bool useProxy = false)
        {
            var client = useProxy ? new HttpClient(handler: this.GetHandlerProxy(), disposeHandler: true) : new HttpClient();

            return await client.GetAsync(url);
        }

        private HttpClientHandler GetHandlerProxy()
        {
            var proxy = new WebProxy
            {
                Address = new Uri(scrapeProxyEndpoint),
                Credentials = new NetworkCredential(userName: "scraperapi", password: this.appConfiguration.ScraperApiKey)
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
            };

            return httpClientHandler;
        }

        private const string scrapeProxyEndpoint = "http://proxy-server.scraperapi.com:8001";
    }
}
