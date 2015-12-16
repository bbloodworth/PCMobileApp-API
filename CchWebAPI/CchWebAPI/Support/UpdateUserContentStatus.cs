using System.Data;

namespace CchWebAPI.Support
{
    public class UpdateUserContentStatus : DataBase
    {
        public int CchId { set { Parameters["CCHID"].Value = value; } }
        public int CampaignId { set { Parameters["CampaignID"].Value = value; } }
        public int ContentId { set { Parameters["ContentID"].Value = value; }}
        public int StatusId { set { Parameters["StatusID"].Value = value; } }
        public string StatusDesc { set { Parameters["StatusDesc"].Value = value; } }

        public UpdateUserContentStatus()
            : base("p_UpdateUserContentStatus")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("CampaignID", SqlDbType.Int);
            Parameters.New("ContentID", SqlDbType.Int);
            Parameters.New("StatusID", SqlDbType.Int);
            Parameters.New("StatusDesc", SqlDbType.NVarChar, Size: 100);
        }
    }
}