using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetLabTestLetterMenuOptions : DataBase
    {
        public List<dynamic> data;

        public dynamic Results { get { return data; } }
        public Int32 TotalResults { get { return data.Count; } }

        public GetLabTestLetterMenuOptions(string cnxString, string letter)
            : base("GetLabTestLetterMenuOptions")
        {
            this.Parameters.New("Letter", System.Data.SqlDbType.NVarChar, Size: 2, Value: letter);
            GetData(cnxString);
        }
        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);
            this.data = (from labs in this.Tables[0].AsEnumerable()
                         select new
                         {
                             ServiceID = labs.Field<Int32>("ServiceID"),
                             ServiceName = labs.Field<String>("ServiceName")
                         }).ToList<dynamic>();
        }
    }
}