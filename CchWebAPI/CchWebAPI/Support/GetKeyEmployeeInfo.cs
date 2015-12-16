using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetKeyEmployeeInfo : DataBase
    {
        public String Email { set { this.Parameters["Email"].Value = value; } }

        private DataTable EmployeeTable { get { return this.Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        private object this[String ColName] { get { return this.EmployeeRow[ColName]; } }

        public int CCHID { get { return int.Parse(this["CCHID"].ToString()); } }
        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetKeyEmployeeInfo()
            : base("GetKeyEmployeeInfo")
        { this.Parameters.New("Email", System.Data.SqlDbType.NVarChar, Size: 150); }
    }
}