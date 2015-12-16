using System;
using System.Threading.Tasks;
using DynamicAnimation.Models;

namespace DynamicAnimation.Common
{
    public class HelperService
    {
        public static void LogAnonEvent(ExperienceEvents experienceEvent)
        {
            LogAnonEvent(experienceEvent, string.Empty);
        }

        public static void LogAnonEvent(ExperienceEvents experienceEvent, string message)
        {
            var experienceLog = new ExperienceLogRequest
            {
                EventId = Convert.ToInt32(experienceEvent),
                EmployerId = CampaignSessionModel.Current.EmployerId,
                ExperienceUserId = CampaignSessionModel.Current.ExperienceUserId
            };

            switch (experienceEvent)
            {
                case ExperienceEvents.StartIntro:
                    experienceLog.LogComment = string.Format("Start Intro Animation {0}",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.IntroContentId.ToString();
                    break;
                case ExperienceEvents.EndIntro:
                    experienceLog.LogComment = string.Format("End Intro Animation {0} ",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.IntroContentId.ToString();
                    break;
                case ExperienceEvents.VisitResourcePage:
                    experienceLog.LogComment = string.Format("Visit Resource Page {0} ", message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.ExitAndroidApp:
                    experienceLog.LogComment = string.Format("Exit Android App");
                    break;
                case ExperienceEvents.ExitiOSApp:
                    experienceLog.LogComment = string.Format("Exit iOS App");
                    break;
                case ExperienceEvents.AuthenticationFail:
                    experienceLog.LogComment = string.Format("Authentication {0} failed",
                        message);
                    break;
                case ExperienceEvents.InvalidQueryParameters:
                    experienceLog.LogComment = string.Format("Invalid format for query parameters: {0}", message);
                    break;
                case ExperienceEvents.NoQueryParameters:
                    experienceLog.LogComment = "Query parameters Not Found";
                    break;
                case ExperienceEvents.StartWebsite:
                    experienceLog.LogComment = string.Format("Animations Website launched ");
                    break;
                case ExperienceEvents.LoadTimer:
                    experienceLog.LogComment = string.Format("Load Timer: {0}",
                        message);
                    break;
                case ExperienceEvents.Info:
                    experienceLog.LogComment = string.Format("Info: {0}", message);
                    break;
                case ExperienceEvents.Debug:
                    experienceLog.LogComment = string.Format("Debug: {0}", message);
                    break;
                case ExperienceEvents.Warning:
                    experienceLog.LogComment = string.Format("Warning: {0}", message);
                    break;
                // Following events require authentication from a Web API client
                //case ExperienceEvents.AuthenticationSuccess:
                //    experienceLog.LogComment = string.Format("Authentication {0} successful",
                //        message);
                //    break;
                //case ExperienceEvents.StartAnimation:
                //    experienceLog.LogComment = string.Format("Start Animation {0} ",
                //        message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.EndAnimation:
                //    experienceLog.LogComment = string.Format("End Animation {0} ",
                //        message);
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    break;
                //case ExperienceEvents.StartQuiz:
                //    experienceLog.LogComment = string.Format("Start Quiz {0}", message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.EndQuiz:
                //    experienceLog.LogComment = string.Format("End Quiz {0}", message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.HelpfulYes:
                //    experienceLog.LogComment = string.Format("Animation {0} WAS Helpful",
                //        message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.HelpfulNo:
                //    experienceLog.LogComment = string.Format("Animation {0} WAS NOT Helpful",
                //        message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.ReplayAnimation:
                //    experienceLog.LogComment = string.Format("Replay Animation {0}", message);
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                //    break;
                //case ExperienceEvents.GoToCch:
                //    experienceLog.LogComment = string.Format("Go to CCH Website");
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    break;
                //case ExperienceEvents.GoToPlan:
                //    experienceLog.LogComment = string.Format("Go to Employer Plan Website");
                //    experienceLog.CchId = CampaignSessionModel.Current.CchId;
                //    break;
                case ExperienceEvents.Error:
                    experienceLog.LogComment = string.Format("Unexpected Error: {0}", message);
                    break;
                default:
                    experienceLog.LogComment = string.Format("Invalid Action");
                    break;
            }
            Task.Run(() => WebApiService.LogAnonEvent(experienceLog));
        }

        public static void LogUserEvent(ExperienceEvents experienceEvent)
        {
            LogUserEvent(experienceEvent, string.Empty);
        }

        public static void LogUserEvent(ExperienceEvents experienceEvent, string message)
        {
            var experienceLog = new ExperienceLogRequest
            {
                EventId = Convert.ToInt32(experienceEvent),
                EmployerId = CampaignSessionModel.Current.EmployerId,
                ExperienceUserId = CampaignSessionModel.Current.ExperienceUserId,
                CchId = CampaignSessionModel.Current.CchId
            };

            switch (experienceEvent)
            {
                case ExperienceEvents.AuthenticationSuccess:
                    experienceLog.LogComment = string.Format("Authentication for {0} successful",
                        message);
                    break;
                case ExperienceEvents.StartAnimation:
                    experienceLog.LogComment = string.Format("Start Animation {0} ",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.EndAnimation:
                    experienceLog.LogComment = string.Format("End Animation {0} ",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.StartQuiz:
                    experienceLog.LogComment = string.Format("Start Quiz {0}", message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.EndQuiz:
                    experienceLog.LogComment = string.Format("End Quiz {0}", message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.HelpfulYes:
                    experienceLog.LogComment = string.Format("Animation {0} WAS Helpful",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.HelpfulNo:
                    experienceLog.LogComment = string.Format("Animation {0} WAS NOT Helpful",
                        message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.ReplayAnimation:
                    experienceLog.LogComment = string.Format("Replay Animation {0}", message);
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.GoToCch:
                    experienceLog.LogComment = string.Format("Go to CCH Website");
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.GoToPlan:
                    experienceLog.LogComment = string.Format("Go to Employer Plan Website");
                    experienceLog.ContentId = CampaignSessionModel.Current.ContentId.ToString();
                    break;
                case ExperienceEvents.Error:
                    experienceLog.LogComment = string.Format("Unexpected Error: {0}", message);
                    break;
                default:
                    experienceLog.LogComment = string.Format("Invalid Action");
                    break;
            }
            string authHash = CampaignSessionModel.Current.AuthorizationHash;
            Task.Run(() => WebApiService.LogUserEvent(experienceLog, authHash));
        }

        public static ExperienceLogResponse LogInitialEvent(int employerId)
        {
            return WebApiService.LogInitialEvent(employerId);
        }

    }
}