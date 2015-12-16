using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    public class InsertUserSurveyAnswer : DataBase
    {
        public int CchId { set { Parameters["CCHID"].Value = value; } }
        public int CampaignId { set { Parameters["CampaignID"].Value = value; } }
        public int SurveyId { set { Parameters["SurveyID"].Value = value; }}
        public int QuestionId { set { Parameters["QuestionID"].Value = value; } }
        public int AnswerId { set { Parameters["AnswerID"].Value = value; } }
        public string FreeFormAnswer { set { Parameters["FreeFormAnswerText"].Value = value; } }

        public InsertUserSurveyAnswer()
            : base("p_InsertUserSurveyAnswer")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("CampaignID", SqlDbType.Int);
            Parameters.New("SurveyID", SqlDbType.Int);
            Parameters.New("QuestionID", SqlDbType.Int);
            Parameters.New("AnswerID", SqlDbType.Int);
            Parameters.New("FreeFormAnswerText", SqlDbType.NVarChar, Size: 500);
        }
    }
}