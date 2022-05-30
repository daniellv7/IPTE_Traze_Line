using System.Threading;

namespace Ipte.Machine.Devices
{
    public class UnloadingSegment : SimpleSegment
    {
        public UnloadingSegment(string zoneId, string laneId)
            : base(zoneId, laneId)
        {
        }

        public override void Unload()
        {
            Thread.Sleep(3000);
            OnItemTransferOut(Product.SerialNumber, LaneId);
            Product = null;
            Log("Product dissapeared");
        }
    }
}
