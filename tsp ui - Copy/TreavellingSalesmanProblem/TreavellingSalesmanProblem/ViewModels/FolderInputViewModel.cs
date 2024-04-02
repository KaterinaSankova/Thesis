using System;
using System.Windows.Input;
using TSP.Commands;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class FolderInputViewModel : InputViewModel
    {
        private string _sourceFolderPath = "";
        private string _resultFolderPath = "";

        public string SourceFolderPath
        {
            get
            {
                return _sourceFolderPath;
            }
            set
            {
                _sourceFolderPath = value;
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

        public ICommand ClearResultFolderCommand { get; }

        public FolderInputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel) : base(navigationStore, createFileInputViewModel, createFolderInputViewModel, createGenerateInputViewModel)
        {
            ClearResultFolderCommand = new ClearResultFolderCommand(this);
        }
    }
}
