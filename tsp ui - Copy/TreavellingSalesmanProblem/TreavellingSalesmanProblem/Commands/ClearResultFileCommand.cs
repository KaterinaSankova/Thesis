using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class ClearResultFileCommand : CommandBase
    {
        private FileInputViewModel input;

        public ClearResultFileCommand(FileInputViewModel input)
        {
            this.input = input;
            input.PropertyChanged += OnInputChange;
        }

        private void OnInputChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileInputViewModel.ResultFilePath))
            {
                OnCanExecutedChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return (!string.IsNullOrEmpty(input.ResultFilePath)) && base.CanExecute(parameter);
        }

        public override void Execute(object parameter) => input.ResultFilePath = "";
    }
}
