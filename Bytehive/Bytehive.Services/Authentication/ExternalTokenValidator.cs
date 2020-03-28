using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bytehive.Services.AppConfig;
using Bytehive.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Bytehive.Services.Authentication
{
    public sealed class ExternalTokenValidator : IExternalTokenValidator
    {
        private IAppConfiguration appConfiguration;

        public ExternalTokenValidator(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }


        public async Task<bool> ValidateFacebook(string token)
        {
            var response = await HttpClient.GetAsync(string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}", token, appConfiguration.FacebookAppId, appConfiguration.FacebookAppSecret));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ValidateGoogle(string token)
        {
            var response = await HttpClient.GetAsync(string.Format("https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={0}", token));

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        private HttpClient HttpClient
        {
            get
            {
                if(this.httpClient == null)
                {
                    this.httpClient = new HttpClient();
                }

                return this.httpClient;
            }
        }

        private HttpClient httpClient;
    }
}
