using System.Data;

namespace CchWebAPI.Support
{
    public sealed class GetPasswordQuestions : DataBase
    {
        public GetPasswordQuestions()
            : base("GetPasswordQuestions")
        {
            GetFrontEndData();
        }

        public DataTable QuestionsTable
        {
            get { return (Tables.Count > 0 ? Tables[0] : new DataTable("Empty"));  }
        }
    }
}