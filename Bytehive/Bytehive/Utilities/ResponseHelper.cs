using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bytehive.Utilities
{
    public class ResponseHelper
    {

        public static HttpContent CreateJsonResponseMessage(object data)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.Content = new StringContent(JsonConvert.SerializeObject(data));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response.Content;
        }
    }
}
