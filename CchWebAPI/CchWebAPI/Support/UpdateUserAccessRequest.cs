using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CchWebAPI.Support
{
    public class UpdateUserAccessRequest : DataBase
    {
        private DataTable pastCareRequests = null;


        public Guid UserID
        {
            get
            {
                return (Guid)this.Parameters["UserID"].Value;
            }
            set
            {
                this.Parameters["UserID"].Value = value;
            }
        }

        public int CCHID
        {
            get
            {
                return (int)this.pastCareRequests.Rows[0]["CCHID"];
            }
            set
            {
                this.pastCareRequests.Rows[0]["CCHID"] = value;
            }
        }
        public int CCHID_Dependent
        {
            get
            {
                return (int)this.pastCareRequests.Rows[0]["CCHID_Dependent"];
            }
            set
            {
                this.pastCareRequests.Rows[0]["CCHID_Dependent"] = value;
            }
        }
        public bool RequestAccessFromDependent
        {
            get
            {
                return (bool)this.pastCareRequests.Rows[0]["RequestAccessFromDependent"];
            }
            set
            {
                this.pastCareRequests.Rows[0]["RequestAccessFromDependent"] = value;
            }
        }
        public bool GrantAccessToDependent
        {
            get
            {
                return (bool)this.pastCareRequests.Rows[0]["GrantAccessToDependent"];
            }
            set
            {
                this.pastCareRequests.Rows[0]["GrantAccessToDependent"] = value;
            }
        }
        public string DependentEmail
        {
            get
            {
                return (string)this.pastCareRequests.Rows[0]["DependentEmail"];
            }
            set
            {
                this.pastCareRequests.Rows[0]["DependentEmail"] = value;
            }
        }

        public UpdateUserAccessRequest()
            : base("UpdateUserAccessRequest")
        {
            this.Parameters.New("UserID", System.Data.SqlDbType.UniqueIdentifier);
            this.Parameters.New("PastCareRequests", System.Data.SqlDbType.Structured);

            pastCareRequests = new DataTable("PastCareRequests");
            pastCareRequests.Columns.Add("CCHID", typeof(int));
            pastCareRequests.Columns.Add("CCHID_Dependent", typeof(int));
            pastCareRequests.Columns.Add("RequestAccessFromDependent", typeof(bool));
            pastCareRequests.Columns.Add("GrantAccessToDependent", typeof(bool));
            pastCareRequests.Columns.Add("DependentEmail", typeof(string));
            pastCareRequests.Rows.Add(pastCareRequests.NewRow());
        }

        public override void PostData(string ConnectionString)
        {
            this.Parameters["PastCareRequests"].Value = pastCareRequests;
            base.PostData(ConnectionString);
        }
    }
}