using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bytehive.Storage.AppConfig
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            this.AzureBlobConnectionString = root.GetSection("Storage").GetSection("AzureBlobConnectionString").Value;
        }

        public string AzureBlobConnectionString { get; set; }
    }
}
