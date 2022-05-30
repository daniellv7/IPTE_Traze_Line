using System.Configuration;

namespace IPTE_Base_Project.Common.Log
{
    class LogProviderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        [ConfigurationProperty("default")]
        public string DefaultProviderName
        {
            get
            {
                return base["default"] as string;
            }
        }
    }
}
