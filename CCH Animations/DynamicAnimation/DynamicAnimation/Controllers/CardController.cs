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
        public ActionResult Index()
        {
            ViewBag.Message = "Virtual ID Cards";
            Response.ContentType = "image/svg+xml";

            if (null == Request.QueryString["tkn"]) {
                LogUtil.Log("Missing security token in CardController.", 
                    new InvalidOperationException("Security Token is required."));

                return View();
            }

            var qparam = Request.QueryString["tkn"];
            string[] qparams = qparam.Split('|');

            if(qparams.Length != 2) {
                LogUtil.Log("Invalid arguments in CardController.", 
                    new InvalidOperationException(
                        string.Format("QueryString parameters expected 2 but got {0}.", qparam.Length)));

                return View();
            }


            var employerId = int.Parse(qparams[0]);
            var token = qparams[1];

            var campaignSession = CampaignSessionModel.Current;
            campaignSession.EmployerId = employerId;

            var logResponse = HelperService.LogInitialEvent(employerId);
            campaignSession.ExperienceUserId = logResponse.ExperienceUserId;

            CampaignSessionModel.Current = campaignSession;

            Task<MemberCardDataModel> taskCardData = WebApiService.GetMemberCardData(employerId, token);
            MemberCardDataModel cardData = taskCardData.Result;
                    
            var cardTypeFileName = cardData.CardTypeFileName;
            var cardTypeId = cardData.CardTypeId;
            var cardViewModeId = cardData.CardViewModeId;

            var infoMessage = string.Format("Card Type ID: {0}  View Mode ID: {1}  File Name: {2}",
                cardTypeId, cardViewModeId, cardTypeFileName);
            HelperService.LogAnonEvent(ExperienceEvents.Debug, infoMessage);

            if (cardTypeId < 1 || cardViewModeId < 1 || string.IsNullOrEmpty(cardTypeFileName)) {
                taskCardData = WebApiService.GetMemberCardData(employerId, token);
                cardData = taskCardData.Result;

                cardTypeFileName = cardData.CardTypeFileName;
                cardTypeId = cardData.CardTypeId;
                cardViewModeId = cardData.CardViewModeId;

                infoMessage = string.Format("Retry - Card Type ID: {0}  View Mode ID: {1}  File Name: {2}",
                    cardTypeId, cardViewModeId, cardTypeFileName);
                HelperService.LogAnonEvent(ExperienceEvents.Debug, infoMessage);
            }

            if (cardTypeId < 1 || cardViewModeId < 1 || string.IsNullOrEmpty(cardTypeFileName)) {
                var errMessage = string.Format("Card Type ID: {0}  View Mode ID: {1}  File Name: {2}",
                    cardTypeId, cardViewModeId, cardTypeFileName);

                HelperService.LogAnonEvent(ExperienceEvents.Error, errMessage);

                LogUtil.Log(string.Format("Unable to resolve card data for token {0}.", token), 
                    new InvalidOperationException(errMessage));

                return View();
            };

            ViewBag.EffectiveDate = cardData.EffectiveDate;
            ViewBag.EffectiveDateValue = cardData.EffectiveDateValue;
            ViewBag.EmployeeId = cardData.EmployeeId;
            ViewBag.EmployeeIdValue = cardData.EmployeeIdValue;
            ViewBag.EmployeeName = cardData.EmployeeName;
            ViewBag.EmployeeNameValue = cardData.EmployeeNameValue;
            ViewBag.GroupDesignation = cardData.GroupDesignation;
            ViewBag.GroupDesignationValue = cardData.GroupDesignationValue;
            ViewBag.GroupName = cardData.GroupName;
            ViewBag.GroupNameValue = cardData.GroupNameValue;
            ViewBag.GroupNumber = cardData.GroupNumber;
            ViewBag.GroupNumberValue = cardData.GroupNumberValue;
            ViewBag.GroupId = cardData.GroupId;
            ViewBag.GroupIdValue = cardData.GroupIdValue;
            ViewBag.InNetworkCoinsurance = cardData.InNetworkCoinsurance;
            ViewBag.InNetworkCoinsuranceValue = cardData.InNetworkCoinsuranceValue;
            ViewBag.MemberId = cardData.MemberId;
            ViewBag.MemberIdValue = cardData.MemberIdValue;
            ViewBag.MemberMedicalId = cardData.MemberMedicalId;
            ViewBag.MemberMedicalIdValue = cardData.MemberMedicalIdValue;
            ViewBag.MemberName = cardData.MemberName;
            ViewBag.MemberNameValue = cardData.MemberNameValue;
            ViewBag.NetworkDesignationValue = cardData.NetworkDesignationValue;
            ViewBag.OnPlan = cardData.OnPlan;
            ViewBag.OnPlanValue = cardData.OnPlanValue;
            ViewBag.OutNetworkCoinsurance = cardData.OutNetworkCoinsurance;
            ViewBag.OutNetworkCoinsuranceValue = cardData.OutNetworkCoinsuranceValue;
            ViewBag.PlanName = cardData.PlanName;
            ViewBag.PlanNameValue = cardData.PlanNameValue;
            ViewBag.RxBin = cardData.RxBin;
            ViewBag.RxBinValue = cardData.RxBinValue;
            ViewBag.RxGrp = cardData.RxGrp;
            ViewBag.RxGrpValue = cardData.RxGrpValue;
            ViewBag.RxId = cardData.RxId;
            ViewBag.RxIdValue = cardData.RxIdValue;
            ViewBag.RxPcn = cardData.RxPcn;
            ViewBag.RxPcnValue = cardData.RxPcnValue;
            ViewBag.PlanType = cardData.PlanType;
            ViewBag.PlanTypeValue = cardData.PlanTypeValue;
            ViewBag.CoverageType = cardData.CoverageType;
            ViewBag.CoverageTypeValue = cardData.CoverageTypeValue;
            ViewBag.CardIssuedDateValue = cardData.CardIssuedDateValue;
            ViewBag.PayorIDValue = cardData.PayorIdValue;
                        
            string viewMode;
            switch (cardViewModeId) {
                case 1:
                    viewMode = "Front";
                    break;
                case 2:
                    viewMode = "Back";
                    break;
                case 3:
                    viewMode = "Full_Front";
                    break;
                case 4:
                    viewMode = "Full_Back";
                    break;
                default:
                    viewMode = "Front";
                    break;
            }

            return View(string.Format("{0}_{1}", cardTypeFileName, viewMode));
        }
    }
}
