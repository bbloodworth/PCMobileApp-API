using System;

namespace CchWebAPI.Support
{
    public class GetVideoCampaignMemberInfo : DataBase
    {
        public string VideoCampaignMemberId
        {
            get
            {
                return (string)Parameters["VideoCampaignMemberId"].Value;
            }
            set
            {
                Parameters["VideoCampaignMemberId"].Value = value;
            }
        }
        public string VideoDefinitionName
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoDefinitionName"].ToString() : string.Empty;
            }
        }
        public string IntroVideoDefinitionId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["IntroVideoDefinitionId"].ToString() : string.Empty;
            }
        }
        public string VideoCampaignId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoCampaignId"].ToString() : string.Empty;
            }
        }
        public bool IsVideoCampaignActive
        {
            get
            {
                return Tables[0].Rows.Count > 0 && Convert.ToBoolean(Tables[0].Rows[0]["IsVideoCampaignActive"].ToString());
            }
        }
        public string LastName
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["LastName"].ToString() : string.Empty;
            }
        }
        public string DateOfBirth
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["DateOfBirth"].ToString() : string.Empty;
            }
        }
        public string MemberSsn
        {
            get
            {
                string ssn = Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["MemberSSN"].ToString() : string.Empty;
                ssn = ssn.Length > 4 ? ssn.Substring(ssn.Length - 4, 4) : ssn;
                return ssn;
            }
        }
        public string VideoCampaignFileId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoCampaignFileId"].ToString() : string.Empty;
            }
        }
        public string IntroVideoDefinitionName
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["IntroVideoDefinitionName"].ToString() : string.Empty;
            }
        }
        public string CchEmployerLink
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["CCHLink"].ToString() : string.Empty;
                // return "Placeholder for CchEmployerLink";
            }
        }
        public string EmployerBenefitsLink
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["BenefitsLink"].ToString() : string.Empty;
                // return "Placeholder for EmployerBenefitsLink";
            }
        }


        public GetVideoCampaignMemberInfo()
            : base("GetVideoCampaignMemberInfo")
        {
            Parameters.New("VideoCampaignMemberId", System.Data.SqlDbType.NVarChar);
        }
        public GetVideoCampaignMemberInfo(string videoCampaignMemberId)
            : base("GetVideoCampaignMemberInfo")
        {
            Parameters.New("VideoCampaignMemberId", 
                System.Data.SqlDbType.NVarChar, 
                Value: videoCampaignMemberId);
        }
    }
}