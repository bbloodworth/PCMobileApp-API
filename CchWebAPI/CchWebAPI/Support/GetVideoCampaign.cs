using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    public class GetVideoCampaign : DataBase
    {
        public int VideoCampaignId
        {
            get
            {
                return (int)Parameters["VideoCampaignId"].Value;
            }
            set
            {
                Parameters["VideoCampaignId"].Value = value;
            }
        }
        public string VideoDefinitionId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["VideoDefinitionId"].ToString() : string.Empty;
            }
        }
        public string IntroVideoDefinitionId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["IntroVideoDefinitionId"].ToString() : string.Empty;
            }
        }
        public bool IsVideoCampaignActive
        {
            get
            {
                return Tables[0].Rows.Count > 0 && Convert.ToBoolean(Tables[0].Rows[0]["IsActive"].ToString());
            }
        }

        public GetVideoCampaign()
            : base("GetVideoCampaign")
        {
            Parameters.New("VideoCampaignId", System.Data.SqlDbType.Int);
        }
        public GetVideoCampaign(int videoCampaignId)
            : base("GetVideoCampaign")
        {
            Parameters.New("VideoCampaignId", 
                System.Data.SqlDbType.Int, 
                Value: videoCampaignId);
        }
     }
}