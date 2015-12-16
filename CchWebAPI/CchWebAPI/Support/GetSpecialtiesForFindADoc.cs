using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public sealed class GetSpecialtiesForFindADoc : DataBase
    {
        public GetSpecialtiesForFindADoc() 
            : base("GetSpecialtiesForFindADoc")
        {
            GetFrontEndData();
        }
        public T FindByID<T>(Int32 ID) where T : struct
        {
            DataRow[] drs = this.Tables[0].Select(String.Format("SpecialtyID = {0}", ID));
            if (drs.Length < 1)
                return default(T);
            else
                return Serializer.DeserializeDataRow<T>(drs[0]);
        }
    }
}