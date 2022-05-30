/* 
 * Base class for every View Model in the application.
 * Based on Windows Presentation Foundation Framework.
 * Implements INotifyPropertyChanged for every derived class
 * so it is not necessary implement it class by class.
 * It also includes GetValue and SetValue that allow
 * read/write properties in derived class without harcoded
 * strings for the properties.
 * Author: Juan Jose Migallon de la Fuente
 * Date:   01/06/2017
 */

using IPTE_Base_Project.Common;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Ipte.TS1.UI.Controls;
using System.Windows;

namespace IPTE_Base_Project.ViewModels
{
    public abstract class BaseViewModel : Logging, INotifyPropertyChanged
    {
        private Dictionary<string, object> properties;
        public string UserName;
       
        private bool _adminlevel;
        public bool AdminLevel
        {
            get { return _adminlevel; }
            set
            {
                _adminlevel = value;
               
                OnPropertyChanged("AdminLevel");
            }
        }

        private bool _operatorlevel;
        public bool OperatorLevel
        {
            get { return _operatorlevel; }
            set
            {
                _operatorlevel = value;
                OnPropertyChanged("OperatorLevel");
            }
        }

        private bool _liderlevel;
        public bool LiderLevel
        {
            get { return _liderlevel; }
            set
            {
                _liderlevel = value;
                OnPropertyChanged("LiderLevel");
            }
        }

        private bool _maintainancelevel;
        public bool MaintainanceLevel
        {
            get { return _maintainancelevel; }
            set
            {
                _maintainancelevel = value;
                OnPropertyChanged("MaintainanceLevel");
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected BaseViewModel()
        {
            this.properties = new Dictionary<string, object>();
            AccessManager.UserChanged += (Object sender, EventArgs e) =>
             {
                 UserName = AccessManager.UserName;
             };

            AccessManager.AccessLevelChanged += (Object sender, EventArgs e) =>
            {
                SetLevel(AccessManager.AccessLevel);
            };


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

        protected virtual void SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
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

        #region Functions for validation
        
        #endregion

        #region Functions for Log

        public void SetDebugLevel(string level)
        {
            Logger current = (Logger)log.Logger;
            current.Level = current.Hierarchy.LevelMap[level];
        }

        public void SetLevel(UserLevel ULevel)
        {
            switch (ULevel)
            {
                case UserLevel.Administrator:
                    AdminLevel = true;
                    MaintainanceLevel = false;
                    LiderLevel = false;
                    OperatorLevel = false;
                    break;
                case UserLevel.Maintenance:
                    AdminLevel = false;
                    MaintainanceLevel = true;
                    LiderLevel = false;
                    OperatorLevel = false;
                    break;
                case UserLevel.Supervisor:
                    AdminLevel = false;
                    MaintainanceLevel = false;
                    LiderLevel = true;
                    OperatorLevel = false;
                    break;
                case UserLevel.Operator:
                    AdminLevel = false;
                    MaintainanceLevel = false;
                    LiderLevel = false;
                    OperatorLevel = true;
                    break;
            }
        }
    #endregion
}
}
