using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestNm2.ViewModel
{
    public class CommandHandler : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new NullReferenceException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public CommandHandler(Action execute) : this(execute, null) 
        {
        
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>

        public bool CanExecute(object parameter)
        {
            //return _canExecute();
            if (_canExecute == null)
                return true;
            return _canExecute.Invoke();
            //return true;
        }
        
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
