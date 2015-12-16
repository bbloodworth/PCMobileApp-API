using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetClientSpecialtyInfo : DataBase
    {
        const string C_SPECIALTYINFO = "CoreProduct";

        public GetClientSpecialtyInfo()
            : base("GetClientSpecialtyInfo")
        {
            this.Parameters.New("SpecialtyType", SqlDbType.VarChar, 25, Value: C_SPECIALTYINFO);
        }

        public GetClientSpecialtyInfo(String SpecialtyType)
            : base("GetClientSpecialtyInfo")
        {
            this.Parameters.New("SpecialtyType", SqlDbType.VarChar, 25, Value: SpecialtyType);
        }
    }
}
