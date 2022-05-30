using IPTE_Base_Project.Common.Utils.Plc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.DataSources.DeviceConfig
{
    public class PlcConfig
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string PlcType { get; set; }
        public bool IsSimulated { get; set; }
        public int MonitoringRefreshTime { get; set; }
        public Dictionary<string, PlcSiemensAddress> PlcAddressCollection { get; set; }

        public PlcConfig(string name, string ip, string port, string plcType, bool isSimulated, int monitoringRefreshTime, Dictionary<string, PlcSiemensAddress> plcAddressCollection)
        {
            Name = name;
            Ip = ip;
            Port = port;
            PlcType = plcType;
            IsSimulated = isSimulated;
            MonitoringRefreshTime = monitoringRefreshTime;
            PlcAddressCollection = plcAddressCollection;
        }
    }
}
