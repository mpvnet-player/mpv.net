
using System;
using System.Windows.Input;

namespace mpvnet
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action<object> ExecuteAction;

        Predicate<object> CanExecutePredicate;

        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate = null)
        {
            ExecuteAction = executeAction;
            CanExecutePredicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter) => CanExecutePredicate == null || CanExecutePredicate(parameter);

        public void Execute(object parameter) => ExecuteAction(parameter);

        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
