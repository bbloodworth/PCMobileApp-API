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
    public class CardController : ApiController
    {
        private const string CardFilesFolder = "C:\\inetpub\\Resources\\";
        private const string InkScapeExePath = "C:\\Program Files\\Inkscape\\inkscape.exe";

        [HttpGet]
        public HttpResponseMessage GetMemberCardUrls(string localeCode)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetMemberCardTokens gmct = new GetMemberCardTokens())
                {
                    gmct.CchId = Request.CCHID();
                    gmct.LocaleCode = localeCode;
                    gmct.EmployerId = Request.EmployerID();
                    gmct.GetData(gecs.ConnString);

                    data.Results = gmct.MemberTokens;
                    data.TotalCount = gmct.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetMemberCardData(int employerId, string token)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
            {
                using (GetMemberIdCardData gmidcd = new GetMemberIdCardData())
                {
                    gmidcd.SecurityToken = token;
                    gmidcd.GetData(gecs.ConnString);

                    data.CardTypeFileName = gmidcd.CardTypeFileName;

                    string memberDataText = gmidcd.CardMemberDataText;
                    MemberCardDataRecord memberCardData =
                        JsonConvert.DeserializeObject<MemberCardDataRecord>(memberDataText);

                    if (memberCardData != null)
                    {
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
                hrm = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc.Message);
            }

            return hrm;
        }

        private bool GetMemberCardWebRequest(int employerId, MemberCardWebRequest cardWebRequest)
        {
            try {
                var webClient = new WebClient();
                var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();
                var url = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, employerId, cardWebRequest.CardToken);

                var cardSvgFile = string.Format("{0}card_{1}_{2}.svg",
                    CardFilesFolder, employerId, cardWebRequest.CardToken);

                webClient.DownloadFile(url, cardSvgFile);
                return true;
            }
            catch (Exception ex) {
                //TODO: Log error?
                return false;
            }
        }

        private string GetMemberCardPdfFile(int employerId, MemberCardWebRequest cardWebRequest)
        {
            // Convert the SVG file into a PNG file
            string inkscapeArguments = string.Format("--file={0}card_{1}_{2}.svg --export-pdf={0}card_{1}_{2}.pdf",
                CardFilesFolder, employerId, cardWebRequest.CardToken);

            int timeout = "Email.Timeout".GetConfigurationNumericValue();
            Thread.Sleep(timeout);

            ProcessStartInfo psi = new ProcessStartInfo(InkScapeExePath, inkscapeArguments);
            Process inkscapeProcess = Process.Start(psi);
            if (inkscapeProcess != null)
                inkscapeProcess.WaitForExit();

            // Send PDF file as an email attachment to designated recipient
            string cardPdfFile = string.Format("{0}card_{1}_{2}.pdf",
                CardFilesFolder, employerId, cardWebRequest.CardToken);

            Thread.Sleep(timeout);
            return cardPdfFile;
        }

        private List<string> DeleteResourceFiles(int employerId, MemberCardWebRequest cardWebRequest)
        {
            string cardSvgFile = string.Format("{0}card_{1}_{2}.svg",
                CardFilesFolder, employerId, cardWebRequest.CardToken);

            string cardPdfFile = string.Format("{0}card_{1}_{2}.pdf",
                CardFilesFolder, employerId, cardWebRequest.CardToken);

            try
            {
                if (File.Exists(cardSvgFile))
                {
                    File.Delete(cardSvgFile);
                }
                if (File.Exists(cardPdfFile))
                {
                    File.Delete(cardPdfFile);
                }
            }
            catch
            {
                // ignored
            }
            string[] remainingFiles = Directory.GetFiles(CardFilesFolder);
            return remainingFiles.ToList();
        }
    }
}
