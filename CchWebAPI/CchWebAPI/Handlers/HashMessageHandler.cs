using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Collections.Specialized;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace CchWebAPI.Handlers
{
    using Support;

    public class HashMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(response);

            String AuthString = "";
            if (request.Headers.Contains("AuthHash"))
            {
                AuthString = request.Headers.GetValues("AuthHash").ToArray()[0].ToString();
            }
            else
            {
                NameValueCollection nvc = request.RequestUri.ParseQueryString();
                if (nvc.AllKeys.Length == 0)
                    return tcs.Task;
                if (!nvc.AllKeys.Contains("z"))
                    return tcs.Task;
                AuthString = nvc.GetValues("z")[0].ToString();
            }

            if (String.IsNullOrWhiteSpace(AuthString))
                return tcs.Task;

            CCHEncrypt c = new CCHEncrypt(
                AuthString,
                request.EncryptionKey(),
                Properties.Settings.Default.SecretKey
            );

            if (!c.Keys.Contains<String>("CCHID"))
                return tcs.Task;
            request.CCHID(Convert.ToInt32(c["CCHID"].ToString()));

            if (!c.Keys.Contains<String>("UserID"))
                return tcs.Task;
            request.UserID(c["UserID"].ToString());

            if (!c.Keys.Contains<String>("EmployerID"))
                return tcs.Task;
            request.EmployerID(Convert.ToInt32(c["EmployerID"].ToString()));

            if (c.Keys.Contains<String>("UserName"))
                request.UserName(c["UserName"].ToString());

            if (c.Keys.Contains<String>("ConnectionString"))
                request.ConnectionString(c["ConnectionString"].ToString());

            return base.SendAsync(request, cancellationToken);
        }
    }
}