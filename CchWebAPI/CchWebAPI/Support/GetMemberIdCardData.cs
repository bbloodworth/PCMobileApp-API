using System.Data;

namespace CchWebAPI.Support
{
    public class GetMemberIdCardData : DataBase
    {
        public string SecurityToken
        {
            set
            {
                Parameters["SecurityToken"].Value = value;
            }
        }

        public int CardTypeId
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (int)Tables[0].Rows[0]["CardTypeID"] : 0;
            }
        }

        public int CardViewModeId
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (int)Tables[0].Rows[0]["CardViewModeID"] : 0;
            }
        }

        public string CardMemberDataText
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["CardMemberDataText"] : string.Empty;
            }
        }

        public string CardTypeFileName
        {
            get
            {
                return Tables.Count > 0 && Tables[0].Rows.Count > 0 ? (string)Tables[0].Rows[0]["CardTypeFileName"] : string.Empty;
            }
        }

        public GetMemberIdCardData()
            : base("p_GetMemberIdCardData")
        {
            Parameters.New("SecurityToken", SqlDbType.VarChar, 36);
        }

        public GetMemberIdCardData(string securityToken)
            : base("p_GetMemberIdCardData")
        {
            Parameters.New("SecurityToken", SqlDbType.VarChar, 36, securityToken);
        }
    }
}