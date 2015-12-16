using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.PComm.Controllers
{
    public class VideoCampaignController : ApiController
    {
        public HttpResponseMessage GetVideoCampaign(string hsId, int employerId, int campaignId)
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
                        using (var gvc = new GetVideoCampaign())
                        {
                            gvc.VideoCampaignId = campaignId;
                            gvc.GetData(gecs.ConnString);

                            if (!gvc.HasThrownError)
                            {
                                data.VideoCampaignId = campaignId;
                                data.VideoDefinitionId = gvc.VideoDefinitionId.ToUpper();
                                data.IntroVideoDefinitionId = gvc.IntroVideoDefinitionId.ToUpper();
                                data.IsVideoCampaignActive = gvc.IsVideoCampaignActive;
                                hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
                            }
                        }
                    }
                }
            }
            return hrm;
        }

        public HttpResponseMessage GetCampaignMemberInfo(string hsId, int employerId, string campaignMemberId)
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
                        using (var gvcmi = new GetVideoCampaignMemberInfo())
                        {
                            gvcmi.VideoCampaignMemberId = campaignMemberId;
                            gvcmi.GetData(gecs.ConnString);

                            if (!gvcmi.HasThrownError)
                            {
                                data.DateOfBirth = gvcmi.DateOfBirth;
                                data.IntroVideoDefinitionId = gvcmi.IntroVideoDefinitionId.ToUpper();
                                data.IntroVideoDefinitionName = gvcmi.IntroVideoDefinitionName;
                                data.IsVideoCampaignActive = gvcmi.IsVideoCampaignActive;
                                data.LastName = gvcmi.LastName;
                                data.MemberSsn = gvcmi.MemberSsn;
                                data.VideoCampaignFileId = gvcmi.VideoCampaignFileId.ToUpper();
                                data.VideoCampaignId = gvcmi.VideoCampaignId;
                                data.VideoDefinitionName = gvcmi.VideoDefinitionName;
                                data.CchEmployerLink = gvcmi.CchEmployerLink;
                                data.EmployerBenefitsLink = gvcmi.EmployerBenefitsLink;
                                hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
                            }
                        }
                    }
                }
            }
            return hrm;
        }
    }
}
