/* 
 * Base class for implementing commands.
 * Author: Juan Jose Migallon de la Fuente
 * Date:   01/06/2017
 */

using IPTE_Base_Project.Common.Utils.MediatorPattern;
using System;
using System.Windows.Input;

namespace IPTE_Base_Project.Common
{
    public class RelayCommand : ICommand
    {
        #region Data members and accessors

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }
        readonly Predicate<object> canExecute;

        public void Execute(object parameter)
        {
            execute(parameter);
        }
        readonly Action<object> execute;

        private event EventHandler CanExecuteChangedInternal;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChangedInternal.Raise(this);
        }

        #endregion

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
            this.execute = execute;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion
    }

    public class RelayCommand<T> : ICommand
    {
        #region Data members and accessors

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute((T)parameter);
        }
        readonly Predicate<T> canExecute;

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
        readonly Action<T> execute;

        #endregion

        #region Constructors

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
            this.execute = execute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion
    }
}
