using System;
using System.Windows.Input;

namespace TwitterClient.Common
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }

    public class GenericRelayCommand<T> : ICommand
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
    }
}
