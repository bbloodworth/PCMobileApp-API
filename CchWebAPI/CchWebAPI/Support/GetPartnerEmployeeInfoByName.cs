using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetPartnerEmployeeInfoByName : DataBase
    {
        public String FirstName { set { this.Parameters["firstName"].Value = value; } }
        public String LastName { set { this.Parameters["lastName"].Value = value; } }
        public String DOB { set { this.Parameters["dob"].Value = value; } }
        public String SubscriberMedicalID { set { this.Parameters["medicalID"].Value = value; } }
        //public String RelationshipCode { set { this.Parameters["relationshipCode"].Value = value; } }

        private DataTable EmployeeTable { get { return this.Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        private object this[String ColName] { get { return this.EmployeeRow[ColName]; } }

        public int CCHID { get { return int.Parse(this["CCHID"].ToString()); } }
        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetPartnerEmployeeInfoByName()
            : base("GetPartnerEmployeeInfoByName")
        { 
            this.Parameters.New("firstName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("lastName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("dob", SqlDbType.NChar, Size: 10);
            this.Parameters.New("medicalID", SqlDbType.NVarChar, Size: 50);
            //this.Parameters.New("relationshipCode", SqlDbType.NChar, Size: 2);
        }
    }
}