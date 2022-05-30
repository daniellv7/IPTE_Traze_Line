using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.Common.Utils.Plc;
using IPTE_Base_Project.DataSources.DeviceConfig;
using IPTE_Base_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ipte.TS1.UI.Controls;
using IPTE_Base_Project.Variants;
using System.Xml.Linq;
using libplctag;
using libplctag.DataTypes;
using IPTE_Base_Project.Common;


namespace IPTE_Base_Project.Devices
{
    public class ABDevicePLC : BaseModel
    {
        #region Constructor

        public ABDevicePLC(PLC_Settings settings)
        {
            run();
        }

        void run()
        {
            List<object> ObjectList = new List<object>();

            ObjectList.Add(new Tag<BoolPlcMapper, bool>());

            //ObjectList.ElementAt[]          
            var myTag = new Tag<DintPlcMapper, int>()
            {
                Name = "TestTag2",
                Gateway = "10.80.11.1",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip,
                Timeout = TimeSpan.FromSeconds(5)
            };

            var myTag1 = new Tag<BoolPlcMapper, bool>()
            {
                Name = "TestTag",
                Gateway = "10.80.11.1",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip,
                Timeout = TimeSpan.FromSeconds(5)
            };

            var stringTag = new Tag<StringPlcMapper, string[]>()
            {
                Name = "TestTag",
                Gateway = "10.80.11.1",
                Path = "1,0",
                PlcType = PlcType.ControlLogix,
                Protocol = Protocol.ab_eip,
                Timeout = TimeSpan.FromSeconds(5),
                ArrayDimensions = new int[] { 100 }
            };

            //var r = new Random((int)DateTime.Now.ToBinary());
            //for (int ii = 0; ii < stringTag.Value.Length; ii++)
            //    stringTag.Value[ii] = r.Next().ToString();

            //myTag.Value = 28;
            //myTag.Write();
            //myTag.Read();
            //int test = myTag.Value;
            //myTag.Value = 12;
            //myTag.Write();

        }
        #endregion
    }
}
