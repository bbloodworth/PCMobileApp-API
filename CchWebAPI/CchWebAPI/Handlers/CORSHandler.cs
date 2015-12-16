using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace CchWebAPI.Handlers
{
    public class CORSHandler : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        private List<String> AllowedOrigins
        {
            get
            {
                return Properties.Settings.Default.AllowedOrigins.Split('|').ToList<String>();
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isCORSRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            
            if (isCORSRequest)
            {
                if (AllowedOrigins.Contains("*") ||  AllowedOrigins.Contains(request.Headers.GetValues(Origin).First()))
                {
                    // This CORS request is allowed
                }
                else
                {
                    HttpResponseMessage badResp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    TaskCompletionSource<HttpResponseMessage> tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(badResp);
                    return tsc.Task;
                }
                ;
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew<HttpResponseMessage>(() =>
                        {
                            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

                            string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                            if (accessControlRequestMethod != null)
                                response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);

                            string requestedHeaders = string.Join(",", request.Headers.GetValues(AccessControlRequestHeaders));
                            if (!string.IsNullOrEmpty(requestedHeaders))
                                response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);

                            return response;
                        }, cancellationToken);
                }
                else
                {
                    return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(t =>
                        {
                            HttpResponseMessage resp = t.Result;
                            resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                            return resp;
                        });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}