using IPTE_Base_Project.Common;
using IPTE_Base_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sharp7;
using System.Threading;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.Variants;
using System.Windows.Data;
using System.Windows.Media;

namespace IPTE_Base_Project.ViewModels.Manipulate
{
    public class PlcViewModel : BaseViewModel
    {
        public PlcModel Model { get; set; }
        public Dictionary<string, PlcModel> devices = new Dictionary<string, PlcModel>();
        public Dictionary<int, string> Querys = new Dictionary<int, string>();

        //Thread VariableMonitorTread;
        bool StopMonitor = false;
        string Cell;
        MachineSettings Settings;

        public PlcViewModel(Dictionary<string, PlcModel> DEVICES,MachineSettings setting)
        {
            Settings = setting;
            devices = DEVICES;
            Mediator.Register("WindowClose", OnStop);
            Mediator.Register("ViewResetButtonClicked", OnReset);
            Mediator.Register("WindowClose", OnStopPLCMonitor);
            foreach (var Models in devices)
            {
                Thread VariableMonitorTread = new Thread(() => Monitor(Models.Value));
                //VariableMonitorTread.SetApartmentState(ApartmentState.STA);
                VariableMonitorTread.Start();
            }
           
        }

        private void Monitor(PlcModel model)
        {
            int HS_Control = 0;
            do
            {
                if (model.Plc.PLC_Conection)
                {
                    //HS Read
                    if (model.ReadIntTag("Call_HS")!= HS_Control)
                    {
                        
                        HS_Control = model.ReadIntTag("Call_HS");
                        HS_Command(HS_Control, model);
                        
                    }
                    //Live Set
                    model.WriteBoolTag("LIVE_BIT_W", true);
                    
                    
                   
                }
                Thread.Sleep(100);

            }
            while (StopMonitor == false);

        }

        private void HS_Command(int HS, PlcModel model)
        {
            string SerialNumber = model.ReadStringTag("SerialNumber");
            switch (HS)
            {

                case 0:   // Waiting 

                    log.Info(model.DeviceName + " =>PC: " + 0);
                    model.WriteIntTag("PC_HS", 0);
                    log.Info(model.DeviceName + " <=PC: " + 0);
                    Querys.Add(0,model.DeviceName);
                    Mediator.NotifyColleagues("Itac_CAll", new object[] { model });
                    break;

                case 100: //Interlocking

                    log.Info(model.DeviceName + " =>PC: " + 100 + " Interlock Request:" + SerialNumber);
                    model.WriteIntTag("PC_HS", 200);
                    log.Info(model.DeviceName + " <=PC: " + 200 +" Interlock ITAC:"+ SerialNumber);
                    Querys.Add(100, model.DeviceName);
                    Mediator.NotifyColleagues("Itac_CAll_Interlock", new object[] { model, SerialNumber });
                    break;

                case 110: //Booking

                    log.Info(model.DeviceName + " =>PC: " + 110 + " Booking Request:" + SerialNumber);
                    model.WriteIntTag("PC_HS", 200);
                    log.Info(model.DeviceName + " <=PC: " + 200 + " Booking Request:" + SerialNumber);
                    Querys.Add(110, model.DeviceName);
                    Mediator.NotifyColleagues("Itac_CAll_Booking", new object[] { model , SerialNumber });
                    break;

                default:  // No Valid Task
                    break;

            }
        }


        private void OnStopPLCMonitor(object[] param)
        {
            StopMonitor = true;
        }

        #region Commands


        public ICommand ChangeTagValueCommand
        {
            get
            {
                if (changeTagValueCommand == null)
                {
                    changeTagValueCommand = new RelayCommand(param => ChangeTagValue(param), CanExe);
                }
                return changeTagValueCommand;
            }
        }
        private RelayCommand changeTagValueCommand;

        public ICommand ResetCommand
        {
            get
            {
                if (resetCommand == null)
                {
                    resetCommand = new RelayCommand(param => Reset(), CanExe);
                }
                return resetCommand;
            }
        }
        private RelayCommand resetCommand;

        public ICommand TerminateCommand
        {
            get
            {
                if (terminateCommand == null)
                {
                    terminateCommand = new RelayCommand(param => Terminate(param), CanExe);
                }
                return terminateCommand;
            }
        }
        private RelayCommand terminateCommand;

        public ICommand RetryCommand
        {
            get
            {
                if (retryCommand == null)
                {
                    retryCommand = new RelayCommand(param => Model.WriteBoolTagBlink("RETRY",500), CanExe);
                }
                return retryCommand;
            }
        }
        private RelayCommand retryCommand;

        public ICommand ScrapCommand
        {
            get
            {
                if (scrapcommand == null)
                {
                    scrapcommand = new RelayCommand(param => Model.WriteBoolTagBlink("SCRAP",500), CanExe);
                }
                return scrapcommand;
            }
        }
        private RelayCommand scrapcommand;

        public ICommand RerouteStation
        {
            get
            {
                if (rerouteStation == null)
                {

                    rerouteStation = new RelayCommand(param => RetryCom(param), CanExe);
                }
                return rerouteStation;
            }
        }
        private RelayCommand rerouteStation;
        
        private void OnReset(object[] param)
        {
            foreach (var Models in devices)
            {
                Models.Value.Reset();
            }
            //Model.Reset();
        }


        public void Reset()
        {
            foreach (var Models in devices)
            {
                Models.Value.Reset();
            }
            //Model.Reset();
        }

        public void Terminate(object o)
        {
            foreach (var Models in devices)
            {
                Models.Value.Close();
            }
           // Model.Close();
        }

        public void OnStop(object[] param)
        {
            foreach (var Models in devices)
            {
                Models.Value.Close();
            }
            //Model.Close();
        }

        public bool CanExe(object o)
        {
            return true;
        }

        public void ChangeTagValue(object o)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string tag = o as string;
            
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        public void RetryCom(object o)
        {
            Model.WriteBoolTagBlink(o as string,500);
        }
        
        #endregion

    }
   
}
