using System;
using System.Windows.Controls;
using Ipte.TS1.UI.Analytics;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;

namespace Ipte.UI.Pages
{
    /// <summary>
    /// Interaction logic for AnalyzeMachineEventsPage.xaml
    /// </summary>
    public partial class AnalyzeMachineEventsPage : UserControl
    {
        public AnalyzeMachineEventsPage()
        {
            InitializeComponent();

            Controller.Instance.EquipmentError += AddDataPointAsync;
            Controller.Instance.EquipmentAlarm += AddDataPointAsync;
        }

        private void AddDataPointAsync(object sender, CamxEventArgs e)
        {
            string eventSource = string.Empty;
            string eventId = string.Empty;

            if (e is EquipmentErrorEventArgs)
            {
                eventSource = (e as EquipmentErrorEventArgs).ZoneList + " @ " + (e as EquipmentErrorEventArgs).LaneList;
                eventId = (e as EquipmentErrorEventArgs).ErrorId;
            }
            else if (e is EquipmentAlarmEventArgs)
            {
                eventSource = (e as EquipmentAlarmEventArgs).ZoneList + " @ " + (e as EquipmentAlarmEventArgs).LaneList;
                eventId = (e as EquipmentAlarmEventArgs).AlarmId;
            }

            Action add = () =>
            {
                try { MachineEventsChart.AddMachineEvent(e.DateTime, eventSource, eventId); }
                catch { }
            };

            Dispatcher.BeginInvoke(add);
        }

        public void Initialize() { }
        public bool UpdatingEnabled { get; set; }
    }
}

