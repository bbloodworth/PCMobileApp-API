namespace CchWebAPI.Areas.Animation.Models
{
    public class UserContentRecord
    {
        public string ContentId;
        public string ContentType;
        public string ContentStatus;
        public string ContentTitle;
        public string ContentCaption;
        public string ContentName;
        public string ContentDescription;
        public string Duration;
        public string ContentImageFileName;
        public string ContentFileLocation;
        public string Points;
        public string ContentSavingsAmt;
        public string MemberContentData;
        public string NumberOfQuestions;
        public string ParentContentId;
        public string ContentUrl;
        public string ContentPhoneNum;
        public string CreateDate;
    }

    public class UserContentSummary
    {
        public string TotalPoints;
        public string TotalRewards;
    }
}