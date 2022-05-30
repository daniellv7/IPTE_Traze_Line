using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Ipte.TS1.UI.Analytics;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;

namespace Ipte.UI.Pages
{
    public partial class AnalyzeSpcPage : UserControl
    {
        public AnalyzeSpcPage()
        {
            InitializeComponent();

            //Controller.Instance.RoutingTask.SpcMarkMeasured += AddDataPointAsync;
        }

        public void Initialize()
        {
            chart.Update();
        }

        private void AddDataPointAsync(object sender, Point3D position, string source)
        {
            DateTime eventTime = FastClock.Now; // use fastclock here to prevent blocking of sender task

            try
            {
                SpcChart.AddSpcDataPoint(
                    eventTime,
                    string.Empty,
                    string.Format("{0} SPC mark {1}", source, "X"),
                    "[mm]",
                    position.X);

                SpcChart.AddSpcDataPoint(
                    eventTime,
                    string.Empty,
                    string.Format("{0} SPC mark {1}", source, "Y"),
                    "[mm]",
                    position.Y);
            }
            catch { }
        }
    }
}
