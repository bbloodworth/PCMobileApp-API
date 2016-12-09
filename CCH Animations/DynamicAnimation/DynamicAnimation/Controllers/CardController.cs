using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DynamicAnimation.Common;
using DynamicAnimation.Models;

using ClearCost.IO.Log;

namespace DynamicAnimation.Controllers
{
    public class CardController : Controller
    {
        // GET: Card
        public async Task<ActionResult> Index() {
            ViewBag.Message = "Virtual ID Cards";
            Response.ContentType = "image/svg+xml";

            if (null == Request.QueryString["tkn"]) {
                LogUtil.Log("Missing security token in CardController.",
                    new InvalidOperationException("Security Token is required."));

                return View();
            }

            var tokenSegments = Request.QueryString["tkn"].Split('|');

            if (tokenSegments.Length != 2) {
                LogUtil.Log("Invalid arguments in CardController.",
                    new InvalidOperationException(
                        string.Format("QueryString token expected 2 segments but got {0}.",
                        tokenSegments.Length)));

                return View();
            }

            var employerId = int.Parse(tokenSegments[0]);
            var jwt = tokenSegments[1];

            HandleCampaignSession(employerId);

            var cardDetail = await WebApiService.GetCardDetail(employerId, jwt);

            HelperService.LogAnonEvent(ExperienceEvents.Debug, FormatInfoMessage(cardDetail));

            if (cardDetail.CardTypeId < 1 || cardDetail.CardViewModeId < 1 || string.IsNullOrEmpty(cardDetail.CardTypeFileName)) {
                cardDetail = await WebApiService.GetCardDetail(employerId, jwt);
                HelperService.LogAnonEvent(ExperienceEvents.Debug,
                    string.Format("Retry - Card Type ID: {0}  View Mode ID: {1}  File Name: {2}",
                    cardDetail.CardTypeId, cardDetail.CardViewModeId, cardDetail.CardTypeFileName));
            }

            if (cardDetail.Expired)
                return View("Timeout");

            if (cardDetail.Invalid)
                return View("InvalidEmployerId");

            if (cardDetail.CardTypeId < 1 || cardDetail.CardViewModeId < 1 || string.IsNullOrEmpty(cardDetail.CardTypeFileName)) {
                HelperService.LogAnonEvent(ExperienceEvents.Error, FormatInfoMessage(cardDetail));

                LogUtil.Log(string.Format("Unable to resolve card data for token {0} for employer {1}.",
                    jwt, employerId),
                    new InvalidOperationException(FormatInfoMessage(cardDetail)));

                return View();
            };

            MapViewBag(cardDetail);

            return View(string.Format("{0}_{1}", cardDetail.CardTypeFileName, ResolveViewMode(cardDetail)));
        }

        private static void HandleCampaignSession(int employerId) {
            var campaignSession = CampaignSessionModel.Current;
            campaignSession.EmployerId = employerId;

            var logResponse = HelperService.LogInitialEvent(employerId);
            campaignSession.ExperienceUserId = logResponse.ExperienceUserId;

            CampaignSessionModel.Current = campaignSession;
        }

