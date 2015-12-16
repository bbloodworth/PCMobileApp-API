using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Dynamic;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class DataBase : DataSet
    {
        #region Variables
        private String Procedure = String.Empty;
        private CommandType ProcedureType = CommandType.StoredProcedure;
        protected ParamList Parameters = new ParamList();
        private int postReturn;
        private bool _hasThrownError = false;
        public bool HasThrownError
        {
            get { return _hasThrownError; }
        }

        #endregion
        #region Properties
        public String OrderDirection
        {
            set
            {
                if (this.Parameters["OrderDirection"] == null)
                    this.Parameters.New("OrderDirection", SqlDbType.NVarChar, 4, value);
                else
                    this.Parameters["OrderDirection"].Value = value;
            }
        }
        public String OrderByField
        {
            set
            {
                if (this.Parameters["OrderByField"] == null)
                    this.Parameters.New("OrderByField", SqlDbType.NVarChar, 50, value);
                else
                    this.Parameters["OrderByField"].Value = value;
            }
        }
        public int PostReturn { get { return postReturn; } }
        #endregion

        public DataBase() { }
        public DataBase(String ProcedureName, Boolean IsRawQuery = false)
            : base(ProcedureName)
        { this.Procedure = ProcedureName; if (IsRawQuery) ProcedureType = CommandType.Text; }

        public virtual void AddParameter(String ParameterName, object value)
        {
            this.Parameters.Add(new SqlParameter(ParameterName, value));
        }
        public virtual void GetData(String connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    comm.CommandTimeout = 60;
                    if (this.Parameters.Count > 0) this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try { da.Fill(this); }
                        catch (SqlException sqEx) { CaptureError(sqEx); }
                        catch (Exception ex) { CaptureError(ex); }
                        finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void GetFrontEndData()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.Parameters.Count > 0) this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try { da.Fill(this); }
                        catch (SqlException sqEx) { CaptureError(sqEx); }
                        catch (Exception ex) { CaptureError(ex); }
                        finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void GetHealthgradesData()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_HealthGrades"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.Parameters.Count > 0) this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        try { da.Fill(this); }
                        catch (SqlException sqEx) { CaptureError(sqEx); }
                        catch (Exception ex) { CaptureError(ex); }
                        finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    }
                    comm.Parameters.Clear();
                }
            }
        }

        public virtual void PostData(String ConnectionString)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.Parameters.Count > 0) this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    try
                    { conn.Open(); postReturn = comm.ExecuteNonQuery(); }
                    catch (SqlException sqEx) { CaptureError(sqEx); }
                    catch (Exception ex) { CaptureError(ex); }
                    finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }
        }
        public virtual void PostFrontEndData()
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand(Procedure, conn))
                {
                    comm.CommandType = ProcedureType;
                    if (this.Parameters.Count > 0) this.Parameters.ForEach(delegate(SqlParameter Parameter) { comm.Parameters.Add(Parameter); });
                    try
                    { conn.Open(); postReturn = comm.ExecuteNonQuery(); }
                    catch (SqlException sqEx) { CaptureError(sqEx); }
                    catch (Exception ex) { CaptureError(ex); }
                    finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }
        }

        public void CaptureError(Exception exception, Boolean IsFrontEnd = false)
        {
            _hasThrownError = true;

            String Source = exception.Source,
                StackTrace = exception.StackTrace,
                Message = exception.Message,
                Procedure = "", Server = "",
                Cnx;
            if (exception.GetType() == typeof(SqlException))
            {
                Procedure = ((SqlException)exception).Procedure;
                Server = ((SqlException)exception).Server;
            }
            Cnx = WebConfigurationManager.ConnectionStrings["CCH_FrontEnd"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(Cnx))
            {
                using (SqlCommand comm = new SqlCommand("CaptureAPIError", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("Source", Source);
                    comm.Parameters.AddWithValue("StackTrace", StackTrace);
                    comm.Parameters.AddWithValue("Message", Message);
                    comm.Parameters.AddWithValue("Procedure", Procedure);
                    comm.Parameters.AddWithValue("Server", Server);
                    try { conn.Open(); comm.ExecuteNonQuery(); }
                    catch (SqlException) { }
                    catch (Exception) { }
                    finally { if (null != conn && conn.State != ConnectionState.Closed) { conn.Close(); } }
                    comm.Parameters.Clear();
                }
            }
        }
        public void ForEach<T>(Action<T> action, Int32 TableIndex = 0) where T : struct
        {
            DataTable workingTable = new DataTable("Empty");
            if(this.Tables.Count >= (TableIndex + 1)) workingTable = this.Tables[TableIndex];
            foreach (DataRow dr in workingTable.Select())
                action(Serializer.DeserializeDataRow<T>(dr));
        }
        public IEnumerable<ExpandoObject> ExpandoRows(int TableIndex = 0)
        {
            if (this.Tables.Count >= (TableIndex + 1))
            {
                DataTable workingTable = this.Tables[TableIndex];
                for(int r = 0; r < workingTable.Rows.Count; r++)
                {
                    dynamic expandoRow = new ExpandoObject();
                    for (int c = 0; c < workingTable.Columns.Count; c++)
                    {
                        string cName = workingTable.Columns[c].ColumnName;
                        (expandoRow as IDictionary<string, object>)[cName] = workingTable.Rows[r][c];
                    }
                    yield return expandoRow;
                }
            }
            else yield break;
        }
    }
}