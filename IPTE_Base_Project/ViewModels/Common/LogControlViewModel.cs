using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using log4net;
using System.Collections.ObjectModel;
using IPTE_Base_Project.Common;
using System.Windows.Input;
using System.Windows.Data;

namespace IPTE_Base_Project.ViewModels
{
    public partial class LogControlViewModel : BaseViewModel, IAppender
    {
        #region Data members and Accessors

        private const int MAX_MESSAGES = 100;

        private static ILog _logger = LogManager.GetLogger(typeof(LogControlViewModel));
        private ObservableCollection<LogMessage> messageList;
        private string name;
        private bool isLogsChangedPropertyInViewModel;
        //Lock for the messageList
        private static object _lock;

        public static ILog Logger
        {
            get { return _logger; }
        }
                
        public ObservableCollection<LogMessage> MessageList
        {
            get
            {
                return messageList;
            }

            set
            {
                messageList = value;
            }
        }

        public ObservableCollection<LogMessage> MessageListDebug
        {
            get
            {
                return messageList;
            }

            set
            {
                messageList = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
        
        public bool IsLogsChangedPropertyInViewModel
        {
            get
            {
                return isLogsChangedPropertyInViewModel;
            }

            set
            {
                isLogsChangedPropertyInViewModel = value;
            }
        }

        #endregion

        public LogControlViewModel()
        {
            MessageList = new ObservableCollection<LogMessage>();

            _lock = new object();

            //Enable the cross acces to this collection elsewhere
            BindingOperations.EnableCollectionSynchronization(MessageList, _lock);
        }

        public void Close()
        {
            //throw new NotImplementedException();
        }

        //Appender interface
        public void DoAppend(LoggingEvent loggingEvent)
        {
            try
            {
                //Add text to the observable list that the UI is binding to
                MessageList.Add(new LogMessage(loggingEvent.TimeStamp + " - " + loggingEvent.MessageObject.ToString()));

                if(loggingEvent.Level==Level.Debug)
                    MessageListDebug.Add(new LogMessage(loggingEvent.TimeStamp + " - " + loggingEvent.MessageObject.ToString()));

                //MessageList.Insert(0,new LogMessage(loggingEvent.MessageObject.ToString()));

                if (MessageList.Count > MAX_MESSAGES)
                {
                    MessageList.RemoveAt(0);
                }
                IsLogsChangedPropertyInViewModel = true;
            }
            catch 
            {

            }
        }
    }
}
