using System;
using System.Windows.Input;
using TSP.Commands;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class FileInputViewModel : InputViewModel
    {
        private string _sourceFilePath = "";
        private string _resultFilePath = "";

        public string SourceFilePath
        {
            get
            {
                return _sourceFilePath;
            }
            set
            {
                _sourceFilePath = value;
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

        public ICommand ClearResultFileCommand { get; }

        public FileInputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel) : base(navigationStore, createFileInputViewModel, createFolderInputViewModel, createGenerateInputViewModel)
        {
            ClearResultFileCommand = new ClearResultFileCommand(this);
        }
    }
}
