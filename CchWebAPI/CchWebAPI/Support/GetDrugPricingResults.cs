using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetDrugPricingResults : DataBase
    {
        public Int32 DrugID { set { this.Parameters["DrugID"].Value = value; } }
        public String GPI { set { this.Parameters["GPI"].Value = value; } }
        public Double Quantity { set { this.Parameters["Quantity"].Value = value; } }
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 CCHID { set { this.Parameters["CCHID"].Value = value; } }
        public String UserID { set { this.Parameters["UserID"].Value = value; } }
        public Boolean DD { set { this.Parameters["DD"].Value = value; } }
        public Int32 PastCareID { set { this.Parameters.New("PastCareID", SqlDbType.Int, Value: value); } }
        private Int32 _totalPharmacies = 0;  //  lam, 20130730, MOB-33

        public Int32 Distance { get { return (Int32)Parameters["Distance"].Value; } set { this.Parameters.New("Distance", System.Data.SqlDbType.Int, Value: value); } }
        public new String OrderByField { get { return (String)Parameters["OrderByField"].Value; } set { this.Parameters.New("OrderByField", System.Data.SqlDbType.NVarChar, 50, value); } }
        public new String OrderDirection { get { return (String)Parameters["OrderDirection"].Value; } set { this.Parameters.New("OrderDirection", System.Data.SqlDbType.NVarChar, 4, value); } }
        public Int32 FromRow { get { return  (Int32)(Parameters["FromRow"].Value ?? 0); } set { this.Parameters.New("FromRow", System.Data.SqlDbType.Int, Value: value); } }
        public Int32 ToRow { get { return (Int32)(Parameters["ToRow"].Value ?? 0); } set { this.Parameters.New("ToRow", System.Data.SqlDbType.Int, Value: value); } }

        private List<dynamic> resultData = null;
        private List<dynamic> cpData = null;
        private List<dynamic> bpData = null;
        private List<dynamic> diData = null;

        public dynamic Pharmacies { get { return (resultData ?? new List<dynamic>()); } }
        public Decimal MinPrice { get { return (resultData ?? new List<dynamic>()).DefaultIfEmpty(new { Price = 0 }).Min(pharmacy => (Decimal)pharmacy.Price); } }
        public Decimal MaxPrice { get { return (resultData ?? new List<dynamic>()).DefaultIfEmpty(new { Price = 0 }).Max(pharmacy => (Decimal)pharmacy.Price); } }
        //public Int32 TotalPharmacies { get { return (resultData ?? new List<dynamic>()).Count; } }  lam, 20130730, MOB-33
        public Int32 TotalPharmacies { get { return (resultData != null ? _totalPharmacies : 0); } }  //  lam, 20130730, MOB-33
        public dynamic CurrentPharmacy { get { return (cpData == null ? new { } : (cpData.Count > 1 ? cpData : cpData.First())); } }
        public dynamic BestPrice { get { return (bpData == null ? new { } : (bpData.Count > 1 ? bpData : bpData.First().BestPrice)); } }
        public dynamic DrugInfo { get { return (diData == null ? new { } : (diData.Count > 1 ? diData : diData.First())); } }
        

        public GetDrugPricingResults()
            : base("GetDrugPricingResults")
        {
            this.Parameters.New("DrugID", System.Data.SqlDbType.Int);
            this.Parameters.New("GPI", System.Data.SqlDbType.NVarChar, 50);
            this.Parameters.New("Quantity", System.Data.SqlDbType.Decimal);
            this.Parameters.New("Latitude", System.Data.SqlDbType.Float);
            this.Parameters.New("Longitude", System.Data.SqlDbType.Float);
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
            this.Parameters.New("UserID", System.Data.SqlDbType.NVarChar, 36);
            this.Parameters.New("DD", System.Data.SqlDbType.Bit);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);
            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
            {
                _totalPharmacies = (this.Tables[0] != null ? this.Tables[0].Rows.Count : 0);
                this.resultData = (from pharmacies in this.Tables[0].AsEnumerable()
                                   select new
                                   {
                                       PharmacyName = pharmacies.Field<String>("PharmacyName"),
                                       WebURL = pharmacies.Field<String>("WebURL"),
                                       Address1 = pharmacies.Field<String>("Address1"),
                                       Address2 = pharmacies.Field<String>("Address2"),
                                       City = pharmacies.Field<String>("City"),
                                       State = pharmacies.Field<String>("State"),
                                       Zipcode = pharmacies.Field<String>("Zipcode"),
                                       Telephone = pharmacies.Field<String>("Telephone"),
                                       Email = pharmacies.Field<String>("Email"),
                                       Latitude = pharmacies.Field<Double>("Latitude"),
                                       Longitude = pharmacies.Field<Double>("Longitude"),
                                       Price = pharmacies.Field<Decimal>("Price"),
                                       YourCost = pharmacies.Field<Decimal>("YourCost"),
                                       Distance = pharmacies.Field<Decimal>("Distance"),
                                       PharmacyID = pharmacies.Field<Int32>("PharmacyID"),
                                       PharmacyLocationID = pharmacies.Field<Int32>("PharmacyLocationID"),
                                       MAIL_RETAIL_IND = pharmacies.Field<String>("MAIL_RETAIL_IND"),
                                       CurrentPharmText = pharmacies.Field<String>("CurrentPharmText"),
                                       BestPriceText = pharmacies.Field<String>("BestPriceText"),
                                       RowOrderKey = pharmacies.Field<Int32>("RowOrderKey")
                                   }).ToList<dynamic>();
            }
            //if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0 && FromRow <= this.Tables[0].Rows.Count)
            //{
            //    Int32 rangeStart = 0, rangeCount = this.Tables[0].Rows.Count;
            //    if (ToRow - FromRow <= rangeCount)
            //    {
            //        if (FromRow > 0) rangeStart = FromRow;
            //        if (ToRow > 0 && ToRow < rangeCount) rangeCount = ToRow - FromRow + 1;
            //        if (FromRow > 0 && ToRow >= this.Tables[0].Rows.Count) rangeCount = this.Tables[0].Rows.Count - FromRow;  //  lam, 20130730, MOB-95 added "=" after ">" ToRow > this.Tables[0].Rows.Count
            //    }
            //    _totalPharmacies = (this.Tables[0] != null ? this.Tables[0].Rows.Count : 0);  //  lam, 20130730, MOB-33
            //    this.resultData = (from pharmacies in this.Tables[0].AsEnumerable()
            //                       select new
            //                       {
            //                           PharmacyName = pharmacies.Field<String>("PharmacyName"),
            //                           WebURL = pharmacies.Field<String>("WebURL"),
            //                           Address1 = pharmacies.Field<String>("Address1"),
            //                           Address2 = pharmacies.Field<String>("Address2"),
            //                           City = pharmacies.Field<String>("City"),
            //                           State = pharmacies.Field<String>("State"),
            //                           Zipcode = pharmacies.Field<String>("Zipcode"),
            //                           Telephone = pharmacies.Field<String>("Telephone"),
            //                           Email = pharmacies.Field<String>("Email"),
            //                           Latitude = pharmacies.Field<Double>("Latitude"),
            //                           Longitude = pharmacies.Field<Double>("Longitude"),
            //                           Price = pharmacies.Field<Decimal>("Price"),
            //                           YourCost = pharmacies.Field<Decimal>("YourCost"),
            //                           Distance = pharmacies.Field<Int32>("Distance"), 
            //                           PharmacyID = pharmacies.Field<Int32>("PharmacyID"),
            //                           PharmacyLocationID = pharmacies.Field<Int32>("PharmacyLocationID"),
            //                           MAIL_RETAIL_IND = pharmacies.Field<String>("MAIL_RETAIL_IND"),
            //                           CurrentPharmText = pharmacies.Field<String>("CurrentPharmText"),
            //                           BestPriceText = pharmacies.Field<String>("BestPriceText"),
            //                           RowOrderKey = pharmacies.Field<Int32>("RowOrderKey")
            //                       }).ToList<dynamic>().GetRange(rangeStart, rangeCount);
            //}
            if(this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
                this.cpData = (from cp in this.Tables[1].AsEnumerable()
                               select new
                               {
                                   ShowCurrentPharmacy = cp.Field<String>("ShowCurrentPharmacy"),
                                   CurrentPrice = cp.Field<dynamic>("CurrentPrice")
                               }).ToList<dynamic>();
            if(this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
                this.bpData = (from bp in this.Tables[2].AsEnumerable()
                               select new
                               {
                                   BestPrice = (Decimal)(bp.Field<dynamic>("BestPrice") ?? 0)
                               }).ToList<dynamic>();
            if(this.Tables.Count >= 4 && this.Tables[3].Rows.Count > 0)
                this.diData = (from di in this.Tables[3].AsEnumerable()
                               select new
                               {
                                   DrugDisplayName = di.Field<String>("DrugDisplayName"),
                                   Strength = di.Field<String>("Strength")
                               }).ToList<dynamic>();
            
        }
    }
}