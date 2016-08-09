using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class InsertExperienceLog : DataBase
    {
        public int ExperienceEventId
        {
            set { Parameters["ExperienceEventID"].Value = value; }
        }
        public String ExperienceEventDesc
        {
            set { Parameters["ExperienceEventDesc"].Value = value; }
        }
        public String ExperienceUserId
        {
            set { Parameters["ExperienceUserID"].Value = value; }
        }
        public int CCHID
        {
            set { Parameters["CCHID"].Value = value; }
        }
        public int ContentId
        {
            set { Parameters["ContentID"].Value = value; }
        }
        public String Comment
        {
            set { Parameters["Comment"].Value = value; }
        }
        public String DeviceId
        {
            set { Parameters["DeviceID"].Value = value; }
        }
        public String ClientVersion
        {
            set { Parameters["ClientVersion"].Value = value; }
        }

        public InsertExperienceLog()
            : base("p_InsertExperienceLog")
        {
            Parameters.New("ExperienceEventID", SqlDbType.Int);
            Parameters.New("ExperienceEventDesc", SqlDbType.NVarChar, Size: 100);
            Parameters.New("ExperienceUserID", SqlDbType.NVarChar, Size: 36);
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("ContentID", SqlDbType.Int);
            Parameters.New("Comment", SqlDbType.NVarChar, Size: 250);
            Parameters.New("DeviceID", SqlDbType.NVarChar, Size: 50);
            Parameters.New("ClientVersion", SqlDbType.NVarChar, Size: 50);
        }
    }
}
