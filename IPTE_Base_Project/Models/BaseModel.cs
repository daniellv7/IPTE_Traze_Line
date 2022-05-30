using IPTE_Base_Project.Common;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPTE_Base_Project.Models
{
    public abstract class BaseModel : Logging, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        private Dictionary<string, object> properties;

        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseModel()
        {
            properties = new Dictionary<string, object>();
        }

        public void LogSeqInfo(string msg)
        {
            Mediator.NotifyColleagues("SequenceMessage", new object[] { msg });
            log.Info(msg);
        }

        public void LogSeqError(string msg)
        {
            Mediator.NotifyColleagues("SequenceMessage", new object[] { msg });
            log.Error(msg);
        }

        public void LogSeqWarn(string msg)
        {
            Mediator.NotifyColleagues("SequenceMessage", new object[] { msg });
            log.Warn(msg);
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (!properties.ContainsKey(propertyName))
            {
                return default(T);
            }
            else
            {
                return (T)properties[propertyName];
            }
        }

        protected void SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!properties.ContainsKey(propertyName))
            {
                properties.Add(propertyName, default(T));
            }

            var oldValue = GetValue<T>(propertyName);
            if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                properties[propertyName] = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null) { handler(this, e); }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Logging

        public void SetDebugLevel(string level)
        {
            log.Debug("ParametersModel => SetDebugLevel");

            Logger current = (Logger)log.Logger;
            current.Level = current.Hierarchy.LevelMap[level];

            log.Debug("ParametersModel <= SetDebugLevel");
        }

        #endregion
    }
}
