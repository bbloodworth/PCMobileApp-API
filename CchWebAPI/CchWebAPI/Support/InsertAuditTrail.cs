using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class InsertAuditTrail : DataBase
    {

        public int CchId
        {
            set { Parameters["CCHID"].Value = value; }
        }
        public string SessionId
        {
            set { Parameters["SessionID"].Value = value; }
        }
        public String Action
        {
            set { Parameters["Action"].Value = value; }
        }
        public String Domain
        {
            set { Parameters["Domain"].Value = value; }
        }

        public InsertAuditTrail()
            : base("CreateAuditTrail")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("SessionID", SqlDbType.NVarChar, Size: 36);
            Parameters.New("Action", SqlDbType.NVarChar, Size: 200);
            Parameters.New("Domain", SqlDbType.NVarChar, Size: 30);
        }
    }
}