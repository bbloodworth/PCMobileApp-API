using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace CchWebAPI.Handlers
{
    using Support;
    using System.Collections.Specialized;
    public class ApiKeyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            TaskCompletionSource<HttpResponseMessage> tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);

            String apiKey = "";
            if (request.RequestUri.Query.Contains("vnd"))
            {
                NameValueCollection nvc = request.RequestUri.ParseQueryString();
                apiKey = nvc.GetValues("vnd").First();
            }
            else
            {
                if (!request.Headers.Contains("ApiKey"))
                    return tsc.Task;

                IEnumerable<String> apiKeys = request.Headers.GetValues("ApiKey");
                if (apiKeys == null)
                    return tsc.Task;

                String[] apiValues = apiKeys.ToArray<String>();
                if (apiValues.Length == 0)
                    return tsc.Task;

                apiKey = apiValues[0];
                if (apiKey == "")
                    return tsc.Task;

            }
            using (GetEncryptionKeyForAPI gekfa = new GetEncryptionKeyForAPI(apiKey))
            {
                if (gekfa.Tables.Count == 0)
                    return tsc.Task;
                if (gekfa.Tables[0].Rows.Count == 0)
                    return tsc.Task;
                if (gekfa.Tables[0].Rows[0][0].ToString() == "")
                    return tsc.Task;

                request.EncryptionKey(gekfa.Tables[0].Rows[0][0].ToString());
                string isP = gekfa.Tables[0].Rows[0][1].ToString();
                request.IsPartner(isP.Equals("True"));
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}