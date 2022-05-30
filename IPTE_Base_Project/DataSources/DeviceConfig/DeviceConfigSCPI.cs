using IPTE_Base_Project.DataSources.DeviceConfig;
using IPTE_Base_Project.Devices.CommChannel;
using System.Collections.Generic;

namespace IPTE_Base_Project.DataSources
{
    public class DeviceConfigSCPI : IDeviceConfig
    {
        #region Data members and accessors
        public string DeviceName { get; set; }
        public string Name { get; set; }
        public string IviVisaAddress { get; set; }
        public InstrumentType InstrumentType { get; set; }
        public CommChannelType CommChannelType { get; set; }
        public Dictionary<string, DeviceMethod> Interfaces { get; set; }
        public Dictionary<string, string> Commands { get; set; }

        #endregion

        #region Constructors

        public DeviceConfigSCPI(string deviceName, string name, string iviVisaAddress, CommChannelType commChannelType, InstrumentType instrumentType, Dictionary<string, DeviceMethod> interfaces, Dictionary<string, string> commands)
        {
            DeviceName = deviceName;
            Name = name;
            IviVisaAddress = iviVisaAddress;
            CommChannelType = commChannelType;
            InstrumentType = instrumentType;
            Interfaces = interfaces;
            Commands = commands;
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
