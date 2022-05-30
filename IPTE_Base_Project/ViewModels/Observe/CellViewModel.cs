using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using GuiControlLibrary;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.Common.Utils;
using IPTE_Base_Project.Common;
using IPTE_Base_Project.Models;
using IPTE_Base_Project.Models.Devices;
using System.Reflection;
using IPTE_Base_Project.Variants;
using System.ComponentModel;
using System.Threading;

namespace IPTE_Base_Project.ViewModels.Observe
{
    class CellViewModel : BaseViewModel
    {
        public ObservableCollection<GuiMessageItem> Messages
        {
            get { return GetValue<ObservableCollection<GuiMessageItem>>(); }
            set { SetValue(value); }
        }
        Thread Monitor;
       
        public CellModel Model { get; set; }
        public PlcModel PLCModel { get; set; }

        private bool _isserver;
        public bool Isserver
        {
            get
            {
                return _isserver;
            }
            set
            {
                _isserver = value;
                OnPropertyChanged();
            }
        }

        //public bool ITACConnection { get; set; }
        public CellViewModel(CellModel model  , MachineSettings settings)
        {
            
            Model = model;
            Model.ITACConnection = true;
            Model.Monitoring = true;
            //Model.PropertyChanged += ModelPropertychanged;
            Mediator.Register("OpenPopUp", OnPopUpShowCommand);
            Mediator.Register("ErrorHandling", OnError);
            Mediator.Register("ClearErrorMesssges", OnClearErrorMessages);
            Mediator.Register("WhosAlive", OnCellLive);
            Mediator.Register("DeviceErrorStatus", OnDeviceErrorStatus);
            Mediator.Register("ReferenceError", OnReferenceError);
            Mediator.Register("ResetDevice", OnResetDevice);
            Mediator.Register("WindowClose", OnStopM);
            
            Mediator.Register("ServerUpdate", ServerUpdate);

            Mediator.Register("PLCupdate", Plcupdate);
            Mediator.Register("ItacStatus", OnItacStatusChange);
            Mediator.Register("PLCStatus", OnPLCStatusChange);

            Model.CellLabel = settings.Station_Settings.Cell_Number;
            Model.ImagePath = Paths.ImageCell + "\\"+ "CellStation.png";
            Messages = new ObservableCollection<GuiMessageItem>();
            //Isserver = settings.Station_Settings.IsServer;
            if (Isserver)
            {
                Monitor = new Thread(new ThreadStart(LiveMonitor));
                Monitor.Start();
            }
            else
            {
                Monitor = new Thread(new ThreadStart(ServerMonitor));
                Monitor.Start();
            }
           
        }

        /// <summary>
        /// Event listener for sequenceVM asking to show the scan code popUp
        /// </summary>
        /// <param name="param"></param>
        /// 
      
        public void OnPopUpShowCommand(object[] param)
        {
            log.Debug("MainWindowViewModel => OnPopUpShowCommand");

            string message = (string)param[0];

            MessageBox.Show(message, "Alert", MessageBoxButton.OK, MessageBoxImage.Information);

            log.Debug("MainWindowViewModel <= OnPopUpShowCommand");
        }

        #region Calls
        public void OnStopM(object[] param)
        {
            Model.Monitoring = false;
        }

        public void Plcupdate(object[] param)
        {
            Model.PLCActive = (bool)param[0];
        }

        public void ServerUpdate(object[] param)
        {
            Model.ServerActive = (bool)param[0];
            Model.ServerTime = DateTime.Now;
        }
        #endregion

        public void LiveMonitor()
        {
            Type myType = Model.GetType();
            var pi = Model.GetType().GetProperties();
            while (Model.Monitoring)
            {
                ServerMonitor();
                foreach (var property in pi)
                {
                    
                    var PropType = property.PropertyType;
                    if (PropType.Name == "CellVar")
                    {
                        CellVar cell = (CellVar)(property.GetValue(Model, null));
                        string CellName = property.Name;
                        TimeSpan ElapsedTime = DateTime.Now - cell.Timestamp;
                        double Elapsed = ElapsedTime.TotalSeconds;
                        if (Elapsed >= 5)
                        {
                            cell.MachineState = 0;
                            cell.Alive = false;
                            System.Reflection.PropertyInfo p2 = Model.GetType().GetProperty(property.Name);
                            p2.SetValue(Model, cell);
                        }
                        
                    }

                }
                Thread.Sleep(10);
            }
        }

