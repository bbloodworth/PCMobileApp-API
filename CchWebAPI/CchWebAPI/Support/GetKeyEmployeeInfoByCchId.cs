using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetKeyEmployeeInfoByCchId : DataBase
    {
        public int CchId { set { Parameters["CCHID"].Value = value; } }

        private DataTable EmployeeTable { get { return Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        public DataTable DependentTable { get { return ( Tables.Count > 1 ? Tables[1] : new DataTable("EmptyTable")); } }
        private object this[String ColName] { get { return EmployeeRow[ColName]; } }

        public String CnxString { get { return this["ConnectionString"].ToString(); } }

        public GetKeyEmployeeInfoByCchId()
            : base("GetKeyEmployeeInfoByCchId")
        { Parameters.New("CCHID", SqlDbType.Int); }

        public void ForEachDependent<DataRow>(Action<DataRow> action)
        { foreach (DataRow dr in DependentTable.Rows) action(dr); }
    }
}