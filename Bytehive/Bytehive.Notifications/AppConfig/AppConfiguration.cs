using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bytehive.Notifications.AppConfig
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            this.SendGridKey = root.GetSection("NotificationsSettings").GetSection("SendGridKey").Value;
        }

        public string SendGridKey { get; set; }
    }
}
