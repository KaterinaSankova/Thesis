using System.ComponentModel;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class ClearOutputFolderCommand : CommandBase
    {
        private InputViewModel input;

        public ClearOutputFolderCommand(InputViewModel input)
        {
            this.input = input;
            input.PropertyChanged += OnInputChange;
        }

        private void OnInputChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InputViewModel.OutputFolderPath))
            {
                OnCanExecutedChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(input.OutputFolderPath) && base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            input.OutputFolderPath = "";
        }
    }
}
