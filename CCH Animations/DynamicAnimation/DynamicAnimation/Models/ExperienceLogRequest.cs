namespace DynamicAnimation.Models
{
    public class ExperienceLogRequest
    {
        public int EmployerId { get; set; }
        public int EventId { get; set; }
        public string ExperienceUserId { get; set; }
        public string EventName { get; set; }
        public string LogComment { get; set; }
        public int CchId { get; set; }
        public string ContentId { get; set; }
    }

    public class ExperienceLogResponse
    {
        public string ExperienceUserId { get; set; }
    }

    //public class ExperienceLogModel
    //{
    //    public Int32 EmployerId;
    //    public Int32 CchId;
    //    public Int32 ExperienceEventId;
    //    public string ExperienceEvent;
    //    public string ExperienceUserId;
    //    public string Comment;
    //}

    public enum ExperienceEvents
    {
        StartAndroidApp = 1,
        StartMobileWeb = 2,
        StartiOSApp = 3,
        StartAnimation = 4,
        EndAnimation = 5,
        StartQuiz = 6,
        EndQuiz = 7,
        VisitResourcePage = 8,
        ExitAndroidApp = 9,
        ExitiOSApp = 10,
        StartIntro = 11,
        EndIntro = 12,
        HelpfulYes = 13,
        HelpfulNo = 14,
        AuthenticationSuccess = 15,
        AuthenticationFail = 16,
        ReplayAnimation = 17,
        GoToCch = 18,
        GoToPlan = 19,
        NoQueryParameters = 20,
        InvalidQueryParameters = 21,
        Error = 22,
        StartWebsite = 23,
        LoadTimer = 24,
        Info = 25,
        Debug = 26, 
        Warning = 27
    }
}