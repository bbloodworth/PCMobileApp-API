namespace CchWebAPI.Areas.Animation.Models
{
    public class UserSurveyAnswerRequest
    {
        public string SurveyId;
        public int QuestionId;
        public int AnswerId;
        public string FreeFormAnswer;
    }
}