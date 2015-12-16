using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class GetVideoCampaignFileIdByCchId : DataBase
    {
        public Int32 CampaignId { set { Parameters["VideoCampaignId"].Value = value; } }
        public Int32 CchId { set { Parameters["CchId"].Value = value; } }

        public GetVideoCampaignFileIdByCchId()
            : base("GetVideoCampaignFileIdByCchId")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int);
            Parameters.New("CchId", SqlDbType.Int);
        }

        public GetVideoCampaignFileIdByCchId(int campaignId, int cchid)
            : base("GetVideoCampaignFileIdByCchId")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int, Value: campaignId);
            Parameters.New("CchId", SqlDbType.Int, Value: cchid);
        }

        public string VideoCampaignFileId
        {
            get { return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoCampaignFileId"].ToString() : string.Empty; }
        }
    }
}