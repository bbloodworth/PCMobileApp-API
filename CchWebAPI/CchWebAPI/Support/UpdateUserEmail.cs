using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class UpdateUserEmail : DataBase
    {
        #region Parameter Set Properties
        public String UserName { set { this.Parameters["UserName"].Value = value; } }
        public String Email { set { this.Parameters["Email"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        #endregion

        public Int32 ReturnStatus { get { return Int32.Parse(this.Parameters["retVal"].Value.ToString()); } }

        public UpdateUserEmail()
            : base("UpdateUserEmail")
        {
            this.Parameters.New("UserName", SqlDbType.NVarChar, Size: 256);
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256);
            this.Parameters.New("UserID", SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
        }
        public void UpdateClientSide(String Email, Int32 CCHID, String connectionString)
        {
            if (this.Parameters.Count > 0) this.Parameters.Clear();

            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256, Value: Email);
            this.Parameters.New("CCHID", SqlDbType.Int, Value: CCHID);
            this.Parameters.New("retVal", SqlDbType.Int);
            this.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            this.PostData(connectionString);
        }
    }
}