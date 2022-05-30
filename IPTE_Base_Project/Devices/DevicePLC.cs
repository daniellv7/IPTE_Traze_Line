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
using Sharp7;
using Ipte.TS1.UI.Controls;
using IPTE_Base_Project.Variants;
using System.Xml.Linq;
using IPTE_Base_Project.Common;

namespace IPTE_Base_Project.Devices
{
    public class DevicePLC : BaseModel
    {
        #region Data members and accessors

        public Dictionary<string, PlcSiemensAddress> PlcAddressCollection { get; set; }
        private Thread commThread;
        public bool monitoring;
        public bool threadStopped;
        private bool _Plcconection;
        public bool PLC_Conection
        {
            get
            {
                return _Plcconection;
            }
            set
            {
                if (value != _Plcconection)
                {
                    _Plcconection = value;
                    Mediator.NotifyColleagues("PLCupdate", new object[] { _Plcconection });
                }
            }
        }
        public byte[] RBuffer;
        public byte[] WBuffer;

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        protected bool Initialized
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        protected bool AutoConnect
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        protected int Delay_Read_Time { get; set; }
        static protected int Read_DB { get; set; }
        static protected int Read_Bites { get; set; }
        static protected int Read_Start { get; set; }

        static protected int Write_DB { get; set; }
        static protected int Write_Bites { get; set; }
        static protected int Write_Start { get; set; }

        public S7Client client;

        public List<string> LocalList = new List<string> { "PLCSTATUS" , "ERROR_MESSAGE" };
        public Dictionary<string, List<string>> OutputList { get; set; }
        public Dictionary<string, List<string>> InputsList { get; set; }
        public Dictionary<string, List<string>> VariantList { get; set; }
        public Dictionary<string, List<string>> ConfigurationtList { get; set; }

        public int MaxRetries { get; set; }
        public int MonitoringRefreshTime { get; set; }  //refresco de lectura
        public int ResponseTimeout { get; set; }        
        private const int WAIT_TIME = 200;              //espera después de una acción
        private const int MAX_STRING_LENGHT = 20;
        private readonly string ip;
        private int HomeIndex;

        bool SimulatePlc { get; set; }

        public bool Abort { get; set; }

        #endregion

        #region Constructors

        public DevicePLC(PLC_Settings settings)
        {
            Name = settings.PLC_Name;
            Read_DB = settings.PLC_Read_DB;
            Read_Bites = settings.PLC_Read_Bites;
            Read_Start = settings.PLC_Read_Start;
            Write_DB = settings.PLC_Write_DB;
            Write_Bites = settings.PLC_Write_Bites;
            Write_Start = settings.PLC_Write_Start;
            RBuffer = new byte[Read_Bites+1];
            WBuffer = new byte[Write_Bites+1];
            ip = settings.IP;
            Delay_Read_Time = settings.PLC_Delay_Time;
            SimulatePlc = settings.PLC_Simulated;
            AutoConnect = settings.AutoConnect;
            client = new S7Client();
            Abort = false;
            InputsList=ReadInputsData(Paths.IOList, settings.Input_Index);
            OutputList=ReadOutputsData(Paths.IOList, settings.Output_Index);
            Mediator.Register("ViewResetButtonClicked", OnResetMachine);
            Mediator.Register("WindowClose", onStopMonitoring);
            Mediator.Register("HomeIndex", OnHomeIndexChange);

        }

       
            public void DeviceReset()
        {
            try
            {
                client.Disconnect();
                
                if (!client.Connected)
                {
                    int test = client.ConnectTo(ip, 0, 0);
                    if (test == 0)
                    {
                        
                        log.Debug(Name+": "+Translation.Translate.ConnectionSuccess);
                        commThread = new Thread(new ThreadStart(RxTxThread));
                        StartMonitoring();
                    }

                    else
                    {
                        GuiMessageBox.Show("Error", Name + ": " + Translation.Translate.ConnectionFail);
                        log.Error(Name + ": " + Translation.Translate.ConnectionFail);
                    }

                }

            }
            catch (Exception exc)
            {
                throw new Exception(Translation.Translate.ConnectionExeption + exc);
            }
        }

        #endregion

