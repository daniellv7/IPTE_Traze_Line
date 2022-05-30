using Ipte.TS1.IO.Beckhoff.Tc2;
using System;

namespace Ipte.Machine.Hardware
{
    public class GeneralIO : StructBase
    {
        public Input<bool> EmergencyStop { get; set; }
        public Input<bool> ResetButton { get; set; }
        public Input<bool> AirPressureOk { get; set; }
        public Input<bool> ServiceKey { get; set; }
        public Input<bool> StartButton { get; set; }
        public Input<bool> StopButton { get; set; }
        public Input<bool> PauseButton { get; set; }

        public Output<bool> StartButtonLight { get; set; }
        public Output<bool> StopButtonLight { get; set; }
        public Output<bool> PauseButtonLight { get; set; }
        public Output<bool> BeaconAutomatic { get; set; }
        public Output<bool> BeaconService { get; set; }
        public Output<bool> BeaconError { get; set; }
        public Output<bool> BeaconBlue { get; set; }
        public Output<bool> Buzzer { get; set; }
        public Output<bool> MainValve { get; set; }

        public GeneralIO()
            : base(".GeneralInputs", ".GeneralOutputs")
        {
            EmergencyStop = AddInput<bool>("", "EmergencyStop", ".EmergencyStop");
            ResetButton = AddInput<bool>("", "ResetButton", ".ResetButton");
            AirPressureOk = AddInput<bool>("", "AirPressureOk", ".AirPressureOk");
            ServiceKey = AddInput<bool>("", "ServiceKey", ".ServiceKey");
            StartButton = AddInput<bool>("", "StartButton", ".StartButton");
            StopButton = AddInput<bool>("", "StopButton", ".StopButton");
            PauseButton = AddInput<bool>("", "PauseButton", ".PauseButton");

            StartButtonLight = AddOutput<bool>("", "StartButtonLight", ".StartButtonLight");
            StopButtonLight = AddOutput<bool>("", "StopButtonLight", ".StopButtonLight");
            PauseButtonLight = AddOutput<bool>("", "PauseButtonLight", ".PauseButtonLight");
            BeaconAutomatic = AddOutput<bool>("", "BeaconAutomatic", ".BeaconAutomatic");
            BeaconService = AddOutput<bool>("", "BeaconService", ".BeaconService");
            BeaconError = AddOutput<bool>("", "BeaconError", ".BeaconError");
            BeaconBlue = AddOutput<bool>("", "BeaconBlue", ".BeaconBlue");
            Buzzer = AddOutput<bool>("", "Buzzer", ".Buzzer");
            MainValve = AddOutput<bool>("", "MainValve", ".MainValve");
        }

        internal void Connect(object adsAmsAddress, int v)
        {
            throw new NotImplementedException();
        }
    }
}
