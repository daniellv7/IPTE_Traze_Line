using IPTE_Base_Project.DataSources;
using IPTE_Base_Project.Devices.CommChannel;

namespace IPTE_Base_Project.Devices
{
    public interface IDevice
    {
        ICommChannel CommChannel { get; set; }

        int Status { get; set; }
        string DeviceName { get; set; }
        string Name { get; set; }
        InstrumentType InstrumentType { get; set; }

        void Init();
        void Reset();
        void Terminate();
        void DeviceReset();
    }
}
