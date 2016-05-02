using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;
using CchWebAPI.Services;

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
                ConfigService configService = new ConfigService(employerId);

                string configValue = configService.GetValue(configKey);

                dynamic data = new ExpandoObject();
                data.Results = configValue;

                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }
    }
}
