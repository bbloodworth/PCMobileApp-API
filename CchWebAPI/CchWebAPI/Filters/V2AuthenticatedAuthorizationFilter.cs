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
            if (!isOriginAllowed(context.Request))
                return;

            if (!IsApiKeyValid(context))
                return;

            if (!ProcessAuthHash(context))
                return;

            if (!ExtendSession(context))
                return;

            return;
        }
        public Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        private bool isOriginAllowed(HttpRequestMessage request) {
            bool isCORSRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;

            if (isCORSRequest)
            {
                if (AllowedOrigins.Contains("*") || AllowedOrigins.Contains(request.Headers.GetValues(Origin).First()))
                {
                    // This CORS request is allowed
                    LogUtil.Trace(string.Format("CORS request allowed. Wildcard origins is {0}.  " +
                        "Request origin is {1}", AllowedOrigins.Contains("*"),
                        request.Headers.GetValues(Origin).First()));

                    return true;
                }
                else
                {
                    LogUtil.Trace(string.Format("CORS request blocked.  Allowed Origins are {0}.  Headers were:  {0}.",
                        FormatOrigins(), FormatHeaders(request)));

                    request.CreateResponse(HttpStatusCode.Forbidden);

                    return false;
                }
            }
            else
            {
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
            if(string.IsNullOrEmpty(context.Request.UserID())) {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User is invalid.");
                return false;
            }

            //Get an instance of the user sent in
            var user = Membership.GetUser(
                new Guid(context.Request.UserID()));

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
                new Guid(context.Request.UserID()),
                true);

            return true;
        }

        
    }
}