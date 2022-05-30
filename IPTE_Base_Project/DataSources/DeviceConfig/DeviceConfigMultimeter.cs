using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPTE_Base_Project.Devices.CommChannel;

namespace IPTE_Base_Project.DataSources.DeviceConfig
{
    public class DeviceConfigMultimeter : DeviceConfigSCPI
    {
        public List<double> RangeDCVoltage { get; set; }
        public List<double> RangeDCCurrent { get; set; }
        public List<double> RangeRes { get; set; }
        public List<double> RangeFRes { get; set; }
        public List<double> NPLC { get; set; }

        public DeviceConfigMultimeter(string deviceName, string name, string iviVisaAddress, CommChannelType commChannelType, InstrumentType instrumentType, Dictionary<string, DeviceMethod> interfaces, Dictionary<string, string> commands, List<double> rangeDCVoltage, List<double> rangeDCCurrent, List<double> rangeRes, List<double> rangeFRes, List<double> nplc) 
            : base(deviceName, name, iviVisaAddress, commChannelType, instrumentType, interfaces, commands)
        {
            RangeDCVoltage = rangeDCVoltage;
            RangeDCCurrent = rangeDCCurrent;
            RangeRes = rangeRes;
            RangeFRes = rangeFRes;
            NPLC = nplc;
        }
    }
}
