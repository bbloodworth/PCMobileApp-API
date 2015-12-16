using System.Data;

namespace CchWebAPI.Support
{
    public class GetCampaignIntro : DataBase
    {
        public int CampaignId
        {
            get
            {
                return (int)Parameters["CampaignId"].Value;
            }
            set
            {
                Parameters["CampaignId"].Value = value;
            }
        }
        public string ContentName
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["ContentName"].ToString() : string.Empty;
            }
        }
        public string ContentType
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["ContentTypeDesc"].ToString() : string.Empty;
            }
        }
        public int ContentId
        {
            get
            {
                return Tables[0].Rows.Count > 0 ? int.Parse( Tables[0].Rows[0]["ContentID"].ToString()) : 0; 
                
            }
        }

        public GetCampaignIntro()
            : base("p_GetCampaignIntro")
        {
            Parameters.New("CampaignId", SqlDbType.Int);
        }
        public GetCampaignIntro(int campaignId)
            : base("p_GetCampaignIntro")
        {
            Parameters.New("CampaignId", 
                SqlDbType.Int, 
                Value: campaignId);
        }
    }
}