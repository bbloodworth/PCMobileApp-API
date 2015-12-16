namespace CchWebAPI.Support
{
    public class GetVideoCampaignMemberData : DataBase
    {
        public GetVideoCampaignMemberData()
            : base("GetVideoCampaignMemberData")
        {
            Parameters.New("VideoCampaignFileId", System.Data.SqlDbType.NVarChar);
        }

        public GetVideoCampaignMemberData(string videoCampaignFileId)
            : base("GetVideoCampaignMemberData")
        {
            Parameters.New("VideoCampaignFileId",
                System.Data.SqlDbType.NVarChar,
                Value: videoCampaignFileId);
        }

        public string VideoCampaignFileId
        {
            set
            {
                Parameters["VideoCampaignFileId"].Value = value;
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