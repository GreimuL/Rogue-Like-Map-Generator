using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RogueLikeMapGenerator.viewmodel
{
    //Async 함수를 위한 command
    class AsyncRelayCommand : ICommand
    {
        Func<object, bool> canExecute;
        Func<object, Task> executionAction;

        public AsyncRelayCommand(Func<object, Task> action) : this(action, null) { }
        public AsyncRelayCommand(Func<object, Task> action, Func<object, bool> canExecute)
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

        public async void Execute(object parameter)
        {
            try
            {
                await executionAction(parameter);
            }
            catch { }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
