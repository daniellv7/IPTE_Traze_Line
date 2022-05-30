using IPTE_Base_Project.DataSources.DeviceConfig;
using IPTE_Base_Project.Devices;
using IPTE_Base_Project.Models.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sharp7;
using System.Threading;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Variants;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using IPTE_Base_Project.Common.Utils.MediatorPattern;

namespace IPTE_Base_Project.Models
{
    public class PlcModel : BaseModel, IDeviceModel
    {
        public Dictionary<string, Variant> RcvAvailableVariants { get; set; } = new Dictionary<string, Variant>();

        #region Tags
        //Variables For Rework View  
        #region Status
        private int _machinestatus;
        public int MACHINESTATUS
        {
            get
            {
                _machinestatus = ReadIntTag();
                Mediator.NotifyColleagues("MachineStatus", new object[] { _machinestatus });
                return _machinestatus;
            }
            set
            {
                _machinestatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Data types and accessors
        public string DeviceName { get; set; }
        public DevicePLC Plc { get; set; }
        public ABDevicePLC abplc { get; set; }

        public MachineSettings Settings;
        #endregion

        #region Constructors
        public PlcModel(PLC_Settings settings)
        {
            Plc = new DevicePLC(settings);
            abplc = new ABDevicePLC(settings);
            DeviceName = Plc.Name;
            Plc.PropertyChanged += PlcModelPropertychanged;
            // Plc.StartMonitoring();
            // Plc.PropertyChanged += this.PropertyChanged;
        }

        public void DeviceReset()
        {
           // DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            Plc.DeviceReset();
            Plc.Init();
            Plc.StartMonitoring();
           // DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        #region Methods
 
        #region PLC Write
        public void WriteBoolTagBlink(string tag,int Time)
        {
            List<string> Parameters;
            Plc.OutputList.TryGetValue(tag, out Parameters);
           
            try
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                Task t = Task.Run(() =>
                {
                    //Commandbusy = true;
                    S7.SetBitAt(ref Plc.WBuffer, Int32.Parse(Adress[0]), Int32.Parse(Adress[1]), true);
                    Thread.Sleep(Time);
                    S7.SetBitAt(ref Plc.WBuffer, Int32.Parse(Adress[0]), Int32.Parse(Adress[1]), false);
                    // Commandbusy = false;
                });
                if (t.IsCompleted)
                    t.Dispose();
            }
            catch { };
           
        }

        public void WriteBoolTag(string tag,bool State)
        {
            List<string> Parameters;
            Plc.OutputList.TryGetValue(tag, out Parameters);

            try
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                Task t = Task.Run(() =>
                {
                    //Commandbusy = true;
                    S7.SetBitAt(ref Plc.WBuffer, Int32.Parse(Adress[0]), Int32.Parse(Adress[1]), State);
                    // Commandbusy = false;
                });
                if (t.IsCompleted)
                    t.Dispose();
            }
            catch { };

        }

        public void WriteIntTag(string tag,int Value)
        {
            List<string> Parameters;
            Plc.OutputList.TryGetValue(tag, out Parameters);

            try
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                Task t = Task.Run(() =>
                {
                    S7.SetIntAt(Plc.WBuffer, Int16.Parse(Adress[0]), Convert.ToInt16(Value));
                });
                if (t.IsCompleted)
                    t.Dispose();
            }
            catch { };
            
        }
        public void WriteStringTag(string tag, string Value)
        {
            List<string> Parameters;
            Plc.OutputList.TryGetValue(tag, out Parameters);

            try
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                Task t = Task.Run(() =>
                {
                    S7.SetStringAt(Plc.WBuffer, Int16.Parse(Adress[0]), Value.Length, Value);
                });
                if (t.IsCompleted)
                    t.Dispose();
            }
            catch { };
            
        }
        #endregion

