using System.ComponentModel;
using System.Data;

namespace CchWebAPI.Support
{
    [DesignerCategory("")]
    public class GetUserContentPreference : DataBase
    {
        public int CCHID
        {
            get
            {
                return (int)Parameters["CCHID"].Value;
            }
            set
            {
                Parameters["CCHID"].Value = value;
            }
        }

        public bool EmailInd
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 && (bool)Tables[0].Rows[0]["EmailInd"];
            }
        }
        public bool SmsInd
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 && (bool)this.Tables[0].Rows[0]["SmsInd"];
            }
        }
        public bool OsBasedAlertInd
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 && (bool)this.Tables[0].Rows[0]["OsBasedAlertInd"];
            }
        }
        public string LocaleCode
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["DefaultLocaleCode"] : string.Empty;
            }
        }
        public string ContactPhoneNumber
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["PreferredContactPhoneNum"] : string.Empty;
            }
        }

        public GetUserContentPreference()
            : base("p_GetUserContentPreference")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
        }
        public GetUserContentPreference(int cchid)
            : base("p_GetUserContentPreference")
        {
            this.Parameters.New("CCHID", SqlDbType.Int, Value: cchid);
        }
    }
}