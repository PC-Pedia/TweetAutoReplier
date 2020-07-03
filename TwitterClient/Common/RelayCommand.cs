using System;
using System.Windows.Input;

namespace TwitterClient.Common
{
    public class RelayCommand<T> : ICommand
    {
        public Predicate<T> CanExecuteDelegate { get; set; }
        public Action<T> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate((T)parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke((T)parameter);
        }

        public RelayCommand(Action<T> executeDelegate)
        {
            ExecuteDelegate = executeDelegate;
        }

        public RelayCommand(Action<T> executeDelegate, Predicate<T> canExecuteDelegate) : this(executeDelegate)
        {
            CanExecuteDelegate = canExecuteDelegate;
        }
    }
}
