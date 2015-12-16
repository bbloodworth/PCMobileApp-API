using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetPartnerEmployeeInfo : DataBase
    {
        public String HandShakeKey { set { this.Parameters["hsKey"].Value = value; } }
        public String PartnerIDValue { set { this.Parameters["PartnerIDValue"].Value = value; } }
        public String OrganizationID { set { this.Parameters["orgID"].Value = value; } }

        private DataTable EmployeeTable { get { return this.Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        private object this[String ColName] { get { return this.EmployeeRow[ColName]; } }

        public int CCHID { get { return int.Parse(this["CCHID"].ToString()); } }
        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetPartnerEmployeeInfo()
            : base("GetPartnerEmployeeInfo")
        { 
            this.Parameters.New("PartnerIDValue", SqlDbType.NVarChar, Size: 30);
            this.Parameters.New("hsKey", System.Data.SqlDbType.NChar, Size: 36);
            this.Parameters.New("orgID", System.Data.SqlDbType.NVarChar, Size: 30);
        }
    }
}