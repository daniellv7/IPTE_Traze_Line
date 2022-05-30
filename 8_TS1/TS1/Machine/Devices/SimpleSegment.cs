using Ipte.Machine.Configuration;
using Ipte.TS1.IO;
using Ipte.TS1.StateMachine.CAMX;
using System.IO;
using System.Threading;
using Ipte.Machine.Config;
using Ipte.Machine.Hardware.IO;
using Ipte.Machine.Interfaces;
using Ipte.TS1.UI.Analytics;

namespace Ipte.Machine.Devices
{
    public class SimpleSegment : CamxZone, IQueryProduct
    {
        private string _productFileName;
        private EquipmentWarningEventArgs _myWarning;

        public SimpleSegment(string zoneId, string laneId) :
            base(zoneId, laneId)
        {
            _productFileName = Paths.DataDirectory + "\\" + zoneId + "@" + laneId + ".data";
            PreviousZone = new HandshakeWithPreviousZone();
            NextZone = new HandshakeWithNextZone();
            _myWarning = new EquipmentWarningEventArgs("Components low", laneId, zoneId) { Description = "Please help!" };
        }

        public HandshakeWithPreviousZone PreviousZone { get; private set; }
        public HandshakeWithNextZone NextZone { get; private set; }
        public Panel Product { get; protected set; }

        public SegmentSettings Settings { get; set; }
        public SegmentIO IO { get; set; }

        protected override void Initialize()
        {
            Log("Initializing..");
            Product = Panel.FromFile<Panel>(_productFileName);
            Thread.Sleep(Settings.InitDelay);
            Log("Init done.");
        }

        protected override void DoStep()
        {
            DoRequests();

            if (Product == null)
                Load();
            else if (!Product.IsProcessed && !Product.IsFailed)
            {
                Process();
                Product.IsProcessed = true;
            }
            else
                Unload();

            if (true)//todo: condition
                Warning(_myWarning);
            else
                ResetWarning(_myWarning);
        }

        protected override void Uninitialize()
        {
            Log("Uninitializing..");

            if (Product != null)
                Product.Save(_productFileName);
            else
                File.Delete(_productFileName);

            Log("Uninit done");
        }

        public virtual void Load()
        {
            Log("Loading...");

            if (!Settings.SimulationMode)
            {
                IO.CloseStopper.Value = true;
                while (!IO.StopperIsClosed.WaitTrue(3000))
                {
                    var solution = Error("Stopper stuck",
                        new[] { Solution.Retry, Solution.Ignore },
                        "Could not close stopper. Please check input" + IO.StopperIsClosed.Address);

                    if (solution == Solution.Ignore) break;
                }
            }

            while (!PreviousZone.BoardAvailable)
            {
                DoRequests();
                Thread.Sleep(1);
            }

            Product = PreviousZone.Data as Panel;

            try
            {
                PreviousZone.NotBusy = true;

                if (!Settings.SimulationMode)
                {

                    IO.RunConveyor.Value = true;
                    while (!IO.PanelOnStopper.WaitTrueFiltered(10000, 100))
                    {
                        var solution = Error("Product stuck",
                            new[] { Solution.Retry, Solution.Ignore, Solution.Reset },
                            "Please open the door and check if product arrived on the sensor " + IO.PanelOnStopper.Address);

                        if (solution == Solution.Ignore) break;
                    }
                }
                else
                {
                    Thread.Sleep(Settings.LoadingDelay);
                }

                //transfer panel in
                Thread.Sleep(Settings.LoadingDelay);
            }
            finally
            {
                //IO.RunConveyor.Value = false;
                while (PreviousZone.BoardAvailable)
                {
                    Thread.Sleep(1);
                }

                PreviousZone.NotBusy = false;
                Log("Loading done..");
            }
        }

        public virtual void Unload()
        {
            Log("Unloading..");

            if (!Settings.SimulationMode)
            {
                IO.CloseStopper.Value = false;
                while (!IO.StopperIsOpen.WaitTrue(3000))
                {
                    var solution = Error("Stopper stuck",
                        new[] { Solution.Retry, Solution.Ignore },
                        "Could not open stopper. Please check input" + IO.StopperIsOpen.Address);

                    if (solution == Solution.Ignore) break;
                }
            }

            NextZone.Data = Product;
            NextZone.BoardAvailable = true;

            while (!NextZone.NotBusy)
            {
                if (NextZone.TryAbortOffering())
                {
                    DoRequests();
                    NextZone.BoardAvailable = true;
                }

                Thread.Sleep(1);
            }

            try
            {
                //transfer
                if (!Settings.SimulationMode)
                {
                    IO.RunConveyor.Value = true;
                }
                else
                {
                    Thread.Sleep(Settings.UnloadingDelay);
                }
                Product = null;
            }
            finally
            {
                NextZone.BoardAvailable = false;
                while (NextZone.NotBusy)
                {
                    try { DoRequests(); } catch { }
                    Thread.Sleep(1);
                }

                if (!Settings.SimulationMode)
                {
                    IO.RunConveyor.Value = false;
                }
                Log("Unloading done.");
            }
        }

        public virtual void Process()
        {
            //SpcChart.AddSpcDataPoint();

            var recipe = RecipeBuffer.GetRecipe(Product.RecipePath);

            foreach (var module in Product.Modules)
            {
                module.IsFailed = true;
            }
        }

        object IQueryProduct.Product { get { return Product; } }
    }
}
