using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetPastCareCategoriesForAPI : DataBase
    {
        #region Public Data Structures
        public struct SaveSpentResult
        {
            public decimal YouCouldHaveSaved;
            public decimal YouSpent;
        }
        public struct CategoryData
        {
            public string Category;
            public string TotalSpent;
            public string TotalYouCouldHaveSaved;
        }
        public struct Member
        {
            public string Name;
            public int CCHID;
        }
        #endregion

        #region Private Results
        private SaveSpentResult youCouldHaveSavedYouSpent;
        private List<CategoryData> categories = null;
        private DateTime asOfDate;
        private List<Member> members = null;
        #endregion

        #region Public Results
        public SaveSpentResult YouCouldHaveSavedYouSpent
        { get { return this.youCouldHaveSavedYouSpent; } }
        public List<CategoryData> Categories
        { get { return this.categories; } }
        public String AsOfDate
        { get { return this.asOfDate.ToShortDateString(); } }
        public List<Member> Members
        { get { return this.members; } }
        #endregion

        public int CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public string SubscriberMedicalID { set { this.Parameters["SubscriberMedicalID"].Value = value; } }

        public GetPastCareCategoriesForAPI()
            : base("GetPastCareCategoriesForAPI")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("SubscriberMedicalID", System.Data.SqlDbType.NVarChar, Size: 50);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                members = (from memberRow in this.Tables[0].AsEnumerable()
                           select new Member
                           {
                               Name = memberRow.GetData("Member"),
                               CCHID = memberRow.GetData<int>("CCHID")
                           }).ToList<Member>();
            }

            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
            {
                youCouldHaveSavedYouSpent = (from saveSpentRow in this.Tables[1].AsEnumerable()
                                             select new SaveSpentResult
                                             {
                                                 YouCouldHaveSaved = saveSpentRow.GetData<decimal>("YouCouldHaveSaved"),
                                                 YouSpent = saveSpentRow.GetData<decimal>("YouSpent")
                                             }).FirstOrDefault<SaveSpentResult>();
            }

            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                categories = (from categoryRow in this.Tables[2].AsEnumerable()
                              select new CategoryData
                              {
                                  Category = HttpUtility.UrlEncode(categoryRow.GetData("Category")),
                                  TotalSpent = categoryRow.IsNull("TotalYouSpent") ? "--" : categoryRow.GetData("TotalYouSpent"),
                                  TotalYouCouldHaveSaved = GetSavingsFromRow(categoryRow)
                              }).ToList<CategoryData>();
            }

            if (this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
            {
                asOfDate = (from asOfRow in this.Tables[3].AsEnumerable()
                            select asOfRow.GetData<DateTime>("AsOfDate")).FirstOrDefault<DateTime>();
            }
        }

        private static String GetSavingsFromRow(DataRow dr)
        {
            if (dr.IsNull("TotalYouCouldHaveSaved")) return "--";
            if (dr.GetData<decimal>("TotalYouSpent") <= 0.0m) return "--";
            if (dr.GetData<decimal>("TotalYouCouldHaveSaved") == 0.0m) return "Smart Shopper!";
            return dr.GetData("TotalYouCouldHaveSaved");
        }
    }
}