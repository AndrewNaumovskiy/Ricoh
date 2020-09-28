using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RicohXamarin
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object param)
        {
            return _canExecute == null || _canExecute(param);
        }

        public void Execute(object param)
        {
            _execute(param);
        }
    }
}
