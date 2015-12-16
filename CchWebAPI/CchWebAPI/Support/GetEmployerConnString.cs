using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetEmployerConnString : DataBase
    {
        public String ConnString
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["ConnectionString"].ToString() : string.Empty;
            }
        }
        public String Insurer
        {
            get
            {
                return Tables[0].Rows[0]["Insurer"].ToString();
            }
        }
        public GetEmployerConnString(Int32 EmpID)
            : base("GetEmployerConnString")
        {
            Parameters.New("EmployerID", System.Data.SqlDbType.Int, Value: EmpID);
            GetFrontEndData();
        }
    }
}