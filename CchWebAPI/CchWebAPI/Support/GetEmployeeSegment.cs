using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetEmployeeSegment : DataBase
    {
        private DataTable EmployeeTable { get { return Tables[0] ?? new DataTable("Empty"); } }
        private DataRow EmployeeRow { get { return EmployeeTable.Rows[0] ?? EmployeeTable.NewRow(); } }
        private object this[String colName] { get { return EmployeeRow[colName] ?? string.Empty; } }

        public string PropertyCode { get { return this["PropertyCode"].ToString(); } }
        public string BirthYear { get { return this["BirthYear"].ToString(); } }
        public string Insurer { get { return this["Insurer"].ToString(); } }

        public int CchId { set { Parameters["CCHID"].Value = value; } }
        public int EmployerId { set { Parameters["EmployerId"].Value = value; } }

        public GetEmployeeSegment()
            : base("GetEmployeeSegment")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("EmployerId", SqlDbType.Int);
        }
        public GetEmployeeSegment(int cchId, int employerId)
            : base("GetEmployeeSegment")
        {
            Parameters.New("CCHID", SqlDbType.Int, Value: cchId);
            Parameters.New("EmployerId", SqlDbType.Int);
        }
    }
}
