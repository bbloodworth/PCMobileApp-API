using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetSelectedPharmacyDetails : DataBase
    {
        public Int32 DrugID { set { this.Parameters["DrugID"].Value = value; } }
        public String GPI { set { this.Parameters["GPI"].Value = value; } }
        public Double Quantity { set { this.Parameters["Quantity"].Value = value; } }
        public Int32 PharmacyID { set { this.Parameters["PharmacyID"].Value = value; } }
        public Int32 PharmacyLocationID { set { this.Parameters["PharmacyLocationID"].Value = value; } }
        public Int32 PastCareID { set { this.Parameters.New("PastCareID", SqlDbType.Int, Value: value); } }

        private List<dynamic> data = null;
        private List<dynamic> tData = null;
        private List<dynamic> transfer = null;

        public dynamic Details { get { return (this.data.Count > 1 ? this.data : this.data.FirstOrDefault()); } }
        public Int32 TotalDetails { get { return this.data.Count; } }
        public bool HasTransferDetails { get { return this.tData.First().ShowTransferInfo == 1 ? true : false; } }
        public dynamic TransferInfo { get { return (this.transfer.Count > 1 ? this.transfer : this.transfer.First()); } }

        public GetSelectedPharmacyDetails()
            : base("GetSelectedPharmacyDetails")
        {
            this.Parameters.New("DrugID", System.Data.SqlDbType.Int);
            this.Parameters.New("GPI", System.Data.SqlDbType.NVarChar, 50);
            this.Parameters.New("Quantity", System.Data.SqlDbType.Decimal);
            this.Parameters.New("PharmacyID", System.Data.SqlDbType.Int);
            this.Parameters.New("PharmacyLocationID", System.Data.SqlDbType.Int);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            data = (from details in this.Tables[0].AsEnumerable()
                    select new
                    {
                        Details = details.Field<String>("Details"),
                        PharmacyName = details.Field<String>("PharmacyName"),
                        WebURL = details.Field<String>("WebURL"),
                        Address1 = details.Field<String>("Address1"),
                        Address2 = details.Field<String>("Address2"),
                        City = details.Field<String>("City"),
                        State = details.Field<String>("State"),
                        Zipcode = details.Field<String>("Zipcode"),
                        Telephone = details.Field<String>("Telephone"),
                        Email = details.Field<String>("Email"),
                        Latitude = details.Field<Double>("Latitude"),
                        Longitude = details.Field<Double>("Longitude"),
                        Fax = details.Field<String>("Fax"),
                        Hours = details.Field<String>("Hours"),
                        Price = details.Field<Decimal>("Price"),
                        PharmacyID = details.Field<Int32>("PharmacyID"),
                        PharmacyLocationID = details.Field<Int32>("PharmacyLocationID")
                    }).ToList<dynamic>();
            tData = (from transfer in this.Tables[1].AsEnumerable()
                     select new
                     {
                         ShowTransferInfo = transfer.Field<dynamic>("ShowTransferInfo")
                     }).ToList<dynamic>();
            if (tData[0].ShowTransferInfo == 1)
            {
                transfer = (from tdetails in this.Tables[2].AsEnumerable()
                            select new
                            {
                                Details = tdetails.Field<String>("Details"),
                                PharmacyName = tdetails.Field<String>("PharmacyName"),
                                WebURL = tdetails.Field<String>("WebURL"),
                                Address1 = tdetails.Field<String>("Address1"),
                                Address2 = tdetails.Field<String>("Address2"),
                                City = tdetails.Field<String>("City"),
                                State = tdetails.Field<String>("State"),
                                Zipcode = tdetails.Field<String>("Zipcode"),
                                Telephone = tdetails.Field<String>("Telephone"),
                                Email = tdetails.Field<String>("Email"),
                                Latitude = tdetails.Field<Double>("Latitude"),
                                Longitude = tdetails.Field<Double>("Longitude"),
                                Fax = tdetails.Field<String>("Fax"),
                                Hours = tdetails.Field<String>("Hours"),
                                Price = tdetails.Field<Decimal>("Price"),
                                PrescriptionNumber = tdetails.Field<String>("PrescriptionNumber"),
                                PharmacyID = tdetails.Field<Int32>("PharmacyID"),
                                PharmacyLocationID = tdetails.Field<Int32>("PharmacyLocationID")
                            }).ToList<dynamic>();
            }
        }
    }
}