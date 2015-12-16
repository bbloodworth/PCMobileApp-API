using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetDrugList_AutoComplete : DataBase
    {
        public String SearchString { set { this.Parameters["SearchString"].Value = value; } }

        private List<dynamic> data;

        public dynamic Suggestions { get { return this.data; } }
        public Int32 TotalSuggestions { get { return this.data.Count; } }

        public GetDrugList_AutoComplete()
            : base("GetDrugList_AutoComplete")
        {
            this.Parameters.New("SearchString", System.Data.SqlDbType.NVarChar, 100);
        }
        public GetDrugList_AutoComplete(String CnxString, String SearchString)
        {
            this.Parameters.New("SearchString", System.Data.SqlDbType.NVarChar, 100, SearchString);
            this.GetData(CnxString);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);
            data = (from suggestions in this.Tables[0].AsEnumerable()
                    select new
                    {
                        Rank = suggestions.Field<Int32>("Rank"),
                        DrugName = suggestions.Field<String>("DrugName"),
                        DrugID = suggestions.Field<Int32>("DrugID")
                    }).ToList<dynamic>();
        }
    }
}