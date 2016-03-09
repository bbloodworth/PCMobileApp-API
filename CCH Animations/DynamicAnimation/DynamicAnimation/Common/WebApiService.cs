using System;
using System.Linq;
using System.Net.Http; 
using System.Threading.Tasks; 
using DynamicAnimation.Models; 

namespace DynamicAnimation.Common
{
    public class WebApiService {
        public static AuthorizationResponse GetAuthorizationByCchId(int employerId, int cchId) {
            var authResponse = new AuthorizationResponse();

            var requestUrl = string.Format("v1/Animation/Membership/LoginById/{0}/{1}/{2}",
                employerId, cchId, 
                "AnzovinHandshakeId".GetConfigurationValue());

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try {
                using (var client = BaseService.GetClient()) {
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode) {
                        authResponse = response.Content.ReadAsAsync<AuthorizationResponse>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null
                        ? exc.Message
                        : exc.InnerException.InnerException == null
                            ? exc.InnerException.Message
                            : exc.InnerException.InnerException.Message);
            }
            return authResponse;
        }

        public static CampaignIntroModel GetCampaignIntro(int employerId, int campaignId) {
            var campaignIntro = new CampaignIntroModel();

            var requestUrl = string.Format("v1/Animation/Campaign/Intro/{0}/{1}/{2}",
                employerId, campaignId, "AnzovinHandshakeId".GetConfigurationValue());

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try {
                using (var client = BaseService.GetClient()) {
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode) {
                        campaignIntro = response.Content.ReadAsAsync<CampaignIntroModel>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return campaignIntro ?? new CampaignIntroModel();
        }

        public static CampaignSessionModel GetCampaignSession(CampaignSessionModel campaignSession) {
            var userContent = new UserContentRecord();
            var contentResponse = new UserContentResponse();
            /// Call new p_GetCampaignIntro 
            /// and new p_GetCampaignContents
            
            var requestUrl = string.Format("v1/Animation/UserContent/UserContent/1/{0}",
                campaignSession.CampaignContentId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try {
                using (var client = BaseService.GetClient()) {
                    if (client.DefaultRequestHeaders.Contains("AuthHash")) {
                        client.DefaultRequestHeaders.Remove("AuthHash");
                    }

                    client.DefaultRequestHeaders.Add("AuthHash", campaignSession.AuthorizationHash);

                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode) {
                        contentResponse = response.Content.ReadAsAsync<UserContentResponse>().Result;
                        userContent = contentResponse.Results.FirstOrDefault();

                        if (userContent != null) {
                            campaignSession.AnimationTopic = userContent.ContentName;
                            campaignSession.JavaScriptFileName = userContent.ContentFileLocation;
                            campaignSession.BannerImageName = userContent.ContentImageFileName;
                            campaignSession.MemberContentData = userContent.MemberContentData;
                        }
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return campaignSession ?? new CampaignSessionModel();
        }

        public static AuthorizationResponse GetAuthorization(string lastName, string dateOfBirth, string lastFour) {
            var authResponse = new AuthorizationResponse();
            lastName = lastName.Replace(" ", "_");

            var request = new AuthMemberDataRequest {
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                LastFourSsn = lastFour
            };

            string requestUrl = string.Format("v1/Animation/Membership/Register1/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            try {
                using (var client = BaseService.GetClient()) {
                    var response = client.PostAsJsonAsync(requestUrl, request).Result;

                    if (response.IsSuccessStatusCode) {
                        authResponse = response.Content.ReadAsAsync<AuthorizationResponse>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return authResponse;
        }

        public static VideoCampaignModel GetVideoCampaign
            (int campaignId, int employerId) {
            var videoCampaign = new VideoCampaignModel();

            var requestUrl = string.Format("v1/PComm/VideoCampaign/{0}/{1}/{2}",
                "HandshakeId".GetConfigurationValue(),
                employerId, campaignId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            // request.Content = null;
            try {
                using (var client = BaseService.GetClient()) {
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode) {
                        videoCampaign = response.Content.ReadAsAsync<VideoCampaignModel>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error, 
                    exc.InnerException == null ? 
                    exc.Message : 
                    exc.InnerException.InnerException == null ? 
                    exc.InnerException.Message : exc.InnerException.InnerException.Message );
            }
            return videoCampaign ?? new VideoCampaignModel();
        }

        public static VideoCampaignMemberModel GetVideoCampaignMemberData 
            (string videoCampaignMemberId, int employerId) {
            var videoCampaignMember = new VideoCampaignMemberModel();

            var requestUrl = string.Format("v1/PComm/VideoCampaign/MemberInfo/{0}/{1}/{2}",
                "HandshakeId".GetConfigurationValue(),
                employerId, videoCampaignMemberId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            using (var client = BaseService.GetClient()) {
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode) {
                    videoCampaignMember = response.Content.ReadAsAsync<VideoCampaignMemberModel>().Result;
                }
            }
            return videoCampaignMember;
        }

        public static UserSessionVideoModel GetUserSessionVideoData(string videoCampaignMemberId, int employerId) {
            var videoCampaignMember = GetVideoCampaignMemberData(videoCampaignMemberId, employerId);

            var userSessionVideo = new UserSessionVideoModel {
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
                  string.IsNullOrEmpty(userSessionVideo.VideoCampaignFileId))) {
                userSessionVideo.VideoWithSignedAccessSignature =
                    AzureBlobManager.GetBlobSasUrl(userSessionVideo.PrivateContainerName,
                        userSessionVideo.VideoCampaignFileId);
            }
            return userSessionVideo;
        }

        public static async Task LogAnonEvent(ExperienceLogRequest experienceLog) {
            var requestUrl = string.Format("v1/Animation/Experience/LogAnonEvent/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            using (var client = BaseService.GetClient()) {
                var response = await client.PostAsJsonAsync(
                    requestUrl, experienceLog);
            }
        }

        public static async Task LogUserEvent(ExperienceLogRequest experienceLog, string authHash) {
            var requestUrl = string.Format("v1/Animation/Experience/LogUserEvent/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            using (var client = BaseService.GetClient()) {
                if (!client.DefaultRequestHeaders.Contains("AuthHash")) {
                    client.DefaultRequestHeaders.Add("AuthHash", authHash);
                }
                var response = await client.PostAsJsonAsync(
                    requestUrl, experienceLog);
            }
        }

        public static ExperienceLogResponse LogInitialEvent(int employerId) {
            var experienceResponse = new ExperienceLogResponse();
            var experienceLog = new ExperienceLogRequest() {
                EmployerId = employerId,
                EventName = "StartWebsite",
                LogComment = string.Format("Animations Website launched ")
            };

            var requestUrl = string.Format("v1/Animation/Experience/LogInitial/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());
            try {
                using (var client = BaseService.GetClient()) {
                    var response = client.PostAsJsonAsync(requestUrl, experienceLog).Result;
                    if (response.IsSuccessStatusCode) {
                        experienceResponse = response.Content.ReadAsAsync<ExperienceLogResponse>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
                experienceResponse.ExperienceUserId = Guid.NewGuid().ToString();
            }
            return experienceResponse;
        }

        public static async Task<MemberCardDataModel> GetCardDetail(int employerId, string token) {
            var memberData = new MemberCardDataModel();
            var requestUrl = string.Format("v1/Animation/Card/MemberData?employerId={0}&token={1}", employerId, token);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            try {
                using (var client = BaseService.GetClient()) {
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode) {
                        memberData = response.Content.ReadAsAsync<MemberCardDataModel>().Result;
                    }
                }
            }
            catch (Exception exc) {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return memberData;
        }
    }
}