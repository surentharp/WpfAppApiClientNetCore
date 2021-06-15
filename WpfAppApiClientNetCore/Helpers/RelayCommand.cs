using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfAppApiClientNetCore.Helpers
{
    class RelayCommand : ICommand
    {
        private Action mAction;
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
