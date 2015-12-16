using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class UpdateUserPhone : DataBase
    {
        public String Phone { set { Parameters["Phone"].Value = value; } }
        public Int32 CCHID { set { Parameters["CCHID"].Value = value; } }

        public UpdateUserPhone()
            : base("UpdateUserPhone")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("Phone", SqlDbType.NVarChar, Size: 50);
        }
    }
}