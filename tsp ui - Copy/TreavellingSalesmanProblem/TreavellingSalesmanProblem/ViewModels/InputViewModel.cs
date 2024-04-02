using System;
using System.Windows.Input;
using TSP.Commands;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class InputViewModel : ViewModelBase
    {
        private bool _nearestAddition = true;
        private bool _doubleTree = false;
        private bool _christofides = false;
        private bool _kernighanLin = false;
        private bool _kernighanLinRb = false;
        private bool _stopwatch = true;
        private bool _ignoreNotFoundResultFiles = true;
        private string _outputFolderPath = "";

        public bool NearestAddition
        {
            get
            {
                return _nearestAddition;
            }
            set
            {
                _nearestAddition = value;
                OnPropertyChanged(nameof(NearestAddition));
            }
        }

        public bool DoubleTree
        {
            get
            {
                return _doubleTree;
            }
            set
            {
                _doubleTree = value;
                OnPropertyChanged(nameof(DoubleTree));
            }
        }

        public bool Christofides {
            get
            {
                return _christofides;
            }
            set
            {
                _christofides = value;
                OnPropertyChanged(nameof(Christofides));
            }
        }

        public bool KernighanLin
        {
            get
            {
                return _kernighanLin;
            }
            set
            {
                _kernighanLin = value;
                OnPropertyChanged(nameof(KernighanLin));
            }
        }

        public bool KernighanLinRb
        {
            get
            {
                return _kernighanLinRb;
            }
            set
            {
                _kernighanLinRb = value;
                OnPropertyChanged(nameof(KernighanLinRb));
            }
        }

        public bool Stopwatch
        {
            get
            {
                return _stopwatch;
            }
            set
            {
                _stopwatch = value;
                OnPropertyChanged(nameof(Stopwatch));
            }
        }

        public bool IgnoreNotFoundResultFiles
        {
            get
            {
                return _ignoreNotFoundResultFiles;
            }

            set
            {
                _ignoreNotFoundResultFiles = value;
                OnPropertyChanged(nameof(IgnoreNotFoundResultFiles));
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

        public int NumberOfSelectedAlgorithms
        {
            get
            {
                int i = 0;
                if (NearestAddition)
                    i++;
                if (DoubleTree)
                    i++;
                if (Christofides)
                    i++;
                if (KernighanLin)
                    i++;
                if (KernighanLinRb)
                    i++;
                return i;
            }
        }

        public ICommand NavigateToFileInput { get; }

        public ICommand NavigateToFolderInput { get; }

        public ICommand NavigateToGenerateInput { get; }

        public ICommand ClearOutputFolderCommand { get; }

        public InputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel)
        {
            NavigateToFileInput = new NavigateCommand(navigationStore, createFileInputViewModel);
            NavigateToFolderInput = new NavigateCommand(navigationStore, createFolderInputViewModel);
            NavigateToGenerateInput = new NavigateCommand(navigationStore, createGenerateInputViewModel);
            ClearOutputFolderCommand = new ClearOutputFolderCommand(this);
        }
    }
}
