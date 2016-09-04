using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetKeyEmployeeInfoByCchId : DataBase
    {
        public int CchId { set { Parameters["CCHID"].Value = value; } }

        public DataTable EmployeeTable { get { return Tables[0] ?? new DataTable("Empty"); } }
        public DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        public DataTable DependentTable { get { return ( Tables.Count > 1 ? Tables[1] : new DataTable("EmptyTable")); } }

        public String EmployeeFirstName { get { return EmployeeRow["FirstName"].ToString(); } }
        public String EmployeeLastname { get { return EmployeeRow["LastName"].ToString(); } }
        public String EmployeeEmail { get { return EmployeeRow["Email"].ToString(); } }
        public object this[String ColumnName]
        {
            get { return (EmployeeRow.Table.Columns.Contains(ColumnName) ? 
                    EmployeeRow[ColumnName] : null); }
        }

        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetKeyEmployeeInfoByCchId()
            : base("GetKeyEmployeeInfoByCchId")
        { Parameters.New("CCHID", SqlDbType.Int); }

        public void ForEachDependent<DataRow>(Action<DataRow> action)
        { foreach (DataRow dr in DependentTable.Rows) action(dr); }
    }
}