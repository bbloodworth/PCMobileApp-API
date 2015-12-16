using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetSpecialtySubCategoriesForWeb : DataBase
    {
        public List<String> Results
        {
            get
            {
                if (this.Tables.Count > 0 && this.Tables[0].Rows.Count > 0)
                    return (from result in this.Tables[0].AsEnumerable()
                            select result.Field<String>("SubCategory")).ToList<String>();
                else
                    return null;
            }
        }

        public GetSpecialtySubCategoriesForWeb()
            : base("GetSpecialtySubCategoriesForWeb")
        {
            this.Parameters.New("Specialty", System.Data.SqlDbType.VarChar, 70);
        }

        public GetSpecialtySubCategoriesForWeb(String SubCategory)
            : base("GetSpecialtySubCategoriesForWeb")
        {
            this.Parameters.New("Specialty", System.Data.SqlDbType.VarChar, 70, SubCategory);
        }
    }
}