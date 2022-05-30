using System;
using System.Windows.Controls;
using Ipte.TS1.UI.Analytics;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;
using Ipte.Machine.Config;
using Ipte.Machine.Configuration;

namespace Ipte.UI.Pages
{
    /// <summary>
    /// Interaction logic for AnalyzeCycleTimePage.xaml
    /// </summary>
    public partial class AnalyzeCycleTimesPage : UserControl
    {
        public AnalyzeCycleTimesPage()
        {
            InitializeComponent();

            // bind to machine
            ///if (Controller.Instance.MiddleSegment!= null) Controller.Instance.MiddleSegment.CycleCompleted += AddDataPointAsync;
        }

        private void AddDataPointAsync(object sender, TimeSpan elapsed, Product product)
        {
            DateTime eventTime = FastClock.Now; // use fastclock here to prevent blocking of sender task
            CamxZone ipc = sender as CamxZone;
            string device = ipc.ZoneId + " @ " + ipc.LaneId;

            Action add = () =>
            {
                try { CycleTimeChart.AddCycleTime(eventTime, "", device, elapsed); }
                catch { }
            };

            Dispatcher.BeginInvoke(add);
        }

        public void Initialize() { }
        public bool UpdatingEnabled { get; set; }
    }
}

