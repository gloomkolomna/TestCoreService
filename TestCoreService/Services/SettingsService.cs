using Microsoft.Extensions.Configuration;

namespace TestCoreService.Services
{
    public class SettingsService : ISettingService
    {
        private readonly IConfiguration _configuration;
        public SettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int WorkersCount => _configuration.GetValue<int>("WorkersCount");

        public int RunInterval => _configuration.GetValue<int>("RunInterval");

        public string ResultPath => _configuration.GetValue<string>("ResultPath");

        public string InstanceName => _configuration.GetValue<string>("InstanceName");
    }
}
