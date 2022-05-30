using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.DataSources.DeviceConfig
{
    public class DeviceMethod
    {
        public string Name { get; set; }
        public string Commands { get; set; }
        public string Param { get; set; }

        public DeviceMethod(string name, string commands, string param)
        {
            Name = name;
            Commands = commands;
            Param = param;
        }
    }
}
