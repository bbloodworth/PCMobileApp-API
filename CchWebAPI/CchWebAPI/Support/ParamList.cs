using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace CchWebAPI.Support
{
    public class ParamList : CollectionBase, IEnumerable<SqlParameter>, IDisposable
    {
        /// <summary>
        /// Retrieve a Parameter in the list by list index
        /// </summary>
        /// <param name="Index">Index in the collection to retrieve</param>
        /// <returns>System.Data.SqlClient.SqlParameter</returns>
        /// <example>this[1].Value = "Test"</example>
        public SqlParameter this[int Index] { get { return (SqlParameter)List[Index]; } }
        /// <summary>
        /// Retrieve a Parameter in the list by the Parameter Name
        /// </summary>
        /// <param name="ParameterName">Name of the SqlParameter to retrieve</param>
        /// <returns>System.Data.SqlClient.SqlParameter</returns>
        /// <example>this["CCHID"].Value = 1</example>
        public SqlParameter this[String ParameterName] 
        { 
            get 
            { 
                return (from param in this 
                        where param.ParameterName == ParameterName 
                        select param)
                        .DefaultIfEmpty<SqlParameter>(new SqlParameter("Empty", null))
                        .First<SqlParameter>(); 
            } 
        }
        /// <summary>
        /// Adds an already created System.Data.SqlClient.SqlParameter to the list.  If it already exists it is updated
        /// </summary>
        /// <param name="Parameter">System.Data.SqlClient.SqlParameter</param>
        [DebuggerStepThrough]
        public void Add(SqlParameter Parameter) { if (!List.Contains(Parameter)) List.Add(Parameter); else this[Parameter.ParameterName].Value = Parameter.Value; }
        /// <summary>
        /// Creates a new System.Data.SqlClient.SqlParameter and adds it to the list
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <param name="Size"></param>
        [DebuggerStepThrough]
        public void New(String ParameterName, SqlDbType Type, int Size = 0, Object Value = null)
        {
            SqlParameter spTemp = null;
            if (Type == SqlDbType.NVarChar)
                spTemp = new SqlParameter(ParameterName, Type, Size);
            else
                spTemp = new SqlParameter(ParameterName, Type);
            spTemp.Value = Value;
            List.Add(spTemp);
        }
        /// <summary>
        /// ForEach SqlParameter in the list, do the specified action
        /// </summary>
        /// <typeparam name="SqlParameter"></typeparam>
        /// <param name="action"></param>
        [DebuggerStepThrough]
        public void ForEach<SqlParameter>(Action<SqlParameter> action) { foreach (SqlParameter item in List) action(item); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ParamList()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this != null)
                this.Dispose();
        }

        public new IEnumerator<SqlParameter> GetEnumerator()
        {
            foreach (SqlParameter param in this.List)
            {
                yield return param;
            }
        }
    }
}