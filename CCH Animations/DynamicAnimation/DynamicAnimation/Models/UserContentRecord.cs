namespace DynamicAnimation.Models
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
        public int ContentDurationSecondsCount;
        public string ContentImageFileName;
        public string ContentFileLocation;
        public int ContentPointsCount;
        public string ContentSavingsAmt;
        public string MemberContentData;
        public string NumberOfQuestions;
    }

    public class UserContentSummary
    {
        public string TotalPoints;
        public string TotalRewards;
    }
}
