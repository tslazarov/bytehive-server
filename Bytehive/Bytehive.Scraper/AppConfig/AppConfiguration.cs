using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bytehive.Scraper.AppConfig
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            this.ScraperApiKey = root.GetSection("ScraperSettings").GetSection("ScraperApiKey").Value;
        }

        public string ScraperApiKey { get; set; }
    }
}
