using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Dynamic;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")] //Used to prevent VS from opening the class in design view on double click
    public class GetDoctorsForServiceAlt :DataBase
    {
        #region Public Data Structures
        public struct AlternateProviderResult
        {
            public bool AlternateProviderData;
        }
        public struct DoctorResult
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
            public bool AntiTransparency;
        }
        public struct DescriptionResult
        {
            public string Description;
        }
        public struct SpecialtyResult
        {
            public int FindADoc;
            public string Specialty;
            public string Description;
            public string Title;
            public string DoctorTitleText;
            public string LearnMore;
        }
        public struct ThinDataResult 
        {
            public bool ThinData;
        }
        #endregion

        #region Public SQL Parameter Value Set Properties
        public String ServiceName { set { this.Parameters["ServiceName"].Value = value; } }
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Boolean DD { set { this.Parameters["DD"].Value = value; } }
        private Int32 _totalDocs = 0;  //  lam, 20130730, MOB-33
        #endregion

        #region Public "On Occasion" SQL Parameter Value Set Properties
        public Int32 ServiceID { set { this.Parameters.New("ServiceID", SqlDbType.Int, Value: value); } }
        public Int32 Distance { set { this.Parameters.New("Distance", SqlDbType.Int, Value: value); } }
        public new String OrderByField { set { this.Parameters.New("OrderByField", SqlDbType.NVarChar, 50, value); } }
        public new String OrderDirection { set { this.Parameters.New("OrderDirection", SqlDbType.NVarChar, 4, value); } }
        public String MemberMedicalID { set { this.Parameters.New("MemberMedicalID", SqlDbType.NVarChar, 50, value); } }
        public Int32 FromRow { set { this.Parameters.New("FromRow", SqlDbType.Int, Value: value); fromRow = value; } }
        public Int32 ToRow { set { this.Parameters.New("ToRow", SqlDbType.Int, Value: value); } }
        #endregion

        #region Private Result Lists
        private List<DoctorResult> doctors = new List<DoctorResult>();
        private List<DescriptionResult> description = new List<DescriptionResult>();
        private List<SpecialtyResult> specialties = new List<SpecialtyResult>();
        private List<ThinDataResult> thinData = new List<ThinDataResult>();
        private List<DoctorResult> preferredDocs = new List<DoctorResult>();
        private bool endOfResults = false;
        private AlternateProviderResult alternateProviderResult;
        private Int32 fromRow;
        #endregion

        #region Public Result Data Properties
        public List<DoctorResult> Doctors { get { return this.doctors; } }
        public Int32 MinRange { get { return (from ds in this.Tables[0].AsEnumerable() select ds.GetData<int>("RangeMin")).Min(); } }
        public Int32 MaxRange { get { return (from ds in this.Tables[0].AsEnumerable() select ds.GetData<int>("RangeMax")).Max(); } }
        //public Int32 TotalDocs { get { return (this.doctors ?? new List<DoctorResult>()).Count; } }
        public Int32 TotalDocs { get { return (_totalDocs); } }  //  lam, 20130730, MOB-33
        public String Description { get { return this.description.Count > 0 ? this.description.First().Description : string.Empty; } }
        public SpecialtyResult SpecialtyInfo { get { return this.specialties.FirstOrDefault(); } } 
        public Boolean ThinData { get { return this.thinData.Count > 0 ? this.thinData.First().ThinData : false; } }
        public List<DoctorResult> PreferredDocs { get { return this.preferredDocs; } }
        public Boolean EndOfResults { get { return endOfResults; } }
        public bool AlternateProvider { get { return alternateProviderResult.AlternateProviderData; } }
        #endregion

        public GetDoctorsForServiceAlt()
            : base("GetDoctorsForService_Alt")
        {
            this.Parameters.New("ServiceName", System.Data.SqlDbType.NVarChar, Size: 200);
            this.Parameters.New("Latitude", System.Data.SqlDbType.Float);
            this.Parameters.New("Longitude", System.Data.SqlDbType.Float);
            this.Parameters.New("SpecialtyID", System.Data.SqlDbType.Int);
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("UserID", System.Data.SqlDbType.NVarChar, Size: 36);
            this.Parameters.New("DD", System.Data.SqlDbType.Bit);
        }

        #region Methods
        public void GetData(string ConnectionString, CCHEncrypt ce)
        {
            base.GetData(ConnectionString);

            ce.Add("TaxID", "");

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0 )
            { 
                _totalDocs = (this.Tables[0] != null ? this.Tables[0].Rows.Count : 0);  //  lam, 20130730, MOB-33

                doctors = (from docRow in this.Tables[0].AsEnumerable()
                           select new DoctorResult
                           {
                               ServiceName = docRow.GetData("ServiceName"),
                               TaxId = docRow.EncryptTaxID(ce),
                               NPI = docRow.GetData("NPI"),
                               PracticeName = docRow.GetData("PracticeName"),
                               OrganizationLocationID = docRow.GetData("OrganizationLocationID"),
                               ProviderName = docRow.GetData("ProviderName"),
                               RangeMin = docRow.GetData<int>("RangeMin"),
                               RangeMax = docRow.GetData<int>("RangeMax"),
                               Latitude = docRow.GetData<double>("Latitude"),
                               Longitude = docRow.GetData<double>("Longitude"),
                               LocationAddress1 = docRow.GetData("LocationAddress1"),
                               YourCostMin = docRow.GetData<int>("YourCostMin"),
                               YourCostMax = docRow.GetData<int>("YourCostMax"),
                               LocationCity = docRow.GetData("LocationCity"),
                               LocationState = docRow.GetData("LocationState"),
                               LocationZip = docRow.GetData("LocationZip"),
                               LocationPhone = docRow.GetData("LocationTelephone"),
                               RowNumber = docRow.GetData<int>("RowNumber"),
                               Distance = docRow.GetData("Distance"),
                               NumericDistance = docRow.GetData<decimal>("NumericDistance"),
                               FairPrice = docRow.GetData<bool>("FairPrice"),
                               HGRecognized = docRow.GetData<int>("HGRecognized"),
                               LatLong = docRow.GetData("LatLong"),
                               HGOverallRating = docRow.GetData<double>("HGOverallRating"),
                               HGPatientCount = docRow.GetData<int>("HGPatientCount"),
                               RowOrderKey = docRow.GetData<int>("RowOrderKey"),
                               AntiTransparency = docRow.GetData<bool>("AntiTransparency")
                           }).ToList<DoctorResult>();
            }
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
                description = (from desc in this.Tables[1].AsEnumerable()
                               select new DescriptionResult
                               {
                                   Description = desc.GetData("Description")
                               }).ToList<DescriptionResult>();
            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
                specialties = (from specs in this.Tables[2].AsEnumerable()
                               select new SpecialtyResult
                               {
                                   FindADoc = specs.GetData<int>("FindADoc"),
                                   Specialty = specs.GetData("Specialty"),
                                   Description = specs.GetData("Description"),
                                   Title = specs.GetData("Title"),
                                   DoctorTitleText = specs.GetData("DoctorTitleText"),
                                   LearnMore = specs.GetData("LearnMore")
                               }).ToList<SpecialtyResult>();
            if (this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
            {
                thinData = (from td in this.Tables[3].AsEnumerable()
                            select new ThinDataResult
                            {
                                ThinData = td.GetData<bool>("ThinData")
                            }).ToList<ThinDataResult>();

                alternateProviderResult = (from specs in this.Tables[3].AsEnumerable()
                                           select new AlternateProviderResult
                                           {
                                               AlternateProviderData = specs.GetData<bool>("AltFlg")
                                           }).First<AlternateProviderResult>();
            }
            if (this.Tables.Count >= 5 && this.Tables[4].Rows.Count > 0)
            {
                if (fromRow == 0)
                {
                    preferredDocs = (from prefDocRow in this.Tables[4].AsEnumerable()
                                     where prefDocRow.GetData("ServiceName") != null 
                                     select new DoctorResult
                                     {
                                         ServiceName = prefDocRow.GetData("ServiceName"),
                                         TaxId = prefDocRow.EncryptTaxID(ce),
                                         NPI = prefDocRow.GetData("NPI"),
                                         PracticeName = prefDocRow.GetData("PracticeName"),
                                         OrganizationLocationID = prefDocRow.GetData("OrganizationLocationID"),
                                         ProviderName = prefDocRow.GetData("ProviderName"),
                                         RangeMin = prefDocRow.GetData<int>("RangeMin"),
                                         RangeMax = prefDocRow.GetData<int>("RangeMax"),
                                         Latitude = prefDocRow.GetData<double>("Latitude"),
                                         Longitude = prefDocRow.GetData<double>("Longitude"),
                                         LocationAddress1 = prefDocRow.GetData("LocationAddress1"),
                                         YourCostMin = prefDocRow.GetData<int>("YourCostMin"),
                                         YourCostMax = prefDocRow.GetData<int>("YourCostMax"),
                                         LocationCity = prefDocRow.GetData("LocationCity"),
                                         LocationState = prefDocRow.GetData("LocationState"),
                                         LocationZip = prefDocRow.GetData("LocationZip"),
                                         LocationPhone = prefDocRow.GetData("LocationTelephone"),
                                         RowNumber = prefDocRow.GetData<int>("RowNumber"),
                                         Distance = prefDocRow.GetData("Distance"),
                                         NumericDistance = prefDocRow.GetData<decimal>("NumericDistance"),
                                         FairPrice = prefDocRow.GetData<bool>("FairPrice"),
                                         HGRecognized = prefDocRow.GetData<int>("HGRecognized"),
                                         LatLong = prefDocRow.GetData("LatLong"),
                                         HGOverallRating = prefDocRow.GetData<double>("HGOverallRating"),
                                         HGPatientCount = prefDocRow.GetData<int>("HGPatientCount"),
                                         RowOrderKey = prefDocRow.GetData<int>("RowOrderKey"),
                                         AntiTransparency = prefDocRow.GetData<bool>("AntiTransparency")
                                     }).ToList<DoctorResult>();
                    _totalDocs += (preferredDocs != null && preferredDocs.Count > 0 ? preferredDocs.Count : 0); 
                }
            }
        }
        #endregion
    }
}