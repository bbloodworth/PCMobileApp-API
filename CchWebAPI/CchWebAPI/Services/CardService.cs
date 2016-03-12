using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.IO;

using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Support;
using ClearCost.Data;
using ClearCost.Data.Security;
using ClearCost.IO.Log;
using ClearCost.Security.JWT;
using Newtonsoft.Json;
using NLog;
using System.Data.Entity;

namespace CchWebAPI.Services {
    public class CardService {
        //TODO: Make these configurable
        private readonly static string _cardFilesFolder = "C:\\inetpub\\Resources\\";
        private readonly static string _phantomJsExePath = "C:\\inetpub\\Resources\\phantomjs.exe";

        #region Locales
        private static Dictionary<string, bool> _supportedLocales;
        private static Dictionary<string, bool> SupportedLocales {
            get {
                if (_supportedLocales == null) {
                    _supportedLocales = new Dictionary<string, bool>();
                    _supportedLocales.Add("en-us", true);
                    _supportedLocales.Add("es-us", false);
                }

                return _supportedLocales;
            }
        }

        public string DefaultLocale {
            get {
                return SupportedLocales.FirstOrDefault(l => l.Value.Equals(true)).Key;
            }
        }

        public string ResolveLocale(string localeCode) {
            var result = SupportedLocales.FirstOrDefault(l => l.Key.Contains(localeCode)).Key;

            if (string.IsNullOrEmpty(result))
                result = DefaultLocale;

            return result;
        }
        #endregion

        #region Business Logic
        public CardUrlResult GetMemberCardUrls(string localeCode, int employerId, int cchId) {
            var message = string.Empty;
            CardUrlResult result = null;

            var employer = PlatformDataCache.Employers.FirstOrDefault(e => e.Id.Equals(employerId));

            if (employer == null) {
                message = string.Format("Card url request with employer {0} does not match any known employer",
                    employerId);
                LogUtil.Log(message, LogLevel.Warn);
                return result;
            }

            var resolvedLocaleCode = ResolveLocale(localeCode);
            var cardResults = GetCardResults(cchId, employer, resolvedLocaleCode);

            if (cardResults == null || cardResults.Count() == 0) {
                message = string.Format("Unable to resolve member id cards for cchId {0} and localCode {1}",
                    cchId, localeCode);
                LogUtil.Log(message, LogLevel.Info);
            }
            else {
                LogUtil.Trace(string.Format("Resolved member id cards {0} for cchId {0} and localCode {1}",
                    cchId, localeCode));
                result = BuildResult(employer, cardResults);
            }

            return result;
        }

        private static CardUrlResult BuildResult(Employer employer, List<dynamic> cardResults) {
            var result = new CardUrlResult();
            result.Results = new List<CardResult>();
            var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();

            cardResults.ForEach(cr => {
                var cardToken = new CardToken() {
                    EmployerId = employer.Id,
                    CardDetail = JsonConvert.DeserializeObject<CardDetail>(cr.MemberCard.CardMemberDataText),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt16("TimeoutInMinutes".GetConfigurationValue()))
                };

                cardToken.CardDetail.CardTypeFileName = cr.CardType.FileName;
                cardToken.CardDetail.CardTypeId = cr.MemberCard.CardTypeId;
                cardToken.CardDetail.CardViewModeId = cr.CardViewMode.Id;

                var jwt = JwtService.EncryptPayload(JsonConvert.SerializeObject(cardToken));

                var cardResult = new CardResult() {
                    CardName = cr.CardTypeTranslation.CardTypeName,
                    ViewMode = cr.CardViewMode.Name,
                    CardUrl = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, employer.Id, jwt),
                    SecurityToken = jwt
                };

                result.Results.Add(cardResult);
            });

