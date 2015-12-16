using System.Data;

namespace CchWebAPI.Support
{
    public class GetEmployeeByCchIdForCallCenter : DataBase
    {
        public int CchId
        {
            set { Parameters["CCHID"].Value = value; }
        }

        public string Email
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["Email"] : string.Empty; }
        }

        public string MemberSsn
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["MemberSSN"] : string.Empty; }
        }

        public string MemberFullSsn
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["MemberSSN_Full"] : string.Empty; }
        }

        public string MobilePhone
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["MobilePhone"] : string.Empty; }
        }

        public string AlternatePhone
        {
            get { return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["Phone"] : string.Empty; }
        }

        public GetEmployeeByCchIdForCallCenter()
            : base("GetEmployeeByCCHIDForCallCenter")
        {
            Parameters.New("CCHID", SqlDbType.Int);
        }
    }
}
