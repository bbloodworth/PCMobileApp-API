using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DynamicAnimation.Models;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace DynamicAnimation.Common
{
    public class WebApiService
    {
        public static AuthorizationResponse GetAuthorizationByCchId(int employerId, int cchId)
        {
            AuthorizationResponse authResponse = new AuthorizationResponse();

            string requestUrl = string.Format("v1/Animation/Membership/LoginById/{0}/{1}/{2}",
                employerId, cchId, 
                "AnzovinHandshakeId".GetConfigurationValue());

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try
            {
                HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    authResponse = response.Content.ReadAsAsync<AuthorizationResponse>().Result;
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null
                        ? exc.Message
                        : exc.InnerException.InnerException == null
                            ? exc.InnerException.Message
                            : exc.InnerException.InnerException.Message);
            }
            return authResponse;
        }

        public static CampaignIntroModel GetCampaignIntro(int employerId, int campaignId)
        {
            var campaignIntro = new CampaignIntroModel();

            string requestUrl = string.Format("v1/Animation/Campaign/Intro/{0}/{1}/{2}",
                employerId, campaignId, "AnzovinHandshakeId".GetConfigurationValue());

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try
            {
                HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    campaignIntro = response.Content.ReadAsAsync<CampaignIntroModel>().Result;
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return campaignIntro ?? new CampaignIntroModel();
        }

        public static CampaignSessionModel GetCampaignSession(CampaignSessionModel campaignSession)
        {
            UserContentRecord userContent = new UserContentRecord();
            UserContentResponse contentResponse = new UserContentResponse();
            /// Call new p_GetCampaignIntro 
            /// and new p_GetCampaignContents
            
            string requestUrl = string.Format("v1/Animation/UserContent/UserContent/1/{0}",
                campaignSession.CampaignContentId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try
            {
                if (BaseService.Client.DefaultRequestHeaders.Contains("AuthHash"))
                {
                    BaseService.Client.DefaultRequestHeaders.Remove("AuthHash");
                }
                BaseService.Client.DefaultRequestHeaders.Add("AuthHash", campaignSession.AuthorizationHash);

                HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    //campaignIntro = response.Content.ReadAsAsync<CampaignIntroModel>().Result;
                    //campaignSession.IntroAnimationName = campaignIntro.ContentName;
                    contentResponse = response.Content.ReadAsAsync<UserContentResponse>().Result;
                    userContent = contentResponse.Results.FirstOrDefault();

                    if (userContent != null)
                    {
                        campaignSession.AnimationTopic = userContent.ContentName;
                        campaignSession.JavaScriptFileName = userContent.ContentFileLocation;
                        campaignSession.BannerImageName = userContent.ContentImageFileName;
                        campaignSession.MemberContentData = userContent.MemberContentData;
                    }
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return campaignSession ?? new CampaignSessionModel();
        }

        public static AuthorizationResponse GetAuthorization(string lastName, string dateOfBirth, string lastFour)
        {
            AuthorizationResponse authResponse = new AuthorizationResponse();
            lastName = lastName.Replace(" ", "_");

            // var campaignSession = CampaignSessionModel.Current;
            AuthMemberDataRequest request = new AuthMemberDataRequest
            {
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                LastFourSsn = lastFour
            };

            string requestUrl = string.Format("v1/Animation/Membership/Register1/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            try
            {
                HttpResponseMessage response = BaseService.Client.PostAsJsonAsync(requestUrl, request).Result;

                if (response.IsSuccessStatusCode)
                {
                    authResponse = response.Content.ReadAsAsync<AuthorizationResponse>().Result;
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
            }
            return authResponse;


            //string requestUrl = string.Format("v1/Animation/Campaign/Intro/{0}/{1}/{2}",
            //    employerId, campaignId, "HandshakeId".GetConfigurationValue());

            //var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", email, password))));

            //try
            //{
            //    HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

            //    if (response.IsSuccessStatusCode)
            //    {
            //        campaignIntro = response.Content.ReadAsAsync<CampaignIntroModel>().Result;
            //        AuthorizationResponse authResponse = response.Content.ReadAsAsync<AuthorizationResponse>().Result;
            //        UserContext.Initialize(email, authResponse);
            //        isSuccess = true;
            //    }
            //}
            //catch (Exception exc)
            //{
            //    HelperService.LogAnonEvent(ExperienceEvents.Error,
            //        exc.InnerException == null ?
            //        exc.Message :
            //        exc.InnerException.InnerException == null ?
            //        exc.InnerException.Message : exc.InnerException.InnerException.Message);
            //}
        }

        public static VideoCampaignModel GetVideoCampaign
            (int campaignId, int employerId)
        {
            var videoCampaign = new VideoCampaignModel();

            string requestUrl = string.Format("v1/PComm/VideoCampaign/{0}/{1}/{2}",
                "HandshakeId".GetConfigurationValue(),
                employerId, campaignId);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            // request.Content = null;
            try
            {
                HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    videoCampaign = response.Content.ReadAsAsync<VideoCampaignModel>().Result;
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error, 
                    exc.InnerException == null ? 
                    exc.Message : 
                    exc.InnerException.InnerException == null ? 
                    exc.InnerException.Message : exc.InnerException.InnerException.Message );
            }
            return videoCampaign ?? new VideoCampaignModel();
        }

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

        public static async Task LogAnonEvent(ExperienceLogRequest experienceLog)
        {
            string requestUrl = string.Format("v1/Animation/Experience/LogAnonEvent/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            HttpResponseMessage response = await BaseService.Client.PostAsJsonAsync(
                requestUrl, experienceLog);

            if (response.IsSuccessStatusCode)
            {
            }
        }

        public static async Task LogUserEvent(ExperienceLogRequest experienceLog, string authHash)
        {
            string requestUrl = string.Format("v1/Animation/Experience/LogUserEvent/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());

            if (!BaseService.Client.DefaultRequestHeaders.Contains("AuthHash"))
            {
                BaseService.Client.DefaultRequestHeaders.Add("AuthHash", authHash);
            }
            HttpResponseMessage response = await BaseService.Client.PostAsJsonAsync(
                requestUrl, experienceLog);

            if (response.IsSuccessStatusCode)
            {
            }
        }

        public static ExperienceLogResponse LogInitialEvent(int employerId)
        {
            ExperienceLogResponse experienceResponse = new ExperienceLogResponse();
            ExperienceLogRequest experienceLog = new ExperienceLogRequest()
            {
                EmployerId = employerId,
                EventName = "StartWebsite",
                LogComment = string.Format("Animations Website launched ")
            };

            string requestUrl = string.Format("v1/Animation/Experience/LogInitial/{0}",
                "AnzovinHandshakeId".GetConfigurationValue());
            try
            {
                var response = BaseService.Client.PostAsJsonAsync(requestUrl, experienceLog).Result;
                if (response.IsSuccessStatusCode)
                {
                    experienceResponse = response.Content.ReadAsAsync<ExperienceLogResponse>().Result;
                }
            }
            catch (Exception exc)
            {
                HelperService.LogAnonEvent(ExperienceEvents.Error,
                    exc.InnerException == null ?
                    exc.Message :
                    exc.InnerException.InnerException == null ?
                    exc.InnerException.Message : exc.InnerException.InnerException.Message);
                experienceResponse.ExperienceUserId = Guid.NewGuid().ToString();
            }
            return experienceResponse;
        }

        public static async Task<MemberCardDataModel> GetMemberCardData(int employerId, string token)
        {
            MemberCardDataModel memberData = new MemberCardDataModel();

            string requestUrl = string.Format("v1/Animation/Card/MemberData/{0}/{1}",
                employerId, token);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            try
            {
                //HttpResponseMessage response = BaseService.Client.SendAsync(request).Result;

                ////throw new Exception("Error Test in GetMemberCardData");

                //if (response.IsSuccessStatusCode)
                //{
                //    memberData = response.Content.ReadAsAsync<MemberCardDataModel>().Result;
                //}

                await BaseService.Client.SendAsync(request).ContinueWith(t =>
                {
                    HttpResponseMessage response = t.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        memberData = response.Content.ReadAsAsync<MemberCardDataModel>().Result;
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception exc)
            {
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