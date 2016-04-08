using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Services;
using CchWebAPI.Support;
using Newtonsoft.Json;

using ClearCost.IO.Log;
//using ClearCost.Security.JWT;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class CardController : ApiController { 

        [HttpGet]
        public HttpResponseMessage GetMemberCardUrls(string localeCode) {
            var hrm = Request.CreateResponse(HttpStatusCode.NoContent);
            var service = new CardService();
            var result = service.GetMemberCardUrls(localeCode, Request.EmployerID(), Request.CCHID());

            if (result != null && result.TotalCount > 0)
                hrm = Request.CreateResponse(HttpStatusCode.OK, result);

            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetMemberCardData(int employerId, string token) {
            var service = new CardService();
            var cardDetail = service.GetCardDetail(employerId, token);

            return Request.CreateResponse(HttpStatusCode.OK, cardDetail);
        }

        [HttpPost]
        public HttpResponseMessage SendIdCardEmail([FromBody] MemberCardWebRequest cardWebRequest) {
            var hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);
            var employerId = Request.EmployerID();
            dynamic data = new ExpandoObject();

            try {

                var cardService = new CardService();
                var result = cardService.SendIdCardEmail(employerId, cardWebRequest);

                if (result.Item1)
                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)result.Item2);
                else
                    hrm = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.Item3);

            }
            catch (Exception exc) {
                LogUtil.Log("Exception in CardController.SendIdCardEmail", exc);
                hrm = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message);
            }

            return hrm;
        }
    }
}
