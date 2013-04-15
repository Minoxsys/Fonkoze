using System.Configuration;

namespace Web.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public string this[string key]
        {
            get { return ConfigurationManager.AppSettings[key]; }
        } 
        public ConfigurationKeys Keys { get; private set; }

        public ConfigurationService()
        {
            Keys = new ConfigurationKeys();
        }
    }
}