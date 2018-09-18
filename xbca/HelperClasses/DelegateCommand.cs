namespace Xbca.HelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Represents the delegate command.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// Universal command.
        /// </summary>
        /// <param name="execute">Action that's called when Execute is invoked.</param>
        public DelegateCommand(Action<object> execute)
                       : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// Universal command that takes two delegates.
        /// </summary>
        /// <param name="execute">Action that is called when Execute is invoked.</param>
        /// <param name="canExecute">Predicate that is evaluated when CanExecute is invoked.</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when can execute predicate is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Evaluates the commands canExecute predicate or always returns true if it's null.
        /// </summary>
        /// <param name="parameter">Command parameter that is passed to the command.</param>
        /// <returns><c>true</c>if the command can execute; otherwise <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }

            return this._canExecute(parameter);
        }

        /// <summary>
        /// Executes the action that was passed to the command.
        /// </summary>
        /// <param name="parameter">Command parameter that is passed to the command.</param>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        /// <summary>
        /// Triggers the CanExecuteChanged event. Causes the UI to reevaluate the CanExecute method.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
