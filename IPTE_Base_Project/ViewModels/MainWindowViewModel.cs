using IPTE_Base_Project.Common;
using System.ComponentModel;
using System.Windows.Input;
using IPTE_Base_Project.Models;
using System;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.ViewModels.Observe;
using IPTE_Base_Project.ViewModels.Manipulate;
using IPTE_Base_Project.ViewModels.Configure;
using IPTE_Base_Project.ViewModels.Analitics;
using IPTE_Base_Project.ViewModels.Common;
using IPTE_Base_Project.Managers;
using IPTE_Base_Project.DataSources.Config;
using System.Linq;
using IPTE_Base_Project.DataSources;
using IPTE_Base_Project.DataSources.DeviceConfig;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Collections.Generic;
using IPTE_Base_Project.Models.Devices;
using Ipte.TS1.UI.Analytics;
using Ipte.TS1.UI.Controls;
using IPTE_Base_Project.Variants;
using System.Windows.Forms;
using System.Threading;
using WPFLocalizeExtension.Engine;

namespace IPTE_Base_Project.ViewModels
{
   public  class MainWindowViewModel : ValidationBaseViewModel
    {
        #region Data members and accessors
        //
        private Visibility _SettingView = Visibility.Collapsed;
        public Visibility SettingsVisibility
        {
            get
            {
                return _SettingView;
            }
            set
            {
                _SettingView = value;
                OnPropertyChanged("SettingsVisibility");
            }
        }
        private Visibility _DataView = Visibility.Collapsed;
        public Visibility DataBaseVisibility
        {
            get
            {
                return _DataView;
            }
            set
            {
                _DataView = value;
                OnPropertyChanged("DataBaseVisibility");
            }
        }
        private Visibility _PlcView = Visibility.Collapsed;
        public Visibility PLCVisivility
        {
            get
            {
                return _PlcView;
            }
            set
            {
                _PlcView = value;
                OnPropertyChanged("PLCVisivility");
            }
        }
        private Visibility _VariableView = Visibility.Collapsed;
        public Visibility VariableVisibility
        {
            get
            {
                return _VariableView;
            }
            set
            {
                _VariableView = value;
                OnPropertyChanged("VariableVisibility");
            }
        }
        private Visibility _ShuttleTrackVisible = Visibility.Collapsed;
        public Visibility ShuttleTrackVisible
        {
            get
            {
                return _ShuttleTrackVisible;
            }
            set
            {
                _ShuttleTrackVisible = value;
                OnPropertyChanged("VariableVisibility");
            }
        }

        public MachineSettings Settings;
        public MachineSettings settingsSnapshot { get; set; }
        //Models

        //ViewModels
        private DevicesViewModel devicesVM;

        private GeneralSettingsViewModel generalSettingsVM;
        private MachineSettingsViewModel machineSettingsVM;
        private HardwareSettingsViewModel hardwareSettingsVM;
        private OtherSettingsViewModel otherSettingsVM;

        internal DevicesViewModel DevicesVM
        {
            get { return devicesVM; }
            set { devicesVM = value; }
        }

        internal GeneralSettingsViewModel GeneralSettingsVM
        {
            get { return generalSettingsVM; }
            set { generalSettingsVM = value; }
        }
        internal MachineSettingsViewModel MachineSettingsVM
        {
            get { return machineSettingsVM; }
            set { machineSettingsVM = value; }
        }
        internal HardwareSettingsViewModel HardwareSettingsVM
        {
            get { return hardwareSettingsVM; }
            set { hardwareSettingsVM = value; }
        }
        internal OtherSettingsViewModel OtherSettingsVM
        {
            get { return otherSettingsVM; }
            set { otherSettingsVM = value; }
        }
        internal ITAC_ControlViewModel ITAC_control;
        
        internal PlcViewModel PlcVM { get; set; }
       
        internal CellViewModel Cell { get; set; }

        internal AnaliticsViewModel AnaliticsVM { get; set; }
       
       
        bool UserMonitor = true;
        #endregion

