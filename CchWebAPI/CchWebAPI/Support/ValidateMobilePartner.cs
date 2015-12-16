using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class ValidateMobilePartner : DataBase
    {
        public ValidateMobilePartner(String HandShakeKey, String OrganizationID)
            : base("ValidateMobilePartner")
        {
            //this.Parameters.Add(new System.Data.SqlClient.SqlParameter("hsKey", HandShakeKey));
            //this.Parameters.Add(new System.Data.SqlClient.SqlParameter("orgID", OrganizationID));

            this.Parameters.New("hsKey", System.Data.SqlDbType.NChar, Value: HandShakeKey);
            this.Parameters.New("orgID", System.Data.SqlDbType.NVarChar, Value: OrganizationID);
            GetFrontEndData();
        }
        public void ForEachProvider(Action<Boolean,Boolean,int,string,string> a)
        {
            DataRow[] Providers = this.Tables[0].Select();
            foreach (DataRow dr in Providers) 
                a(
                    Convert.ToBoolean(dr[0].ToString()), 
                    Convert.ToBoolean(dr[1].ToString()), 
                    Convert.ToInt32(dr[2].ToString()), 
                    dr[3].ToString(),
                    dr[4].ToString()
                );
        }
    }
}