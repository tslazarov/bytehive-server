using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bytehive.Services.AppConfig
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            this.FacebookAppId = root.GetSection("AuthSettings").GetSection("FacebookAppId").Value;
            this.FacebookAppSecret = root.GetSection("AuthSettings").GetSection("FacebookAppSecret").Value;
            this.GoogleClientId = root.GetSection("AuthSettings").GetSection("GoogleClientId").Value;
        }

        public string FacebookAppId { get; set; }

        public string FacebookAppSecret { get; set; }

        public string GoogleClientId { get; set; }
    }
}
