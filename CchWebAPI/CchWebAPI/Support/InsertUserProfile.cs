using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class InsertUserProfile : DataBase
    {
        public Guid UserID { set { this.Parameters["UserID"].Value = value; } }
        public Int32 EmployerID { set { this.Parameters["EmployerID"].Value = value; } }
        public String FirstName { set { this.Parameters["FirstName"].Value = value; } }
        public String LastName { set { this.Parameters["LastName"].Value = value; } }
        public String Email { set { this.Parameters["Email"].Value = value; } }
        public String MessageCode { set { this.Parameters["MessageCode"].Value = value; } }

        public InsertUserProfile()
            : base("InsertUserProfile")
        {
            this.Parameters.New("UserID", SqlDbType.UniqueIdentifier);
            this.Parameters.New("EmployerID", SqlDbType.Int);
            this.Parameters.New("FirstName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("LastName", SqlDbType.NVarChar, Size: 100);
            this.Parameters.New("Email", SqlDbType.NVarChar, Size: 256);
            this.Parameters.New("MessageCode", SqlDbType.NVarChar, Size: 100);
        } 
    }
}