using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetServiceList_AutoComplete : DataBase
    {
        public String SearchString { set { this.Parameters["SearchString"].Value = value; } }

        public GetServiceList_AutoComplete()
            : base("GetServiceList_AutoComplete")
        {
            this.Parameters.New("SearchString", System.Data.SqlDbType.NVarChar, 100);
        }
        public GetServiceList_AutoComplete(String CnxString, String SearchString)
            : base("GetServiceList_AutoComplete")
        {
            this.Parameters.New("SearchString", System.Data.SqlDbType.NVarChar, 100, SearchString);
            this.GetData(CnxString);
        }
    }
}