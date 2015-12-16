using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Dynamic;
using CchWebAPI.Models;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetFacilitiesForServiceAlt : DataBase
    {
        #region Public Data Structures
        public struct AlternateProviderResult
        {
            public bool AlternateProviderData;
        }
        public struct HealthGradeDisplayTypeResult
        {
            public string HealthGradeResultTypeData;
        }
        public struct FacilityResult
        {
            public string ServiceName;
            public string TaxId;
            public string NPI;
            public string PracticeName;
            public string OrganizationLocationID;
            public string ProviderName;
            public int RangeMin;
            public int RangeMax;
            public double Latitude;
            public double Longitude;
            public string LocationAddress1;
            public int YourCostMin;
            public int YourCostMax;
            public string LocationCity;
            public string LocationState;
            public string LocationZip;
            public string LocationPhone;
            public int RowNumber;
            public string Distance;
            public decimal NumericDistance;
            public bool FairPrice;
            public int HGRecognized;
            public string LatLong;
            public double HGOverallRating;
            public int HGPatientCount;
            public int RowOrderKey;
            public int HGRecognizedDocCount;
            public int HGDocCount;
            public string HGOfficeID;
            public bool AntiTransparency;
            public string LSN;
        }
        public struct DescriptionResult
        {
            public string Description;
        }
        public struct ThinDataResult
        {
            public bool ThinData;
        }
        #endregion

        #region Public SQL Parameter Value Set Properties
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
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
        private List<FacilityResult> resultData = new List<FacilityResult>();
        private List<DescriptionResult> learnMoreData = new List<DescriptionResult>();
        private List<ThinDataResult> thinDataData = new List<ThinDataResult>();
        private List<FacilityResult> preferredData = new List<FacilityResult>();
        private AlternateProviderResult alternateProviderResult;
        private HealthGradeDisplayTypeResult healthGradeDisplayTypeResult;
        
        #endregion

        #region Public Result Data Properties
        public List<FacilityResult> ResultData { get { return this.resultData; } }
        public Int32 MinRange { get { return (from ds in this.Tables[0].AsEnumerable() select ds.GetData<int>("RangeMin")).Min(); } }
        public Int32 MaxRange { get { return (from ds in this.Tables[0].AsEnumerable() select ds.GetData<int>("RangeMax")).Max(); } }
        public Int32 TotalResults { get { return this.ResultData.Count; } }
        public String LearnMoreData { get { return (this.learnMoreData.Count > 0 ? this.learnMoreData.First().Description : string.Empty); } }
        public Boolean ThinDataData { get { return (this.thinDataData.Count > 0 ? thinDataData.First().ThinData : false); } }
        public List<FacilityResult> PreferredData { get { return this.preferredData; } }
        public bool AlternateProvider { get { return alternateProviderResult.AlternateProviderData; } }
        public string HealthGradeDisplayType { get { return healthGradeDisplayTypeResult.HealthGradeResultTypeData; } }

        #endregion

        public GetFacilitiesForServiceAlt()
            : base("GetFacilitiesForService_Alt")
        {
            this.Parameters.New("Latitude", System.Data.SqlDbType.Float);
            this.Parameters.New("Longitude", System.Data.SqlDbType.Float);
            this.Parameters.New("SpecialtyID", System.Data.SqlDbType.Int);
            this.Parameters.New("ServiceID", System.Data.SqlDbType.Int);
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("UserID", System.Data.SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("DD", SqlDbType.Bit);
        }

        #region Methods
        public void GetData(string ConnectionString, CCHEncrypt ce)
        {
            base.GetData(ConnectionString);

            ce.Add("TaxID", "");

            //Regular results
            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                Int32 rangeStart = 0, rangeCount = this.Tables[0].Rows.Count;
                if (ToRow - FromRow <= rangeCount)
                {
                    if (FromRow > 0) rangeStart = FromRow;
                    if (ToRow > 0 && ToRow < rangeCount) rangeCount = ToRow - FromRow + 1;
                }

                resultData = (from result in this.Tables[0].AsEnumerable()
                              select new FacilityResult
                              {
                                  ServiceName = result.GetData("ServiceName"),
                                  TaxId = result.EncryptTaxID(ce),
                                  NPI = result.GetData("NPI"),
                                  PracticeName = result.GetData("PracticeName"),
                                  OrganizationLocationID = result.GetData("OrganizationLocationID"),
                                  ProviderName = result.GetData("ProviderName"),
                                  RangeMin = result.GetData<int>("RangeMin"),
                                  RangeMax = result.GetData<int>("RangeMax"),
                                  Latitude = result.GetData<double>("Latitude"),
                                  Longitude = result.GetData<double>("Longitude"),
                                  LocationAddress1 = result.GetData("LocationAddress1"),
                                  YourCostMin = result.GetData<int>("YourCostMin"),
                                  YourCostMax = result.GetData<int>("YourCostMax"),
                                  LocationCity = result.GetData("LocationCity"),
                                  LocationState = result.GetData("LocationState"),
                                  LocationZip = result.GetData("LocationZip"),
                                  LocationPhone = result.GetData("LocationTelephone"),
                                  RowNumber = result.GetData<int>("RowNumber"),
                                  Distance = result.GetData("Distance"),
                                  NumericDistance = result.GetData<decimal>("NumericDistance"),
                                  FairPrice = result.GetData<bool>("FairPrice"),
                                  HGRecognized = result.GetData<int>("HGRecognized"),
                                  LatLong = result.GetData("LatLong"),
                                  HGOverallRating = result.GetData<double>("HGOverallRating"),
                                  HGPatientCount = result.GetData<int>("HGPatientCount"),
                                  RowOrderKey = result.GetData<int>("RowOrderKey"),
                                  HGRecognizedDocCount = result.GetData<int>("HGRecognizedDocCount"),
                                  HGDocCount = result.GetData<int>("HGDocCount"),
                                  HGOfficeID = result.GetData("HGOfficeID"),
                                  AntiTransparency = result.GetData<bool>("AntiTransparency"),
                                  LSN = result.GetData("LSN")
                              }).ToList<FacilityResult>();//.GetRange(rangeStart, rangeCount);
            }

            // Description Info
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
                learnMoreData = (from desc in this.Tables[1].AsEnumerable()
                                 select new DescriptionResult
                                 {
                                     Description = desc.GetData("Description")
                                 }).ToList<DescriptionResult>();

            // Thin Data Info
            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
            {
                thinDataData = (from specs in this.Tables[2].AsEnumerable()
                                select new ThinDataResult
                                {
                                    ThinData = specs.GetData<bool>("ThinData")
                                }).ToList<ThinDataResult>();

                alternateProviderResult = (from specs in this.Tables[2].AsEnumerable()
                                           select new AlternateProviderResult
                                           {
                                               AlternateProviderData = specs.GetData<bool>("AltFlg")
                                           }).First<AlternateProviderResult>();
                healthGradeDisplayTypeResult = (from specs in this.Tables[2].AsEnumerable()
                                                select new HealthGradeDisplayTypeResult
                                                {
                                                    HealthGradeResultTypeData = specs.GetData("NCCTFaSHGResultsDisplayType")
                                                }).FirstOrDefault<HealthGradeDisplayTypeResult>();
            }

            // Preferred Providers
            if (this.Tables.Count >= 5 && this.Tables[4].Rows.Count > 0)
                if (FromRow == 0)
                {
                    preferredData = (from result in this.Tables[4].AsEnumerable()
                                     select new FacilityResult
                                     {
                                         ServiceName = result.GetData("ServiceName"),
                                         TaxId = result.EncryptTaxID(ce),
                                         NPI = result.GetData("NPI"),
                                         PracticeName = result.GetData("PracticeName"),
                                         OrganizationLocationID = result.GetData("OrganizationLocationID"),
                                         ProviderName = result.GetData("ProviderName"),
                                         RangeMin = result.GetData<int>("RangeMin"),
                                         RangeMax = result.GetData<int>("RangeMax"),
                                         Latitude = result.GetData<double>("Latitude"),
                                         Longitude = result.GetData<double>("Longitude"),
                                         LocationAddress1 = result.GetData("LocationAddress1"),
                                         YourCostMin = result.GetData<int>("YourCostMin"),
                                         YourCostMax = result.GetData<int>("YourCostMax"),
                                         LocationCity = result.GetData("LocationCity"),
                                         LocationState = result.GetData("LocationState"),
                                         LocationZip = result.GetData("LocationZip"),
                                         LocationPhone = result.GetData("LocationTelephone"),
                                         RowNumber = result.GetData<int>("RowNumber"),
                                         Distance = result.GetData("Distance"),
                                         NumericDistance = result.GetData<decimal>("NumericDistance"),
                                         FairPrice = result.GetData<bool>("FairPrice"),
                                         HGRecognized = result.GetData<int>("HGRecognized"),
                                         LatLong = result.GetData("LatLong"),
                                         HGOverallRating = result.GetData<double>("HGOverallRating"),
                                         HGPatientCount = result.GetData<int>("HGPatientCount"),
                                         RowOrderKey = result.GetData<int>("RowOrderKey"),
                                         HGRecognizedDocCount = result.GetData<int>("HGRecognizedDocCount"),
                                         HGDocCount = result.GetData<int>("HGDocCount"),
                                         HGOfficeID = result.GetData("HGOfficeID"),
                                         AntiTransparency = result.GetData<bool>("AntiTransparency"),
                                         LSN = result.GetData("LSN")
                                     }).ToList<FacilityResult>();
                }
        }
        #endregion
    }
}