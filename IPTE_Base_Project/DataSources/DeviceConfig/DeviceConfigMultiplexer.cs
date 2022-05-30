using IPTE_Base_Project.DataSources.DeviceConfig;
using System;

namespace IPTE_Base_Project.DataSources
{
    public class DeviceConfigMultiplexer : IDeviceConfig
    {
        #region Data members and accessors
        
        public string DeviceName { get; set; }
        public string Name { get; set; }
        public InstrumentType InstrumentType { get; set; }
        public MultiplexerType MultiplexerType { get; set; }
        public string Topology { get; set; }
        public int MonitoringRefreshTime { get; set; }

        #endregion

        #region Constructors

        public DeviceConfigMultiplexer(string deviceName, string name, InstrumentType instrumentType, MultiplexerType multiplexerType, string topology, int monitoringRefreshTime)
        {
            DeviceName = deviceName;
            Name = name;
            InstrumentType = instrumentType;
            MultiplexerType = multiplexerType;
            Topology = topology;
            MonitoringRefreshTime = monitoringRefreshTime;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
