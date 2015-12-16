using System;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class ValidateMobileProvider : DataBase
    {
        public ValidateMobileProvider(String HandShakeKey)
            : base("ValidateMobileProvider")
        {
            this.Parameters.New("hsKey", System.Data.SqlDbType.NChar, Size: 36, Value: HandShakeKey);
            GetFrontEndData();
        }
        public void ForEachProvider(Action<Boolean> a)
        {
            DataRow[] Providers = this.Tables[0].Select();
            foreach (DataRow dr in Providers) a(Convert.ToBoolean(dr[0].ToString()));
        }
    }
}