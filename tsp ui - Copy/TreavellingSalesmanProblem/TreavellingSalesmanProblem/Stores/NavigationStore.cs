using System;
using System.ComponentModel;
using TSP.Enum;
using TSP.ViewModels;

namespace TSP.Stores
{
    public class NavigationStore : ViewModelBase
    {
        private ViewModelBase _currentViewModel = new();

        public NavigationStore()
        {
            _currentViewModel.PropertyChanged += InputChanged;
        }

        private void InputChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnCurrentViewModelChanged();
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel is InputViewModel oldViewModel)
                {
                    _currentViewModel = value;
                    ((InputViewModel)_currentViewModel).OutputFolderPath = oldViewModel.OutputFolderPath;
                    ((InputViewModel)_currentViewModel).NearestAddition = oldViewModel.NearestAddition;
                    ((InputViewModel)_currentViewModel).DoubleTree = oldViewModel.DoubleTree;
                    ((InputViewModel)_currentViewModel).Christofides = oldViewModel.Christofides;
                    ((InputViewModel)_currentViewModel).KernighanLin = oldViewModel.KernighanLin;
                    ((InputViewModel)_currentViewModel).KernighanLinRb = oldViewModel.KernighanLinRb;
                    ((InputViewModel)_currentViewModel).Stopwatch = oldViewModel.Stopwatch;
                    if (oldViewModel is FileInputViewModel && _currentViewModel is FolderInputViewModel)
                    {
                        ((FolderInputViewModel)_currentViewModel).IgnoreNotFoundResultFiles = oldViewModel.IgnoreNotFoundResultFiles;

                    }
                    if (oldViewModel is FolderInputViewModel && _currentViewModel is FileInputViewModel model)
                    {
                        model.IgnoreNotFoundResultFiles = oldViewModel.IgnoreNotFoundResultFiles;
                    }
                    _currentViewModel.PropertyChanged += InputChanged;
                }
                else
                {
                    _currentViewModel = value;
                }
                OnCurrentViewModelChanged();
            }
        }

        public InputMode GetInputMode()
        {
            switch (_currentViewModel.GetType().Name)
            {
                case nameof(FileInputViewModel):
                    return InputMode.File;
                case nameof(FolderInputViewModel):
                    return InputMode.Folder;
                case nameof(GenerateInputViewModel):
                    return InputMode.Generate;
                default:
                    throw new Exception("Invalid view model");
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        public event Action CurrentViewModelChanged;
    }
}