        /// <summary>
        /// Initializes PLC communication.
        /// </summary>
        /// <returns>Returns true if plc communication was correctly initialized</returns>
        public bool Init()
        {
            if (SimulatePlc)
            {
                Initialized = false;
                return true;
            }
            try
            {

                int test= client.ConnectTo(ip, 0, 0);
                if (test == 0)
                    log.Debug(Name + ": " + Translation.Translate.ConnectionSuccess);
                else
                {
                    log.Error(Name + ": " + Translation.Translate.ConnectionFail);
                    Mediator.NotifyColleagues("ErrorHandling", new object[] { Name + ": " + Translation.Translate.ConnectionFail });
                }
                    
                commThread = new Thread(new ThreadStart(RxTxThread));
            }
            catch (Exception exc)
            {
                log.Error(Name + ": " + Translation.Translate.ConnectionExeption, exc);
                Initialized = false;
                return false;
                //throw new Exception("Exception while initializing plc - exc: " + exc);
            }
            Initialized = true;
            return true;
        }

        /// <summary>
        /// Terminates PLC communication.
        /// </summary>
        /// <returns>Returns true if plc communication was correctly finished</returns>
        public bool Terminate()
        {
            try
            {
               // client.Disconnect();
            }
            catch (Exception exc)
            {
                throw new Exception(Name + ": " + Translation.Translate.ConnectionExeption + exc);
            }

            return true;
        }

        /// <summary>
        /// Resets plc. First terminates communication, then initializes it again.
        /// </summary>
        /// <returns>Returns true if plc communication was correctly reset</returns>
        private void OnResetMachine(object[] param)
        {
            try
            {
                client.Disconnect();
               
                //client = new S7Client();
                
                if (!client.Connected)
                {
                    int test = client.ConnectTo(ip, 0, 0);
                    if (test  == 0)
                    {
                        log.Debug(Name + ": " + Translation.Translate.ConnectionSuccess);
                        commThread = new Thread(new ThreadStart(RxTxThread));
                        StartMonitoring();
                    }
                       
                    else
                    {
                        Mediator.NotifyColleagues("ErrorHandling", new object[] { Name + ": " + Translation.Translate.ConnectionFail });
                        // GuiMessageBox.Show("Error", "Connection to PLC Fail");
                        log.Error(Name + ": " + Translation.Translate.ConnectionFail);
                    }
                        
                }
                
            }
            catch (Exception exc)
            {
                throw new Exception(Name + ": " + Translation.Translate.ConnectionExeption + exc);
            }

           
        }

        private void RxTxThread()
        {
            int SizeRead = 0;
            int SizeWritten = 0;
            int Result;
            while (monitoring==true && SimulatePlc==false)
            {
                try
                {
                    if(client.Connected)
                    {
                        PLC_Conection = true;
                        //Read from PLC
                        Result = client.ReadArea(S7Consts.S7AreaDB, Read_DB, Read_Start, Read_Bites, S7Consts.S7WLByte, RBuffer, ref SizeRead);
                        //write to PLC
                        Result = client.WriteArea(S7Consts.S7AreaDB, Write_DB, Write_Start, Write_Bites, S7Consts.S7WLByte, WBuffer, ref SizeWritten);
                       
                        // Status Active PC=>PLC
                       // S7.SetBitAt(ref WBuffer, 0, 0,true);//Status Bit to PLC
                        foreach (var Input in InputsList)
                        {
                            OnPropertyChanged(Input.Key);
                        }
                        foreach (var Local in LocalList)
                        {
                            OnPropertyChanged(Local);
                        }
                    }
                    else
                    {
                        if (PLC_Conection!=false)
                        {
                            log.Error(Translation.Translate.ConnectionLost);
                            Mediator.NotifyColleagues("ErrorHandling", new object[] { Translation.Translate.ConnectionFail });
                        }
                        PLC_Conection = false;
                        
                        if (AutoConnect == true)
                        {
                            Thread.Sleep(3000);
                            ReconnectPLC();
                        }


                        else
                        {
                            break;
                        }
                        
                    }
                   
                }
                catch (Exception e)
                {
                    
                }
                Thread.Sleep(Delay_Read_Time);//Set Only if connected by wifi , wired must be 0.
            }
            if(SimulatePlc)
             log.Debug(Name + ": " + "Simulated");
            threadStopped = true;
        }

