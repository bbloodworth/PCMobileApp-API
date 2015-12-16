using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class UserContentController : ApiController
    {
        public HttpResponseMessage GetEmployerNotifications(string hsId, int employerId, int pageSize,
            string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateResponse(HttpStatusCode.NoContent);

                dynamic data = new ExpandoObject();
                using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                {
                    if (!string.IsNullOrEmpty(gecs.ConnString))
                    {
                        using (GetAnonymousContent gac = new GetAnonymousContent())
                        {
                            if (pageSize > 0)
                            {
                                gac.PagesSize = pageSize;
                            }
                            if (!string.IsNullOrEmpty(baseContentId))
                            {
                                int campaignId = baseContentId.GetCampaignId();
                                int contentId = baseContentId.GetContentId();

                                if (campaignId == 0 && contentId > 0)
                                {
                                    gac.BaseContentId = contentId;
                                }
                            }
                            gac.GetData(gecs.ConnString);

                            data.Results = gac.Results;
                            data.TotalCount = gac.TotalCount;
                        }
                        if (data.TotalCount > 0)
                        {
                            hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                        }
                    }
                }
            }
            return hrm;
        }

        public HttpResponseMessage GetEmployerNotificationsLocale(string hsId, string localeCode, int employerId, int pageSize,
            string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateResponse(HttpStatusCode.NoContent);

                dynamic data = new ExpandoObject();
                using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                {
                    if (!string.IsNullOrEmpty(gecs.ConnString))
                    {
                        using (GetAnonymousContent gac = new GetAnonymousContent())
                        {
                            gac.LocaleCode = localeCode;
                            if (pageSize > 0)
                            {
                                gac.PagesSize = pageSize;
                            }
                            if (!string.IsNullOrEmpty(baseContentId))
                            {
                                int campaignId = baseContentId.GetCampaignId();
                                int contentId = baseContentId.GetContentId();

                                if (campaignId == 0 && contentId > 0)
                                {
                                    gac.BaseContentId = contentId;
                                }
                            }
                            gac.GetData(gecs.ConnString);

                            data.Results = gac.Results;
                            data.TotalCount = gac.TotalCount;
                        }
                        if (data.TotalCount > 0)
                        {
                            hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                        }
                    }
                }
            }
            return hrm;
        }

        public HttpResponseMessage GetUserContent(int pageSize, string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserContent guc = new GetUserContent())
                {
                    if (pageSize > 0)
                    {
                        guc.PagesSize = pageSize;
                    }
                    if (!string.IsNullOrEmpty(baseContentId))
                    {
                        int campaignId = baseContentId.GetCampaignId();
                        int contentId = baseContentId.GetContentId();

                        if (contentId > 0)
                        {
                            guc.CampaignId = campaignId;
                            guc.BaseContentId = contentId;
                        }
                    }
                    guc.CchId = Request.CCHID();
                    guc.GetData(gecs.ConnString);

                    data.Results = guc.Results;
                    data.TotalCount = guc.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        public HttpResponseMessage GetUserContentLocale(string localeCode, int pageSize, string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserContent guc = new GetUserContent())
                {
                    guc.LocaleCode = localeCode;
                    if (pageSize > 0)
                    {
                        guc.PagesSize = pageSize;
                    }
                    if (!string.IsNullOrEmpty(baseContentId))
                    {
                        int campaignId = baseContentId.GetCampaignId();
                        int contentId = baseContentId.GetContentId();

                        if (contentId > 0)
                        {
                            guc.CampaignId = campaignId;
                            guc.BaseContentId = contentId;
                        }
                    }
                    guc.CchId = Request.CCHID();
                    guc.GetData(gecs.ConnString);

                    data.Results = guc.Results;
                    data.TotalCount = guc.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        public HttpResponseMessage GetUserRewards(int pageSize, string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserRewards gur = new GetUserRewards())
                {
                    if (pageSize > 0)
                    {
                        gur.PagesSize = pageSize;
                    }
                    if (!string.IsNullOrEmpty(baseContentId))
                    {
                        int campaignId = baseContentId.GetCampaignId();
                        int contentId = baseContentId.GetContentId();

                        if (contentId > 0)
                        {
                            gur.CampaignId = campaignId;
                            gur.BaseContentId = contentId;
                        }
                    }
                    gur.CchId = Request.CCHID();
                    gur.GetData(gecs.ConnString);

                    data.TotalPoints = gur.Summary.TotalPoints;
                    data.TotalRewards = gur.Summary.TotalRewards;
                    data.Results = gur.Results;
                    data.TotalCount = gur.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        public HttpResponseMessage GetUserRewardsLocale(string localeCode, int pageSize, string baseContentId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserRewards gur = new GetUserRewards())
                {
                    gur.LocaleCode = localeCode;
                    if (pageSize > 0)
                    {
                        gur.PagesSize = pageSize;
                    }
                    if (!string.IsNullOrEmpty(baseContentId))
                    {
                        int campaignId = baseContentId.GetCampaignId();
                        int contentId = baseContentId.GetContentId();

                        if (contentId > 0)
                        {
                            gur.CampaignId = campaignId;
                            gur.BaseContentId = contentId;
                        }
                    }
                    gur.CchId = Request.CCHID();
                    gur.GetData(gecs.ConnString);

                    data.TotalPoints = gur.Summary.TotalPoints;
                    data.TotalRewards = gur.Summary.TotalRewards;
                    data.Results = gur.Results;
                    data.TotalCount = gur.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage UserPreferences()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserContentPreference gucp = new GetUserContentPreference())
                {
                    gucp.CCHID = Request.CCHID();
                    gucp.GetData(gecs.ConnString);

                    data.SmsInd = gucp.SmsInd;
                    data.EmailInd = gucp.EmailInd;
                    data.OsBasedAlertInd = gucp.OsBasedAlertInd;
                    data.LocaleCode = gucp.LocaleCode;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage UserPreferences([FromBody] UserContentPreferenceRequest preferenceRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.NoContent, "Unexpected Error");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (InsertUpdateUserContentPreference iuucp = new InsertUpdateUserContentPreference())
                {
                    iuucp.CCHID = Request.CCHID();
                    iuucp.EmailInd = preferenceRequest.EmailInd;
                    iuucp.OsBasedAlertInd = preferenceRequest.OsBasedAlertInd;
                    iuucp.SmsInd = preferenceRequest.SmsInd;
                    iuucp.LocaleCode = preferenceRequest.LocaleCode;

                    iuucp.PostData(gecs.ConnString);
                    if (iuucp.PostReturn == 1)
                    {
                        hrm = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserContentStatus([FromBody]UserContentStatusRequest statusRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, "Unexpected Error");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (UpdateUserContentStatus uucs = new UpdateUserContentStatus())
                {
                    uucs.CampaignId = statusRequest.ContentId.GetCampaignId();
                    uucs.CchId = Request.CCHID();
                    uucs.ContentId = statusRequest.ContentId.GetContentId();
                    uucs.StatusId = statusRequest.StatusId;
                    uucs.StatusDesc = statusRequest.StatusDesc;

                    uucs.PostData(gecs.ConnString);
                    if (uucs.PostReturn == 1)
                    {
                        hrm = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            return hrm;
        }
    }
}
