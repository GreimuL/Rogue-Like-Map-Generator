using System;
using System.Windows.Input;

namespace RogueLikeMapGenerator.viewmodel
{
    class RelayCommand : ICommand
    {
        Func<object, bool> canExecute;
        Action<object> executionAction;

        public RelayCommand(Action<object> action) : this(action, null) { }
        public RelayCommand(Action<object> action, Func<object, bool> canExecute)
        {
            executionAction = action;
            this.canExecute = canExecute;
        }


        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            else
            {
                return canExecute.Invoke(parameter);
            }
        }

        public void Execute(object parameter)
        {
            executionAction.Invoke(parameter);
        }
    }
}
