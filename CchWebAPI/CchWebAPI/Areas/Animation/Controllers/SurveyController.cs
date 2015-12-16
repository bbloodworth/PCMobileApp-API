using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class SurveyController : ApiController
    {
        public HttpResponseMessage GetSurveyQuestionsAndAnswers(string surveyId)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetSurveyQuestionsAndAnswers gsqaa = new GetSurveyQuestionsAndAnswers())
                {
                    gsqaa.CampaignId = surveyId.GetCampaignId();
                    gsqaa.SurveyId = surveyId.GetContentId();
                    gsqaa.GetData(gecs.ConnString);

                    data.Results = gsqaa.Results;
                    data.TotalCount = gsqaa.TotalCount;
                }
            }
            if (data.TotalCount > 0)
            {
                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage InsertUserSurveyAnswer([FromBody]UserSurveyAnswerRequest surveyRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, "Unexpected Error");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (InsertUserSurveyAnswer iusa = new InsertUserSurveyAnswer())
                {
                    iusa.CampaignId = surveyRequest.SurveyId.GetCampaignId();
                    iusa.CchId = Request.CCHID();
                    iusa.SurveyId = surveyRequest.SurveyId.GetContentId();
                    iusa.AnswerId = surveyRequest.AnswerId;
                    iusa.FreeFormAnswer = surveyRequest.FreeFormAnswer;
                    iusa.QuestionId = surveyRequest.QuestionId;
                    
                    iusa.PostData(gecs.ConnString);
                    if (iusa.PostReturn == 1)
                    {
                        hrm = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            return hrm;
        }
    }
}
