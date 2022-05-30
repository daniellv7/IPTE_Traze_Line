using System;
using System.Windows.Controls;
using Ipte.TS1.UI.Analytics;
//using Ipte.TS1.StateMachine.CAMX;
//using Ipte.Machine;
//using Ipte.Machine.Config;
//using Ipte.Machine.Configuration;

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
            //AnalyticsControl.AssignDatabase2("c:\\programdata\\mydb.sqlite3");
            //for (int i = 0; i < 100; i++)
            //{
               

                // DateTime Stime2 = new DateTime(2019, 9, 10, 12, 00, 00);
            //   // MachineEventsChart.AddMachineEvent(DateTime.Now, "Cell 100", "Alerta");
            //    DateTime Stime = new DateTime(2019 ,9, 12,12,00,00);
                
            //    //CycleTimeChart.AddCycleTime(DateTime.Now + TimeSpan.FromSeconds(i * 10), "My recipe", TimeSpan.FromSeconds(25));
            //   // SpcChart.AddSpcDataPoint(DateTime.Now + TimeSpan.FromSeconds(i * 10), "My recipe", "screw torque", "Nm", 2);
            //}
            //// bind to machine
            /////if (Controller.Instance.MiddleSegment!= null) Controller.Instance.MiddleSegment.CycleCompleted += AddDataPointAsync;
        }

        private void AddDataPointAsync(object sender, TimeSpan elapsed)
        {
            DateTime eventTime = DateTime.Now; // use fastclock here to prevent blocking of sender task
          //  CamxZone ipc = sender as CamxZone;
           // string device = ipc.ZoneId + " @ " + ipc.LaneId;

            Action add = () =>
            {
                try { CycleTimeChart.AddCycleTime(eventTime, "", elapsed); }
                catch { }
            };

            Dispatcher.BeginInvoke(add);
            
        }

        public void Initialize() { }
        public bool UpdatingEnabled { get; set; }

        private void GuiButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Chart_Event.Update();
        }
    }
}

