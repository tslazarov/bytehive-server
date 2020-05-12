using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bytehive.Payment.AppConfig
{
    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            this.PayPalClientId = root.GetSection("PaymentSettings").GetSection("PayPal").GetSection("PayPalClientId").Value;
            this.PayPalClientSecret = root.GetSection("PaymentSettings").GetSection("PayPal").GetSection("PayPalClientSecret").Value;
        }

        public string PayPalClientId { get; set; }

        public string PayPalClientSecret { get; set; }
    }
}
