using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (_currentViewModel is InputViewModel)
                {
                    var oldViewModel = (InputViewModel)_currentViewModel;
                    _currentViewModel = value;
                    ((InputViewModel)_currentViewModel).OutputFolderPath = oldViewModel.OutputFolderPath;
                    ((InputViewModel)_currentViewModel).NearestAddition = oldViewModel.NearestAddition;
                    ((InputViewModel)_currentViewModel).DoubleTree = oldViewModel.DoubleTree;
                    ((InputViewModel)_currentViewModel).Christofides = oldViewModel.Christofides;
                    ((InputViewModel)_currentViewModel).KernighanLin = oldViewModel.KernighanLin;
                    ((InputViewModel)_currentViewModel).Stopwatch = oldViewModel.Stopwatch;
                    ((InputViewModel)_currentViewModel).NumberOfCalculations = oldViewModel.NumberOfCalculations;
                    if (oldViewModel is FileInputViewModel && _currentViewModel is FolderInputViewModel)
                    {
                        ((FolderInputViewModel)_currentViewModel).IgnoreNotFoundResultFiles = oldViewModel.IgnoreNotFoundResultFiles;

                    }
                    if (oldViewModel is FolderInputViewModel && _currentViewModel is FileInputViewModel)
                    {
                        ((FileInputViewModel)_currentViewModel).IgnoreNotFoundResultFiles = oldViewModel.IgnoreNotFoundResultFiles;
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

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        public event Action CurrentViewModelChanged;
    }
}
