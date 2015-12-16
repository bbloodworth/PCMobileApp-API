using System;
using System.ComponentModel;
using System.Data;

namespace CchWebAPI.Support
{
    [DesignerCategory("")]
    public class InsertUserLoginHistory : DataBase
    {
        public String UserName { set { Parameters["Username"].Value = value; } }
        public String Domain { set { Parameters["Domain"].Value = value; } }
        public Int32 CCHID
        {
            set
            {
                Parameters.New("CCHID", SqlDbType.Int, Value: value);
            }
        }
        public int CchApplicationId { set { Parameters["CCHApplicationID"].Value = value; } }

        public InsertUserLoginHistory()
            : base("InsertUserLoginHistory")
        {
            Parameters.New("Username", SqlDbType.NVarChar, Size: 50);
            Parameters.New("Domain", SqlDbType.NVarChar, Size: 30);
            Parameters.New("CCHApplicationID", SqlDbType.Int);
        }
    }
}