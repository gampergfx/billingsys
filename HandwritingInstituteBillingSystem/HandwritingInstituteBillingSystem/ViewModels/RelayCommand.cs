using System;
using System.Windows.Input;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    class RelayCommand : ICommand
    {
        private Action<object> _action;
        private readonly Func<object, bool> _canExecuteMethod;

        public RelayCommand(Action<object> action, Func<object,bool> canExecuteMethod)
        {
            _action = action;
            _canExecuteMethod = canExecuteMethod;
        }

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if(_canExecuteMethod!=null)
            return _canExecuteMethod(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion
    }
}