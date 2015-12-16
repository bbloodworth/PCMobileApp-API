using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using DynamicAnimation.Common;

namespace DynamicAnimation.Handlers
{
    public class ProxyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("ApiKey"))
            {
                request.Headers.Add("ApiKey", "AnzovinApiKey".GetConfigurationValue());
            }
            if (request.Headers.Contains("Host"))
            {
                request.Headers.Remove("Host");
            }
            request.Headers.Add("Host", "AnzovinHost".GetConfigurationValue());

            if (!request.RequestUri.Segments.Contains("AnzovinHandshakeId".GetConfigurationValue()))
            {
                string newRequestUri = string.Format("{0}/{1}",
                    request.RequestUri, "AnzovinHandshakeId".GetConfigurationValue());

                request.RequestUri = new Uri(newRequestUri, UriKind.RelativeOrAbsolute);
            }
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var contentString = request.Content.ReadAsStringAsync().Result;
            var contentType = request.Content.Headers.ContentType;
            request.Content = null;
            request.Content = new StringContent(contentString);
            request.Content.Headers.ContentType = contentType;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
