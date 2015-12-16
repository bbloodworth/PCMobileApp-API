using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetDrugDetailOptions : DataBase
    {
        public Double Latitude { set { this.Parameters["Latitude"].Value = value; } }
        public Double Longitude { set { this.Parameters["Longitude"].Value = value; } }
        public Int32 DrugID { set { this.Parameters["DrugID"].Value = value; } }

        public List<dynamic> drugData = null;

        public dynamic Drugs { get { return (from dd in drugData.AsEnumerable() select new { dd.GPI, dd.Strength, dd.QuantityUOM }).Distinct(); } }
        public Int32 DistinctDrugs { get { return (from dd in drugData.AsEnumerable() select dd.GPI).Distinct().Count(); } }

        public GetDrugDetailOptions()
            : base("GetDrugDetailOptions")
        {
            this.Parameters.New("Latitude", System.Data.SqlDbType.Float);
            this.Parameters.New("Longitude", System.Data.SqlDbType.Float);
            this.Parameters.New("DrugID", System.Data.SqlDbType.Int);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            //this.drugData = (from drugs in this.Tables[0].AsEnumerable()
            //                 select new
            //                 {
            //                     GPI = drugs.Field<String>("GPI") ?? String.Empty,
            //                     Strength = drugs.Field<String>("Strength") ?? String.Empty,
            //                     QuantityUOM = drugs.Field<String>("QuantityUOM") ?? String.Empty,
            //                     Quantity = drugs.Field<Decimal>("Quantity"),
            //                     DisplayQty = drugs.Field<String>("DisplayQty") ?? String.Empty,
            //                     Pills = drugs.Field<String>("Pills") ?? String.Empty,
            //                     StrengthNumeric = drugs.Field<dynamic>("StrengthNumeric") //Updated to dynamic from Decimal due to nulls comming back in dev data.
            //                 }).ToList<dynamic>();

            this.drugData = (from drugs in this.Tables[0].AsEnumerable()
                             select new
                             {
                                 GPI = drugs.Field<String>("GPI") ?? String.Empty,
                                 Strength = drugs.Field<String>("Strength") ?? String.Empty,
                                 QuantityUOM = drugs.Field<String>("StandardDisplayName") ?? String.Empty,
                                 Quantity = drugs.Field<Decimal>("Quantity"),
                                 DisplayQty = drugs.Field<String>("DisplayQuantity") ?? String.Empty,
                                 Pills = drugs.Field<String>("DisplayQuantity").Split(' ').Count() == 2 ? drugs.Field<String>("DisplayQuantity").Split(' ')[1] : String.Empty,
                                 StrengthNumeric = drugs.Field<dynamic>("s2") //Updated to dynamic from Decimal due to nulls comming back in dev data.
                             }).ToList<dynamic>();
        }

        public dynamic GetQuantitiesForDrug(String GPI)
        {
            return (from dd in this.drugData.AsEnumerable()
                    where dd.GPI == GPI
                    select new 
                    { 
                        dd.GPI, 
                        dd.Quantity, 
                        dd.DisplayQty, 
                        dd.Pills, 
                        dd.StrengthNumeric 
                    }).Distinct();
        }
        public Int32 GetTotalQuantitiesForDrug(String GPI)
        {
            return (from dd in this.drugData.AsEnumerable()
                    where dd.GPI == GPI
                    select new { dd.Quantity, dd.Pills }).Distinct().Count();
        }

    }
}