using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetSpecialtiesForWeb : DataBase
    {
        public List<String> Results
        {
            get
            {
                if (this.Tables.Count > 0 && this.Tables[0].Rows.Count > 0)
                    return (from result in this.Tables[0].AsEnumerable()
                            select result.Field<String>("Specialty")).ToList<String>();
                else
                    return null;
            }
        }

        //public GetSpecialtiesForWeb()
        //    : base("GetSpecialtiesForWeb")
        //{
        //    this.GetFrontEndData();
        //}

        public GetSpecialtiesForWeb(String CnxString)
            : base("GetSpecialtiesForWeb")
        {
            this.GetData(CnxString);
        }
    }
}