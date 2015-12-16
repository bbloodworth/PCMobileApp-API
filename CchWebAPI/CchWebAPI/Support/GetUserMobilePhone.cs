using System.Data;

namespace CchWebAPI.Support
{
    public class GetUserMobilePhone : DataBase
    {
        public int CchId
        {
            set { Parameters["CCHID"].Value = value; }
        }

        public string MobilePhone
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["MobilePhone"] : string.Empty; }
        }

        public GetUserMobilePhone()
            : base("p_GetUserMobilePhone")
        {
            Parameters.New("CCHID", SqlDbType.Int);
        }
    }
}
