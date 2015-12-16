using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class LogVideoEvent : DataBase
    {
        public String VideoCampaignMemberId
        {
            set { Parameters["VideoCampaignMemberId"].Value = value; }
        }
        public String VideoLogEvent
        {
            set { Parameters["VideoLogEvent"].Value = value; }
        }
        public Int32 VideoLogEventId
        {
            set { Parameters["VideoLogEventId"].Value = value; }
        }
        public String Comment
        {
            set { Parameters["Comment"].Value = value; }
        }

        public LogVideoEvent()
            : base("LogVideoEvent")
        {
            Parameters.New("VideoCampaignMemberId", SqlDbType.NVarChar, Size: 50);
            Parameters.New("VideoLogEvent", SqlDbType.NVarChar, Size: 50);
            Parameters.New("VideoLogEventId", SqlDbType.Int);
            Parameters.New("Comment", SqlDbType.NVarChar, Size: 300);
        }
    }
}