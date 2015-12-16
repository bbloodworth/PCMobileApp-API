using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetUserNotificationSettings : DataBase
    {
        public int CCHID
        {
            get
            {
                return (int)this.Parameters["CCHID"].Value;
            }
            set
            {
                this.Parameters["CCHID"].Value = value;
            }
        }

        public bool EmailAlerts
        {
            get
            {
                return this.Tables[0].Rows.Count > 0 ? (bool)this.Tables[0].Rows[0]["OptInEmailAlerts"] : false;
            }
        }
        public bool TextAlerts
        {
            get
            {
                return this.Tables[0].Rows.Count > 0 ? (bool)this.Tables[0].Rows[0]["OptInTextMsgAlerts"] : false;
            }
        }
        public string MobilePhone
        {
            get
            {
                return this.Tables[0].Rows.Count > 0 ? (string)this.Tables[0].Rows[0]["MobilePhone"] : string.Empty;
            }
        }
        public bool HealthShopper
        {
            get
            {
                return this.Tables[0].Rows.Count > 0 ? (bool)this.Tables[0].Rows[0]["OptInPriceConcierge"] : false;
            }
        }

        public GetUserNotificationSettings()
            : base("GetUserNotificationSettings")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int);
        }
        public GetUserNotificationSettings(int cchid)
            : base("GetUserNotificationSettings")
        {
            this.Parameters.New("CCHID", System.Data.SqlDbType.Int, Value: cchid);
        }
    }
}