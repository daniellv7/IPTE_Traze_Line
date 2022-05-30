using IPTE_Base_Project.Common;
using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace IPTE_Base_Project.Devices.CommChannel
{
    public class CommChannelTCPIP : Logging, ICommChannel
    {
        #region Data members and accessors

        private const string NO_ERROR = "No error";//String returned by device when there are no errors
        private const int MAX_ERRORS = 100;//Maximun number of errors

        #region CommChannelErrorEvent
        public event EventHandler CommChannelError;
        protected virtual void OnCommChannelError()
        {
            CommChannelError?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        private bool isInitialized = false;
        public bool IsInitialized()
        {
            return isInitialized;
        }

        private string tcpIpAddress;
        public string TcpIpAddress //IP address from the instrument
        {
            get { return tcpIpAddress; }
            set { tcpIpAddress = value; }
        }

        private FormattedIO488 session;
        private FormattedIO488 Session
        {
            get
            {
                if (session == null)
                {
                    try
                    {
                        session = new FormattedIO488();
                        if (ResourceManager != null)
                        {
                            session.IO = (IMessage)ResourceManager.Open(tcpIpAddress, AccessMode.NO_LOCK, 10000);
                            session.IO.SendEndEnabled = true;
                            session.IO.TerminationCharacterEnabled = true;
                        }
                        else
                            OnCommChannelError();
                    }
                    catch { OnCommChannelError(); }
                }
                return session;
            }
        }

        private ResourceManager resourceManager;
        public ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                        resourceManager = new ResourceManager();
                }
                return resourceManager;
            }
        }
        #endregion

        #region Constructors

        public CommChannelTCPIP(string askedIP)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            isInitialized = false;
            tcpIpAddress = askedIP;
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods
        [HandleProcessCorruptedStateExceptions]
        public void WriteCommand(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            log.Debug(String.Format("Command: {0}", command));
            try
            {
                if (Session.IO != null)
                    Session.WriteString(command, true);//flush and end by default
                else
                    log.Warn(String.Format("CommChannelTCPIP not initialized: {0}", TcpIpAddress));
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }
                StringBuilder exc2 = new StringBuilder("Exception while writing command ");
                exc2.Append(command);
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                                method.ReflectedType.FullName,
                                method.Name,
                                string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void WriteCommand(string commands, char separator)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string[] commandArray = commands.Split(separator);
            foreach (string command in commandArray)
            {
                WriteCommand(command);
            }
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }

        [HandleProcessCorruptedStateExceptions]
        public string ReadResponse()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (Session.IO != null)
                    return Session.ReadString();
                else
                {
                    log.Warn(String.Format("CommChannelTCPIP not initialized: {0}", TcpIpAddress));
                    return String.Format("CommChannelTCPIP not initialized: {0}", TcpIpAddress);
                }
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }
                StringBuilder exc2 = new StringBuilder("Exception while reading response");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public string Query(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            log.Debug(String.Format("Command: {0}", command));
            try
            {
                if (Session.IO != null)
                {
                    Session.WriteString(command, true);//flush and end by default
                    return Session.ReadString();
                }
                else
                {
                    log.Warn(String.Format("CommChannelTCPIP not initialized: {0}", TcpIpAddress));
                    return String.Format("CommChannelTCPIP not initialized: {0}", TcpIpAddress);
                }
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }

                StringBuilder exc2 = new StringBuilder("Exception while querying command ");
                exc2.Append(command);
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public string Query(string commands, char separator)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string[] commandArray = commands.Split(separator);
            foreach (string command in commandArray)
            {
                WriteCommand(command);
            }
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return ReadResponse();
        }

        public bool Reset()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand("*RST");
                CheckErrors();
                return true;
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }


                StringBuilder exc2 = new StringBuilder("Exception while resetting ");
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return false;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public bool ClearStatus()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand("*CLS");
                CheckErrors();
                return true;
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }

                StringBuilder exc2 = new StringBuilder("Exception while clearing status");
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return false;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void CheckErrors()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                List<string> res = ReadAllErrors();
                if (!res.First().Equals(NO_ERROR))
                {
                    throw new Exception("Errors detected in instrument " + this.ToString() + " Errors: " + res.ToString());
                }
            }
            catch (Exception exc)
            {
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }

                log.Error(exc.ToString());
                throw new Exception("Exception while checking errors :", exc);
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public string ReadOneError()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand("SYSTEM:ERROR?");
                string aux = Session.ReadString();
                if (aux.Contains(NO_ERROR))
                {
                    return NO_ERROR;
                }
                else
                {
                    return aux;
                }
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                try
                {
                    if (Session.IO != null) Session.IO.Close();
                }
                catch { }
                
                StringBuilder exc2 = new StringBuilder("Exception while reading error");
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public List<string> ReadAllErrors()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                string response;
                int count = 0;
                List<string> errorList = new List<string>();
                do
                {
                    response = ReadOneError();
                    errorList.Add(response);
                    count++;
                }
                while (response != NO_ERROR && count <= MAX_ERRORS);
                return errorList;
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                if (Session.IO != null) Session.IO.Close();
                StringBuilder exc2 = new StringBuilder("Exception while reading all errors");
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public string Identification()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand("*IDN?");
                string response = ReadResponse();
                CheckErrors();
                return response;
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                if (Session.IO != null) Session.IO.Close();
                StringBuilder exc2 = new StringBuilder("Exception while identificating");
                exc2.Append("in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public bool Initialize()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (isInitialized)
                {
                    return true;
                }
                isInitialized = Reset() && ClearStatus();
                CheckErrors();
                string test = Query("*IDN?");
                return isInitialized;
            }
            catch (Exception exc)
            {
                OnCommChannelError();
                if (Session.IO != null) Session.IO.Close();
                StringBuilder exc2 = new StringBuilder("Exception while initializing with IP ");
                exc2.Append(TcpIpAddress);
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString(), exc);
                return false;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void Terminate()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                isInitialized = false;
                if (session != null)
                {
                    if (session.IO != null) session.IO.Close();
                    session = null;
                    resourceManager = null;
                }
            }
            catch (Exception exc)
            {
                if (Session.IO != null) Session.IO.Close();
                log.Error(exc);
                session = null;
                resourceManager = null;
            }
            finally
            {
                session = null;
                resourceManager = null;
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TCPIPDevice ");
            sb.Append("(");
            sb.Append("TCPIP ");
            sb.Append(this.tcpIpAddress);
            //sb.Append(", name ");
            //sb.Append(this.name);
            sb.Append(")");
            return sb.ToString();
        }

        public double ReadResponseDouble()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string str = ReadResponse();
            double result = double.NaN;
            try
            {
                if (!double.TryParse(str, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
                    return double.NaN;
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return double.NaN;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public double QueryDouble(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            string str = Query(command);
            double result = double.NaN;
            try
            {
                if (!double.TryParse(str, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
                    return double.NaN;
                return result;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return double.NaN;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SessionReset()
        {
            session = null;
        }
        #endregion
    }
}