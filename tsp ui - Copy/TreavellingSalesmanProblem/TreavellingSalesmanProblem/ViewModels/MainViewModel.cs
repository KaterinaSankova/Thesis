using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Models;
using TSP.Stores;

namespace TSP.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ResultsViewModel _resultsViewModel;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public ResultsViewModel CurrentResultsViewModel => _resultsViewModel;

        public MainViewModel(NavigationStore navigationStore, ResultsViewModel resultsViewModel)
        {
            _navigationStore = navigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            _resultsViewModel = resultsViewModel;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
