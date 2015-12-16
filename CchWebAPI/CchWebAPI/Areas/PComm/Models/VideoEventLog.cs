using System;

namespace CchWebAPI.Areas.PComm.Models
{
    public enum LogEvents
    {
        StartIntro = 1,
        EndIntro = 2,
        StartVideo = 3,
        EndVideo = 4,
        VideoHelpfulYes = 5,
        VideoHelpfulNo = 6,
        AuthenticateSuccess = 7,
        Replay = 8,
        GoToCCH = 9,
        GoToPlan = 10,
        AuthenticateFail = 11
    }

    public class VideoEventLog
    {
        public Int32 EmployerId;
        public string VideoCampaignMemberId;
        public Int32 VideoLogEventId;
        public string VideoLogEvent;
        public string Comment;
    }
}