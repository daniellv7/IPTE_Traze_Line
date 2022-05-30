using Ipte.TS1.IO.Beckhoff.Tc2;

namespace Ipte.Machine.Hardware.IO
{
    public class SegmentIO : StructBase
    {
        public Input<bool> PanelOnStopper { get; set; }
        public Input<bool> StopperIsOpen { get; set; }
        public Input<bool> StopperIsClosed { get; set; }

        public Output<bool> RunConveyor { get; set; }
        public Output<bool> CloseStopper { get; set; }

        public SegmentIO(string inputStructName, string outputStructName)
            : base(inputStructName, outputStructName)
        {
            PanelOnStopper = AddInput<bool>("", "Panel on stopper", ".PanelOnStopper");
            StopperIsOpen = AddInput<bool>("", "Stopper is open", ".StopperIsOpen");
            StopperIsClosed = AddInput<bool>("", "Stopper is closed", ".StopperIsClosed");

            RunConveyor = AddOutput<bool>("", "Run conveyor", ".RunConveyor");
            CloseStopper = AddOutput<bool>("", "Close stopper", ".CloseStopper");
        }
    }
}
