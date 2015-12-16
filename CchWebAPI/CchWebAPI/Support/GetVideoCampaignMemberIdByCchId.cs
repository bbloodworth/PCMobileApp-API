using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetVideoCampaignMemberIdByCchId : DataBase
    {
        public Int32 CampaignId { set { Parameters["VideoCampaignId"].Value = value; } }
        public Int32 CchId { set { Parameters["CchId"].Value = value; } }

        public GetVideoCampaignMemberIdByCchId()
            : base("GetVideoCampaignMemberIdByCchId")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int);
            Parameters.New("CchId", SqlDbType.Int);
        }

        public GetVideoCampaignMemberIdByCchId(int campaignId, int cchid)
            : base("GetVideoCampaignMemberIdByCchId")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int, Value: campaignId);
            Parameters.New("CchId", SqlDbType.Int, Value: cchid);
        }

        public string VideoCampaignMemberId
        {
            get { return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoCampaignMemberId"].ToString() : string.Empty; }
        }
    }
}