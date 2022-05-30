using IPTE_Base_Project.Common;
using System;
using System.Reflection;

namespace IPTE_Base_Project.Devices.CommChannel
{
    public class CommChannelSerial : Logging, ICommChannel
    {
        #region CommChannelErrorEvent
        public event EventHandler CommChannelError;
        protected virtual void OnCommChannelError(EventArgs e)
        {
            CommChannelError?.Invoke(this, e);
        }
        #endregion
        private bool isInitialized = false;
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public void SessionReset()
        {

        }

        public bool Initialize()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public string Query(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return "Method not defined";
        }

        public string Query(string commands, char separator)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return "Method not defined";
        }

        public string ReadResponse()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return "Method not defined";
        }

        public bool Reset()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return false;
        }

        public void Terminate()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        public void WriteCommand(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        public void WriteCommand(string commands, char separator)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        public double ReadResponseDouble()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return double.NaN;
        }

        public double QueryDouble(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return double.NaN;
        }
    }
}
