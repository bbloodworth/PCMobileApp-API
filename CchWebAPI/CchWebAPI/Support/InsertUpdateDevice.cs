using System;
using System.Data;

namespace CchWebAPI.Support
{
    public class InsertUpdateDevice : DataBase
    {
        public string DeviceId { set { Parameters["DeviceID"].Value = value; } }
        public int ClientAllowPushInd { set { Parameters["ClientAllowPushInd"].Value = value; } }
        public int NativeAllowPushInd { set { Parameters["NativeAllowPushInd"].Value = value; } }
        public DateTime LastPushPromptDate { set { Parameters["LastPushPromptDate"].Value = value; } }

        public InsertUpdateDevice()
            : base("p_InsertUpdateDevice")
        {
            Parameters.New("DeviceID", SqlDbType.NVarChar, Size: 36);
            Parameters.New("ClientAllowPushInd", SqlDbType.Int);
            Parameters.New("NativeAllowPushInd", SqlDbType.Int);
            Parameters.New("LastPushPromptDate", SqlDbType.DateTime);
        }
    }
}
