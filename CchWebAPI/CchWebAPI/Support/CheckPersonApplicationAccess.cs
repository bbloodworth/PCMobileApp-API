using System;
using System.Data;

namespace CchWebAPI.Support
{
    public sealed class CheckPersonApplicationAccess : DataBase
    {
        #region Parameter Set properties
        public bool HasAccess
        {
            get
            {
                var row = this.Tables[0].Rows[0];
                if (row == null)
                    return false; 

                return ((int)row[0]) > 0;
            }
        }
        #endregion

        public CheckPersonApplicationAccess(int cchId, string connectionString)
            : base("p_CheckPersonApplicationAccess")
        {
            this.Parameters.New("CCHID", SqlDbType.Int);
            this.Parameters["CCHID"].Value = cchId;
            this.Parameters.New("CCHApplicationID", SqlDbType.Int);
            this.Parameters["CCHApplicationID"].Value = 2; //API is always 2

            this.GetData(connectionString);
        }

        public string ErrorMessage
        {
            get
            {
                return "Application access has not been granted.";
            }
        }
    }
}