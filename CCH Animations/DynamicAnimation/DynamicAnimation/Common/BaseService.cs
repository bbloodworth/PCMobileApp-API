using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DynamicAnimation.Common
{
    public class BaseService
    {
        public static HttpClient GetClient() {

            var client = new HttpClient();
            client.BaseAddress = new Uri("APIBaseAddress".GetConfigurationValue());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", "APIClientKey".GetConfigurationValue());
            client.DefaultRequestHeaders.Add("Keep-Alive", "false");

            return client;

        }
    }
}