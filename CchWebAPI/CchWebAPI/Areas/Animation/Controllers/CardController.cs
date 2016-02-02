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

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class CardController : ApiController { 

        [HttpGet]
        public HttpResponseMessage GetMemberCardUrls(string localeCode) {
            var hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (var gecs = new GetEmployerConnString(Request.EmployerID())) {
                using (var gmct = new GetMemberCardTokens()) {
                    gmct.CchId = Request.CCHID();
                    gmct.LocaleCode = localeCode;
                    gmct.EmployerId = Request.EmployerID();
                    gmct.GetData(gecs.ConnString);

                    data.Results = gmct.MemberTokens;
                    data.TotalCount = gmct.TotalCount;
                }
            }

            if (data.TotalCount > 0)
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);

            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetMemberCardData(int employerId, string token) {
            var hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);
            dynamic data = new ExpandoObject();

            using (var gecs = new GetEmployerConnString(employerId)) {
                using (var gmidcd = new GetMemberIdCardData()) {
                    gmidcd.SecurityToken = token;
                    gmidcd.GetData(gecs.ConnString);

                    data.CardTypeFileName = gmidcd.CardTypeFileName;

                    var memberDataText = gmidcd.CardMemberDataText;
                    var memberCardData =
                        JsonConvert.DeserializeObject<MemberCardDataRecord>(memberDataText);

                    if (memberCardData == null) {
                        LogUtil.Log(string.Format("Unable to resolve token {0} for employer {1}. " +
                            "Possible session timeout or multi-device login", token, employerId),
                            new InvalidOperationException("Invalid security token"));
                    }
                    else {
                        LogUtil.Trace(string.Format("Resolved token {0} for employer {1}.", 
                            token, employerId));

                        memberCardData.CardTypeId = gmidcd.CardTypeId;
                        memberCardData.CardViewModeId = gmidcd.CardViewModeId;

                        data.CardTypeId = memberCardData.CardTypeId;
                        data.CardViewModeId = memberCardData.CardViewModeId;
                        data.MemberName = memberCardData.MemberName;
                        data.MemberNameValue = memberCardData.MemberNameValue;
                        data.MemberMedicalId = memberCardData.MemberMedicalId;
                        data.MemberMedicalIdValue = memberCardData.MemberMedicalIdValue;
                        data.MemberId = memberCardData.MemberId;
                        data.MemberIdValue = memberCardData.MemberIdValue;
                        data.EmployeeName = memberCardData.EmployeeName;
                        data.EmployeeNameValue = memberCardData.EmployeeNameValue;
                        data.EmployeeId = memberCardData.EmployeeId;
                        data.EmployeeIdValue = memberCardData.EmployeeIdValue;
                        data.EffectiveDate = memberCardData.EffectiveDate;
                        data.EffectiveDateValue = memberCardData.EffectiveDateValue;
                        data.InNetworkCoinsurance = memberCardData.InNetworkCoinsurance;
                        data.InNetworkCoinsuranceValue = memberCardData.InNetworkCoinsuranceValue;
                        data.OutNetworkCoinsurance = memberCardData.OutNetworkCoinsurance;
                        data.OutNetworkCoinsuranceValue = memberCardData.OutNetworkCoinsuranceValue;
                        data.NetworkDesignationValue = memberCardData.NetworkDesignationValue;
                        data.GroupDesignation = memberCardData.GroupDesignation;
                        data.GroupDesignationValue = memberCardData.GroupDesignationValue;
                        data.GroupName = memberCardData.GroupName;
                        data.GroupNameValue = memberCardData.GroupNameValue;
                        data.GroupNumber = memberCardData.GroupNumber;
                        data.GroupNumberValue = memberCardData.GroupNumberValue;
                        data.RxBin = memberCardData.RxBin;
                        data.RxBinValue = memberCardData.RxBinValue;
                        data.RxPcn = memberCardData.RxPcn;
                        data.RxPcnValue = memberCardData.RxPcnValue;
                        data.RxGrp = memberCardData.RxGrp;
                        data.RxGrpValue = memberCardData.RxGrpValue;
                        data.RxId = memberCardData.RxId;
                        data.RxIdValue = memberCardData.RxIdValue;
                        data.PlanName = memberCardData.PlanName;
                        data.PlanNameValue = memberCardData.PlanNameValue;
                        data.OnPlan = memberCardData.OnPlan;
                        data.OnPlanValue = memberCardData.OnPlanValue;
                        data.PlanType = memberCardData.PlanType;
                        data.PlanTypeValue = memberCardData.PlanTypeValue;
                        data.CoverageType = memberCardData.CoverageType;
                        data.CoverageTypeValue = memberCardData.CoverageTypeValue;
                        //TODO: One mismatch - is this a bug or intentional?  
                        //Rest are one for one match. Why all this mapping noise? 
                        //Can we just return the object?
                        //It seems like the XValue is the field and the non XValue
                        //are the field labels for UI localization purposes.
                        //so this seems like a bug to me.
                        data.GroupId = memberCardData.GroupIdValue;
                        data.GroupIdValue = memberCardData.GroupIdValue;
                        data.CardIssuedDateValue = memberCardData.CardIssuedDateValue;
                        data.PayorIdValue = memberCardData.PayorIdValue;

                        hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                    }
                }
            }

            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetCardUrl(string localeCode)
        {
            LogUtil.Log("TestCodeException", new InvalidOperationException("This should never get called"));
            string nani = localeCode;

            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);
            dynamic data = new ExpandoObject();

            int employerId = Request.EmployerID();
            int cchId = Request.CCHID();
            string cardBaseAddress = "CardBaseAddress".GetConfigurationValue();

            // This is a temporary stub for the Virtual ID Cards development
            string cardUrl = string.Format("{0}/?vcid={1}|{2}",
                cardBaseAddress, employerId, cchId);

            cardUrl = "https://reydavid.blob.core.windows.net/cards/MedID_cigna_marySmith.svg";
            //data.CardUrl = cardUrl;

            // This is a temporary stub for the Virtual ID Cards development
            List<CardResult> cards = new List<CardResult>();

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
