using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetLastCategoriesForWeb : DataBase
    {
        public List<dynamic> Results
        {
            get
            {
                if (this.Tables.Count > 0 && this.Tables[0].Rows.Count > 0)
                    return (from result in this.Tables[0].AsEnumerable()
                            select new
                            {
                                CategoryLvl4 = result.Field<String>("CategoryLvl4"),
                                SpecialtyID = result.Field<int>("SpecialtyID"),
                                ServiceID = result.Field<int>("ServiceID")
                            }).ToList<dynamic>();
                else
                    return null;
            }
        }

        public GetLastCategoriesForWeb()
            : base("GetLastCategoriesForWeb")
        {
            this.Parameters.New("Specialty", System.Data.SqlDbType.VarChar, 70);
            this.Parameters.New("SubCategory", System.Data.SqlDbType.VarChar, 150);
        }

        public GetLastCategoriesForWeb(string category, string subcategory)
            : base("GetLastCategoriesForWeb")
        {
            this.Parameters.New("Specialty", System.Data.SqlDbType.VarChar, 70, category);
            this.Parameters.New("SubCategory", System.Data.SqlDbType.VarChar, 150, subcategory);
        }
    }
}