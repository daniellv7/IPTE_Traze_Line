//using IPTE_Base_Project.DataSources;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Common.Configuration;
using IPTE_Base_Project.Managers;
using IPTE_Base_Project.Devices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Linq;

namespace IPTE_Base_Project
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindowView mainWindowView = new MainWindowView();
            mainWindowView.SourceInitialized += (s, a) => mainWindowView.WindowState = WindowState.Maximized;
            mainWindowView.Show();
            //log.Info("APPLICATION STARTED");
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            //log.Info("APPLICATION STOPPED");
        }
    }
}
