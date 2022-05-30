using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Ipte.TS1.UI.Controls;
using Ipte.TS1.UI.i18n;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;
using Ipte.Machine.Configuration;
using Ipte.UI.Dialogs;
using Ipte.TS1.UI.Analytics;
using System.IO;
using System.Linq;

namespace Ipte.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region WinAPI call

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        #endregion

        #region Private fields

        private DispatcherTimer _buttonUpdater;
        private DateTime _logoffTime = DateTime.MaxValue;

        #endregion

        #region Constructor / Destructor

        public MainWindow()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                return;
            }

            //Output the trace errors to a log file
            string traceLogFile = string.Concat(Paths.LogDirectory, @"\trace log ", DateTime.Today.ToString("yyy MM dd"), ".log");
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(traceLogFile) { TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Callstack, IndentSize = 4 });

            try
            {
                //initialize controller.
                Controller.Instance.GetType();
                Application.Current.MainWindow = this;
            }
            catch (Exception ex)
            {
                GuiMessageBox.Show(ex.ToString(), this.i18nTranslate("Could not instanciate Hardware!"));
                Environment.Exit(0);
            }

            try
            {
                //initialize UI
                InitializeComponent();

                Title = Paths.CellName;
                AnalyticsControl.AssignDatabase2(Paths.StatisticsDatabase);

                if (File.Exists(Paths.SettingsDirectory + "\\CustomerLogo.png"))
                    frame.CustomerLogo = new System.Windows.Media.Imaging.BitmapImage(new Uri(Paths.SettingsDirectory + "\\CustomerLogo.png"));
                else
                    frame.CustomerLogo = null;
            }
            catch (Exception ex)
            {
                //often application crashes because main form can't init pages. At least have courecy of showing error message here.
                GuiMessageBox.Show(ex.ToString(), this.i18nTranslate("Could not instanciate UI!"));
                Environment.Exit(0);
            }

            AccessManager.UserChanged += (Object sender, EventArgs e) =>
            {
                Controller.Instance.User = AccessManager.UserName;
                if (AccessManager.AccessLevel > UserLevel.Operator)
                {
                    _logoffTime = DateTime.UtcNow + TimeSpan.FromSeconds(Controller.Instance.Settings.AutoLogOffTimeS);
                }
            };

            AccessManager.AccessLevelChanged += (Object sender, EventArgs e) =>
            {
                adminConfigTab.Visibility = AccessManager.AccessLevel == UserLevel.Administrator ? Visibility.Visible : Visibility.Collapsed;
                adminConfigPage.Visibility = AccessManager.AccessLevel == UserLevel.Administrator ? Visibility.Visible : Visibility.Collapsed;

                // When page not available anymore: show first available page
                // When tab not available anymore: show first avialable page of first available tab
                var lastTab = frame.SelectedItem as TabItem; if (lastTab == null) return;
                var lastTabControl = lastTab.Content as TabControl; if (lastTabControl == null) return;
                var lastPage = lastTabControl.SelectedItem as TabItem; if (lastPage == null) return;
                if (lastPage.Visibility == Visibility.Collapsed)
                {
                    var item = lastTabControl.Items.Cast<TabItem>().Where(i => i.Visibility == Visibility.Visible).First();
                    item.IsSelected = true;
                }
                if (lastTabControl.Visibility == Visibility.Collapsed)
                {
                    var firstTab = frame.Items.Cast<TabItem>().Where(i => i.Visibility == Visibility.Visible).First();
                    frame.SelectedItem = firstTab;
                    var page = (firstTab.Content as TabControl);
                    var item = page.Items.Cast<TabItem>().Where(i => i.Visibility == Visibility.Visible).First();
                    page.SelectedItem = item;
                }
            };

            //connect control button
            frame.StartButtonClick += Start;
            frame.PauseButtonClick += Pause;
            frame.StopButtonClick += Stop;

            frame.AlarmButtonClick += delegate (Object sender, RoutedEventArgs e)
            {
                frame.SelectedIndex = 0;
                this.ObserveWindow.SelectedIndex = 0;
            };

            frame.HelpButtonClick += delegate (object sender, RoutedEventArgs e)
            {
                DocumentViewerDialog dlg = new DocumentViewerDialog();
                dlg.Show();
            };

            frame.AboutButtonClick += delegate (object sender, RoutedEventArgs e)
            {
                AboutBox aboutbox = new AboutBox();
                aboutbox.Owner = this;
                aboutbox.ShowDialog();
            };

            frame.SelectionChanged += WindowChanged;

            //Initiate state updater
            _buttonUpdater = new DispatcherTimer();
            _buttonUpdater.Interval = TimeSpan.FromMilliseconds(100);
            _buttonUpdater.Tick += new EventHandler(buttonUpdater_Tick);
            _buttonUpdater.IsEnabled = true;

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (Controller.Instance.State > CamxState.Off)
            {
                MessageBoxResult result = GuiMessageBox.Show(
                    this,
                    this.i18nTranslate("The machine must be stopped before the application may exit. Press 'Yes' to stop the machine immediately."),
                    this.i18nTranslate("Do you want to stop the machine now?"),
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Controller.Instance.Stop();
                    frame.IsStartButtonBlinking = false;
                    frame.IsPauseButtonBlinking = false;
                    frame.IsStopButtonBlinking = true;
                }

                e.Cancel = true;
                return;
            }

            //Make UI transparent to indicate that app is closing. The closing itself my take some time.
            Opacity = 0.8;
            Effect = new BlurEffect();
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            _buttonUpdater.IsEnabled = false;
            cellPage.UpdatingEnabled = false;
            devicesPage.UpdatingEnabled = false;
            ioPage.UpdatingEnabled = false;
            cellPage.DisconnectEquipment();

            try
            {
                Controller.Instance.Dispose();//Release the controller
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to dispose controller: {0}", ex);
            }

            try
            {
                Trace.Flush();
            }
            catch
            {
            }

            base.OnClosing(e);
        }

        #endregion

        #region Page change handler

        private void WindowChanged(object sender, SelectionChangedEventArgs e)
        {
            //the selectionchanged event is also raised when listbox changes
            if (e != null && !(e.OriginalSource is TabControl))
            {
                return;
            }

            cellPage.UpdatingEnabled = false;
            devicesPage.UpdatingEnabled = false;
            ioPage.UpdatingEnabled = false;

            switch (frame.SelectedIndex)
            {
                case 0://Observe window
                    switch (ObserveWindow.SelectedIndex)
                    {
                        case 0:
                            cellPage.Initialize();
                            cellPage.UpdatingEnabled = true;
                            break;
                        case 1:
                            devicesPage.UpdatingEnabled = true;
                            break;
                    }
                    break;
                case 1://settings
                    switch (ConfigureWindow.SelectedIndex)
                    {
                        case 0:
                            //configPage.Initialize();
                            break;
                        case 1:
                            adminConfigPage.Initialize();
                            break;
                    }
                    break;
                case 2://I/o
                    switch (ManipulateWindow.SelectedIndex)
                    {
                        case 0:
                            //actionsPage.UpdatingEnabled = true;
                            break;
                        case 1:
                            ioPage.UpdatingEnabled = true;
                            break;
                    }
                    break;
                case 3://statistics
                    switch (AnalyzeWindow.SelectedIndex)
                    {
                        case 0:
                            cyclePage.Initialize();
                            break;
                        case 1:
                            machineEventsPage.Initialize();
                            break;
                        case 2:
                            inOutPage.Initialize();
                            break;
                        case 3:
                            SpcPage.Initialize();
                            break;
                    }
                    break;
            }
        }

        #endregion

        #region Start, Pause, Stop, etc button management

        public void Start(Object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.Instance.Start();
                frame.IsStartButtonBlinking = true;
                frame.IsPauseButtonBlinking = false;
                frame.IsStopButtonBlinking = false;
            }
            catch (InvalidOperationException ex)
            {
                GuiMessageBox.Show(this, ex.Message, this.i18nTranslate("Failed to start"));
            }
        }

        public void Pause(Object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.Instance.Pause();
                frame.IsStartButtonBlinking = false;
                frame.IsPauseButtonBlinking = true;
                frame.IsStopButtonBlinking = false;
            }
            catch (InvalidOperationException ex)
            {
                GuiMessageBox.Show(this, ex.Message, this.i18nTranslate("Failed to pause"));
            }
        }

        public void Stop(Object sender, RoutedEventArgs e)
        {
            try
            {
                Controller.Instance.Stop();
                frame.IsStartButtonBlinking = false;
                frame.IsPauseButtonBlinking = false;
                frame.IsStopButtonBlinking = true;
            }
            catch (InvalidOperationException ex)
            {
                GuiMessageBox.Show(this, ex.Message, this.i18nTranslate("Failed to stop"));
            }
        }

        void buttonUpdater_Tick(object sender, EventArgs e)
        {
            #region Automatic logoff

            if (AccessManager.AccessLevel > UserLevel.Operator)
            {
                LASTINPUTINFO lii = new LASTINPUTINFO();
                lii.cbSize = (uint)Marshal.SizeOf(lii);

                if (GetLastInputInfo(ref lii))
                {
                    var idleTime = TimeSpan.FromMilliseconds(Environment.TickCount - lii.dwTime);
                    _logoffTime = DateTime.UtcNow - idleTime + TimeSpan.FromSeconds(Controller.Instance.Settings.AutoLogOffTimeS);
                }


                if (DateTime.UtcNow > _logoffTime)
                {
                    AccessManager.LogOut();
                }
            }

            #endregion

            Controller controller = Controller.Instance;

            frame.ServiceIndicatorVisibility = controller.ServiceMode ? Visibility.Visible : Visibility.Hidden;
            frame.IsAlarmButtonEnabled = controller.HasAlarm || controller.HasError || controller.HasWarning;
            frame.IsAlarmButtonBlinking = frame.IsAlarmButtonEnabled;

            switch (controller.State)
            {
                case CamxState.Off:
                    frame.StartButtonVisibility = Visibility.Visible;
                    frame.PauseButtonVisibility = Visibility.Hidden;
                    frame.IsStartButtonEnabled = !controller.EmergencyStop && !controller.ServiceMode;// && (controller.IsHalConnected || controller.Settings.Mode == Mode.Simulation);
                    frame.IsStartButtonBlinking = controller.PendingRequest == Request.Start;
                    frame.IsPauseButtonEnabled = false;
                    frame.IsStopButtonEnabled = false;
                    frame.IsStopButtonBlinking = false;
                    break;
                case CamxState.Down:
                    frame.StartButtonVisibility = Visibility.Visible;
                    frame.PauseButtonVisibility = Visibility.Hidden;
                    frame.IsStartButtonEnabled = !controller.EmergencyStop && !controller.ServiceMode;// && (controller.IsHalConnected || controller.Settings.Mode == Mode.Simulation);
                    frame.IsPauseButtonEnabled = false;
                    frame.IsStopButtonEnabled = true;
                    frame.IsPauseButtonBlinking = false;
                    frame.IsStartButtonBlinking = false;
                    break;
                case CamxState.Setup:
                    frame.StartButtonVisibility = Visibility.Visible;
                    frame.PauseButtonVisibility = Visibility.Hidden;
                    frame.IsStartButtonEnabled = false;
                    frame.IsPauseButtonEnabled = false;
                    frame.IsStopButtonEnabled = true;
                    break;
                default:
                    frame.StartButtonVisibility = Visibility.Hidden;
                    frame.PauseButtonVisibility = Visibility.Visible;
                    frame.IsStartButtonEnabled = false;
                    frame.IsPauseButtonEnabled = !controller.EmergencyStop;
                    frame.IsStopButtonEnabled = true;
                    frame.IsStartButtonBlinking = false;
                    break;
            }

            #region Synchronize hardware start/stop buttons with gui ones

            try
            {
                if (controller.IsHalConnected && controller.GeneralIO.IsConnected)
                {
                    if (frame.StartButtonVisibility == Visibility.Visible)
                    {
                        // check if HW start button has been pressed
                        if (controller.GeneralIO.StartButton && frame.IsStartButtonEnabled)
                        {
                            Start(this, new RoutedEventArgs());
                        }

                        // update HW start button light according to SW button
                        if (frame.IsStartButtonBlinking)
                        {
                            controller.GeneralIO.StartButtonLight.Value = DateTime.UtcNow.Millisecond > 500;
                        }
                        else
                        {
                            controller.GeneralIO.StartButtonLight.Value = frame.IsStartButtonEnabled;
                        }
                    }
                    else
                    {
                        // if SW start button is disabled or not visible - also HW button light is off
                        controller.GeneralIO.StartButtonLight.Value = false;
                    }

                    //if (frame.PauseButtonVisibility == Visibility.Visible)
                    //{
                    //    // check if HW pause button has been pressed
                    //    if (controller.GeneralIO.PauseButton && frame.IsPauseButtonEnabled)
                    //    {
                    //        Pause(this, new RoutedEventArgs());
                    //    }

                    //    // update HW pause button light according to SW button
                    //    if (frame.IsPauseButtonBlinking)
                    //    {
                    //        controller.GeneralIO.PauseButtonLight.Value = DateTime.UtcNow.Millisecond > 500;
                    //    }
                    //    else
                    //    {
                    //        controller.GeneralIO.PauseButtonLight.Value = frame.IsPauseButtonEnabled;
                    //    }
                    //}
                    //else
                    //{
                    //    // if SW pause button is disabled or not visible - also HW button light is off
                    //    controller.GeneralIO.PauseButtonLight.Value = false;
                    //}

                    if (frame.StopButtonVisibility == Visibility.Visible)
                    {
                        // check if HW stop button has been pressed
                        if (controller.GeneralIO.StopButton && frame.IsStopButtonEnabled)
                        {
                            Stop(this, new RoutedEventArgs());
                        }

                        // update HW stop button light according to SW button
                        if (frame.IsStopButtonBlinking)
                        {
                            controller.GeneralIO.StopButtonLight.Value = DateTime.UtcNow.Millisecond > 500;
                        }
                        else
                        {
                            controller.GeneralIO.StopButtonLight.Value = frame.IsStopButtonEnabled;
                        }
                    }
                    else
                    {
                        // if SW stop button is disabled or not visible - also HW button light is off
                        controller.GeneralIO.StopButtonLight.Value = false;
                    }
                }
            }
            catch
            {
                //io might not be connected. ignore for now.
            }

            #endregion
        }

        #endregion

        private void frame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

