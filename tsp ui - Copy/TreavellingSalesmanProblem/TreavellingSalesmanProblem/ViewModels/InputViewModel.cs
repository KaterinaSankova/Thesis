using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravellingSalesmanProblem.Algorithms.Enums;
using TSP.Commands;
using TSP.Models;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class InputViewModel : ViewModelBase
    {
        private bool _nearestAddition;
        private bool _doubleTree;
        private bool _christofides;
        private bool _kernighanLin;
        private bool _stopwatch;
        private bool _ignoreNotFoundResultFiles = true;
        private string _outputFolderPath;

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
            NearestAddition = true;
        }
    }
}
