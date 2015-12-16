using System;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections.Generic;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public sealed class GetDrugLetterMenu : DataBase
    {
        public List<dynamic> data;

        public dynamic Letters { get { return data; } }
        public Int32 TotalLetters { get { return data.Count; } }

        public GetDrugLetterMenu(String CnxString)
            : base("GetDrugLetterMenu")
        {
            GetData(CnxString);
        }
        public GetDrugLetterMenu(String CnxString, String Latitude, String Longitude)
            : base("GetDrugLetterMenu")
        {
            this.Parameters.New("Latitude", SqlDbType.Float, Value: Latitude);
            this.Parameters.New("Longitude", SqlDbType.Float, Value: Longitude);
            GetData(CnxString);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);
            this.data = (from letters in this.Tables[0].AsEnumerable()
                         select new
                         {
                             MenuLetter = letters.Field<String>("MenuLetter")
                         }).ToList<dynamic>();
        }
    }
}