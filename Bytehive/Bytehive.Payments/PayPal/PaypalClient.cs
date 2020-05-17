using Bytehive.Payments.AppConfig;
using Bytehive.Payments.Contracts;
using PayPalCheckoutSdk.Core;
using PayPalHttp;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Bytehive.Payments.PayPal
{
    public class PayPalClient : IPayPalClient
    {
        private IAppConfiguration appConfiguration;

        public PayPalClient(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        private PayPalEnvironment GetEnvironment()
        {
            return new SandboxEnvironment(this.appConfiguration.PayPalClientId, this.appConfiguration.PayPalClientSecret);
        }

        public HttpClient Client()
        {
            return new PayPalHttpClient(GetEnvironment());
        }

        public HttpClient Client(string refreshToken)
        {
            return new PayPalHttpClient(GetEnvironment(), refreshToken);
        }

        public string ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}