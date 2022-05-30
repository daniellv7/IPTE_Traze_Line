using Ipte.TS1.IO.Beckhoff.Tc2;

namespace Ipte.Machine.Hardware.IO
{
    public class LoadingSegmentIO : SegmentIO
    {
        public Input<bool> SmemaPreviousAvailable { get; set; }
        public Output<bool> SmemaPreviousNotBusy { get; set; }

        public LoadingSegmentIO(string inputStructName, string outputStructName)
            : base(inputStructName, outputStructName)
        {
            SmemaPreviousAvailable = AddInput<bool>("", "Smema BA", ".SmemaPreviousAvailable");
            SmemaPreviousNotBusy = AddOutput<bool>("", "Semam NB", ".SmemaPreviousNotBusy");
        }
    }
}
