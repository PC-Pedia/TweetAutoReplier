using System;
using System.Windows.Input;

namespace TwitterClient.Contracts
{
    class RelayCommand : ICommand
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
}
