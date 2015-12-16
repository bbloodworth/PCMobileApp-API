using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetEmployerContent : DataBase
    {
        public String SpecialtyNetworkText
        {
            get
            {
                return this.Tables[0].Rows[0]["SpecialtyNetworkText"].ToString();
            }
        }
        public String PastCareDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["PastCareDisclaimerText"].ToString();
            }
        }
        public String RxResultDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["RxResultDisclaimerText"].ToString();
            }
        }
        public String AllResult1DisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["AllResult1DisclaimerText"].ToString();
            }
        }
        public String AllResult2DisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["AllResult2DisclaimerText"].ToString();
            }
        }
        public String SpecialtyDrugDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["SpecialtyDrugDisclaimerText"].ToString();
            }
        }
        public String MentalHealthDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["MentalHealthDisclaimerText"].ToString();
            }
        }
        public String ServiceNotFoundDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["ServiceNotFoundDisclaimerText"].ToString();
            }
        }
        public String NcctDisclaimerText
        {
            get
            {
                return this.Tables[0].Rows[0]["NCCTDisclaimerText"].ToString();
            }
        }
        public GetEmployerContent(Int32 EmpID)
            : base("GetEmployerContent")
        {
            this.Parameters.New("EmployerID", System.Data.SqlDbType.Int, Value: EmpID);
            this.GetFrontEndData();
        }
    }
}