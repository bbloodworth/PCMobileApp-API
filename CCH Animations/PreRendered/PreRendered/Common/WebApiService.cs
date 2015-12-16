using System.Net.Http;
using System.Threading.Tasks;
using PreRendered.Models;

namespace PreRendered.Common
{
    public class WebApiService
    {
        public static VideoCampaignMemberModel GetVideoCampaignMemberData
            (string videoCampaignMemberId, int employerId)
        {
            var videoCampaignMember = new VideoCampaignMemberModel();

            string requestUrl = string.Format("v1/PComm/VideoCampaign/MemberInfo/{0}/{1}/{2}",
                "HandshakeId".GetConfigurationValue(),
                employerId, videoCampaignMemberId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                videoCampaignMember = response.Content.ReadAsAsync<VideoCampaignMemberModel>().Result;
            }
            //videoCampaignMember.CchEmployerLink = "https://www.clearcosthealth.com/RBC";
            //videoCampaignMember.DateOfBirth = "09/18/1986";
            //videoCampaignMember.EmployerBenefitsLink = "https://iservices.rbc.com/iservices/logwindow.asp";
            //videoCampaignMember.IntroVideoDefinitionId = "IntroductoryVideoFileId".GetConfigurationValue();
            //videoCampaignMember.IntroVideoDefinitionName = "SampleIntro";
            //videoCampaignMember.VideoCampaignFileId = "CatMassage.mp4";
            //videoCampaignMember.VideoCampaignFileId = "3E66EB7E-3B53-47D8-A734-7A455BC30B00.mp4";
            //videoCampaignMember.IsVideoCampaignActive = 1;
            //videoCampaignMember.LastName = "Smith";
            //videoCampaignMember.MemberSsn = "6543";
            //videoCampaignMember.VideoCampaignId = "1";
            //videoCampaignMember.VideoDefinitionName = "BenefitsExplainer";

            return videoCampaignMember;
        }

        public static UserSessionVideoModel GetUserSessionVideoData(string videoCampaignMemberId, int employerId)
        {
            VideoCampaignMemberModel videoCampaignMember = 
                GetVideoCampaignMemberData(videoCampaignMemberId, employerId);

            var userSessionVideo = new UserSessionVideoModel
            {
                VideoCampaignMemberId = videoCampaignMemberId,
                EmployerId = employerId,
                CchEmployerLink = videoCampaignMember.CchEmployerLink,
                DateOfBirth = videoCampaignMember.DateOfBirth,
                EmployerBenefitsLink = videoCampaignMember.EmployerBenefitsLink,
                IntroVideoDefinitionId = 
                    string.IsNullOrEmpty(videoCampaignMember.IntroVideoDefinitionId) || 
                    videoCampaignMember.IntroVideoDefinitionId.Contains(".mp4") ? 
                    videoCampaignMember.IntroVideoDefinitionId : 
                    string.Format("{0}.mp4", videoCampaignMember.IntroVideoDefinitionId),
                IntroVideoDefinitionName = videoCampaignMember.IntroVideoDefinitionName,
                VideoCampaignFileId =
                    string.IsNullOrEmpty(videoCampaignMember.VideoCampaignFileId) ||
                    videoCampaignMember.VideoCampaignFileId.Contains(".mp4") ?
                    videoCampaignMember.VideoCampaignFileId :
                    string.Format("{0}.mp4", videoCampaignMember.VideoCampaignFileId),
                IsVideoCampaignActive = videoCampaignMember.IsVideoCampaignActive,
                LastName = videoCampaignMember.LastName,
                MemberSsn = videoCampaignMember.MemberSsn,
                FileType = "mp4",
                PublicContainerName = "PublicStorageContainer".GetConfigurationValue(),
                PublicContainerUrl = "PublicStorageContainerUrl".GetConfigurationValue(),
                PrivateContainerName = "PrivateStorageContainer".GetConfigurationValue(),
                PrivateContainerUrl = "PrivateStorageContainerUrl".GetConfigurationValue(),
                PosterName = "PosterName".GetConfigurationValue(),
                VideoCampaignId = videoCampaignMember.VideoCampaignId,
                VideoDefinitionName = videoCampaignMember.VideoDefinitionName
            };

            if (!(string.IsNullOrEmpty(userSessionVideo.PrivateContainerName) ||
                  string.IsNullOrEmpty(userSessionVideo.VideoCampaignFileId)))
            {
                userSessionVideo.VideoWithSignedAccessSignature =
                    AzureBlobManager.GetBlobSasUrl(userSessionVideo.PrivateContainerName,
                        userSessionVideo.VideoCampaignFileId);
            }
            return userSessionVideo;
        }

        public static async Task LogAnEvent(VideoEventLogModel videoEventLog)
        {
            string requestUrl = string.Format("v1/PComm/VideoEvent/Log/{0}", 
                "HandshakeId".GetConfigurationValue());

            HttpResponseMessage response = await BaseService.Client.PostAsJsonAsync(
                requestUrl, videoEventLog);

            if (response.IsSuccessStatusCode)
            {
            }
        }
    }
}