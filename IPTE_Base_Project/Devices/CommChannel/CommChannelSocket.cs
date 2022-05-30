using IPTE_Base_Project.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace IPTE_Base_Project.Devices.CommChannel
{
    public class CommChannelSocket : Logging, ICommChannel
    {
        private const string NO_ERROR = "No error";//String returned by device when there are no errors
        private const int MAX_ERRORS = 100;//Maximun number of errors
        private const int SOCKET_TIMEOUT = 10000;//socket timeout       

        protected IPAddress ipAddress;
        protected int ipPort;
        protected IPEndPoint ip;

        #region CommChannelErrorEvent
        public event EventHandler CommChannelError;
        protected virtual void OnCommChannelError(EventArgs e)
        {
            CommChannelError?.Invoke(this, e);
        }
        #endregion

        private Socket socket;
        protected Socket Socket
        {
            get
            {
                if (socket == null || !socket.Connected)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    socket.ReceiveTimeout = SOCKET_TIMEOUT;
                    socket.SendTimeout = SOCKET_TIMEOUT;
                    socket.Connect(ip);
                }
                return socket;
            }
        }

        private bool isInitialized = false;
        public bool IsInitialized()
        {
            return isInitialized;
        }

        public CommChannelSocket(string askedIP, int askedPort)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                isInitialized = false;
                ipAddress = IPAddress.Parse(askedIP);
                ipPort = askedPort;
                ip = new IPEndPoint(ipAddress, ipPort);
            }
            catch (Exception exc)
            {
                StringBuilder exc2 = new StringBuilder("Exception while building Socket Device with IP and port ");
                exc2.Append(askedIP + ":" + askedPort);
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
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

                //When the device is initialized the instrument is reset and the status is cleared
                //so there is common starting point.
                isInitialized = Connect() && Reset() && ClearStatus();
                return isInitialized;
            }
            catch (Exception exc)
            {
                StringBuilder exc2 = new StringBuilder("Exception while initializing Socket Device ");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
                throw new Exception(exc2.ToString());
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public bool Connect()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                if (socket != null)
                {
                    // Connect the socket to the remote endpoint. 
                    socket.Connect(ip);
                }
                return socket.Connected;
            }
            catch (Exception exc)
            {
                log.Error(exc);
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
                if (socket != null)
                {
                    // Release the socket.  
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket = null;
                }
            }
            catch (Exception exc)
            {
                socket = null;
                log.Error(exc);
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        #region Common
        public string Query(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand(command);
                return ReadResponse();
            }
            catch (Exception exc)
            {
                StringBuilder exc2 = new StringBuilder("Exception while querying command ");
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
                log.Error(exc2.ToString());
                log.Error(exc);
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
            string readResponse = ReadResponse();
            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            return readResponse;
        }

        public string ReadResponse()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                byte[] data = new byte[1024];
                int receivedDataLength = Socket.Receive(data);
                return Encoding.ASCII.GetString(data, 0, receivedDataLength);
            }
            catch (Exception exc)
            {
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
                log.Error(exc2.ToString());
                log.Error(exc);
                return null;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void WriteCommand(string command)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes(command + "\n");   //append new line character by default

                // Send the data through the socket.
                Socket.Send(msg);
            }
            catch (Exception exc)
            {
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
                log.Error(exc2.ToString());
                log.Error(exc);
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

        public double ReadResponseDouble()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                // TODO

                return double.NaN; // Return value
            }
            catch (Exception exc)
            {
                log.Error(exc);
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
            try
            {
                // TODO

                return double.NaN; // Return value
            }
            catch (Exception exc)
            {
                log.Error(exc);
                return double.NaN;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }
        #endregion

        public string ReadOneError()
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            try
            {
                WriteCommand("SYSTEM:ERROR?");
                string aux = ReadResponse();
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
                StringBuilder exc2 = new StringBuilder("Exception while reading error");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
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
                StringBuilder exc2 = new StringBuilder("Exception while reading all errors");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
                return null;
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
                log.Error(exc);
                throw new Exception("Exception while checking errors :", exc);
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
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
                StringBuilder exc2 = new StringBuilder("Exception while resetting ");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
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
                StringBuilder exc2 = new StringBuilder("Exception while clearing status");
                exc2.Append(" in instrument ");
                exc2.AppendLine(this.ToString());
                exc2.Append("Detail: ");
                var method = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = string.Format("{0}.{1}({2})",
                               method.ReflectedType.FullName,
                               method.Name,
                               string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));
                exc2.Append(fullName);
                log.Error(exc2.ToString());
                log.Error(exc);
                return false;
            }
            finally
            {
                DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("SocketDevice ");
            sb.Append("(");
            sb.Append("address ");
            sb.Append(this.ipAddress);
            sb.Append(", port ");
            sb.Append(this.ipPort);
            sb.Append(")");
            return sb.ToString();
        }

        public void SessionReset()
        {
            socket = null;
        }
    }
}
