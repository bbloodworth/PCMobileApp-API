using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;
using System.Linq;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class ExperienceController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage LogInitialExperience(string hsId, [FromBody] ExperienceLogRequest eventLogRequest)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.NoContent, "Unexpected Error");

                if (eventLogRequest.EmployerId > 0)
                {
                    //append ClientVersion from header
                    eventLogRequest.ClientVersion = Request.Headers.GetValues("X-Client-Version").FirstOrDefault();

                    using (GetEmployerConnString gecs = new GetEmployerConnString(eventLogRequest.EmployerId))
                    {
                        try
                        {
                            using (InsertExperienceLog iel = new InsertExperienceLog())
                            {
                                ExperienceLogResponse elr = new ExperienceLogResponse
                                {
                                    ExperienceUserId = Guid.NewGuid().ToString()
                                };
                                iel.ExperienceEventId = eventLogRequest.EventId;
                                iel.ExperienceEventDesc = eventLogRequest.EventName;
                                iel.ExperienceUserId = elr.ExperienceUserId;
                                iel.Comment = eventLogRequest.LogComment;
                                iel.DeviceId = eventLogRequest.DeviceId;
                                iel.ClientVersion = eventLogRequest.ClientVersion;

                                iel.PostData(gecs.ConnString);
                                hrm = Request.CreateResponse(HttpStatusCode.OK, elr);
                            }
                        }
                        catch (Exception exc)
                        {
                            hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, String.Format("Insert Experience Event Procedure Failed: {0}", exc.Message));
                        }
                    }
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage LogExperienceEvent(string hsId, [FromBody] ExperienceLogRequest eventLogRequest)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.NoContent, "Unexpected Error");

                if (eventLogRequest.EmployerId > 0)
                {
                    using (GetEmployerConnString gecs = new GetEmployerConnString(eventLogRequest.EmployerId))
                    {
                        using (InsertExperienceLog iel = new InsertExperienceLog())
                        {
                            iel.ExperienceEventId = eventLogRequest.EventId;
                            iel.ExperienceEventDesc = eventLogRequest.EventName;
                            if (Request.CCHID() > 0)
                            {
                                iel.CCHID = Request.CCHID();
                            }
                            if (eventLogRequest.CchId > 0)
                            {
                                iel.CCHID = eventLogRequest.CchId;
                            }
                            if (!string.IsNullOrEmpty(eventLogRequest.ContentId))
                            {
                                int contentId = eventLogRequest.ContentId.GetContentId();
                                if (contentId > 0)
                                {
                                    iel.ContentId = contentId;
                                }
                                else
                                {
                                    iel.ContentId = int.Parse(eventLogRequest.ContentId);
                                }
                            }
                            iel.ExperienceUserId = eventLogRequest.ExperienceUserId;
                            iel.Comment = eventLogRequest.LogComment;
                            iel.DeviceId = eventLogRequest.DeviceId;

                            iel.PostData(gecs.ConnString);
                            if (iel.PostReturn == 1)
                            {
                                hrm = Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent,
                                    "Insert Experience Event Procedure Failed.");
                            }
                        }
                    }
                }
            }
            return hrm;
        }
    }
}
