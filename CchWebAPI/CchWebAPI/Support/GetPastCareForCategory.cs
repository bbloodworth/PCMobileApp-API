using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetPastCareForCategory : DataBase
    {
        #region Public Data Structures
        public struct CategoryData
        {
            public string Category;
            public DateTime ServiceDate;
            public string PatientName;
            public string ServiceName;
            public string PracticeName;
            public string PracticeAddress1;
            public string PracticeAddress2;
            public string PracticeCity;
            public string PracticeState;
            public bool FairPriceProvider;
            public decimal AllowedAmount;
            public decimal YouCouldHaveSaved;
            public decimal TotalYouSpent;
            public decimal TotalYouCouldHaveSaved;
            public string ProcedureCode;
            public int PastCareID;

            //public string GPI;
            //public decimal Quantity;
            //public int DrugID;
            //public int PharmacyLocationID;

            public int SpecialtyID;
            public int ServiceID;
        }
        public struct RxCategoryData
        {
            public string Category;
            public DateTime ServiceDate;
            public string PatientName;
            public string ServiceName;
            public string PracticeName;
            public string PracticeAddress1;
            public string PracticeAddress2;
            public string PracticeCity;
            public string PracticeState;
            public bool FairPriceProvider;
            public decimal AllowedAmount;
            public decimal YouCouldHaveSaved;
            public decimal TotalYouSpent;
            public decimal TotalYouCouldHaveSaved;
            public string ProcedureCode;
            public int PastCareID;

            public string GPI;
            public decimal Quantity;
            public int DrugID;
            public int PharmacyLocationID;

            //public int SpecialtyID;
            //public int ServiceID
        }

        public struct Member
        {
            public string Name;
            public int CCHID;
        }
        #endregion

        #region Private Results
        private List<CategoryData> categoryResults = null;
        private List<RxCategoryData> rxCategoryResults = null;
        private DateTime asOfDate;
        private List<Member> members = null;
        #endregion

        #region Public Results
        public List<CategoryData> CategoryResults
        { get { return this.categoryResults; } }
        public List<RxCategoryData> RxCategoryResults
        { get { return this.rxCategoryResults; } }
        public String AsOfDate
        { get { return this.asOfDate.ToShortDateString(); } }
        public List<Member> Members
        { get { return this.members; } }
        #endregion

        public int CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public string SubscriberMedicalID { set { this.Parameters["SubscriberMedicalID"].Value = value; } }
        public string Category { set { this.Parameters["Category"].Value = value; } }

        public GetPastCareForCategory()
            : base("GetPastCareForCategory")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("SubscriberMedicalID", System.Data.SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("Category", System.Data.SqlDbType.NVarChar, Size: 25);
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
                if(this.Parameters["Category"].Value.ToString() == "Prescription Drugs")
                {
                    rxCategoryResults = (from categoryRow in this.Tables[1].AsEnumerable()
                                         select new RxCategoryData
                                         {
                                             Category = categoryRow.GetData("Category"),
                                             ServiceDate = categoryRow.GetData<DateTime>("ServiceDate"),
                                             PatientName = categoryRow.GetData("PatientName"),
                                             ServiceName = categoryRow.GetData("ServiceName"),
                                             PracticeName = categoryRow.GetData("PracticeName"),
                                             PracticeAddress1 = categoryRow.GetData("PracticeAddress1"),
                                             PracticeAddress2 = categoryRow.GetData("PracticeAddress2"),
                                             PracticeCity = categoryRow.GetData("PracticeCity"),
                                             PracticeState = categoryRow.GetData("PracticeState"),
                                             FairPriceProvider = categoryRow.GetData<bool>("FairPriceProvider"),
                                             AllowedAmount = categoryRow.GetData<decimal>("AllowedAmount"),
                                             YouCouldHaveSaved = categoryRow.GetData<decimal>("YouCouldHaveSaved"),
                                             TotalYouSpent = categoryRow.GetData<decimal>("TotalYouSpent"),
                                             TotalYouCouldHaveSaved = categoryRow.GetData<decimal>("TotalYouCouldHaveSaved"),
                                             ProcedureCode = categoryRow.GetData("ProcedureCode"),
                                             GPI = categoryRow.GetData("GPI"),
                                             Quantity = categoryRow.GetData<decimal>("Quantity"),
                                             DrugID = categoryRow.GetData<int>("DrugID"),
                                             PharmacyLocationID = categoryRow.GetData<int>("PharmacyLocationID"),
                                             PastCareID = categoryRow.GetData<int>("PastCareID")
                                         }).ToList<RxCategoryData>();
                }
                else
                {
                    categoryResults = (from categoryRow in this.Tables[1].AsEnumerable()
                                  select new CategoryData
                                  {
                                      Category = categoryRow.GetData("Category"),
                                      ServiceDate = categoryRow.GetData<DateTime>("ServiceDate"),
                                      PatientName = categoryRow.GetData("PatientName"),
                                      ServiceName = categoryRow.GetData("ServiceName"),
                                      PracticeName = categoryRow.GetData("PracticeName"),
                                      PracticeAddress1 = categoryRow.GetData("PracticeAddress1"),
                                      PracticeAddress2 = categoryRow.GetData("PracticeAddress2"),
                                      PracticeCity = categoryRow.GetData("PracticeCity"),
                                      PracticeState = categoryRow.GetData("PracticeState"),
                                      FairPriceProvider = categoryRow.GetData<bool>("FairPriceProvider"),
                                      AllowedAmount = categoryRow.GetData<decimal>("AllowedAmount"),
                                      YouCouldHaveSaved = categoryRow.GetData<decimal>("YouCouldHaveSaved"),
                                      TotalYouSpent = categoryRow.GetData<decimal>("TotalYouSpent"),
                                      TotalYouCouldHaveSaved = categoryRow.GetData<decimal>("TotalYouCouldHaveSaved"),
                                      ProcedureCode = categoryRow.GetData("ProcedureCode"),
                                      SpecialtyID = categoryRow.GetData<int>("SpecialtyID"),
                                      ServiceID = categoryRow.GetData<int>("ServiceID"),
                                      PastCareID = categoryRow.GetData<int>("PastCareID")
                                  }).ToList<CategoryData>();
                }
            }

            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                asOfDate = (from asOfRow in this.Tables[2].AsEnumerable()
                            select asOfRow.GetData<DateTime>("AsOfDate")).FirstOrDefault<DateTime>();
            }
        }
    }
}