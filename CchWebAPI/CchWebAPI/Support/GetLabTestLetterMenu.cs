using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetLabTestLetterMenu : DataBase
    {
        public List<dynamic> data;

        public dynamic Letters { get { return data; } }
        public Int32 TotalLetters { get { return data.Count; } }

        public GetLabTestLetterMenu(String CnxString)
            : base("GetLabTestLetterMenu")
        {
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