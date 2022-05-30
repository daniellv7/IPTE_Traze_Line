using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Models.Devices
{
    public interface IDeviceModel
    {
        string DeviceName { get; set; }
        void DeviceReset();
    }
}
