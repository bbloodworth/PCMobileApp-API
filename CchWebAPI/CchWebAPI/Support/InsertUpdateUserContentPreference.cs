using System.Data;

namespace CchWebAPI.Support
{
    public class InsertUpdateUserContentPreference : DataBase
    {
        public int CCHID { set { Parameters["CCHID"].Value = value; } }
        public int SmsInd { set { Parameters["SMSInd"].Value = value; } }
        public int EmailInd { set { Parameters["EmailInd"].Value = value; }}
        public int OsBasedAlertInd { set { Parameters["OSBasedAlertInd"].Value = value; } }
        public string LocaleCode { set { Parameters["DefaultLocaleCode"].Value = value; } }

        public InsertUpdateUserContentPreference()
            : base("p_InsertUpdateUserContentPreference")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("SMSInd", SqlDbType.Int);
            Parameters.New("EmailInd", SqlDbType.Int);
            Parameters.New("OSBasedAlertInd", SqlDbType.Int);
            Parameters.New("DefaultLocaleCode", SqlDbType.NVarChar, Size: 10);
        }
    }
}