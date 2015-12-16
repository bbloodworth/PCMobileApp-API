using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetKeyProviderInfo : DataBase
    {
        private DataTable EmployeeTable { get { return this.Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return this.EmployeeTable.Rows[0] ?? this.EmployeeTable.NewRow(); } }
        private object this[String ColName] { get { return this.EmployeeRow[ColName] ?? ""; } }

        public String EmployerID { get { return this["EmployerID"].ToString(); } }
        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public String ProviderID { get { return this.Parameters["ProviderID"].Value.ToString(); } set { this.Parameters["ProviderID"].Value = value; } }

        private List<dynamic> education = new List<dynamic>();
        private List<dynamic> hgRatings = new List<dynamic>();

        public dynamic Education { get { return this.education; } }
        public dynamic HGRatings { get { return this.hgRatings; } }

        public GetKeyProviderInfo()
            : base("GetKeyProviderInfo")
        {
            this.Parameters.New("ProviderID", System.Data.SqlDbType.NVarChar, Size: 25);
        }

        public override void GetHealthgradesData()
        {
            base.GetHealthgradesData();
            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
                education = (from ed in this.Tables[0].AsEnumerable()
                             select new
                             {
                                 TableName = ed.Field<dynamic>("TableName"),
                                 ProviderID = ed.Field<dynamic>("ProviderID"),
                                 EducationTitle = ed.Field<dynamic>("EducationTitle"),
                                 InstitutionName = ed.Field<dynamic>("InstitutionName"),
                                 InstitutionCity = ed.Field<dynamic>("InstitutionCity"),
                                 InstitutionNation = ed.Field<dynamic>("InstitutionNation"),
                                 CompletionYear = ed.Field<dynamic>("CompletionYear"),
                                 YearCompletedTitle = ed.Field<dynamic>("YearCompletedTitle")
                             }).ToList<dynamic>();
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
                hgRatings = (from r in this.Tables[1].AsEnumerable()
                             select new
                             {
                                 TableName = r.Field<dynamic>("TableName"),
                                 ProviderID = r.Field<dynamic>("ProviderID"),
                                 SurveyQuestionID = r.Field<dynamic>("SurveyQuestionID"),
                                 SurveyTitle = r.Field<dynamic>("SurveyTitle"),
                                 SurveyDescription = r.Field<dynamic>("SurveyDescription"),
                                 SurveyScore = r.Field<dynamic>("SurveyScore"),
                                 OverallRating = r.Field<dynamic>("OverallRating"),
                                 PatientCount = r.Field<dynamic>("PatientCount")
                             }).ToList<dynamic>();
        }
    }
}