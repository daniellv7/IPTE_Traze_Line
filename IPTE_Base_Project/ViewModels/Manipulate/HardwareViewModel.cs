using IPTE_Base_Project.Common;
using IPTE_Base_Project.Devices;
using IPTE_Base_Project.Devices.Interfaces;
using IPTE_Base_Project.Managers;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;

namespace IPTE_Base_Project.ViewModels.Manipulate
{
    public class HardwareViewModel : BaseViewModel
    {
        #region Data members and accessors
        public List<IDevice> Instruments
        {
            get { return GetValue<List<IDevice>>(); }
            set
            {
                SetValue(value);
                writeCommand?.RaiseCanExecuteChanged();
                queryCommand?.RaiseCanExecuteChanged();
            }
        }
        public IDevice SelectedInstrument
        {
            get { return GetValue<IDevice>(); }
            set
            {
                SetValue(value);
                Commands = ((IDeviceSCPI)value).Commands;
            }
        }
        public Dictionary<string, string> Commands
        {
            get { return GetValue<Dictionary<string, string>>(); }
            set { SetValue(value); }
        }
        public KeyValuePair<string, string> SelectedCommand
        {
            get { return GetValue<KeyValuePair<string, string>>(); }
            set
            {
                SetValue(value);
                Command = value.Value;
            }
        }
        public string Command
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Result
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        #endregion

        #region Constructor
        public HardwareViewModel()
        {
            
        }
        #endregion

        #region Commands
        private RelayCommand writeCommand;
        public ICommand WriteCommand
        {
            get
            {
                if (writeCommand == null)
                {
                    writeCommand = new RelayCommand(param => Write(param), CanWrite);
                }
                return writeCommand;
            }
        }

        private RelayCommand queryCommand;
        public ICommand QueryCommand
        {
            get
            {
                if (queryCommand == null)
                {
                    queryCommand = new RelayCommand(param => Query(param), CanWrite);
                }
                return queryCommand;
            }
        }
        #endregion

        #region CommandMethods  
        private void Write(object o)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                SelectedInstrument.CommChannel.WriteCommand(Command);
                Result = string.Empty;
            }
            catch { Result = "CommandWrite error"; }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        private bool CanWrite(object o)
        {
            if (SelectedInstrument == null)
            {
                return false;
            }
            return true;
        }

        private void Query(object o)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                Result = SelectedInstrument.CommChannel.Query(Command);
            }
            catch { Result = "Query error"; }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }
        #endregion
    }
}
