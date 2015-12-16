using System.Web;
using DynamicAnimation.Common;

namespace DynamicAnimation.Models
{
    public class UserSessionVideoModel
    {
        public int EmployerId { get; set; }
        public string VideoDefinitionName { get; set; }
        public string VideoCampaignId { get; set; }
        public string IntroVideoDefinitionId { get; set; }
        public string IntroVideoDefinitionName { get; set; }
        public string VideoCampaignMemberId { get; set; }
        public string VideoCampaignFileId { get; set; }
        public bool IsVideoCampaignActive { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string MemberSsn { get; set; }
        public string FileType { get; set; }
        public string PrivateContainerName { get; set; }
        public string PrivateContainerUrl { get; set; }
        public string PublicContainerName { get; set; }
        public string PublicContainerUrl { get; set; }
        public string VideoWithSignedAccessSignature { get; set; }
        public string PosterName { get; set; }
        public string CchEmployerLink { get; set; }
        public string EmployerBenefitsLink { get; set; }

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
                    PublicContainerUrl, IntroVideoDefinitionId, signedAccessSig);
            }
        }

        public static UserSessionVideoModel Current
        {
            get
            {
                var userSessionVideo = new UserSessionVideoModel();
                userSessionVideo.EmployerId = "DefaultEmployerId".GetConfigurationNumericValue();
                userSessionVideo.VideoCampaignMemberId = "DefaultVideoCampaignMemberId".GetConfigurationValue();

                if (HttpContext.Current.Session["UserVideoSession"] != null)
                {
                    userSessionVideo = (UserSessionVideoModel)HttpContext.Current.Session["UserVideoSession"];
                }
                return userSessionVideo;
            }
            set
            {
                HttpContext.Current.Session["UserVideoSession"] = value;
            }
        }
    }
}
