using IPTE_Base_Project.DataSources.DeviceConfig;
using System.Collections.Generic;

namespace IPTE_Base_Project.Devices.Interfaces
{
    interface IDeviceSCPI
    {
        #region Data members and accessors

        //          < [Method], [SCPI commands separated by ';'] >
        Dictionary<string, DeviceMethod> Interfaces { get; set; }

        //          <Name, Command>
        Dictionary<string, string> Commands { get; set; }

        #endregion
    }
}
