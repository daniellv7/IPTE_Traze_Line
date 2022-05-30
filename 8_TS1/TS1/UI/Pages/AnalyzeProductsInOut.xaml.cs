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
    /// Interaction logic for AnalyzeItemEventsPage.xaml
    /// </summary>
    public partial class AnalyzeProductsInOut : UserControl
    {
        public AnalyzeProductsInOut()
        {
            InitializeComponent();

            // bind to machine
            Controller controller = Controller.Instance;
            controller.LoadingSegment.ItemTransferIn += AddDataPointAsync;
            controller.UnloadingSegment.ItemTransferOut += AddDataPointAsync;
        }

        private void AddDataPointAsync(object sender, CamxEventArgs e)
        {
            DateTime eventTime = e.DateTime;
            string message = e.GetType().Name;
            if (e is ItemTransferInEventArgs)
                message = "Item Transfer in";
            if (e is ItemTransferOutEventArgs)
                message = "Item Transfer out";
            string recipeName = string.Empty;

            CamxZone ipc = sender as CamxZone;
            string device = ipc.ZoneId + " @ " + ipc.LaneId;

            Action add = () =>
            {
                try
                {
                    ItemEventsChart.AddItemEvent(eventTime, recipeName, device, message);
                }
                catch { }
            };

            Dispatcher.BeginInvoke(add);
        }

        private void AddCycleDataPointAsync(object sender, TimeSpan elapsed, Product product)
        {
            DateTime eventTime = FastClock.Now; // use fastclock here to prevent blocking of sender task
            string productStatus = product == null ? "GOOD" : product.IsFailed ? "BAD" : "GOOD";
            CamxZone ipc = sender as CamxZone;
            string device = ipc.ZoneId + " @ " + ipc.LaneId;

            Action add = () =>
            {
                try { ItemEventsChart.AddItemEvent(eventTime, "", device, productStatus); }
                catch { }
            };

            Dispatcher.BeginInvoke(add);
        }

        public void Initialize() { }
        public bool UpdatingEnabled { get; set; }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}

