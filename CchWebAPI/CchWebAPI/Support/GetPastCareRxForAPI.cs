using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetPastCareRxForAPI : DataBase
    {
        public EnumerableRowCollection<DataRow> Rows
        {
            get { return this.Tables[0].AsEnumerable(); }
        }

        public GetPastCareRxForAPI()
            : base("GetPastCareRxForAPI")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
        }
        public GetPastCareRxForAPI(int CCHID)
            : base("GetPastCareRxForAPI")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int, Value: CCHID);
        }
    }
}