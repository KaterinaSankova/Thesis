using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if ((e.PropertyName == nameof(FileInputViewModel.OutputFolderPath)) || (e.PropertyName == nameof(FolderInputViewModel.OutputFolderPath)))
            {
                OnCanExecutedChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (input.GetType().Name == nameof(FileInputViewModel))
            {
                return (!string.IsNullOrEmpty(((FileInputViewModel)input).OutputFolderPath)) && base.CanExecute(parameter);
            }
            else
            {
                return (!string.IsNullOrEmpty(((FolderInputViewModel)input).OutputFolderPath)) && base.CanExecute(parameter);
            }
        }

        public override void Execute(object parameter)
        {
            if (input.GetType().Name == nameof(FileInputViewModel))
            {
                ((FileInputViewModel)input).OutputFolderPath = "";
            }
            else
            {
                ((FolderInputViewModel)input).OutputFolderPath = "";
            }
        }
    }
}
