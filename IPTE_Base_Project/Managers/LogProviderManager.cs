using IPTE_Base_Project.Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace IPTE_Base_Project.Managers
{
    class LogProviderManager
    {
        static LogProviderManager()
        {
            Initialize();
        }

        private static LogProviderBase _default;
        /// <summary>
        /// Returns the default configured data provider
        /// </summary>
        public static LogProviderBase Default
        {
            get { return _default; }
        }

        private static LogProviderCollection _providerCollection;
        /// <summary>
        /// .Returns the provider collection
        /// </summary>
        public static LogProviderCollection Providers
        {
            get { return _providerCollection; }
        }

        private static ProviderSettingsCollection _providerSettings;
        public static ProviderSettingsCollection ProviderSettings
        {
            get { return _providerSettings; }
        }

        /// <summary>
        /// Reads the configuration related to the set of configured 
        /// providers and sets the default and collection of providers and settings.
        /// </summary>
        private static void Initialize()
        {
            try
            {
                LogProviderConfiguration configSection = (LogProviderConfiguration)ConfigurationManager.GetSection("LogProviders");
                if (configSection == null)
                {
                    throw new ConfigurationErrorsException("Data provider section is not set.");
                }

                _providerCollection = new LogProviderCollection();
                ProvidersHelper.InstantiateProviders(configSection.Providers, _providerCollection, typeof(LogProviderBase));

                _providerSettings = configSection.Providers;

                if (_providerCollection[configSection.DefaultProviderName] == null)
                {
                    throw new ConfigurationErrorsException("Default data provider is not set.");
                }

                _default = _providerCollection[configSection.DefaultProviderName];
                var defaultSettings = _providerSettings[configSection.DefaultProviderName];

                _default.SetParameters(defaultSettings.Parameters);
            }
            catch (System.TypeInitializationException ex)
            {
                System.Diagnostics.Debug.Write("Exception while initializing log providers: " + ex);
            }
        }
    }
}
