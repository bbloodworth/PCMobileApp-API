using ClearCost.Data;

namespace CchWebAPI.Services
{
    public class ConfigService
    {
        private ClientConfigRepository _clientConfigRepository;

        public ConfigService(int employerId)
        {
            _clientConfigRepository = new ClientConfigRepository(employerId);
        }

        public string GetValue(string configKey)
        {
            string configValue = _clientConfigRepository.GetValue<string>(configKey);
            return configValue;
        }
    }
}
