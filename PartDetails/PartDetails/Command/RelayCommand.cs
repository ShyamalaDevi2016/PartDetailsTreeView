using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PartDetails.Command
{
  public  class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Predicate<object> canExecute;
        public RelayCommand(Action<object> execute)
        : this(execute, DefaultCanExecute)
        {

        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }


        /// <summary>
        /// Defines if command can be executed (default behaviour)
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Always true</returns>
        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// Execute the encapsulated command
        /// </summary>
        /// <param name="parameter">the parameter that represents the execution method</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
     

    }
}
