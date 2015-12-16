using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetFacilitiesForServicePastCare : DataBase
    {
        #region Public Data Structures
        public struct FacilityResult
        {
            public string ServiceName;
            public string TaxId;
            public string NPI;
            public string PracticeName;
            public int OrganizationLocationID;
            public string ProviderName;
            public int AllowedAmount;
            public double Latitude;
            public double Longitude;
            public string LocationAddress1;
            public int YourCost;
            public string LocationCity;
            public string LocationState;
            public string LocationZip;
            public string LocationTelephone;
            public int RowNumber;
            public string Distance;
            public decimal NumericDistance;
            public bool FairPrice;
            public int HGRecognized;
            public string LatLong;
            public double HGOverallRating;
            public int HGPatientCount;
            public int FindAService;
            public int RowOrderKey;
            public int OrganizationID;
            public int HGRecognizedDocCount;
            public int HGDocCount;
            public bool AntiTransparency;
        }
        public struct LearnMoreResult
        {
            public string Description;
        }
        public struct LearnMoreSpecialtyResult
        {
            public int FindADoc;
            public string Specialty;
            public string Description;
            public string Title;
            public string DoctorTitleText;
            public string LearnMore;
        }
        public struct SavingsResult
        {
            public DateTime ServiceDate;
            public string ProcedureCode;
            public double AllowedAmount;
            public double YouCouldHaveSaved;
            public int OrganizationID;
            public int OrganizationLocationID;
            public int SpecialtyID;
        }
        #endregion

        #region Public SQL Parameter Value Set Properties
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public String ProcedureCode { set { this.Parameters["ProcedureCode"].Value = value; } }
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public Int32 PastCareID { set { this.Parameters["PastCareID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String Domain { set { this.Parameters["Domain"].Value = value; } }
        public Boolean DD { set { this.Parameters["DD"].Value = value; } }
        #endregion

        #region Public "On Occasion" SQL Parameter Value Set Properties
        public Int32 Distance { set { this.Parameters.New("Distance", System.Data.SqlDbType.Int, Value: value); } }
        public new String OrderByField { set { this.Parameters.New("OrderByField", System.Data.SqlDbType.NVarChar, 50, value); } }
        public new String OrderDirection { set { this.Parameters.New("OrderDirection", System.Data.SqlDbType.NVarChar, 4, value); } }
        public Int32 FromRow { get { return (Int32)(Parameters["FromRow"].Value ?? 0); } set { this.Parameters.New("FromRow", System.Data.SqlDbType.Int, Value: value); } }
        public Int32 ToRow { get { return (Int32)(Parameters["ToRow"].Value ?? 0); } set { this.Parameters.New("ToRow", System.Data.SqlDbType.Int, Value: value); } }
        #endregion

        #region Private Result Lists
        private List<FacilityResult> resultData = null;
        private List<LearnMoreResult> learnMoreData = null;
        private List<LearnMoreSpecialtyResult> learnMoreSpecialtyData = null;
        private List<SavingsResult> savingsData = null;
        #endregion

        #region Public Result Data Properties
        public List<FacilityResult> ResultData { get { return (this.resultData ?? new List<FacilityResult>()); } }
        public Int32 TotalResults { get { return this.ResultData.Count; } }
        public String LearnMoreData { get { return this.learnMoreData.First().Description; } }
        public List<LearnMoreSpecialtyResult> LearnMoreSpecialty { get { return (this.learnMoreSpecialtyData ?? new List<LearnMoreSpecialtyResult>()); } }
        public List<SavingsResult> SavingsData { get { return (this.savingsData ?? new List<SavingsResult>()); } }
        #endregion

        public GetFacilitiesForServicePastCare()
            : base("GetFacilitiesForServicePastCare")
        {
            this.Parameters.New("ServiceID", System.Data.SqlDbType.Int);
            this.Parameters.New("ProcedureCode", System.Data.SqlDbType.VarChar, Size: 4);
            this.Parameters.New("Latitude", System.Data.SqlDbType.Float);
            this.Parameters.New("Longitude", System.Data.SqlDbType.Float);
            this.Parameters.New("SpecialtyID", System.Data.SqlDbType.Int);
            this.Parameters.New("PastCareID", System.Data.SqlDbType.Int);
            this.Parameters.New("UserID", System.Data.SqlDbType.VarChar, Size: 36);
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("Domain", System.Data.SqlDbType.VarChar, Size: 30);
            this.Parameters.New("DD", SqlDbType.Bit);
        }

        #region Methods
        public void GetData(string ConnectionString, CCHEncrypt ce)
        {
            base.GetData(ConnectionString);

            ce.Add("TaxID", "");

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                resultData = (from result in this.Tables[0].AsEnumerable()
                              select new FacilityResult
                              {
                                  ServiceName = result.GetData("ServiceName"),
                                  TaxId = result.EncryptTaxID(ce),
                                  NPI = result.GetData("NPI"),
                                  PracticeName = result.GetData("PracticeName"),
                                  OrganizationLocationID = result.GetData<int>("OrganizationLocationID"),
                                  ProviderName = result.GetData("ProviderName"),
                                  AllowedAmount = result.GetData<int>("AllowedAmount"),
                                  Latitude = result.GetData<double>("Latitude"),
                                  Longitude = result.GetData<double>("Longitude"),
                                  LocationAddress1 = result.GetData("LocationAddress1"),
                                  YourCost = result.GetData<int>("YourCost"),
                                  LocationCity = result.GetData("LocationCity"),
                                  LocationState = result.GetData("LocationState"),
                                  LocationZip = result.GetData("LocationZip"),
                                  LocationTelephone = result.GetData("LocationTelephone"),
                                  RowNumber = result.GetData<int>("RowNumber"),
                                  Distance = result.GetData("Distance"),
                                  NumericDistance = result.GetData<decimal>("NumericDistance"),
                                  FairPrice = result.GetData<bool>("FairPrice"),
                                  HGRecognized = result.GetData<int>("HGRecognized"),
                                  LatLong = result.GetData("LatLong"),
                                  HGOverallRating = result.GetData<double>("HGOverallRating"),
                                  HGPatientCount = result.GetData<int>("HGPatientCount"),
                                  FindAService = result.GetData<int>("FindAService"),
                                  RowOrderKey = result.GetData<int>("RowOrderKey"),
                                  OrganizationID = result.GetData<int>("OrganizationID"),
                                  HGRecognizedDocCount = result.GetData<int>("HGRecognizedDocCount"),
                                  HGDocCount = result.GetData<int>("HGDocCount"),
                                  AntiTransparency = result.GetData<bool>("AntiTransparency")
                              }).ToList<FacilityResult>();
            }

            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
            {
                learnMoreData = (from desc in this.Tables[1].AsEnumerable()
                                 select new LearnMoreResult
                                 {
                                     Description = desc.GetData("Description")
                                 }).ToList<LearnMoreResult>();
            }

            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                learnMoreSpecialtyData = (from desc in this.Tables[2].AsEnumerable()
                                          select new LearnMoreSpecialtyResult
                                          {
                                              FindADoc = desc.GetData<int>("FindADoc"),
                                              Specialty = desc.GetData("Specialty"),
                                              Description = desc.GetData("Description"),
                                              Title = desc.GetData("Title"),
                                              DoctorTitleText = desc.GetData("DoctorTitleText"),
                                              LearnMore = desc.GetData("LearnMore")
                                          }).ToList<LearnMoreSpecialtyResult>();
            }

            if (this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
            {
                savingsData = (from savings in this.Tables[3].AsEnumerable()
                               select new SavingsResult
                               {
                                   ServiceDate = savings.GetData<DateTime>("ServiceDate"),
                                   ProcedureCode = savings.GetData("ProcedureCode"),
                                   AllowedAmount = savings.GetData<double>("AllowedAmount"),
                                   YouCouldHaveSaved = savings.GetData<double>("YouCouldHaveSaved"),
                                   OrganizationID = savings.GetData<int>("OrganizationID"),
                                   OrganizationLocationID = savings.GetData<int>("OrganizationLocationID"),
                                   SpecialtyID = savings.GetData<int>("SpecialtyID")
                               }).ToList<SavingsResult>();
            }
        }
        #endregion
    }
}