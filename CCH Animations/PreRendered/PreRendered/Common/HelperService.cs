using System;
using System.Threading.Tasks;
using PreRendered.Models;

namespace PreRendered.Common
{
    public class HelperService
    {
        public static void LogAnEvent(LogEvents logEvent)
        {
            LogAnEvent(logEvent, string.Empty);
        }

        public static void LogAnEvent(LogEvents logEvent, string message)
        {
            var userSessionVideo = UserSessionVideoModel.Current;
            
            var videoEventLog = new VideoEventLogModel
            {
                EmployerId = userSessionVideo.EmployerId,
                VideoCampaignMemberId = userSessionVideo.VideoCampaignMemberId,
                VideoLogEventId = Convert.ToInt32(logEvent)
            };

            switch (logEvent)
            {
                case LogEvents.StartIntro:
                    videoEventLog.Comment = string.Format("Start Intro Video {0} for {1} ",
                        userSessionVideo.IntroVideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.EndIntro:
                    videoEventLog.Comment = string.Format("End Intro Video {0} for {1} ",
                        userSessionVideo.IntroVideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.AuthenticateSuccess:
                    videoEventLog.Comment = string.Format("Authentication for {0} successful",
                        userSessionVideo.LastName);
                    break;
                case LogEvents.AuthenticateFail:
                    videoEventLog.Comment = string.Format("Authentication for {0} failed",
                        userSessionVideo.LastName);
                    break;
                case LogEvents.StartVideo:
                    videoEventLog.Comment = string.Format("Start Personal Video {0} for {1} ",
                        userSessionVideo.VideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.EndVideo:
                    videoEventLog.Comment = string.Format("End Personal Video {0} for {1} ",
                        userSessionVideo.VideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.VideoHelpfulYes:
                    videoEventLog.Comment = string.Format("{0} for {1} WAS Helpful",
                        userSessionVideo.VideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.VideoHelpfulNo:
                    videoEventLog.Comment = string.Format("{0} for {1} WAS NOT Helpful",
                        userSessionVideo.VideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.Replay:
                    videoEventLog.Comment = string.Format("Replay Personal Video {0} for {1}",
                        userSessionVideo.VideoDefinitionName, userSessionVideo.LastName);
                    break;
                case LogEvents.GoToCCH:
                    videoEventLog.Comment = string.Format("User {0} navigated to {1}",
                        userSessionVideo.LastName, userSessionVideo.CchEmployerLink);
                    break;
                case LogEvents.GoToPlan:
                    videoEventLog.Comment = string.Format("User {0} navigated to {1}",
                        userSessionVideo.LastName, userSessionVideo.EmployerBenefitsLink);
                    break;
                case LogEvents.InvalidQueryParameters:
                    videoEventLog.Comment = string.Format("Invalid format for Vid query parameter: {0}", message);
                    break;
                case LogEvents.NoQueryParameters:
                    videoEventLog.Comment = "Vid query parameter Not Found";
                    break;
                case LogEvents.Error:
                    videoEventLog.Comment = string.Format("Unexpected Error: {0}", message);
                    break;
                default:
                    videoEventLog.Comment = string.Format("Invalid Action");
                    break;
            }
            Task.Run(() => WebApiService.LogAnEvent(videoEventLog));
        }
    }
}