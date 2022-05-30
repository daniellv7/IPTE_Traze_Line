using IPTE_Base_Project.ViewModels;
using System.Diagnostics;
using System.Windows;

// Una única vez en toda la solucion
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Config/Log4Net.xml", Watch = true)]

namespace IPTE_Base_Project
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        #region Data members and accessors
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private MainWindowViewModel mainWindowVM;
        private LogControlViewModel logControlVM;//log
        #endregion

        public MainWindowView()
        {

           
            log.Debug("MainWindowView => MainWindowView");
            InitializeComponent();
            Frame.IsStartButtonBlinking = true;
            //Log
            logControlVM = new LogControlViewModel();
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(logControlVM);

            #region Help,About,... Buttons

            //Frame.HelpButtonClick += delegate (object sender, RoutedEventArgs e)
            //{
            //    Process.Start(@"\\IPCELL1200\Users\Public\Documentation\");

            //    //DocumentViewerDialog dlg = new DocumentViewerDialog();
            //    //dlg.Show();
            //};

            Frame.AboutButtonClick += delegate (object sender, RoutedEventArgs e)
            {
                //AboutBox aboutbox = new AboutBox();
                //aboutbox.Owner = this;
                //aboutbox.ShowDialog();
            };
            #endregion

            //MainWindow
            mainWindowVM = new MainWindowViewModel();
            DataContext = mainWindowVM;
            CellView.DataContext = mainWindowVM.Cell;

            //Observe
          
            LogView.DataContext = logControlVM;
           // DataView.DBVM = mainWindowVM.DBVM;
           
            MachineSettingsView.Initialize(mainWindowVM.Settings);
           
            //MachineSettingsView.Refresh();
            //ItemView.DataContext = mainWindowVM.AnaliticsVM;
            //Mediator.Register("UserLogin", OnLogin);


            //Configure

            MachineSettingsView.DataContext = mainWindowVM.MachineSettingsVM;

            Closing += mainWindowVM.OnWindowClosing;
            
            log.Debug("MainWindowView <= MainWindowView");
            Frame.IsStartButtonBlinking = false;

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindowVM.ITAC_control.Init();
            foreach (var Models in mainWindowVM.PlcVM.devices)
            {
                Models.Value.Plc.Init();
                Models.Value.Plc.StartMonitoring();
            }

            //mainWindowVM.PlcVM.Model.Plc.Init();
            //mainWindowVM.PlcVM.Model.Plc.StartMonitoring();

        }
    }
}
