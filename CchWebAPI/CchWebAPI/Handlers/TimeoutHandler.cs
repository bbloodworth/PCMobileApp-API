using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;

namespace CchWebAPI.Handlers
{
    public class TimeoutHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(response);

            //Get an instance of the user sent in
            MembershipUser mu = Membership.GetUser(
                new Guid(request.UserID()));
            
            //cancel the request if the user is invalid or the User ID wasn't sent
            if (null == mu) return tcs.Task;
            //Check if the user is still online
            Boolean isOnline = mu.IsOnline;
#if DEBUG
            isOnline = true; 
#endif
            //Cancel the request if they are not online anymore
            if (!isOnline) return tcs.Task;
            //Update the user's timestamp so that they don't timeout prematurely
            Membership.GetUser(
                new Guid(request.UserID()),
                true);

            return base.SendAsync(request, cancellationToken);
        }
    }
}