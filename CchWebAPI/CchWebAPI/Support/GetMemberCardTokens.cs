using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CchWebAPI.Areas.Animation.Models;

namespace CchWebAPI.Support
{
    public class GetMemberCardTokens : DataBase
    {
        private List<CardResult> _memberTokens = new List<CardResult>();

        public List<CardResult> MemberTokens
        {
            get { return _memberTokens; }
        } 

        public int CchId
        {
            get { return (int) Parameters["CCHID"].Value; }
            set { Parameters["CCHID"].Value = value; }
        }

        public string LocaleCode
        {
            set { Parameters["LocaleCode"].Value = value; }
        }

        public int TotalCount
        {
            get { return MemberTokens.Count; }
        }

        private int _employerId;
        public int EmployerId
        {
            set { _employerId = value; }
        }

        public GetMemberCardTokens()
            : base("p_GetMemberIdCardTokens")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("LocaleCode", SqlDbType.VarChar, Size: 10);
        }

        public GetMemberCardTokens(int cchid)
            : base("p_GetMemberIdCardTokens")
        {
            Parameters.New("CCHID", SqlDbType.Int, Value: cchid);
            Parameters.New("LocaleCode", SqlDbType.VarChar, Size: 10);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            if (Tables.Count >= 1 && Tables[0].Rows.Count > 0 )
            {
                //_memberTokens = (from tokenRow in Tables[0].AsEnumerable()
                //    select new MemberCardTokenRecord()
                //    {
                //        CardTypeName = tokenRow.GetData("CardTypeName"), 
                //        CardViewModeName = tokenRow.GetData("CardViewModeName"), 
                //        SecurityTokenGuid = tokenRow.GetData("SecurityTokenGUID")
                //    } ).ToList<MemberCardTokenRecord>();

                string cardBaseAddress = "CardBaseAddress".GetConfigurationValue();

                _memberTokens = (from tokenRow in Tables[0].AsEnumerable()
                                 select new CardResult()
                                 {
                                     CardName = tokenRow.GetData("CardTypeName"),
                                     ViewMode = tokenRow.GetData("CardViewModeName"),
                                     CardUrl = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, _employerId, tokenRow.GetData("SecurityTokenGUID")),
                                     SecurityToken = tokenRow.GetData("SecurityTokenGUID")
                                 }).ToList<CardResult>();
            }
        }
    }
}
