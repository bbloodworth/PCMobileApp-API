using CchWebAPI.Areas.Animation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using CchWebAPI.Support;
using System.IO; 

namespace CchWebAPI.Services {
    public class CardService {
        //TODO: Make these configurable
        private readonly static string _cardFilesFolder = "C:\\inetpub\\Resources\\";
        private readonly static string _inkScapeExePath = "C:\\Program Files\\Inkscape\\inkscape.exe";

        public Tuple<bool, dynamic> GetMemberCardUrls(string localeCode, int employerId, int cchId) {
            var isSuccess = false;
            dynamic data = new ExpandoObject();

            using (GetEmployerConnString gecs = new GetEmployerConnString(employerId)) {
                using (GetMemberCardTokens gmct = new GetMemberCardTokens()) {
                    gmct.CchId = cchId;
                    gmct.LocaleCode = localeCode;
                    gmct.EmployerId = employerId;
                    gmct.GetData(gecs.ConnString);

                    data.Results = gmct.MemberTokens;
                    data.TotalCount = gmct.TotalCount;
                }
            }
            if (data.TotalCount > 0) {
                isSuccess = true;
            }

            return new Tuple<bool, dynamic>(isSuccess, data);
        }

        public Tuple<bool, dynamic, string> SendIdCardEmail(int employerId, MemberCardWebRequest cardWebRequest) {
            var isSuccess = false;
            dynamic data = new ExpandoObject();
            var errorMessage = string.Empty;

            try {
                // Retrieve the web request stream from the Media website and convert it into an SVG file
                data.SvgSuccess = GetMemberCardWebRequest(employerId, cardWebRequest);

                // Convert the SVG file into a PNG file
                string cardPdfFile = GetMemberCardPdfFile(employerId, cardWebRequest);

                string subject = string.IsNullOrEmpty(cardWebRequest.Subject)
                    ? "Member ID Card"
                    : cardWebRequest.Subject;

                var message = string.IsNullOrEmpty(cardWebRequest.Message) ?
                    "Please see the attached PDF to view or print my ID card." :
                    cardWebRequest.Message;

                bool useInternalServer = "Email.UseInternalServer".GetConfigurationValue().Equals("true");

                // Send PDF file as an email attachment to designated recipient
                EmailMessenger.Send(to: cardWebRequest.ToEmail, cc: cardWebRequest.CcEmail,
                    subject: subject, message: message,
                    isHtml: false, attachmentPath: cardPdfFile, isInternalServer: useInternalServer);

                bool deleteWorkFiles = "Email.DeleteWorkFiles".GetConfigurationValue().Equals("true");
                if (deleteWorkFiles) {
                    data.ResidualFiles = DeleteResourceFiles(employerId, cardWebRequest);
                }

                isSuccess = true;
            }
            catch (Exception exc) {
                errorMessage = exc.Message; 
            }

            return new Tuple<bool, dynamic, string>(isSuccess, data, errorMessage);
        }

        private bool GetMemberCardWebRequest(int employerId, MemberCardWebRequest cardWebRequest) {
            try {
                var webClient = new WebClient();
                var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();
                var url = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, employerId, cardWebRequest.CardToken);

                var cardSvgFile = string.Format("{0}card_{1}_{2}.svg",
                    _cardFilesFolder, employerId, cardWebRequest.CardToken);

                webClient.DownloadFile(url, cardSvgFile);
                return true;
            }
            catch (Exception ex) {
                //TODO: Log error?
                return false;
            }
        }

        private string GetMemberCardPdfFile(int employerId, MemberCardWebRequest cardWebRequest) {
            try {
                // Convert the SVG file into a PNG file
                var inkscapeArguments = string.Format("--file={0}card_{1}_{2}.svg --export-pdf={0}card_{1}_{2}.pdf",
                    _cardFilesFolder, employerId, cardWebRequest.CardToken);

                using (var process = new Process()){
                    process.StartInfo.FileName = _inkScapeExePath;
                    process.StartInfo.Arguments = inkscapeArguments;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    if (process != null)
                        process.WaitForExit();
                }

                // Send PDF file as an email attachment to designated recipient
                var cardPdfFile = string.Format("{0}card_{1}_{2}.pdf",
                    _cardFilesFolder, employerId, cardWebRequest.CardToken);

                return cardPdfFile;
            }
            catch (Exception ex) {
                //TODO: Log Exception
                return string.Empty;
            }
        }

        private List<string> DeleteResourceFiles(int employerId, MemberCardWebRequest cardWebRequest) {
            string cardSvgFile = string.Format("{0}card_{1}_{2}.svg",
                _cardFilesFolder, employerId, cardWebRequest.CardToken);

            string cardPdfFile = string.Format("{0}card_{1}_{2}.pdf",
                _cardFilesFolder, employerId, cardWebRequest.CardToken);

            try {
                if (File.Exists(cardSvgFile)) {
                    File.Delete(cardSvgFile);
                }
                if (File.Exists(cardPdfFile)) {
                    File.Delete(cardPdfFile);
                }
            }
            catch (Exception ex){
                // ignored
                Debug.WriteLine(ex.Message);
            }
            string[] remainingFiles = Directory.GetFiles(_cardFilesFolder);
            return remainingFiles.ToList();
        }
    }
}