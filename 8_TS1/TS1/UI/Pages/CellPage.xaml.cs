using Ipte.TS1.UI.Controls;
using Ipte.TS1.UI.i18n;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ipte.UI.Pages
{
    /// <summary>
    /// Interaction logic for CellPage.xaml
    /// </summary>
    public partial class CellPage : UserControl
    {
        #region Constructor
        public CellPage()
        {
            InitializeComponent();

            //do not create controller when opened from visual studio/expression blend designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            ConnectEquipment();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += new EventHandler(UpdatePage);
        }

        public void Initialize()
        {

        }

        public void ConnectEquipment()
        {
            Controller controller = Controller.Instance;
            controller.EquipmentAlarm += InvokeShowEquipmentAlarm;
            controller.EquipmentAlarmCleared += InvokeClearEquipmentAlarm;

            controller.EquipmentError += InvokeShowEquipmentError;
            controller.EquipmentErrorCleared += InvokeClearEquipmentError;

            controller.EquipmentWarning += InvokeShowEquipmentWarning;
            controller.EquipmentWarningCleared += InvokeClearEquipmentWarning;

            controller.EquipmentChangeState += SycnEquipmentChangeState;
        }

        public void DisconnectEquipment()
        {
            //TODO: is it really needed
            Controller controller = Controller.Instance;

            controller.EquipmentAlarm -= InvokeShowEquipmentAlarm;
            controller.EquipmentAlarmCleared -= InvokeClearEquipmentAlarm;

            controller.EquipmentError -= InvokeShowEquipmentError;
            controller.EquipmentErrorCleared -= InvokeClearEquipmentError;

            controller.EquipmentWarning -= InvokeShowEquipmentWarning;
            controller.EquipmentWarningCleared -= InvokeClearEquipmentWarning;

            controller.EquipmentChangeState -= SycnEquipmentChangeState;
        }

        #endregion

        #region Page updating

        private DispatcherTimer _updateTimer;

        void UpdatePage(object sender, EventArgs e)
        {
            Controller controller = Controller.Instance;

            try
            {
                UpdateStateCounters(this, new EquipmentChangeStateEventArgs(controller.State, controller.State, ""));
            }
            catch { };
        }

        public bool UpdatingEnabled
        {
            get { return _updateTimer.IsEnabled; }
            set { _updateTimer.IsEnabled = value; }
        }

        #endregion

        #region Statistics

        public static readonly DependencyProperty AvailabilityProperty = DependencyProperty.Register("Availability", typeof(double), typeof(CellPage), new UIPropertyMetadata(0.0));
        public static readonly DependencyProperty UtilizationProperty = DependencyProperty.Register("Utilization", typeof(double), typeof(CellPage), new UIPropertyMetadata(0.0));
        public static readonly DependencyProperty DownTimeProperty = DependencyProperty.Register("DownTime", typeof(double), typeof(CellPage), new UIPropertyMetadata(0.0));

        private DateTime _lastStateChange = DateTime.Now;
        private TimeSpan _activeTime;
        private TimeSpan _idleTime;
        private TimeSpan _downTime;

        public double Availability
        {
            get { return (double)GetValue(AvailabilityProperty); }
        }

        public double Utilization
        {
            get { return (double)GetValue(UtilizationProperty); }
        }

        public double DownTime
        {
            get { return (double)GetValue(DownTimeProperty); }
        }

        void SycnEquipmentChangeState(object sender, EquipmentChangeStateEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentChangeStateEventArgs>)UpdateStateCounters, sender, e);
        }

        void UpdateStateCounters(object sender, EquipmentChangeStateEventArgs e)
        {
            Controller controller = Controller.Instance;

            switch (e.PreviousState)
            {
                case CamxState.Down:
                    _downTime += DateTime.Now - _lastStateChange;
                    break;
                case CamxState.Setup:
                    _idleTime += DateTime.Now - _lastStateChange;
                    break;
                case CamxState.Blocked:
                    if (controller.HasAlarm || controller.HasError)
                        _downTime += DateTime.Now - _lastStateChange;
                    else
                        _idleTime += DateTime.Now - _lastStateChange;
                    break;
                case CamxState.Starved:
                    if (controller.HasAlarm || controller.HasError)
                        _downTime += DateTime.Now - _lastStateChange;
                    else
                        _idleTime += DateTime.Now - _lastStateChange;
                    break;
                case CamxState.Active:
                    _activeTime += DateTime.Now - _lastStateChange;
                    break;
                case CamxState.Executing:
                    _activeTime += DateTime.Now - _lastStateChange;
                    break;
                default:
                    //ignore off state
                    break;
            }

            _lastStateChange = DateTime.Now;

            TimeSpan sumTime = _downTime + _idleTime + _activeTime;

            if (sumTime.TotalSeconds > 0)
            {
                double availability = (_idleTime + _activeTime).TotalSeconds / sumTime.TotalSeconds;
                double utilization = _activeTime.TotalSeconds / sumTime.TotalSeconds;
                double downTime = _downTime.TotalSeconds / sumTime.TotalSeconds;

                SetValue(AvailabilityProperty, Math.Round(availability, 2));
                SetValue(UtilizationProperty, Math.Round(utilization, 2));
                SetValue(DownTimeProperty, Math.Round(downTime, 2));
            }
        }
        #endregion

        #region Alarm, Error and Warning reporting

        private void InvokeShowEquipmentWarning(object sender, EquipmentWarningEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentWarningEventArgs>)ShowEquipmentWarning, sender, e);
        }

        private void ShowEquipmentWarning(object sender, EquipmentWarningEventArgs e)
        {
            GuiMessageItem message = new GuiMessageItem();
            message.Severity = Severity.Warning;
            message.Device = e.ZoneList + " @ " + e.LaneList;
            message.Caption = e.WarningId;
            message.Description = e.Description;
            message.Tag = e.WarningInstanceId;

            message.CanClose = true;
            message.Close += delegate (object o, RoutedEventArgs a)
            {
                CamxZone device = sender as CamxZone;
                Controller controller = sender as Controller;

                if (device != null) device.ResetWarning(e);
                //if (controller != null) controller.ResetWarning(e);
            };

            MessageControl.Messages.Add(message);
        }

        private void InvokeClearEquipmentWarning(object sender, EquipmentWarningClearedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentWarningClearedEventArgs>)ClearEquipmentWarning, sender, e);
        }

        private void ClearEquipmentWarning(object sender, EquipmentWarningClearedEventArgs e)
        {
            foreach (GuiMessageItem message in MessageControl.Messages)
            {
                if ((string)message.Tag == e.WarningInstanceId)
                {
                    MessageControl.Messages.Remove(message);
                    break;
                }
            }
        }

        private void InvokeShowEquipmentError(object sender, EquipmentErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentErrorEventArgs>)ShowEquipmentError, sender, e);
        }

        private void ShowEquipmentError(object sender, EquipmentErrorEventArgs e)
        {
            GuiMessageItem message = new GuiMessageItem();
            message.Severity = Severity.Error;
            message.Device = e.ZoneList + " @ " + e.LaneList;
            message.Caption = e.ErrorId;
            message.Description = e.Description;
            message.Tag = e.ErrorInstanceId;

            message.CanRetry = e.Solutions.Contains(Solution.Retry);
            message.CanIgnore = e.Solutions.Contains(Solution.Ignore);
            message.CanReset = e.Solutions.Contains(Solution.Reset);
            message.CanClose = e.Solutions.Contains(Solution.Close);

            message.Retry += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetError(e, Solution.Retry); };
            message.Ignore += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetError(e, Solution.Ignore); };
            message.Reset += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetError(e, Solution.Reset); };
            message.Close += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetError(e, Solution.Close); };

            MessageControl.Messages.Add(message);
        }

        private void InvokeClearEquipmentError(object sender, EquipmentErrorClearedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentErrorClearedEventArgs>)ClearEquipmentError, sender, e);
        }

        private void ClearEquipmentError(object sender, EquipmentErrorClearedEventArgs e)
        {
            foreach (GuiMessageItem message in MessageControl.Messages)
            {
                if ((string)message.Tag == e.ErrorInstanceId)
                {
                    MessageControl.Messages.Remove(message);
                    break;
                }
            }
        }

        private void InvokeShowEquipmentAlarm(object sender, EquipmentAlarmEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentAlarmEventArgs>)ShowEquipmentAlarm, sender, e);
        }

        private void ShowEquipmentAlarm(object sender, EquipmentAlarmEventArgs e)
        {
            GuiMessageItem message = new GuiMessageItem();
            message.Severity = Severity.Alarm;
            message.Device = e.ZoneList + " @ " + e.LaneList;
            message.Caption = e.AlarmId;
            message.Description = e.Description;
            message.Tag = e.AlarmInstanceId;

            message.CanRetry = e.Solutions.Contains(Solution.Retry);
            message.CanIgnore = e.Solutions.Contains(Solution.Ignore);
            message.CanReset = e.Solutions.Contains(Solution.Reset);
            message.CanClose = e.Solutions.Contains(Solution.Close);

            message.Retry += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetAlarm(e, Solution.Retry); };
            message.Ignore += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetAlarm(e, Solution.Ignore); };
            message.Reset += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetAlarm(e, Solution.Reset); };
            message.Close += delegate (object o, RoutedEventArgs a) { ((CamxZone)sender).ResetAlarm(e, Solution.Close); };

            MessageControl.Messages.Add(message);
        }

        private void InvokeClearEquipmentAlarm(object sender, EquipmentAlarmClearedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<object, EquipmentAlarmClearedEventArgs>)ClearEquipmentAlarm, sender, e);
        }

        private void ClearEquipmentAlarm(object sender, EquipmentAlarmClearedEventArgs e)
        {
            foreach (GuiMessageItem message in MessageControl.Messages)
            {
                if ((string)message.Tag == e.AlarmInstanceId)
                {
                    MessageControl.Messages.Remove(message);
                    break;
                }
            }
        }

        #endregion
    }
}

