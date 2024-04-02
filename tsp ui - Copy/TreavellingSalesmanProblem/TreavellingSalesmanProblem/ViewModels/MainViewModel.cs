using TSP.Stores;

namespace TSP.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore navigationStore;
        private readonly ResultsViewModel resultsViewModel;

        public ViewModelBase CurrentViewModel => navigationStore.CurrentViewModel;

        public ResultsViewModel CurrentResultsViewModel => resultsViewModel;

        public MainViewModel(ResultsViewModel resultsViewModel, NavigationStore navigationStore)
        {
            this.resultsViewModel = resultsViewModel;
            this.navigationStore = navigationStore;
            this.navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