        #region Commands

        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                {
                    startCommand = new RelayCommand(param => StartAutomaticCycle(param), CanStartAutomaticCycle);
                }
                return startCommand;
            }
        }
        private RelayCommand startCommand;

        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                {
                    stopCommand = new RelayCommand(param => StopAutomaticCycle(param), CanStopAutomaticCycle);
                }
                return stopCommand;
            }
        }
        private RelayCommand stopCommand;

        public ICommand ResetCommand
        {
            get
            {
                if (resetCommand == null)
                {
                    resetCommand = new RelayCommand(param => ResetAutomaticCycle(param), CanResetAutomaticCycle);
                }
                return resetCommand;
            }
        }
        private RelayCommand resetCommand;

        public ICommand AlarmCommand
        {
            get
            {
                if (alarmCommand == null)
                {
                    alarmCommand = new RelayCommand(param => AlarmClick(param), CanAlarmClick);
                }
                return alarmCommand;
            }
        }
        private RelayCommand alarmCommand;

        public ICommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                {
                    pauseCommand = new RelayCommand(param => AlarmClick(param), CanAlarmClick);
                }
                return pauseCommand;
            }
        }
        private RelayCommand pauseCommand;

        #endregion

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private int _selectedsecondaryindex;
        public int SelectedSecondaryIndex
        {
            get { return _selectedsecondaryindex; }
            set
            {
                _selectedsecondaryindex = value;
                OnPropertyChanged();
                Mediator.NotifyColleagues("HomeIndex", new object[] { _selectedsecondaryindex });
            }
        }

        #region Lang
        public List<System.Globalization.CultureInfo> AvailableCultures { get; } = new List<System.Globalization.CultureInfo>() {
        new System.Globalization.CultureInfo("en"),
        new System.Globalization.CultureInfo("es-MX")};

        private System.Globalization.CultureInfo selectedCulture = null;
        public System.Globalization.CultureInfo SelectedCulture
        {
            get { return selectedCulture; }
            set
            {
                if (value == LocalizeDictionary.Instance.Culture)
                {
                    if (selectedCulture == null) selectedCulture = value;
                    return;
                }
                selectedCulture = value;

                if (value.Name == "en")
                {
                    LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo(value.Name);
                }

                Properties.Settings.Default.UICulture = value.Name;
                Properties.Settings.Default.Save();
            }
        }
        #endregion


        #region Constructors

        public MainWindowViewModel()
        {
            //Check for another instance running , if true close current instance
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {

                System.Windows.Forms.MessageBox.Show("Another instance is running");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            Mediator.Register("UserLogout", OnLogout);

           // Dictionary<string, IDeviceModel> devices = new Dictionary<string, IDeviceModel>();

            Dictionary<string, PlcModel> devices = new Dictionary<string, PlcModel>();

            //Settings Call

            Settings = MachineSettings.Load(Paths.MachineSettingsFile);
            settingsSnapshot = Settings.Clone() as MachineSettings;
            SelectedCulture = AvailableCultures[settingsSnapshot.Station_Settings.Language];//Languaje Setup

            //Database cleanup

            AnalyticsControl.AssignDatabase2(Paths.SettingsDirectory + "\\TS1DB.sqlite3");
            MachineEventsChart.DeleteMachineEvents(DateTime.Now);
            ItemEventsChart.DeleteItemEvents(DateTime.Now);
            SpcChart.DeleteSpcDataPoints(DateTime.Now);
            
            //DataRead
            //Read Configuration or parameters to diferent files of configuration 
            
            DataReadModel DataRead = new DataReadModel(settingsSnapshot);

            ITACModel ItacModel = new ITACModel();
            ITAC_control = new ITAC_ControlViewModel(DataRead ,settingsSnapshot.Itac_settings, ItacModel);

            devices.Add("PLC1", new PlcModel(settingsSnapshot.Plc_settings));
            devices.Add("PLC2", new PlcModel(settingsSnapshot.Plc_settings_2));

          
            PlcVM = new PlcViewModel(devices, settingsSnapshot);


            PLCVisivility = Visibility.Visible;

            AccessManager.LogOut();

            SettingsVisibility = Visibility.Collapsed;

            AccessManager.AccessLevelChanged += (Object sender, EventArgs e) => AccesChange();

            MachineSettingsVM = new MachineSettingsViewModel(Settings);
            CellModel cellmodel = new CellModel();
            Cell = new CellViewModel(cellmodel, settingsSnapshot);

        }
        #endregion

        #region CommandMethods     

        private void StartAutomaticCycle(object param)
        {
            // Notify SequenceViewModel to Start
            // Only if the selected tab is the SequenceAuto or the SequenceManual
            //Mediator.NotifyColleagues("PrintTest", new string[] { "" });
            if (SelectedIndex == 0 && SelectedSecondaryIndex == 0)
            {
                Mediator.NotifyColleagues("ViewStartButtonClicked", new string[] { "SequenceAuto" });
            }
            else if (SelectedIndex == 0 && SelectedSecondaryIndex == 1)
            {
                Mediator.NotifyColleagues("ViewStartButtonClicked", new object[] { "SequenceManual" });
            }
        }

        private bool CanStartAutomaticCycle(object param)
        {
        
            //if (SettingsVM.Model.simulatePLC) return true;
            //else return plcVM.model.plcDataSource.getInt("Auto") == 1;
            return true;
        }

        private void StopAutomaticCycle(object param)
        {
        }

        private bool CanStopAutomaticCycle(object param)
        {
            return true;
        }

        private void ResetAutomaticCycle(object param)
        {
            Mediator.NotifyColleagues("ViewResetButtonClicked", new string[] { "" });
            Mediator.NotifyColleagues("ClearErrorMesssges", new string[] { "" });

        }

        private bool CanResetAutomaticCycle(object param)
        {
            return true;
        }

        private void AlarmClick(object param)
        {

        }

        private bool CanAlarmClick(object param)
        {
            return true;
        }

        #endregion

        #region Methods

        private void AccesChange()
        {

            int Level = 0;
            if (AccessManager.AccessLevel == UserLevel.Administrator)
            {
                Level = 4;
                SettingsVisibility = Visibility.Visible;
            }

            else
            {
                Level = 0;
                SelectedIndex = 0;
                SettingsVisibility = Visibility.Collapsed;
            }
            Mediator.NotifyColleagues("LevelChange", new object[] { Level });
        }


        private void OnLogin(object[] obj)
        {
            //UserChange(this, EventArgs.Empty);
            // AccessManager.LogOut();
        }

        private void OnLogout(object[] obj)
        {
            AccessManager.LogOut();
          
        }

        private void LogExamples()
        {

            //Examples of using logs, can be used like this in all classes
            log.Debug("Example of Debug log");
            log.Info("Example of Info log");
            log.Warn("Example of Warn log");
            log.Error("Example of Error log");
            int i = 0;
            try
            {
                var x = 10 / i;
            }
            catch (DivideByZeroException ex)
            {
                log.Error("Developer: we tried to divide by zero", ex);
            }
            log.Fatal("Example of Fatal log");
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            UserMonitor = false;
            Mediator.NotifyColleagues("WindowClose", new string[] { "" });
        }




        #endregion

       
    }
}
