namespace CchWebAPI.Areas.Animation.Models
{
    public class MemberCardTokenRecord
    {
        public string CardTypeName;
        public string CardViewModeName;
        public string SecurityTokenGuid;
    }

    public class CardResult
    {
        public string CardName;
        public string ViewMode;
        public string CardUrl;
        public string SecurityToken;
    }
}