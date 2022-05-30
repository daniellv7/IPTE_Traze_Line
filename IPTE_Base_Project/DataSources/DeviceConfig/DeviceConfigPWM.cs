namespace IPTE_Base_Project.DataSources.DeviceConfig
{
    public class DeviceConfigPWM : IDeviceConfig
    {
        public string DeviceName { get; set; }
        public string Name { get; set; }
        public InstrumentType InstrumentType { get; set; }

        #region Constructors
        public DeviceConfigPWM(string deviceName, string name, InstrumentType instrumentType)
        {
            DeviceName = deviceName;
            Name = name;
            InstrumentType = instrumentType;
        }
        #endregion

        #region Methods
        public override string ToString() { return Name; }
        #endregion
    }
}
