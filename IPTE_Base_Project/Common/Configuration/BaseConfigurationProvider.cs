using System.Configuration;

namespace IPTE_Base_Project.Common.Configuration
{
    class BaseConfigurationProvider : ConfigurationSection
    {
        [ConfigurationProperty("levelsUp")]
        public string LevelsUp
        {
            get
            {
                return base["levelsUp"] as string;
            }
        }

        [ConfigurationProperty("myConfigFolder")]
        public string ConfigFolder
        {
            get
            {
                return base["myConfigFolder"] as string;
            }
        }

        [ConfigurationProperty("myConfigFile")]
        public string ConfigFile
        {
            get
            {
                return base["myConfigFile"] as string;
            }
        }
    }
}