        private void MapViewBag(MemberCardDataModel cardDetail) {
            ViewBag.EffectiveDate = cardDetail.EffectiveDate;
            ViewBag.EffectiveDateValue = cardDetail.EffectiveDateValue;
            ViewBag.EmployeeId = cardDetail.EmployeeId;
            ViewBag.EmployeeIdValue = cardDetail.EmployeeIdValue;
            ViewBag.EmployeeName = cardDetail.EmployeeName;
            ViewBag.EmployeeNameValue = cardDetail.EmployeeNameValue;
            ViewBag.GroupDesignation = cardDetail.GroupDesignation;
            ViewBag.GroupDesignationValue = cardDetail.GroupDesignationValue;
            ViewBag.GroupName = cardDetail.GroupName;
            ViewBag.GroupNameValue = cardDetail.GroupNameValue;
            ViewBag.GroupNumber = cardDetail.GroupNumber;
            ViewBag.GroupNumberValue = cardDetail.GroupNumberValue;
            ViewBag.GroupId = cardDetail.GroupId;
            ViewBag.GroupIdValue = cardDetail.GroupIdValue;
            ViewBag.InNetworkCoinsurance = cardDetail.InNetworkCoinsurance;
            ViewBag.InNetworkCoinsuranceValue = cardDetail.InNetworkCoinsuranceValue;
            ViewBag.MemberId = cardDetail.MemberId;
            ViewBag.MemberIdValue = cardDetail.MemberIdValue;
            ViewBag.MemberMedicalId = cardDetail.MemberMedicalId;
            ViewBag.MemberMedicalIdValue = cardDetail.MemberMedicalIdValue;
            ViewBag.MemberName = cardDetail.MemberName;
            ViewBag.MemberNameValue = cardDetail.MemberNameValue;
            ViewBag.NetworkDesignationValue = cardDetail.NetworkDesignationValue;
            ViewBag.OnPlan = cardDetail.OnPlan;
            ViewBag.OnPlanValue = cardDetail.OnPlanValue;
            ViewBag.OutNetworkCoinsurance = cardDetail.OutNetworkCoinsurance;
            ViewBag.OutNetworkCoinsuranceValue = cardDetail.OutNetworkCoinsuranceValue;
            ViewBag.PlanName = cardDetail.PlanName;
            ViewBag.PlanNameValue = cardDetail.PlanNameValue;
            ViewBag.RxBin = cardDetail.RxBin;
            ViewBag.RxBinValue = cardDetail.RxBinValue;
            ViewBag.RxGrp = cardDetail.RxGrp;
            ViewBag.RxGrpValue = cardDetail.RxGrpValue;
            ViewBag.RxId = cardDetail.RxId;
            ViewBag.RxIdValue = cardDetail.RxIdValue;
            ViewBag.RxPcn = cardDetail.RxPcn;
            ViewBag.RxPcnValue = cardDetail.RxPcnValue;
            ViewBag.PlanType = cardDetail.PlanType;
            ViewBag.PlanTypeValue = cardDetail.PlanTypeValue;
            ViewBag.CoverageType = cardDetail.CoverageType;
            ViewBag.CoverageTypeValue = cardDetail.CoverageTypeValue;
            ViewBag.CardIssuedDateValue = cardDetail.CardIssuedDateValue;
            ViewBag.PayorIDValue = cardDetail.PayorIdValue;
            ViewBag.IndividualDeductibleAmt = cardDetail.IndividualDeductibleAmt;
            ViewBag.FamilyDeductibleAmt = cardDetail.FamilyDeductibleAmt;
            ViewBag.ExamCopayAmt = cardDetail.ExamCopayAmt;
            ViewBag.MaterialsCopayAmt = cardDetail.MaterialsCopayAmt;
            ViewBag.GenderCode = cardDetail.GenderCode;
            ViewBag.BirthDate= cardDetail.BirthDate;
            ViewBag.MemberFirstName = cardDetail.MemberFirstName;
            ViewBag.MemberMiddleName = cardDetail.MemberMiddleName;
            ViewBag.MemberLastName = cardDetail.MemberLastName;
            ViewBag.DateOfBirthMM = cardDetail.DateOfBirthMM;
            ViewBag.DateOfBirthYY = cardDetail.DateOfBirthYY;
            ViewBag.ContractPrefixCode = cardDetail.ContractPrefixCode;
        }

        private string ResolveViewMode(MemberCardDataModel cardDetail) {
            switch (cardDetail.CardViewModeId) {
                case 1:
                    return "Front";
                case 2:
                    return "Back";
                case 3:
                    return "Full_Front";
                case 4:
                    return "Full_Back";
                default:
                    return "Front";
            }
        }

        private string FormatInfoMessage(MemberCardDataModel cardDetail) {
            return string.Format("Card Type ID: {0}  View Mode ID: {1}  File Name: {2}",
                cardDetail.CardTypeId, cardDetail.CardViewModeId, cardDetail.CardTypeFileName);
        }
    }
}
