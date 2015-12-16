namespace DynamicAnimation.Models
{
    public class AuthMemberDataRequest
    {
        public int CampaignId;
        public string LastName;
        public string DateOfBirth;
        public string LastFourSsn;
    }

    public class AuthorizationResponse
    {
        public string AuthHash { get; set; }
    }
}