        public void ReconnectPLC()
        {

            log.Debug(Translation.Translate.ConnectionRetry);
            try
            {

                int test = client.ConnectTo(ip, 0, 0);
                if (test == 0)
                    log.Debug(Name + ": " + Translation.Translate.ConnectionSuccess);
                else
                {
                    log.Error(Name + ": " + Translation.Translate.ConnectionFail);
                }
            }
            catch (Exception exc)
            {
                log.Error(Name + ": " + Translation.Translate.ConnectionExeption, exc);
                Initialized = false;
            }
            
        }

        public void StartMonitoring()
        {
            if (SimulatePlc) return;
            if (!Initialized) return;
            monitoring = true;
            commThread.Start();
        }

        public void onStopMonitoring(Object[] Param)
        {
            if (SimulatePlc) return;

            monitoring = false;
        }

        public void Close()
        {
            monitoring = false;
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            
            var task = Task.Run(() => ClosePlc());
            if (!task.Wait(TimeSpan.FromSeconds(2)))
            {
                //throw new Exception("Timed out");
                Terminate();
            }

            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        private void ClosePlc()
        {
          // DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            if (SimulatePlc) return;

            while (!threadStopped)
                Thread.Sleep(200);

            Terminate();

            //DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        #region sets and gets



        #endregion

        private Dictionary<string, List<string>> ReadInputsData(string FileName, string Index)
        {
            XElement MainNode;
            XDocument XmlDoc = new XDocument();
            XmlDoc = XDocument.Load(FileName);
            //Nodes Load
            MainNode = XmlDoc.Root.Element("Inputs"+Index);
            var IOList = MainNode.Elements();
            Dictionary<string, List<string>> ReadParam = new Dictionary<string, List<string>>();
            foreach (XElement IO in IOList)
            {
                List<string> IOparam = new List<string>();
                string IOName = IO.Element("Name").Value;
                IOparam.Add(IO.Element("Type").Value);
                IOparam.Add(IO.Element("Adress").Value);
                ReadParam.Add(IOName, IOparam);
            }
            return ReadParam;
        }

        private Dictionary<string, List<string>> ReadOutputsData(string FileName,string Index)
        {
            XElement MainNode;
            XDocument XmlDoc = new XDocument();
            XmlDoc = XDocument.Load(FileName);
            //Nodes Load
            MainNode = XmlDoc.Root.Element("Outputs"+Index);
            var IOList = MainNode.Elements();
            Dictionary<string, List<string>> ReadParam = new Dictionary<string, List<string>>();
            foreach (XElement IO in IOList)
            {
                List<string> IOparam = new List<string>();
                string IOName = IO.Element("Name").Value;
                IOparam.Add(IO.Element("Type").Value);
                IOparam.Add(IO.Element("Adress").Value);
                ReadParam.Add(IOName, IOparam);
            }
            return ReadParam;
        }

        private Dictionary<string, List<string>> ReadVariantData(string FileName, string Index)
        {
            XElement MainNode;
            XDocument XmlDoc = new XDocument();
            XmlDoc = XDocument.Load(FileName);
            //Nodes Load
            MainNode = XmlDoc.Root.Element("Variant");
            var IOList = MainNode.Elements();
            Dictionary<string, List<string>> ReadParam = new Dictionary<string, List<string>>();
            foreach (XElement IO in IOList)
            {
                List<string> IOparam = new List<string>();
                string IOName = IO.Element("Name").Value;
                IOparam.Add(IO.Element("Type").Value);
                IOparam.Add(IO.Element("Adress").Value);
                ReadParam.Add(IOName, IOparam);
            }
            return ReadParam;
        }

        private Dictionary<string, List<string>> ReadConfigurationData(string FileName)
        {
            XElement MainNode;
            XDocument XmlDoc = new XDocument();
            XmlDoc = XDocument.Load(FileName);
            //Nodes Load
            MainNode = XmlDoc.Root.Element("Configuration");
            var IOList = MainNode.Elements();
            Dictionary<string, List<string>> ReadParam = new Dictionary<string, List<string>>();
            foreach (XElement IO in IOList)
            {
                List<string> IOparam = new List<string>();
                string IOName = IO.Element("Name").Value;
                IOparam.Add(IO.Element("Type").Value);
                IOparam.Add(IO.Element("Adress").Value);
                ReadParam.Add(IOName, IOparam);
            }
            return ReadParam;
        }

        private void OnHomeIndexChange(object[] param)
        {
            HomeIndex = (int)param[0];
        }

    }
}
