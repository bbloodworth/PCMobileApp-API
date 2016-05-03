using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class MediaController : ApiController
    {
        public HttpResponseMessage GetMediaUrl(string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            if (!string.IsNullOrEmpty(baseContentId))
            {
                int campaignId = baseContentId.GetCampaignId();
                int contentId = baseContentId.GetContentId();

                if (campaignId > 0 && contentId > 0)
                {
                    int employerId = Request.EmployerID();
                    int cchId = Request.CCHID();

                    string mediaBaseAddress = "MediaBaseAddress".GetConfigurationValue();

                    string mediaUrl = string.Format("{0}/?cid={1}|{2}|{3}|{4}",
                        mediaBaseAddress, employerId, campaignId, contentId, cchId);

                    using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                    {
                        using (GetUserContent guc = new GetUserContent())
                        {
                            guc.CampaignId = campaignId;
                            guc.BaseContentId = contentId;
                            guc.PagesSize = 1;

                            guc.CchId = cchId;
                            guc.GetData(gecs.ConnString);

                            if (guc.Results.Count == 1)
                            {
                                string contentType = guc.Results[0].ContentType;

                                if (contentType.Equals("Vimeo"))
                                {
                                    mediaUrl = guc.Results[0].ContentUrl;
                                }
                            }
                        }
                    }
                    data.MediaUrl = mediaUrl;
                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
                }
            }
            return hrm;
        }
    }
}
