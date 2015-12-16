namespace CchWebAPI.Support
{
    public class GetVideoCampaignMemberDataById : DataBase
    {
        public GetVideoCampaignMemberDataById()
            : base("GetVideoCampaignMemberDataById")
        {
            Parameters.New("VideoCampaignMemberId", System.Data.SqlDbType.NVarChar);
        }

        public GetVideoCampaignMemberDataById(string videoCampaignMemberId)
            : base("GetVideoCampaignMemberDataById")
        {
            Parameters.New("VideoCampaignMemberId",
                System.Data.SqlDbType.NVarChar,
                Value: videoCampaignMemberId);
        }

        public string VideoCampaignMemberId
        {
            set
            {
                Parameters["VideoCampaignMemberId"].Value = value;
            }
        }
        public string VideoMemberData
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoData"].ToString() : string.Empty;
            }
        }
    }
}