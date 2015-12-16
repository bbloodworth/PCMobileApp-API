using System.Data;

namespace CchWebAPI.Support
{
    public class UpdateUserMobilePhone : DataBase
    {
        public string MobilePhone { set { Parameters["MobilePhone"].Value = value; } }
        public int CCHID { set { Parameters["CCHID"].Value = value; } }

        public UpdateUserMobilePhone()
            : base("p_UpdateUserMobilePhone")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("MobilePhone", SqlDbType.NVarChar, Size: 50);
        }
    }
}