            return result;
        }

        public CardDetail GetCardDetail(int expectedEmployerId, string token) {

            var cardToken = JsonConvert.DeserializeObject<CardToken>(JwtService.DecryptPayload(token));
            if (cardToken.Expires < DateTime.UtcNow) {
                LogUtil.Log(string.Format("Expired session token {0}", token), LogLevel.Info);
                return new CardDetail() { Expired = true };
            }

            if (!cardToken.EmployerId.Equals(expectedEmployerId)) {
                LogUtil.Log(string.Format("Expected employer {0} and got employer {1} from token {2}.",
                    expectedEmployerId, cardToken.EmployerId, token), LogLevel.Warn);
                return new CardDetail() { Invalid = true };
            }

            LogUtil.Trace(string.Format("Resolved token {0} for employer {1}.",
                token, cardToken.EmployerId));

            return cardToken.CardDetail;
        }

        public Tuple<bool, dynamic, string> SendIdCardEmail(int employerId, MemberCardWebRequest cardWebRequest) {
            var isSuccess = false;
            dynamic data = new ExpandoObject();
            var errorMessage = string.Empty;

            try {
                var fileId = Guid.NewGuid();
                // Retrieve the web request stream from the Media website and convert it into an SVG file
                data.SvgSuccess = GetCardSvg(employerId, cardWebRequest, fileId);

                // Convert the SVG file into a PDF file
                var cardPdfFile = RenderCardPdf(employerId, fileId);

                var subject = string.IsNullOrEmpty(cardWebRequest.Subject)
                    ? "Member ID Card"
                    : cardWebRequest.Subject;

                var message = string.IsNullOrEmpty(cardWebRequest.Message) ?
                    "Please see the attached PDF to view or print my ID card." :
                    cardWebRequest.Message;

                var useInternalServer = "Email.UseInternalServer".GetConfigurationValue().Equals("true");

                // Send PDF file as an email attachment to designated recipient
                EmailMessenger.Send(to: cardWebRequest.ToEmail, cc: cardWebRequest.CcEmail,
                    subject: subject, message: message,
                    isHtml: false, attachmentPath: cardPdfFile, isInternalServer: useInternalServer);

                isSuccess = true;
            }
            catch (Exception exc) {
                errorMessage = exc.Message;
            }

            return new Tuple<bool, dynamic, string>(isSuccess, data, errorMessage);
        }

        private bool GetCardSvg(int employerId, MemberCardWebRequest cardWebRequest, Guid fileId) {
            try {
                var webClient = new WebClient();
                var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();
                var url = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, employerId, cardWebRequest.CardToken);

                var cardSvgFile = string.Format("{0}card_{1}_{2}.svg",
                    _cardFilesFolder, employerId, fileId);

                webClient.DownloadFile(url, cardSvgFile);
                return true;
            }
            catch (Exception ex) {
                LogUtil.Log("Failure in CardService.GetMemberCardWebRequest", ex, Guid.NewGuid(), "CchWebAPI.Services.CardService");
                return false;
            }
        }

        private string RenderCardPdf(int employerId, Guid fileId) {
            // Send PDF file as an email attachment to designated recipient
            var cardPdfFile = string.Format("{0}card_{1}_{2}.pdf",
                _cardFilesFolder, employerId, fileId);

            try {
                // Convert the SVG file into a PNG file
                var phantomJsArgs = string.Format("rasterize.js card_{0}_{1}.svg card_{0}_{1}.pdf",
                    employerId, fileId);

                using (var process = new Process()) {
                    process.StartInfo.WorkingDirectory = _cardFilesFolder;
                    process.StartInfo.FileName = _phantomJsExePath;
                    process.StartInfo.Arguments = phantomJsArgs;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    if (process != null)
                        process.WaitForExit();
                }

                return cardPdfFile;
            }
            catch (Exception ex) {
                LogUtil.Log("Failure in CardService.GetMemberCardPdfFile", ex, Guid.NewGuid(), "CchWebAPI.Services.CardService");
                return string.Empty;
            }
        }
        #endregion 

        #region Data Access
        private static List<dynamic> GetCardResults(int cchId, Employer employer, string resolvedLocaleCode) {
            using (var ctx = new CardContext(employer)) {
                return ctx.MemberCards
                    .Join(
                        ctx.CardTypes,
                        mc => mc.CardTypeId,
                        ct => ct.Id,
                        (mc, ct) => new {
                            MemberCard = mc,
                            CardType = ct
                        }
                    )
                    .Join(
                        ctx.CardTypeTranslations,
                            mc => new { mc.MemberCard.CardTypeId, mc.MemberCard.LocaleId },
                            ctt => new { ctt.CardTypeId, ctt.LocaleId },
                        (mc, ctt) => new {
                            MemberCard = mc.MemberCard,
                            CardType = mc.CardType,
                            CardTypeTranslation = ctt
                        })
                    .Join(
                        ctx.CardViewModes,
                        mc => mc.MemberCard.CardViewModeId,
                        cvm => cvm.Id,
                        (mc, cvm) => new {
                            MemberCard = mc.MemberCard,
                            CardType = mc.CardType,
                            CardTypeTranslation = mc.CardTypeTranslation,
                            CardViewMode = cvm
                        }
                    )
                    .Join(
                        ctx.Locales,
                        mc => mc.MemberCard.LocaleId,
                        l => l.Id,
                        (mc, l) => new {
                            MemberCard = mc.MemberCard,
                            CardType = mc.CardType,
                            CardTypeTranslation = mc.CardTypeTranslation,
                            CardViewMode = mc.CardViewMode,
                            Locale = l
                        }
                    )
                    .Where(r => r.MemberCard.CchId.Equals(cchId)
                        && r.Locale.LocaleCode.Equals(resolvedLocaleCode)).ToList<dynamic>();
            }
        }

        internal class CardContext : ClearCostContext<CardContext> {
            public CardContext(Employer employer) : base(ConnectionFactory.Get(employer.ConnectionString)) {
                Employer = employer;
            }

            public override void ConfigureModel(DbModelBuilder builder) {
                builder.Configurations.Add(new CardType.CardTypeConfiguration());
                builder.Configurations.Add(new CardTypeTranslation.CardTypeTranslationConfiguration());
                builder.Configurations.Add(new CardViewMode.CardViewModeConfiguration());
                builder.Configurations.Add(new Locale.LocaleConfiguration());
                builder.Configurations.Add(new MemberCard.MemberCardConfiguration());
            }

            public Employer Employer { get; private set; }

            public DbSet<CardType> CardTypes { get; set; }
            public DbSet<CardTypeTranslation> CardTypeTranslations { get; set; }
            public DbSet<CardViewMode> CardViewModes { get; set; }
            public DbSet<Locale> Locales { get; set; }
            public DbSet<MemberCard> MemberCards { get; set; }
        }


        public class CardType {
            public int Id { get; set; }
            public string FileName { get; set; }

            internal class CardTypeConfiguration : EntityTypeConfiguration<CardType> {
                public CardTypeConfiguration() {
                    ToTable("CardType");
                    HasKey(u => u.Id);
                    Property(p => p.Id).HasColumnName("CardTypeID");
                    Property(p => p.FileName).HasColumnName("CardTypeFileName");
                }
            }
        }

        public class CardTypeTranslation {
            public int CardTypeId { get; set; }
            public int LocaleId { get; set; }
            public string CardTypeName { get; set; }

            internal class CardTypeTranslationConfiguration : EntityTypeConfiguration<CardTypeTranslation> {
                public CardTypeTranslationConfiguration() {
                    ToTable("CardTypeTranslation");
                    HasKey(p => new { p.CardTypeId, p.LocaleId });
                }
            }
        }

        public class CardViewMode {
            public int Id { get; set; }
            public string Name { get; set; }

            internal class CardViewModeConfiguration : EntityTypeConfiguration<CardViewMode> {
                public CardViewModeConfiguration() {
                    ToTable("CardViewMode");
                    HasKey(p => p.Id);
                    Property(p => p.Id).HasColumnName("CardViewModeID");
                    Property(p => p.Name).HasColumnName("CardViewModeName");
                }
            }
        }

        public class Locale {
            public int Id { get; set; }
            public string LocaleCode { get; set; }
            public string LanguageCode { get; set; }

            internal class LocaleConfiguration : EntityTypeConfiguration<Locale> {
                public LocaleConfiguration() {
                    ToTable("Locale");
                    HasKey(p => p.Id);
                    Property(p => p.Id).HasColumnName("LocaleID");
                    Property(p => p.LanguageCode).HasColumnName("ISOLanguageCode");
                }
            }
        }

        public class MemberCard {
            public int CchId { get; set; }
            public int CardTypeId { get; set; }
            public int LocaleId { get; set; }
            public int CardViewModeId { get; set; }
            public string CardMemberDataText { get; set; }

            internal class MemberCardConfiguration : EntityTypeConfiguration<MemberCard> {
                public MemberCardConfiguration() {
                    ToTable("MemberIDCard");
                    HasKey(p => new { p.CchId, p.CardTypeId, p.LocaleId, p.CardViewModeId });
                }
            }
        }

        #endregion
    }
}