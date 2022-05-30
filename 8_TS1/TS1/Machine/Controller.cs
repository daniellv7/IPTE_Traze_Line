using Ipte.Machine.Configuration;
using Ipte.Machine.Hardware;
using Ipte.TS1.StateMachine.CAMX;
using System;
using System.Threading;
using Ipte.Machine.Config;
using Ipte.Machine.Devices;
using Ipte.Machine.Hardware.IO;

namespace Ipte.Machine
{
    public class Controller : CamxController
    {
        private static Thread _backgroundThread;
        private static Controller _instance;
        public static Controller Instance
        {
            get
            {
                if (_instance == null) _instance = new Controller();
                return _instance;
            }
        }

        private Controller()
            : base("Cell", "")
        {
            Settings = MachineSettings.Load(Paths.MachineSettingsFile);

            GeneralIO = new GeneralIO() { Name = "General" };
            LoadingSegmentIO = new LoadingSegmentIO(".LoadinSegmentInputs", ".LoadingSegmentOutputs") { Name = "Loading segment" };
            MiddleSegmentIO = new SegmentIO(".MiddleSegmentInputs", ".MiddleSegmentOutputs") { Name = "Middle segment" };
            UnloadingSegmentIO = new SegmentIO(".UnloadingSegmentInputs", ".UnloadingSegmentOutputs") { Name = "Unloading segment" };

            LoadingSegment = new LoadingSegment("Loading Segment", "");
            MiddleSegment = new SimpleSegment("Middle Segment", "");
            UnloadingSegment = new UnloadingSegment("Unloading Segment", "");

            LoadingSegment.IO = LoadingSegmentIO;
            MiddleSegment.IO = MiddleSegmentIO;
            UnloadingSegment.IO = UnloadingSegmentIO;

            MiddleSegment.InitializationPriority = 2;

            LoadingSegment.NextZone.Connect(MiddleSegment.PreviousZone);
            MiddleSegment.NextZone.Connect(UnloadingSegment.PreviousZone);

            AddZone(LoadingSegment);
            AddZone(MiddleSegment);
            AddZone(UnloadingSegment);

            _backgroundThread = new Thread(BackgroundThreadFunc);
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();
        }

        public override void Start()
        {
            //if (!IO.AirPressureOk) throw new InvalidOperationException("no air pressure");
            var settingsSnapshot = Settings.Clone() as MachineSettings;

            LoadingSegment.Settings = settingsSnapshot.LoadingSegment;
            MiddleSegment.Settings = settingsSnapshot.MiddleSegment;
            UnloadingSegment.Settings = settingsSnapshot.UnloadingSegment;

            RecipeBuffer.Clear();

            base.Start();
        }

        public MachineSettings Settings { get; set; }

        public LoadingSegment LoadingSegment { get; set; }
        public SimpleSegment MiddleSegment { get; set; }
        public UnloadingSegment UnloadingSegment { get; set; }

        public bool IsHalConnected { get; set; }
        public GeneralIO GeneralIO { get; set; }
        public LoadingSegmentIO LoadingSegmentIO { get; set; }
        public SegmentIO MiddleSegmentIO { get; set; }
        public SegmentIO UnloadingSegmentIO { get; set; }

        private void BackgroundThreadFunc(object o)
        {
            while (true)
            {
                Thread.Sleep(1);

                try
                {
                    if (!GeneralIO.IsConnected) GeneralIO.Connect(Settings.AdsAmsAddress, 801);
                    if (!LoadingSegmentIO.IsConnected) LoadingSegmentIO.Connect("", 801);
                    if (!MiddleSegmentIO.IsConnected) MiddleSegmentIO.Connect("", 801);
                    if (!UnloadingSegmentIO.IsConnected) UnloadingSegmentIO.Connect("", 801);
                    IsHalConnected = true;

                    ServiceMode = GeneralIO.ServiceKey;
                    EmergencyStop = GeneralIO.EmergencyStop;

                    #region Update beacon lights

                    // alarm or error - blink red light
                    if (HasAlarm || HasError)
                    {
                        GeneralIO.BeaconError.Value = DateTime.UtcNow.Millisecond > 500;
                        GeneralIO.BeaconBlue.Value = GeneralIO.BeaconError.Value;
                        GeneralIO.BeaconService.Value = false;
                        GeneralIO.BeaconAutomatic.Value = false;
                    }
                    // state off - all off
                    else if (State == CamxState.Off)
                    {
                        GeneralIO.BeaconError.Value = false;
                        GeneralIO.BeaconBlue.Value = GeneralIO.BeaconError.Value;
                        GeneralIO.BeaconService.Value = false;
                        GeneralIO.BeaconAutomatic.Value = false;
                    }
                    // state setup - only yellow is blinking
                    else if (State == CamxState.Setup)
                    {
                        GeneralIO.BeaconError.Value = false;
                        GeneralIO.BeaconBlue.Value = GeneralIO.BeaconError.Value;
                        GeneralIO.BeaconService.Value = DateTime.UtcNow.Millisecond > 500;
                        GeneralIO.BeaconAutomatic.Value = false;
                    }
                    // state down (paused) - only yellow is on continuously
                    else if (State == CamxState.Down)
                    {
                        GeneralIO.BeaconError.Value = false;
                        GeneralIO.BeaconBlue.Value = GeneralIO.BeaconError.Value;
                        GeneralIO.BeaconService.Value = true;
                        GeneralIO.BeaconAutomatic.Value = false;
                    }
                    else
                    {
                        GeneralIO.BeaconError.Value = false;
                        GeneralIO.BeaconBlue.Value = GeneralIO.BeaconError.Value;

                        // blink yellow if warning active
                        if (HasWarning)
                            GeneralIO.BeaconService.Value = DateTime.UtcNow.Millisecond > 500;
                        else
                            GeneralIO.BeaconService.Value = false;

                        // if blocked or starved - blink green
                        if ((State == CamxState.Starved) || (State == CamxState.Blocked))
                            GeneralIO.BeaconAutomatic.Value = DateTime.UtcNow.Millisecond > 500;
                        else
                            GeneralIO.BeaconAutomatic.Value = true;
                    }

                    #endregion
                }
                catch { }
            }
        }
    }
}
