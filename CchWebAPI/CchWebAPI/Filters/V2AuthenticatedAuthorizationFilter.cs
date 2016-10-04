using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

using Autofac.Integration.WebApi;
using ClearCost.IO.Log;
using ClearCost.Platform;
using CchWebAPI.Extensions;
using CchWebAPI.Support;
using System.Web.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CchWebAPI.Filters {
    public class V2AuthenticatedAuthorizationFilter : IAutofacAuthorizationFilter {

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

        private string FormatHeaders(HttpRequestMessage request)
        {
            var headers = new List<string>();
            request.Headers.ToList().ForEach(h => {
                headers.Add(string.Format("{0}:{1}", h.Key, string.Join("|", h.Value)));
            });

            return string.Join(" | ", headers);
        }

        private string FormatOrigins()
        {
            return string.Join(" | ", AllowedOrigins);
        }

        //TODO: this may need to get more comprehensive in matching requesting Consumer against
        //the domain associated with this ApiKey, but that would require db work.
        public void OnAuthorization(HttpActionContext context) {

            //if (!IsOriginValid(context))
            //    return;

            if (!IsApiKeyValid(context))
                return;

            if (!ProcessAuthHash(context))
                return;

            if (!ExtendSession(context))
                return;

            return;
        }
        public Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken) {
            bool isCORSRequest = actionContext.Request.Headers.Contains(Origin);
            bool isPreflightRequest = actionContext.Request.Method == HttpMethod.Options;

            if (isCORSRequest)
            {
                if (AllowedOrigins.Contains("*") || AllowedOrigins.Contains(actionContext.Request.Headers.GetValues(Origin).First()))
                {
                    // This CORS request is allowed
                    LogUtil.Trace(string.Format("CORS request allowed. Wildcard origins is {0}.  " +
                        "Request origin is {1}", AllowedOrigins.Contains("*"),
                        actionContext.Request.Headers.GetValues(Origin).First()));
                }
                else
                {
                    LogUtil.Trace(string.Format("CORS request blocked.  Allowed Origins are {0}.  Headers were:  {0}.",
                        FormatOrigins(), FormatHeaders(actionContext.Request)));

                    var badResp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    TaskCompletionSource<HttpResponseMessage> tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(badResp);
                    return tsc.Task;
                }

                if (isPreflightRequest)
                {
                    LogUtil.Trace("Is Preflight Request");
                    return Task.Factory.StartNew<HttpResponseMessage>(() => {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add(AccessControlAllowOrigin, actionContext.Request.Headers.GetValues(Origin).First());

                        string accessControlRequestMethod = actionContext.Request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                        if (accessControlRequestMethod != null)
                            response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);

                        string requestedHeaders = string.Join(",", actionContext.Request.Headers.GetValues(AccessControlRequestHeaders));
                        if (!string.IsNullOrEmpty(requestedHeaders))
                            response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);

                        return response;
                    }, cancellationToken);
                }
                else
                {
                    LogUtil.Trace("Is Standard Request");
                    return OnAuthorizationAsync(actionContext, cancellationToken).ContinueWith<HttpResponseMessage>(t => {
                        HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
                        resp.Headers.Add(AccessControlAllowOrigin, actionContext.Request.Headers.GetValues(Origin).First());
                        return resp;
                    });
                }
            }
            else
            {
                return OnAuthorizationAsync(actionContext, cancellationToken);
            }
        }

        private bool IsOriginValid(HttpActionContext context)
        {
            bool isCORSRequest = context.Request.Headers.Contains(Origin);
            bool isPreflightRequest = context.Request.Method == HttpMethod.Options;

            if (isCORSRequest)
            {
                if (AllowedOrigins.Contains("*") || AllowedOrigins.Contains(context.Request.Headers.GetValues(Origin).First()))
                {
                    // This CORS request is allowed
                    LogUtil.Trace(string.Format("CORS request allowed. Wildcard origins is {0}.  " +
                        "Request origin is {1}", AllowedOrigins.Contains("*"),
                        context.Request.Headers.GetValues(Origin).First()));
                }
                else
                {
                    LogUtil.Trace(string.Format("CORS request blocked.  Allowed Origins are {0}.  Headers were:  {0}.",
                        FormatOrigins(), FormatHeaders(context.Request)));

                    return false;
                }

                if (isPreflightRequest)
                {
                    LogUtil.Trace("Is Preflight Request");

                    context.Response = context.Request.CreateResponse(HttpStatusCode.OK);
                    context.Response.Headers.Add(AccessControlAllowOrigin, context.Request.Headers.GetValues(Origin).First());

                    string accessControlRequestMethod = context.Request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                    if (accessControlRequestMethod != null)
                        context.Response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);

                    string requestedHeaders = string.Join(",", context.Request.Headers.GetValues(AccessControlRequestHeaders));
                    if (!string.IsNullOrEmpty(requestedHeaders))
                        context.Response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                }
                else
                {
                    LogUtil.Trace("Is Standard Request");

                    context.Response = context.Request.CreateResponse(HttpStatusCode.OK);
                    context.Response.Headers.Add(AccessControlAllowOrigin, context.Request.Headers.GetValues(Origin).First());
                }

                return true;
            }
            else
            {
                //return OnAuthorizationAsync(actionContext, cancellationToken);
                return true;
            }
        }

        private bool IsApiKeyValid(HttpActionContext context) {

            if (context.Request.Headers == null || context.Request.Headers.Count() < 1) {
                LogUtil.Trace(string.Format("ApiKey missing for {0} from host {1}",
                    context.ActionDescriptor.ActionName,
                    ((ApiController)context.ControllerContext.Controller).Host()));

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");

                return false;
            }

            if (context.Request.Headers.Count(h => h.Key.ToLower().Equals("apikey")) < 1) {
                LogUtil.Trace(string.Format("ApiKey missing for {0} from host {1}",
                    context.ActionDescriptor.ActionName,
                    ((ApiController)context.ControllerContext.Controller).Host()));

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");

                return false;
            }

            var apiKey = context.Request.Headers
                .FirstOrDefault(h => h.Key.ToLower().Equals("apikey")).Value.FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey)) {
                LogUtil.Trace(string.Format("ApiKey blank for {0} from host {1}",
                    context.ActionDescriptor.ActionName,
                    ((ApiController)context.ControllerContext.Controller).Host()));

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");

                return false;
            }

            var apiConsumer = ApiConsumerCache.ApiConsumers.FirstOrDefault(ac => ac.ApiKey.Equals(Guid.Parse(apiKey)) && ac.IsActive);
            if (apiConsumer == null) {
                LogUtil.Trace(string.Format("ApiKey invalid for {0} from host {1}.  Key was {2}",
                    context.ActionDescriptor.ActionName,
                    ((ApiController)context.ControllerContext.Controller).Host(),
                    apiKey));

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Credentials invalid.");

                return false;
            }

            context.Request.EncryptionKey(apiConsumer.Encryptor.ToString());
            context.Request.IsPartner(apiConsumer.IsPartner);

            return true;
        }

        private bool ProcessAuthHash(HttpActionContext context) {
            var request = context.Request;
            var authHash = string.Empty;

            if (request.Headers.Contains("AuthHash")) {
                authHash = request.Headers.GetValues("AuthHash").ToArray()[0].ToString();
            }
            else {
                var nvc = request.RequestUri.ParseQueryString();
                if (nvc.AllKeys.Length == 0) {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");
                    return false;
                }
                if (!nvc.AllKeys.Contains("z")) {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");
                    return false;
                }
                authHash = nvc.GetValues("z")[0].ToString();
            }

            if (string.IsNullOrWhiteSpace(authHash)) {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Authorization required.");
                return false;
            }

            var c = new CCHEncrypt(
                authHash,
                request.EncryptionKey(),
                Properties.Settings.Default.SecretKey
            );

            if (c.Keys.Contains<String>("CCHID"))
                request.CCHID(Convert.ToInt32(c["CCHID"].ToString()));

            if (c.Keys.Contains<String>("UserID"))
                request.UserID(c["UserID"].ToString());

            if (c.Keys.Contains<String>("EmployerID"))
                request.EmployerID(Convert.ToInt32(c["EmployerID"].ToString()));

            if (c.Keys.Contains<String>("UserName"))
                request.UserName(c["UserName"].ToString());

            return true;
        }

        private bool ExtendSession(HttpActionContext context) {
            //var userId = context.Request.UserID();
            var username = context.Request.UserID();
            if (string.IsNullOrEmpty(username)) {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User is invalid.");
                return false;
            }


            //Get an instance of the user sent in
            //var user = Membership.GetUser(new Guid(userId)); //exception from casting email address as GUID
            var user = Membership.GetUser(username);

            //cancel the request if the user is invalid or the User ID wasn't sent
            if (null == user) {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User is invalid.");
                return false;
            }

            //Check if the user is still online
            bool isOnline = user.IsOnline;
#if DEBUG
            isOnline = true;
#endif
            //Cancel the request if they are not online anymore
            if (!isOnline) {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Invalid Session.");
                return false;
            }

            //Update the user's timestamp so that they don't timeout prematurely
            Membership.GetUser(
                new Guid(user.ProviderUserKey.ToString()),
                true);

            return true;
        }

        
    }
}