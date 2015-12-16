using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class CampaignController : ApiController
    {
        public HttpResponseMessage GetCampaignIntro(int employerId, int campaignId, string handshakeId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
            {
                using (GetCampaignIntro gci = new GetCampaignIntro())
                {
                    gci.CampaignId = campaignId;
                    gci.GetData(gecs.ConnString);

                    data.ContentName = gci.ContentName;
                    data.ContentType = gci.ContentType;
                    data.ContentId = gci.ContentId;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                }
            }
            return hrm;
        }

    }
}
