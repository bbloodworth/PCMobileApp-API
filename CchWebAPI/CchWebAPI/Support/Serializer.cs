using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Diagnostics;

namespace CchWebAPI.Support
{
    /// <summary>
    /// Due to slow performance, I'm phasing this out in favor of better methods. JM 3/11/13
    /// </summary>
    public static class Serializer
    {
        [DebuggerStepThrough]
        public static T DeserializeDataRow<T>(object dr) where T : struct
        {
            T result = new T();
            if (typeof(DataRow) == dr.GetType())
            {
                DataRow asRow = (DataRow)dr;
                Type type = typeof(T);
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    string key = field.Name;
                    string stringValue = (asRow.Table.Columns.Contains(key) ? asRow[key].ToString() : null);
                    if(!String.IsNullOrEmpty(stringValue))
                    {
                        object value;
                        var baseType = Nullable.GetUnderlyingType(field.FieldType);
                        if (baseType != null)
                            value = Convert.ChangeType(stringValue, baseType);
                        else
                            value = Convert.ChangeType(stringValue, field.FieldType);
                        field.SetValueDirect(__makeref(result), value);
                    }
                }
            }
            return result;
        }
    }
}