        #region PLC Read
        public bool ReadBoolTag([CallerMemberName] string tag = null)
        {
            List<string> Parameters;
            Plc.InputsList.TryGetValue(tag, out Parameters);
            if (Parameters != null)
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                return S7.GetBitAt(Plc.RBuffer, Int32.Parse(Adress[0]), Int32.Parse(Adress[1]));
            }
            else
                return false;
        }

        public bool ReadBoolTag(string tag,int Mode)
        {
            List<string> Parameters;
            byte[] TBuffer;
            switch (Mode)
            {
                //case 1:
                //    Plc.VariantList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RVariant;
                //    break;
                //case 2:
                //    Plc.ConfigurationtList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RConfig;
                //    break;
                default:
                    Plc.InputsList.TryGetValue(tag, out Parameters);
                    TBuffer = Plc.RBuffer;
                    break;
            }
            var Adress = Parameters.ElementAt(1).Split(',');
            if (Mode == 2)
                return S7.GetBitAt(TBuffer, Int32.Parse(Adress[0])-3000, Int32.Parse(Adress[1]));
            return S7.GetBitAt(TBuffer, Int32.Parse(Adress[0]), Int32.Parse(Adress[1]));
        }

        public int ReadIntTag([CallerMemberName] string tag = null)
        {
            List<string> Parameters;
            Plc.InputsList.TryGetValue(tag, out Parameters);
            if(Parameters != null)
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                return S7.GetIntAt(Plc.RBuffer, Int32.Parse(Adress[0]));
            }
            else
            {
                return 0;
            }
          
        }

       

        public int ReadDIntTag(string tag, int Mode)
        {
            List<string> Parameters;
            byte[] TBuffer;
            switch (Mode)
            {
                //case 1:
                //    Plc.VariantList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RVariant;
                //    break;
                //case 2:
                //    Plc.ConfigurationtList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RConfig;
                //    break;
                default:
                    Plc.InputsList.TryGetValue(tag, out Parameters);
                    TBuffer = Plc.RBuffer;
                    break;
            }
            var Adress = Parameters.ElementAt(1).Split(',');
            if (Mode == 2)
                return S7.GetDIntAt(TBuffer, Int32.Parse(Adress[0]) - 3000);
            return S7.GetDIntAt(TBuffer, Int32.Parse(Adress[0]));

        }


        public long ReadLIntTag(string tag, int Mode)
        {
            List<string> Parameters;
            byte[] TBuffer;
            switch (Mode)
            {
                //case 1:
                //    Plc.VariantList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RVariant;
                //    break;
                //case 2:
                //    Plc.ConfigurationtList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RConfig;
                //    break;
                default:
                    Plc.InputsList.TryGetValue(tag, out Parameters);
                    TBuffer = Plc.RBuffer;
                    break;
            }
            var Adress = Parameters.ElementAt(1).Split(',');
            if (Mode == 2)
                return S7.GetLIntAt(TBuffer, Int32.Parse(Adress[0]) - 3000);
            return S7.GetLIntAt(TBuffer, Int32.Parse(Adress[0]));

        }

        public string ReadStringTag([CallerMemberName] string tag = null)
        {
            List<string> Parameters;
            Plc.InputsList.TryGetValue(tag, out Parameters);
            if (Parameters != null)
            {
                var Adress = Parameters.ElementAt(1).Split(',');
                return S7.GetStringAt(Plc.RBuffer, Int32.Parse(Adress[0]));
            }
            else
                return "";
            
        }

        public string ReadStringTag(string tag, int Mode)
        {
            List<string> Parameters;
            byte[] TBuffer;
            switch (Mode)
            {
                // case 1:
                //    Plc.VariantList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RVariant;
                //    break;
                //case 2:
                //    Plc.ConfigurationtList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RConfig;
                //    break;
                default:
                    Plc.InputsList.TryGetValue(tag, out Parameters);
                    TBuffer = Plc.RBuffer;
                    break;
            }
            
            var Adress = Parameters.ElementAt(1).Split(',');
            if (Mode==2)
                return S7.GetStringAt(TBuffer, Int32.Parse(Adress[0])-3000);

            return S7.GetStringAt(TBuffer, Int32.Parse(Adress[0]));

        }

        public float ReadRealTag(string tag, int Mode)
        {
            List<string> Parameters;
            byte[] TBuffer;
            switch (Mode)
            {
                //case 1:
                //    Plc.VariantList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RVariant;
                //    break;
                //case 2:
                //    Plc.ConfigurationtList.TryGetValue(tag, out Parameters);
                //    TBuffer = Plc.RConfig;
                //    break;
                default:
                    Plc.InputsList.TryGetValue(tag, out Parameters);
                    TBuffer = Plc.RBuffer;
                    break;
            }

            var Adress = Parameters.ElementAt(1).Split(',');
            if (Mode == 2)
                return S7.GetRealAt(TBuffer, Int32.Parse(Adress[0]) - 3000);

            return S7.GetRealAt(TBuffer, Int32.Parse(Adress[0]));

        }

        public string ReadString(int position, byte[] Buffer)
        {
          return S7.GetStringAt(Buffer, position);
        }

        public int ReadInt(int position, byte[] Buffer)
        {
            return S7.GetIntAt(Buffer, position);
        }

        #endregion

        #region Control
        public void Close()
        {
            Plc.Close();
        }

        public void Reset()
        {
           // Plc.Reset();
        }
          #endregion

        #endregion

        #region Event listeners

        private void PlcModelPropertychanged(object sender, PropertyChangedEventArgs args)
        {
         //   DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);

            if (args.PropertyName == "Reference") return;

            RaisePropertyChanged(args.PropertyName);

          //  DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
        #endregion
       
        private string ErrorMessageList(int ErrorCode)
        {
            string Message;

            switch (ErrorCode)
            {
                case 0:
                    Message = "Waiting Part";
                    break;
                case 1:
                    Message = "Error 1";
                    break;
                case 2:
                    Message = "Error 2";
                    break;
                case 3:
                    Message = "Error 3";
                    break;
                case 4:
                    Message = "Error 4";
                    break;
                case 5:
                    Message = "Error 5";
                    break;
                case 6:
                    Message = "Error 6";
                    break;
                default:
                    Message = "Not considered";
                    break;

            }
            return Message;
        }


    }
    
}
