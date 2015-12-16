using System;
using System.Web;
using DynamicAnimation.Common;
using Newtonsoft.Json;

namespace DynamicAnimation.Models
{
    public class CampaignSessionModel
    {
        public int CampaignId { get; set; }
        public int EmployerId { get; set; }
        public string AnimationTopic { get; set; }

        private string _introAnimationType;
        public string IntroAnimationType
        {
            get { return _introAnimationType; }
            set { _introAnimationType = value; }
        }

        private string _introAnimationName;
        public string IntroAnimationName
        {
            get
            {
                string introAnimationName = 
                    string.IsNullOrEmpty(_introAnimationName) ? 
                    string.Empty : 
                    _introAnimationName ;

                if (string.IsNullOrEmpty(_introAnimationType) && 
                    string.IsNullOrEmpty(_introAnimationName) )
                {
                    // do nothing
                }
                else
                {
                    if (!introAnimationName.Equals("NONE"))
                    {
                        if (_introAnimationType.Equals("Video"))
                        {
                            introAnimationName = _introAnimationName.Contains(".mp4")
                                ? _introAnimationName
                                : string.Format("{0}.mp4", _introAnimationName);
                        }
                    }
                }
                return introAnimationName;
            }
            set { _introAnimationName = value; }
        }

        public int IntroContentId { get; set; }
        public int ContentId { get; set; }
        public string CampaignContentId
        {
            get
            {
                return string.Format("{0}.{1}", CampaignId, ContentId);
            }
        }

        public bool IsVideoCampaignActive { get; set; }
        public string FileType { get; set; }
        public string PrivateContainerName { get; set; }
        public string PrivateContainerUrl { get; set; }
        public string PublicContainerName { get; set; }
        public string PublicContainerUrl { get; set; }
        public string VideoWithSignedAccessSignature { get; set; }
        public string PosterName { get; set; }
        public string JavaScriptFileName { get; set; }
        public string BannerImageName { get; set; }
        public string ExperienceUserId { get; set; }

        public int CchId { get; set; }
        public string LastName { get; set; }

        private string _dateOfBirth;
        public string DateOfBirth
        {
            get
            {
                string birth = _dateOfBirth;
                DateTime dBirth;
                if (DateTime.TryParse(_dateOfBirth, out dBirth))
                {
                    birth = string.Format("{0}-{1:00}-{2:00}", dBirth.Year, dBirth.Month, dBirth.Day);
                }
                return birth;
            }
            set { _dateOfBirth = value; }
        }

        public string LastFour { get; set; }
        public string AuthorizationHash { get; set; }
        private string _memberContentData;
        public string MemberContentData {
            get { return string.IsNullOrEmpty(_memberContentData) ? string.Empty : _memberContentData; }
            set { _memberContentData = value; } 
        }

        public string CchLink
        {
            get
            {
                string cchLink = string.Empty;
                if (!string.IsNullOrEmpty(_memberContentData))
                {
                    BenefitPlan1210BareModel benefitPlan =
                        JsonConvert.DeserializeObject<BenefitPlan1210BareModel>(_memberContentData);
                    cchLink = benefitPlan.CCHLink;
                }
                return cchLink;
            }
        }

        public string BenefitsLink
        {
            get
            {
                string benefitsLink = string.Empty;
                if (!string.IsNullOrEmpty(_memberContentData))
                {
                    BenefitPlan1210BareModel benefitPlan =
                        JsonConvert.DeserializeObject<BenefitPlan1210BareModel>(_memberContentData);
                    benefitsLink = benefitPlan.BenefitsLink;
                }
                return benefitsLink;
            }
        }
        public bool HasAnsweredSurvey;

        public string PublicPosterUrl
        {
            get
            {
                string signedAccessSig = string.Empty;
                return string.Format("{0}{1}{2}",
                    PublicContainerUrl, PosterName, signedAccessSig);
            }
        }

        public string PublicIntroVideoUrl
        {
            get
            {
                string signedAccessSig = string.Empty;
                return string.Format("{0}{1}{2}",
                    PublicContainerUrl, IntroAnimationName, signedAccessSig);
            }
        }

        public string JavaScriptFilePath
        {
            get { 
                return string.Format("{0}{1}", 
                    "SourcePath".GetConfigurationValue(), JavaScriptFileName); 
            }
        }

        public string BannerImagePath
        {
            get
            {
                return string.Format("{0}{1}",
                    "SourcePath".GetConfigurationValue(), BannerImageName);
            }
        }

        public string AnonEventLogUrl
        {
            get
            {
                string eventLogUrl = "APIBaseAddress".GetConfigurationValue();
                eventLogUrl = string.Format("{0}/v1/Animation/Experience/LogAnonEvent/{1}",
                    eventLogUrl, "AnzovinHandshakeId".GetConfigurationValue());
                return eventLogUrl;
            }
        }

        public string UserEventLogUrl
        {
            get
            {
                string eventLogUrl = "APIBaseAddress".GetConfigurationValue();
                eventLogUrl = string.Format("{0}/v1/Animation/Experience/LogUserEvent/{1}",
                    eventLogUrl, "AnzovinHandshakeId".GetConfigurationValue());
                return eventLogUrl;
            }
        }

        public static CampaignSessionModel Current
        {
            get
            {
                var campaignSession = new CampaignSessionModel
                {
                    EmployerId = "DefaultEmployerId".GetConfigurationNumericValue(),
                    FileType = "mp4",
                    PublicContainerName = "PublicStorageContainer".GetConfigurationValue(),
                    PublicContainerUrl = "PublicStorageContainerUrl".GetConfigurationValue(),
                    PrivateContainerName = "PrivateStorageContainer".GetConfigurationValue(),
                    PrivateContainerUrl = "PrivateStorageContainerUrl".GetConfigurationValue(),
                    PosterName = "PosterName".GetConfigurationValue()
                };

                if (HttpContext.Current.Session["CampaignSession"] != null)
                {
                    campaignSession = (CampaignSessionModel)HttpContext.Current.Session["CampaignSession"];
                }
                return campaignSession;
            }
            set
            {
                HttpContext.Current.Session["CampaignSession"] = value;
            }
        }
    }
}