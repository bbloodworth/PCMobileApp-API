using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;
using ClearCost.Platform;

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
    }
}
