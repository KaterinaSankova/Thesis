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
                _currentViewModel = value;
                _currentViewModel.PropertyChanged += InputChanged;
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
