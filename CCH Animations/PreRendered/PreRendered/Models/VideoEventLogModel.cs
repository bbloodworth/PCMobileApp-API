using System;

namespace PreRendered.Models
{
    public class VideoEventLogModel
    {
        public Int32 EmployerId;
        public string VideoCampaignMemberId;
        public Int32 VideoLogEventId;
        public string VideoLogEvent;
        public string Comment;
    }

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
        AuthenticateFail = 11,
        NoQueryParameters = 21,
        InvalidQueryParameters = 22,
        Error = 23
    }
}