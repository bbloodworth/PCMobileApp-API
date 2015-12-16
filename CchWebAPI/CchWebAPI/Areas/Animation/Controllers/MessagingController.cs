using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class MessagingController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage UserDevice([FromBody] UserDeviceRequest deviceRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.ExpectationFailed, "Unexpected Error");

            using (GetEmployerConnString gecs = new GetEmployerConnString(deviceRequest.EmployerId))
            {
                using (InsertUpdateDevice iud = new InsertUpdateDevice())
                {
                    iud.DeviceId = deviceRequest.DeviceId;
                    int iClientAllowPushInd;
                    if (int.TryParse(deviceRequest.ClientAllowPushInd, out iClientAllowPushInd))
                    {
                        iud.ClientAllowPushInd = iClientAllowPushInd;
                    }
                    int iNativeAllowPushInd;
                    if (int.TryParse(deviceRequest.NativeAllowPushInd, out iNativeAllowPushInd))
                    {
                        iud.NativeAllowPushInd = iNativeAllowPushInd;
                    }
                    DateTime dPromptDate;
                    if (DateTime.TryParse(deviceRequest.LastPushPromptDate, out dPromptDate))
                    {
                        iud.LastPushPromptDate = dPromptDate;
                    }
                    iud.PostData(gecs.ConnString);
                    if (iud.PostReturn == 1)
                    {
                        hrm = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage PushPromptStatus(int employerId, string hsId, string deviceId)
        {
            dynamic data = new ExpandoObject();
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                new Exception("Client Handshake is Not Authorized"));

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                {
                    using (GetDevicePushPromptStatus gdpps = new GetDevicePushPromptStatus())
                    {
                        gdpps.DeviceId = deviceId;
                        gdpps.GetData(gecs.ConnString);

                        data.PromptStatus = gdpps.PromptStatus ? "1" : "0";
                    }
                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
                }
            }
            return hrm;
        }
    }
}
