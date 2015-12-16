using System.Web.Http;
using System.Net.Http;
using CchWebAPI.Areas.PComm.Models;
using CchWebAPI.Support;
using System.Net;

namespace CchWebAPI.Areas.PComm.Controllers
{
    public class VideoEventController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage LogAnEvent(string hsId, [FromBody] VideoEventLog eventLog)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.NoContent, "Unexpected Error");

                if (eventLog.EmployerId > 0)
                {
                    using (GetEmployerConnString gecs = new GetEmployerConnString(eventLog.EmployerId))
                    {
                        using (LogVideoEvent lve = new LogVideoEvent())
                        {
                            lve.VideoCampaignMemberId = eventLog.VideoCampaignMemberId;
                            lve.VideoLogEvent = eventLog.VideoLogEvent;
                            lve.VideoLogEventId = eventLog.VideoLogEventId;
                            lve.Comment = eventLog.Comment;

                            lve.PostData(gecs.ConnString);
                            if (lve.PostReturn == 1)
                            {
                                hrm = Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent,
                                    "Log Video Event Procedure Failed.");
                            }
                        }
                    }
                }
            }
            return hrm;
        }
    }
}