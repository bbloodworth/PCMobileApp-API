using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.PComm.Controllers
{
    public class VideoServiceController : ApiController
    {
        public HttpResponseMessage GetVideoFilesList(string hsId, int employerId, int campaignId)
        {
            dynamic data = new ExpandoObject();
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.NoContent, "Unexpected Error");

                if (employerId > 0)
                {
                    using (var gecs = new GetEmployerConnString(employerId))
                    {
                        using (var gvcfi = new GetVideoCampaignFileIds())
                        {
                            gvcfi.CampaignId = campaignId;
                            gvcfi.GetData(gecs.ConnString);

                            if (!gvcfi.HasThrownError)
                            {
                                data.ListOfVideoIds = gvcfi.Results;

                                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                            }
                        }
                    }
                }
            }
            return hrm;
        }

        public HttpResponseMessage GetVideoMemberData(string hsId, int employerId, string videoMemberId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.NoContent, "Unexpected Error");

                if (employerId > 0)
                {
                    using (var gecs = new GetEmployerConnString(employerId))
                    {
                        using (var gvcmd = new GetVideoCampaignMemberData())
                        {
                            gvcmd.VideoCampaignFileId = videoMemberId;
                            gvcmd.GetData(gecs.ConnString);

                            if (!gvcmd.HasThrownError)
                            {
                                string videoMemberData = gvcmd.VideoMemberData;

                                hrm = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    RequestMessage = Request, 
                                    Content = new StringContent(videoMemberData)
                                };
                            }
                        }
                    }
                }
            }
            return hrm;
        }
    }
}