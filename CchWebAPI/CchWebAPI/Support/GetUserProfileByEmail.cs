using System.Data;

namespace CchWebAPI.Support
{
    public class GetUserProfileByEmail : DataBase
    {
        public string Email
        {
            set { Parameters["Email"].Value = value; }
        }

        public int EmployerId
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (int)Tables[0].Rows[0]["EmployerID"] : 0; }
        }

        public string FirstName
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["FirstName"] : string.Empty; }
        }

        public string LastName
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["LastName"] : string.Empty; }
        }

        public GetUserProfileByEmail()
            : base("GetUserProfileByEmail")
        {
            Parameters.New("Email", SqlDbType.VarChar, Size: 100);
        }
    }
}