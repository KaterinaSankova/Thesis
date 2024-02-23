using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class ClearResultFolderCommand : CommandBase
    {
        private FolderInputViewModel input;

        public ClearResultFolderCommand(FolderInputViewModel input)
        {
            this.input = input;
            input.PropertyChanged += OnInputChange;
        }

        private void OnInputChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderInputViewModel.ResultFolderPath))
            {
                OnCanExecutedChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return (!string.IsNullOrEmpty(input.ResultFolderPath)) && base.CanExecute(parameter);
        }

        public override void Execute(object parameter) => input.ResultFolderPath = "";
    }
}
