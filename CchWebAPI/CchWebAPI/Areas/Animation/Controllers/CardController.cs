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
using ClearCost.Security.JWT;

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
            var memberCardData = service.GetCardDetail(employerId, token);

            return Request.CreateResponse(HttpStatusCode.OK, memberCardData);
        }

        [HttpGet]
        public HttpResponseMessage GetCardUrl(string localeCode) {
            LogUtil.Log("TestCodeException", new InvalidOperationException("This should never get called"));
            var nani = localeCode;

            var hrm = Request.CreateResponse(HttpStatusCode.NoContent);
            dynamic data = new ExpandoObject();

            var employerId = Request.EmployerID();
            var cchId = Request.CCHID();
            var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();

            // This is a temporary stub for the Virtual ID Cards development
            var cardUrl = string.Format("{0}/?vcid={1}|{2}",
                cardBaseAddress, employerId, cchId);

            cardUrl = "https://reydavid.blob.core.windows.net/cards/MedID_cigna_marySmith.svg";

            // This is a temporary stub for the Virtual ID Cards development
            var cards = new List<CardResult>();

            cards.Add(new CardResult { CardName = "Medical", ViewMode = "Front", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Front.svg" });
            cards.Add(new CardResult { CardName = "Medical", ViewMode = "Back", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Back.svg" });
            cards.Add(new CardResult { CardName = "Medical", ViewMode = "Full", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Full.svg" });
            cards.Add(new CardResult { CardName = "Medical", ViewMode = "Portrait", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Portrait.svg" });
            cards.Add(new CardResult { CardName = "RX", ViewMode = "Front", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Front.svg" });
            cards.Add(new CardResult { CardName = "RX", ViewMode = "Back", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Back.svg" });
            cards.Add(new CardResult { CardName = "RX", ViewMode = "Full", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Full.svg" });
            cards.Add(new CardResult { CardName = "RX", ViewMode = "Portrait", CardUrl = "https://reydavid.blob.core.windows.net/cards/Medical_Portrait.svg" });

            data.Results = cards;
            data.TotalCount = 8;

            hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
            return hrm;
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
