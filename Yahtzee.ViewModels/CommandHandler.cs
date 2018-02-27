using System;
using System.Windows.Input;

namespace Yahtzee.ViewModels
{
    public class CommandHandler : ICommand
    {
        Action<object> _action;
        Func<object, bool> _canExecute;

        public CommandHandler(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public CommandHandler(Action<object> action) : this(action, null)
        {
        }

        public bool CanExecute(object parameter) => _canExecute(parameter);
        public void Execute(object parameter) => _action(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
