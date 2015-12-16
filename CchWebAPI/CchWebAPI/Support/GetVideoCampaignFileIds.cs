using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CchWebAPI.Support
{
    public sealed class GetVideoCampaignFileIds : DataBase
    {
        public Int32 CampaignId { set { this.Parameters["VideoCampaignId"].Value = value; } }

        public GetVideoCampaignFileIds()
            : base("GetVideoCampaignFileIds")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int);
        }

        public GetVideoCampaignFileIds(string cnxString, int campaignId)
            : base("GetVideoCampaignFileIds")
        {
            Parameters.New("VideoCampaignId", SqlDbType.Int, Value: campaignId);
            GetData(cnxString);
        }

        public List<Guid> Results
        {
            get
            {
                if (Tables.Count > 0 && Tables[0].Rows.Count > 0)
                    return (from result in Tables[0].AsEnumerable()
                            select result.Field<Guid>("VideoCampaignFileID")).ToList<Guid>();
                else
                    return null;
            }
        }
    }
}
