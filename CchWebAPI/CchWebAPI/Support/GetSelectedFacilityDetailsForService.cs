using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetSelectedFacilityDetailsForService : DataBase
    {
        public String ServiceName { set { this.Parameters["ServiceName"].Value = value; } }
        public String PracticeName { set { this.Parameters["PracticeName"].Value = value; } }
        public String NPI { set { this.Parameters["NPI"].Value = value; } }
        public Int32 SpecialtyID { set { this.Parameters["SpecialtyID"].Value = value; } }
        public Int32 ServiceID { set { this.Parameters["ServiceID"].Value = value; } }
        public Int32 OrganizationLocationID { set { this.Parameters["OrganizationLocationID"].Value = value; } }

        public GetSelectedFacilityDetailsForService()
            : base("GetSelectedFacilityDetailsForService")
        {
            this.Parameters.New("ServiceName", System.Data.SqlDbType.NVarChar, Size: 200);
            this.Parameters.New("PracticeName", System.Data.SqlDbType.NVarChar, Size: 150);
            this.Parameters.New("NPI", System.Data.SqlDbType.NVarChar, Size: 50);
            this.Parameters.New("SpecialtyID", System.Data.SqlDbType.Int);
            this.Parameters.New("ServiceID", System.Data.SqlDbType.Int);
            this.Parameters.New("OrganizationLocationID", System.Data.SqlDbType.Int);
        }
    }
}