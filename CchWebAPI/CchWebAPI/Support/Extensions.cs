using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;
using System.Dynamic;
using CchWebAPI.Support;
using CchWebAPI.Models;
using System.Text;
using System.Collections;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace CchWebAPI
{
    public static class DataExtensionMethods
    {
        /// <summary>
        /// Extracts the data from a given field in a Data Row and casts it as T.  Returns the default of T if the column is Null
        /// </summary>
        /// <typeparam name="T">The type to cast the return data as</typeparam>
        /// <param name="dr">The DataRow object to get the data from</param>
        /// <param name="Column">The name of the field to get the data from</param>
        /// <returns>The required data casted as T</returns>
        [DebuggerStepThrough]
        public static T GetData<T>(this DataRow dr, String Column) where T : struct
        {
            if (!dr.Table.Columns.Contains(Column)) return default(T);
            if (dr.IsNull(Column)) return default(T);
            return (T)Convert.ChangeType(dr[Column], typeof(T));
        }
        /// <summary>
        /// Extracts the data from the given field in a Data Row and returns it as a String.
        /// </summary>
        /// <param name="dr">The DataRow object to get the data from</param>
        /// <param name="Column">The name of the field to get the data from</param>
        /// <returns>The value of the required field as a String</returns>
        [DebuggerStepThrough]
        public static String GetData(this DataRow dr, String Column)
        {
            if (dr.IsNull(Column)) return default(String);
            return dr[Column].ToString();
        }
        /// <summary>
        /// Accepts a CCHEncrypt object that has the keys already set and encrypts the TaxId field of a data row
        /// </summary>
        /// <param name="dr">The data row containing the Tax Id field to encrypt</param>
        /// <param name="ce">The CCHEncrypt object used to encrypt the string</param>
        /// <returns>String: Encrypted Tax ID</returns>
        public static String EncryptTaxID(this DataRow dr, CCHEncrypt ce)
        {
            ce["TaxID"] = dr.GetData("TaxId");
            return ce.ToString();
        }
    }
    public static class DynamicExtensionMethods
    {
        public static String ToJson(this ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder json = new StringBuilder();
            List<String> keyPairs = new List<string>();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            json.Append("{");
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value is ExpandoObject)
                    keyPairs.Add(String.Format(@"""{0}"":{1}", pair.Key, (pair.Value as ExpandoObject).ToJson()));
                else
                    keyPairs.Add(String.Format(@"""{0}"":{1}", pair.Key, serializer.Serialize(pair.Value)));
            }

            json.Append(String.Join(",", keyPairs.ToArray()));
            json.Append("}");

            return json.ToString();
        }

    }
    public static class DictionaryExtensionMethods
    {
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (expando as IDictionary<string, object>);
            foreach (var kvp in dictionary)
            {
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }
                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }
            return expando;
        }
    }
    public static class HeaderExtensionMethods
    {
        public static bool Parse(this AuthenticationHeaderValue ahv, out string UserName, out string Password)
        {
            UserName = Password = string.Empty;

            if (ahv == null)
                return false;

            if (ahv.Scheme != "Basic")
                return false;

            string encodedUserPass = ahv.Parameter.Trim(),
                userAndPass = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUserPass));
            string[] parts = userAndPass.Split(':');
            UserName = parts[0];
            Password = parts[1];

            return true;
        }
    }
}