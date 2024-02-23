using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TSP.Commands;
using TSP.Models;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class FolderInputViewModel : InputViewModel
    {
        private string _sourceFolderPath;
        private string _resultFolderPath;
        private string _outputFolderPath;

        public string SourceFolderPath
        {
            get
            {
                return _sourceFolderPath;
            }
            set
            {
                _sourceFolderPath = value;
                OnSourcePathChanged();
                OnPropertyChanged(nameof(SourceFolderPath));
            }
        }


        public string ResultFolderPath
        {
            get
            {
                return _resultFolderPath;
            }
            set
            {
                _resultFolderPath = value;
                OnPropertyChanged(nameof(ResultFolderPath));
            }
        }

        public string OutputFolderPath
        {
            get
            {
                return _outputFolderPath;
            }
            set
            {
                _outputFolderPath = value;
                OnPropertyChanged(nameof(OutputFolderPath));
            }
        }

        private void OnSourcePathChanged()
        {
            SourcePathChanged?.Invoke();
        }

        public event Action SourcePathChanged;

        public ICommand ClearResultFolderCommand { get; }
        public ICommand ClearOutputFolderCommand { get; }

        public FolderInputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel) : base(navigationStore, createFileInputViewModel, createFolderInputViewModel, createGenerateInputViewModel)
        {
            ClearResultFolderCommand = new ClearResultFolderCommand(this);
            ClearOutputFolderCommand = new ClearOutputFolderCommand(this);
        }
    }
}
