using System;
using System.Windows.Input;
using TreavellingSalesmanProblem;

namespace TSP.Commands
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        protected void OnCanExecutedChanged()
        {
            App.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }
    }
}
