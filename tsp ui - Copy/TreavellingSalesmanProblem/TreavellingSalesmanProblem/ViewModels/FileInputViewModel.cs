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
    public class FileInputViewModel : InputViewModel
    {
        private string _sourceFilePath;
        private string _resultFilePath;
        private string _outputFolderPath;

        public string SourceFilePath
        {
            get
            {
                return _sourceFilePath;
            }
            set
            {
                _sourceFilePath = value;
                OnSourcePathChanged();
                OnPropertyChanged(nameof(SourceFilePath));
            }
        }


        public string ResultFilePath
        {
            get
            {
                return _resultFilePath;
            }
            set
            {
                _resultFilePath = value;
                OnPropertyChanged(nameof(ResultFilePath));
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

        public ICommand ClearResultFileCommand { get; }
        public ICommand ClearOutputFolderCommand { get; }

        public FileInputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel) : base(navigationStore, createFileInputViewModel, createFolderInputViewModel, createGenerateInputViewModel)
        {
            ClearResultFileCommand = new ClearResultFileCommand(this);
            ClearOutputFolderCommand = new ClearOutputFolderCommand(this);
        }
    }
}