        public void ServerMonitor()
        {
            TimeSpan ElapsedTime = DateTime.Now - Model.ServerTime;
            double Elapsed = ElapsedTime.TotalSeconds;
            if (Elapsed >= 5)
            {
                Model.ServerActive = false;
            }
        }


        public void OnCellLive(object[] param)
        {
            Type myType = Model.GetType();
            string Name = param[0] as string;
            System.Reflection.PropertyInfo pi = Model.GetType().GetProperty(Name);
            CellVar cell = (CellVar)(pi.GetValue(Model, null));
            cell.Timestamp = DateTime.Now;
            cell.Alive = true;
            cell.MachineState = (int)param[2];
            pi.SetValue(Model, cell);
        }

            public void OnError(object[] param)
        {
            GuiMessageItem Error = new GuiMessageItem
            {
                Caption = (string)param[0],
                Severity = Severity.Error,
                CanClose = true,
            };

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Error.Close += delegate (object o, RoutedEventArgs a)
                {
                    Messages.Remove(Error);
                };

                Messages.Add(Error);
            });
        }

        public void OnClearErrorMessages(object[] param)
        {
            Messages.Clear();
        }

        private void OnDeviceErrorStatus(object[] param)
        {
            GuiMessageItem Error = new GuiMessageItem
            {
                Device = param.Length > 0 ? (string)param[0]: string.Empty,
                Caption = param.Length > 1 ? (string)param[1] : string.Empty,
                Description = param.Length > 2 ? (string)param[2] : string.Empty,
                Severity = Severity.Error,
                CanClose = false,
                CanReset = true,
                CanIgnore = false,
                CanRetry = false
            };

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Error.Reset += delegate (object o, RoutedEventArgs a)
                {
                    Mediator.NotifyColleagues("ResetDevice", new object[] {  });
                };

                Messages.Add(Error);
            });
        }
        public void OnReferenceError(object[] param)
        {
            GuiMessageItem Error = new GuiMessageItem
            {
                Device = param.Length > 0 ? (string)param[0] : string.Empty,
                Caption = param.Length > 1 ? (string)param[1] : string.Empty,
                Description = param.Length > 2 ? (string)param[2] : string.Empty,
                
                Severity = Severity.Error,
                CanClose = true,
                CanReset = true,
                CanIgnore = false,
                CanRetry = false
            };

            if (!Messages.Contains(Error))
            {
                Messages.Add(Error);
            }

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Error.Reset += delegate (object o, RoutedEventArgs a)
                {
                    
                    Mediator.NotifyColleagues("ResetReference", new object[] { });
                    Messages.Remove(Error);
                };

                //Messages.Add(Error);
            });

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Error.Close += delegate (object o, RoutedEventArgs a)
                {
                    Messages.Remove(Error);
                };

                //Messages.Add(Error);
            });
        }

        public void OnResetDevice(object[] param)
        {
            GuiMessageItem Error = (GuiMessageItem)param[0];

            //if (devices.ContainsKey(Error.Device))
            //{
            //    devices[Error.Device].DeviceReset();
            //    Messages.Remove(Error);
            //}
        }

        public void OnItacStatusChange(object[] param)
        {
            Model.ITACConnection = (bool)param[0];
        }

        public void OnPLCStatusChange(object[] param)
        {
            Model.PLC_1 = (bool)param[0];
        }

        private void ModelPropertychanged(object sender, PropertyChangedEventArgs args)
        {
            //   DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);

            if (args.PropertyName == "Reference") return;

            RaisePropertyChanged(args.PropertyName);

            //  DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
    }
}
