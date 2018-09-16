using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace xbca.HelperClasses
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Universal command.
        /// </summary>
        /// <param name="execute">Action that's called when Execute is invoked.</param>
        public DelegateCommand(Action<object> execute)
                       : this(execute, null)
        {
        }

        /// <summary>
        /// Universal command that takes two delegates.
        /// </summary>
        /// <param name="execute">Action that is called when Execute is invoked.</param>
        /// <param name="canExecute">Predicate that is evaluated when CanExecute is invoked.</param>
        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Evaluates the commands canExecute predicate or always returns true if it's null.
        /// </summary>
        /// <param name="parameter">Command parameter that is passed to the command.</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        /// <summary>
        /// Executes the action that was passed to the command.
        /// </summary>
        /// <param name="parameter">Command parameter that is passed to the command.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Triggers the CanExecuteChanged event. Causes the UI to reevaluate the CanExecute method.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
