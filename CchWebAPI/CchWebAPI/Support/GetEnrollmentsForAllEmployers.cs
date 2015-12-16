using System;
using System.ComponentModel;
using System.Data;

namespace CchWebAPI.Support
{
    [DesignerCategory("")]
    public class GetEnrollmentsForAllEmployers : DataBase
    {
        public String LastName { set { Parameters["LastName"].Value = value; } }
        public String LastFour { set { Parameters["LastFour"].Value = value; } }
        public String DateOfBirth { set { Parameters["DateOfBirth"].Value = value; } }

        public GetEnrollmentsForAllEmployers()
            : base("GetEnrollmentsForAllEmployers")
        {
            Parameters.New("LastName", SqlDbType.NVarChar, Size: 40);
            Parameters.New("LastFour", SqlDbType.NVarChar, Size: 9);
            Parameters.New("DateOfBirth", SqlDbType.NVarChar, Size: 10);
        }
    }
}