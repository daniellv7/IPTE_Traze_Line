using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.DataSources.DeviceConfig
{
    public interface IDeviceConfig
    {
        string DeviceName { get; set; }
        string Name { get; set; }
        InstrumentType InstrumentType { get; set; }
    }
}
