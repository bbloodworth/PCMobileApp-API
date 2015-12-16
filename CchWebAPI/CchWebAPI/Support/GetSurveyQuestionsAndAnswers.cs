using System.Collections.Generic;
using System.Data;
using System.Linq;
using CchWebAPI.Areas.Animation.Models;

namespace CchWebAPI.Support
{
    public class GetSurveyQuestionsAndAnswers : DataBase
    {
        private int _campaignId;
        public int CampaignId
        {
            set { _campaignId = value; }
        }

        public int SurveyId
        {
            set { Parameters["SurveyId"].Value = value; }
        }

        private List<QuestionsAndAnswers> _results = new List<QuestionsAndAnswers>();

        public List<QuestionsAndAnswers> Results
        {
            get { return _results; }
        }

        public int TotalCount
        {
            get { return Results.Count; } 
        }

        public GetSurveyQuestionsAndAnswers()
            : base("p_GetSurveyQuestionsAndAnswers")
        {
            Parameters.New("SurveyId", SqlDbType.Int);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            if (Tables.Count > 0 && Tables[0].Rows.Count > 0)
            {
                _results = (from result in Tables[0].AsEnumerable()
                            select new QuestionsAndAnswers()
                            {
                                SurveyId = string.Format("{0}.{1}", _campaignId, result.GetData("SurveyId")),
                                AnswerDisplayOrderNum = result.GetData<int>("AnswerDisplayOrderNum"), 
                                AnswerId = result.GetData<int>("AnswerId"), 
                                AnswerText = result.GetData("AnswerText"), 
                                QuestionDisplyOrderNum = result.GetData<int>("QuestionDisplayOrderNum"), 
                                QuestionId = result.GetData<int>("QuestionId"), 
                                QuestionText = result.GetData("QuestionText"), 
                                QuestionType = result.GetData("QuestionTypeDesc")                                
                            }).ToList<QuestionsAndAnswers>();
            }
        }
    }
}