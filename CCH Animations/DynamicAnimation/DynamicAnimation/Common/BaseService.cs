using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DynamicAnimation.Common
{
    public class BaseService
    {
        private static HttpClient _client;

        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri("APIBaseAddress".GetConfigurationValue());
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _client.DefaultRequestHeaders.Add("ApiKey", "APIClientKey".GetConfigurationValue());
                    _client.DefaultRequestHeaders.Add("Keep-Alive", "false");
                }
                return _client;
            }
        }
    }
}