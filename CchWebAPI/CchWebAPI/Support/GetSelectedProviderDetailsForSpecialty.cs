using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    using Models;
    using System.Data;
    [System.ComponentModel.DesignerCategory("")]
    public sealed class GetSelectedProviderDetailsForSpecialty : DataBase
    {
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        //public String ProviderName { set { this.Parameters["ProviderName"].Value = value; } }
        public String NPI { set { this.Parameters["NPI"].Value = value; } }
        //public String TaxID { set { this.Parameters["TaxID"].Value = value; } }
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public Int32 OrganizationLocationID { set { this.Parameters["OrganizationLocationID"].Value = value; } }

        private List<dynamic> results = new List<dynamic>();
        private List<dynamic> specialtyInfo = new List<dynamic>();
        private List<dynamic> healthGradesInfo = new List<dynamic>();

        public dynamic Results { get { return this.results.FirstOrDefault(); } }
        public dynamic SpecialtyInfo { get { return this.specialtyInfo.FirstOrDefault(); } }
        public dynamic HealthGradesInfo { get { return this.healthGradesInfo.FirstOrDefault(); } }

        public GetSelectedProviderDetailsForSpecialty()
            : base("GetSelectedProviderDetailsForSpecialty")
        {
            this.Parameters.New("SpecialtyID", System.Data.SqlDbType.Int);
            //this.Parameters.New("ProviderName", System.Data.SqlDbType.NVarChar, 150);
            this.Parameters.New("NPI", System.Data.SqlDbType.NVarChar, 50);
            //this.Parameters.New("TaxID", System.Data.SqlDbType.NVarChar, 15);
            this.Parameters.New("ServiceID", System.Data.SqlDbType.Int);
            this.Parameters.New("OrganizationLocationID", System.Data.SqlDbType.Int);           
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            if (this.Tables.Count >= 1 && this.Tables[0].Rows.Count > 0)
                results = (from result in this.Tables[0].AsEnumerable()
                         select new
                         {
                             ServiceName = result.Field<dynamic>("ServiceName"),
                             TaxID = result.Field<dynamic>("TaxID"),
                             NPI = result.Field<dynamic>("NPI"),
                             PracticeName = result.Field<dynamic>("PracticeName"),
                             ProviderName = result.Field<dynamic>("ProviderName"),
                             RangeMin = result.Field<dynamic>("RangeMin"),
                             RangeMax = result.Field<dynamic>("RangeMax"),
                             Latitude = result.Field<dynamic>("Latitude"),
                             Longitude = result.Field<dynamic>("Longitude"),
                             LocationAddress1 = result.Field<dynamic>("LocationAddress1"),
                             LocationAddress2 = result.Field<dynamic>("LocationAddress2"),
                             LocationCity = result.Field<dynamic>("LocationCity"),
                             LocationState = result.Field<dynamic>("LocationState"),
                             LocationZip = result.Field<dynamic>("LocationZip"),
                             LocationTelephone = result.Field<dynamic>("LocationTelephone"),
                             FairPrice = result.Field<dynamic>("FairPrice"),
                             HGRecognized = result.Field<dynamic>("HGRecognized"),
                             HGProviderID = result.Field<dynamic>("HGProviderID"),
                             OtherKeyItem1 = result.Field<dynamic>("OtherKeyItem1"),
                             OtherKeyItem1_Text = result.Field<dynamic>("OtherKeyItem1_Text"),
                             OtherKeyItem1_LearnMoreTitle = result.Field<dynamic>("OtherKeyItem1_LearnMoreTitle"),
                             OtherKeyItem1_LearnMoreText = result.Field<dynamic>("OtherKeyItem1_LearnMoreText")
                         }).ToList<dynamic>();
            if (this.Tables.Count >= 2 && this.Tables[1].Rows.Count > 0)
                specialtyInfo = (from si in this.Tables[1].AsEnumerable()
                                 select new
                                 {
                                     Description = si.Field<String>("Description"),
                                     Title = si.Field<String>("Title"),
                                     SpecialtyTitle = si.Field<String>("SpecialtyTitle"),
                                     ServiceName = si.Field<String>("ServiceName")
                                 }).ToList<dynamic>();
            if (this.Tables.Count >= 3 && this.Tables[2].Rows.Count > 0)
                healthGradesInfo = (from hgi in this.Tables[2].AsEnumerable()
                                    where !(hgi.Field<string>("Office") == null && 
                                            hgi.Field<string>("Procedures") == null)
                                    select new
                                    {
                                        Overall = hgi.Field<dynamic>("Overall"),
                                        Office = hgi.Field<dynamic>("Office"),
                                        Procedures = hgi.Field<dynamic>("Procedures"),
                                        Lab = hgi.Field<dynamic>("Lab"),
                                        Imaging = hgi.Field<dynamic>("Imaging"),
                                        SpecialtyReferals = hgi.Field<dynamic>("SpecialtyReferals")
                                    }).ToList<dynamic>();
        }

        public void EncryptTaxIDs(CCHEncrypt ce)
        {
            ce.Add("TaxID", "");
            List<dynamic> newD = new List<dynamic>();
            foreach (dynamic doc in this.results)
            {
                ce["TaxID"] = doc.TaxID;
                newD.Add(new
                {
                    doc.ServiceName,
                    TaxId = ce.ToString(),
                    doc.NPI,
                    doc.PracticeName,
                    doc.ProviderName,
                    doc.RangeMin,
                    doc.RangeMax,
                    doc.Latitude,
                    doc.Longitude,
                    doc.LocationAddress1,
                    doc.LocationAddress2,
                    doc.LocationCity,
                    doc.LocationState,
                    doc.LocationZip,
                    doc.LocationTelephone,
                    doc.FairPrice,
                    doc.HGRecognized,
                    doc.HGProviderID,
                    doc.OtherKeyItem1,
                    doc.OtherKeyItem1_Text,
                    doc.OtherKeyItem1_LearnMoreTitle,
                    doc.OtherKeyItem1_LearnMoreText
                });
            }

            this.results = newD;
        }
    }
}