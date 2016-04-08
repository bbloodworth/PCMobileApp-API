using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using CchWebAPI.Handlers;

namespace CchWebAPI
{
    public static class WebApiConfig
    {
        #region Delegating Handlers

        //Setup private arrays of handlers in heirarchal order for easier management
        private static DelegatingHandler[] AuthenticatingHandlers
        {
            get
            {
                return new DelegatingHandler[] { 
                    new CORSHandler(),
                    new ApiKeyHandler(),
                    new BasicAuthenticationMessageHandler()
                };
            }
        }
        private static DelegatingHandler[] PartnerAuthenticatingHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    new CORSHandler(),
                    new ApiKeyHandler()
                };
            }
        }
        private static DelegatingHandler[] AuthenticatedHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    //new CORSHandler(),
                    new ApiKeyHandler(),
                    new HashMessageHandler(),
                    new TimeoutHandler()
                };
            }
        }
        private static DelegatingHandler[] AccountHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    new CORSHandler(),
                    new ApiKeyHandler(),
                    new LogRequestAndResponseHandler()
                };
            }
        }
        private static DelegatingHandler[] AuthenticatedAccountHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    new CORSHandler(),
                    new ApiKeyHandler(),
                    new HashMessageHandler(), 
                    new LogRequestAndResponseHandler()
                };
            }
        }
        #endregion

        public static void Register(HttpConfiguration config)
        {
            #region Old PComm API Routes

            config.Routes.MapHttpRoute(
                name: "LogVideoEvent",
                routeTemplate: "v1/{area}/{controller}/Log/{hsId}",
                defaults: new
                {
                    hsId = "",
                    action = "LogAnEvent",
                    controller = "VideoEvent",
                    VideoLogId = "0",
                    VideoLogEvent = ""
                },
                constraints: new { area = "PComm", controller = "VideoEvent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "VideoCampaign",
                routeTemplate: "v1/{area}/{controller}/{hsId}/{employerId}/{campaignId}",
                defaults: new
                {
                    hsId = "",
                    action = "GetVideoCampaign",
                    controller = "VideoCampaign",
                    employerId = 0,
                    campaignId = 0
                },
                constraints: new { area = "PComm", controller = "VideoCampaign" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "CampaignMemberInfo",
                routeTemplate: "v1/{area}/{controller}/MemberInfo/{hsId}/{employerId}/{campaignMemberId}",
                defaults: new
                {
                    hsId = "",
                    action = "GetCampaignMemberInfo",
                    controller = "VideoCampaign",
                    employerId = 0,
                    campaignMemberId = ""
                },
                constraints: new { area = "PComm", controller = "VideoCampaign" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "CampaignVideoFileNames",
                routeTemplate: "v1/{area}/{controller}/FilesList/{hsId}/{employerId}/{campaignId}",
                defaults: new
                {
                    hsId = "",
                    action = "GetVideoFilesList",
                    controller = "VideoService",
                    employerId = 0,
                    campaignId = 0
                },
                constraints: new { area = "PComm", controller = "VideoService" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "CampaignVideoMemberData",
                routeTemplate: "v1/{area}/{controller}/MemberData/{hsId}/{employerId}/{videoMemberId}",
                defaults: new
                {
                    hsId = "",
                    action = "GetVideoMemberData",
                    controller = "VideoService",
                    employerId = 0,
                    campaignId = ""
                },
                constraints: new { area = "PComm", controller = "VideoService" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "VideoAuthenticateMemberData",
                routeTemplate: "v1/{area}/{controller}/MemberData/{hsId}",
                defaults: new { action = "GetAuthMemberData", hsID = "" },
                constraints: new { area = "PComm", controller = "VideoAuth" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            #endregion

            #region Animation Experience API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationLogInitialExperience",
                routeTemplate: "v1/{area}/{controller}/LogInitial/{hsId}",
                defaults: new { action = "LogInitialExperience", hsID = "" },
                constraints: new { area = "Animation", controller = "Experience" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationLogAnonymousExperienceEvent",
                routeTemplate: "v1/{area}/{controller}/LogAnonEvent/{hsId}",
                defaults: new { action = "LogExperienceEvent", hsID = "" },
                constraints: new { area = "Animation", controller = "Experience" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationLogUserExperienceEvent",
                routeTemplate: "v1/{area}/{controller}/LogUserEvent/{hsId}",
                defaults: new { action = "LogExperienceEvent", hsID = "" },
                constraints: new { area = "Animation", controller = "Experience" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );
            #endregion

            #region Animation Membership API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipLoginByCchId",
                routeTemplate: "v1/{area}/{controller}/LoginById/{employerId}/{cchId}/{hsId}",
                defaults: new { action = "GetMemberAuthorization", employerId = 0, cchId = 0, hsID = "" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipLogin",
                routeTemplate: "v1/{area}/{controller}/Login/{hsId}",
                defaults: new { action = "Login", hsID = "" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipRegister1",
                routeTemplate: "v1/{area}/{controller}/Register1/{hsId}",
                defaults: new { action = "Register1", hsID = "" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipRegister2",
                routeTemplate: "v1/{area}/{controller}/Register2/{hsId}",
                defaults: new { action = "Register2", hsID = "" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipEmail",
                routeTemplate: "v1/{area}/{controller}/Email",
                defaults: new { action = "UpdateUserEmail" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipMobilePhone",
                routeTemplate: "v1/{area}/{controller}/Phone/Mobile",
                defaults: new { action = "UserMobilePhone" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipAlternatePhone",
                routeTemplate: "v1/{area}/{controller}/Phone/Alt",
                defaults: new { action = "UserAlternatePhone" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipPhoneNumbers",
                routeTemplate: "v1/{area}/{controller}/Phone/Both",
                defaults: new { action = "UserPhoneNumbers" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipGetSecurityQuestions",
                routeTemplate: "v1/{area}/{controller}/SecurityQuestions",
                defaults: new { action = "SecurityQuestions" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipGetSecurityQuestion",
                routeTemplate: "v1/{area}/{controller}/SecurityQuestions/Current",
                defaults: new { action = "GetCurrentSecurityQuestion" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipSetSecurityAnswer",
                routeTemplate: "v1/{area}/{controller}/SecurityAnswer",
                defaults: new { action = "SetSecurityAnswer" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipChangePassword",
                routeTemplate: "v1/{area}/{controller}/Password",
                defaults: new { action = "ChangePassword" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipPasswordReset0",
                routeTemplate: "v1/{area}/{controller}/Password/Reset0",
                defaults: new { action = "PasswordReset0" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipPasswordReset1",
                routeTemplate: "v1/{area}/{controller}/Password/Reset1",
                defaults: new { action = "PasswordReset1" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipPasswordReset2",
                routeTemplate: "v1/{area}/{controller}/Password/Reset2",
                defaults: new { action = "PasswordReset2" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationMembershipSegment",
                routeTemplate: "v1/{area}/{controller}/Google/Segment",
                defaults: new { action = "GetEmployeeSegment" },
                constraints: new { area = "Animation", controller = "Membership" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );
#endregion

            #region Animation UserContent API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationGetUserContent",
                routeTemplate: "v1/{area}/{controller}/UserContent/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetUserContent", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetUserContentLocale",
                routeTemplate: "v1/{area}/{controller}/UserContentLocale/{localeCode}/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetUserContentLocale", localeCode = "en", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetUserReward",
                routeTemplate: "v1/{area}/{controller}/UserReward/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetUserRewards", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetUserRewardLocale",
                routeTemplate: "v1/{area}/{controller}/UserRewardLocale/{LocaleCode}/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetUserRewardsLocale", localeCode = "en", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetAnonymousContent",
                routeTemplate: "v1/{area}/{controller}/Anonymous/{hsId}/{employerId}/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetEmployerNotifications", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetAnonymousContentLocale",
                routeTemplate: "v1/{area}/{controller}/AnonymousLocale/{hsId}/{employerId}/{LocaleCode}/{PageSize}/{BaseContentId}",
                defaults: new { action = "GetEmployerNotificationsLocale", localeCode = "en", pageSize = 0, baseContentId = "" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationUpdateUserPreferences",
                routeTemplate: "v1/{area}/{controller}/Preferences",
                defaults: new { action = "UserPreferences" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationUpdateUserContentStatus",
                routeTemplate: "v1/{area}/{controller}/ContentStatus",
                defaults: new { action = "UpdateUserContentStatus" },
                constraints: new { area = "Animation", controller = "UserContent" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );
            #endregion

            #region Animation Survey API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationGetQuestionsAnswers",
                routeTemplate: "v1/{area}/{controller}/Quiz/{SurveyId}",
                defaults: new { action = "GetSurveyQuestionsAndAnswers", surveyId = "0" },
                constraints: new { area = "Animation", controller = "Survey" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationInsertUserSurveyAnswer",
                routeTemplate: "v1/{area}/{controller}/Answer",
                defaults: new { action = "InsertUserSurveyAnswer" },
                constraints: new { area = "Animation", controller = "Survey" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );
#endregion

            #region Animation Campaign API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationGetCampaignIntro",
                routeTemplate: "v1/{area}/{controller}/Intro/{employerId}/{campaignId}/{handshakeId}",
                defaults: new { action = "GetCampaignIntro", employerId = "0", campaignId = 0, handshakeId = "" },
                constraints: new { area = "Animation", controller = "Campaign" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            #endregion

            #region Animation Media API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationGetMediaUrl",
                routeTemplate: "v1/{area}/{controller}/MediaUrl/{BaseContentId}",
                defaults: new { action = "GetMediaUrl", baseContentId = "" },
                constraints: new { area = "Animation", controller = "Media" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            #endregion

            #region Animation Card API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationGetMemberCardData",
                routeTemplate: "v1/{area}/{controller}/MemberData",
                defaults: new { action = "GetMemberCardData" },
                constraints: new { area = "Animation", controller = "Card" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config), 
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationGetMemberTokens",
                routeTemplate: "v1/{area}/{controller}/CardUrls/{localeCode}",
                defaults: new { action = "GetMemberCardUrls", localeCode = "en" },
                constraints: new { area = "Animation", controller = "Card" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationSendIdCardEmail",
                routeTemplate: "v1/{area}/{controller}/Email",
                defaults: new { action = "SendIdCardEmail" },
                constraints: new { area = "Animation", controller = "Card" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAccountHandlers)
                );

            #endregion

            #region Animation Messaging API Routes

            config.Routes.MapHttpRoute(
                name: "AnimationPushNotificationDevice",
                routeTemplate: "v1/{area}/{controller}/PushPrompt/{hsId}",
                defaults: new { action = "UserDevice", hsID = "" },
                constraints: new { area = "Animation", controller = "Messaging" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            config.Routes.MapHttpRoute(
                name: "AnimationPushNotificationPromptStatus",
                routeTemplate: "v1/{area}/{controller}/PushPrompt/{employerId}/{hsId}/{deviceId}",
                defaults: new { action = "PushPromptStatus", hsID = "" },
                constraints: new { area = "Animation", controller = "Messaging" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            #endregion

            #region PComm PlanInfo Routes

            config.Routes.MapHttpRoute(
            name: "PCommGetHealthPlanSummary",
            routeTemplate: "v1/{area}/{controller}",
            defaults: new { action = "Get" },
            constraints: new { area = "PComm", controller = "HealthPlanSummary" },
            handler: HttpClientFactory.CreatePipeline(
                new HttpControllerDispatcher(config),
                AuthenticatedAccountHandlers)
            );

            #endregion

            #region Settings

            config.Routes.MapHttpRoute(
                name: "AnimationGetConfigValue",
                routeTemplate: "v1/{area}/{controller}/{hsId}/{employerId}/ConfigValue",
                defaults: new { action = "GetConfigurationValue", employerId = 0, hsId = "" },
                constraints: new { area = "Animation", controller = "Settings" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AccountHandlers)
                );

            #endregion

        }
    }
}
