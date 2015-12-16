using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetKeyUserInfo : DataBase
    {
        private DataTable EmployeeTable { get { return this.Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return this.EmployeeTable.Rows[0] ?? this.EmployeeTable.NewRow(); } }
        private object this[String ColName] { get { return this.EmployeeRow[ColName] ?? ""; } }

        public String EmployerID { get { return this["EmployerID"].ToString(); } }
        public String EmployerName { get { return this["EmployerName"].ToString(); } }
        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetKeyUserInfo()
            : base("GetKeyUserInfo")
        {
            this.Parameters.New("UserName", System.Data.SqlDbType.NVarChar, Size: 250);
        }
        public GetKeyUserInfo(String UserID)
            : base("GetKeyUserInfo")
        {
            this.Parameters.New("UserName", System.Data.SqlDbType.NVarChar, Size: 250, Value: UserID);
            this.GetFrontEndData();
        }
    }
}