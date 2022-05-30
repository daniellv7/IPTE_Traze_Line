using IPTE_Base_Project.Common.Configuration;
using System.Configuration;

namespace IPTE_Base_Project.Managers
{
    class BaseConfigurationManager
    {
        static BaseConfigurationManager()
        {
            Initialize();
        }
        
        public static int LevelsUp
        {
            get { return _levelsUp; }
        }
        private static int _levelsUp;

        public static string ConfigFolder
        {
            get { return _configFolder; }
        }
        private static string _configFolder;

        public static string ConfigFile
        {
            get { return _configFile; }
        }
        private static string _configFile;

        private static void Initialize()
        {
            try
            {
                BaseConfigurationProvider baseConfigurationSetUp = (BaseConfigurationProvider)ConfigurationManager.GetSection("BaseConfigurationSetUp");
                if (baseConfigurationSetUp == null)
                {
                    throw new ConfigurationErrorsException("Base configuration is not set. Check App.config file.");
                }

                if (!int.TryParse(baseConfigurationSetUp.LevelsUp, out _levelsUp))
                {
                    throw new ConfigurationErrorsException("Levels Up is not a number. Check App.config file.");
                }
                if (_levelsUp < 0)
                {
                    throw new ConfigurationErrorsException("Levels Up is not a positive number. Check App.config file.");
                }
                _configFolder = baseConfigurationSetUp.ConfigFolder;
                _configFile = baseConfigurationSetUp.ConfigFile;
            }
            catch (System.TypeInitializationException ex)
            {
                System.Diagnostics.Debug.Write("Exception while initializing log providers: " + ex);
            }
        }
    }
}
