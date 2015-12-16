using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace CchWebAPI
{
    public static class HttpRequestMessageExtensions : Object
    {
        public static Int32 CCHID(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("cchid")) return 0;
            else return Convert.ToInt32(r.Properties["cchid"].ToString());
        }
        public static void CCHID(this HttpRequestMessage r, Int32 val)
        {
            if (r.Properties.Keys.Contains("cchid")) r.Properties["cchid"] = val;
            else r.Properties.Add("cchid", val);
        }
        public static Int32 EmployerID(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("empid")) return 0;
            else return Convert.ToInt32(r.Properties["empid"].ToString());
        }
        public static void EmployerID(this HttpRequestMessage r, Int32 val)
        {
            if (r.Properties.Keys.Contains("empid")) r.Properties["empid"] = val;
            else r.Properties.Add("empid", val);
        }
        public static Guid EncryptionKey(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("ek")) return new Guid();
            else return new Guid(r.Properties["ek"].ToString());
        }
        public static void EncryptionKey(this HttpRequestMessage r, String val)
        {
            if (r.Properties.Keys.Contains("ek")) r.Properties["ek"] = val;
            else r.Properties.Add("ek", val);
        }
        public static Boolean IsPartner(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("isp")) return false;
            else return Convert.ToBoolean(r.Properties["isp"].ToString());
        }
        public static void IsPartner(this HttpRequestMessage r, bool val)
        {
            if (r.Properties.Keys.Contains("isp")) r.Properties["isp"] = val;
            else r.Properties.Add("isp", val);
        }
        public static String UserID(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("userkey")) return String.Empty;
            else return r.Properties["userkey"].ToString();
        }
        public static void UserID(this HttpRequestMessage r, String val)
        {
            if (r.Properties.Keys.Contains("userkey")) r.Properties["userkey"] = val;
            else r.Properties.Add("userkey", val);
        }
        public static String UserName(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("un")) return String.Empty;
            else return r.Properties["un"].ToString();
        }
        public static void UserName(this HttpRequestMessage r, String val)
        {
            if (r.Properties.Keys.Contains("un")) r.Properties["userkey"] = val;
            else r.Properties.Add("un", val);
        }
        public static String CallBack(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("cb")) return string.Empty;
            else return r.Properties["cb"].ToString();
        }
        public static void CallBack(this HttpRequestMessage r, String val)
        {
            if (r.Properties.Keys.Contains("cb")) r.Properties["cb"] = val;
            else r.Properties.Add("cb", val);
        }
        public static Boolean UsesAlternateProvider(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("alt")) return false;
            else return Convert.ToBoolean(r.Properties["alt"].ToString());
        }
        public static void UsesAlternateProvider(this HttpRequestMessage r, bool val)
        {
            if (r.Properties.Keys.Contains("alt")) r.Properties["alt"] = val;
            else r.Properties.Add("alt", val);
        }
        public static String ConnectionString(this HttpRequestMessage r)
        {
            if (!r.Properties.Keys.Contains("cnx")) return String.Empty;
            else return r.Properties["cnx"].ToString();
        }
        public static void ConnectionString(this HttpRequestMessage r, String val)
        {
            if (r.Properties.Keys.Contains("cnx")) r.Properties["cnx"] = val;
            else r.Properties.Add("cnx", val);
        }
    }
}