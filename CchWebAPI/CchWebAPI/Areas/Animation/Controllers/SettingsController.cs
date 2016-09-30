using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;
using ClearCost.Platform;
using CchWebAPI.Configuration;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class SettingsController : ApiController
    {
        public HttpResponseMessage GetConfigurationValue(int employerId, string hsId, [FromUri]string configKey)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                new Exception("Client Handshake is Not Authorized"));

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                var configValue = ClientSettingCache.Settings(employerId)
                    .FirstOrDefault(s => s.Key.Equals(configKey));

                dynamic data = new ExpandoObject();
                data.Results = configValue;

                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }
        /// <summary>
        /// Returns an object containing all configuration values.  
        /// </summary>
        /// <param name="employerId"></param>
        /// <param name="hsId"></param>
        /// <returns></returns>
        /// <remarks>
        /// It currently returns only minimum secret answer length.  ClearCost.Data will need to 
        /// be updated to return all of the client configuration values.  Updating ClearCost.Data
        /// broke several things last time.  As new configuration values are created, the Settings
        /// class should be updated so that all configuration values are returned in a single request.
        /// </remarks>
        public HttpResponseMessage GetConfigurationValues(int employerId, string handshakeId) {
            var hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                new Exception("Client Handshake is Not Authorized"));

            if (ValidateConsumer.IsValidConsumer(handshakeId)) {
                var settings = new Models.Settings();
                settings.Security.MinimumSecurityAnswerLength = SecurityConfiguration.Settings.MinimumSecretAnswerLength;

                hrm = Request.CreateResponse(HttpStatusCode.OK, settings);
            }

            return hrm;
        }
    }
}
