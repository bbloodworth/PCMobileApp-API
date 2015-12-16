using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Web.Security;
using System.Security.Principal;
using System.Net;

namespace CchWebAPI.Handlers
{
    public class BasicAuthenticationMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(response);

            string un, pwd;
            if (!request.Headers.Authorization.Parse(out un, out pwd))
                return tcs.Task;
            
            request.UserName(un);
            
            if(!Membership.ValidateUser(un, pwd))
                return tcs.Task;

            MembershipUser mu = Membership.GetUser(un);
            if (null == mu)
                return tcs.Task;

            request.UserID(mu.ProviderUserKey.ToString());

            return base.SendAsync(request, cancellationToken);
        }
    }
}