using System.Data;

namespace CchWebAPI.Support
{
    public class GetDevicePushPromptStatus : DataBase
    {
        public string DeviceId { set { Parameters["DEVICEID"].Value = value; } }

        public GetDevicePushPromptStatus()
            : base("p_GetDevicePushPromptStatus")
        {
            Parameters.New("DEVICEID", SqlDbType.NVarChar, Size: 36);
        }

        public GetDevicePushPromptStatus(string deviceId)
            : base("p_GetDevicePushPromptStatus")
        {
            Parameters.New("DEVICEID", SqlDbType.NVarChar, Size: 36, Value: deviceId);
        }

        public bool PromptStatus
        {
            get
            {
                string promptStatus = Tables[0].Rows.Count > 0 ? Tables[0].Rows[0]["PromptStatus"].ToString() : string.Empty;

                return promptStatus.Equals("1");
            }
        }
    }
}