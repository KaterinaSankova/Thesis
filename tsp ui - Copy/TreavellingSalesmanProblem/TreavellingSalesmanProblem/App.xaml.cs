using System.Threading;
using System.Windows;
using TSP;
using TSP.Models;
using TSP.Stores;
using TSP.ViewModels;

namespace TreavellingSalesmanProblem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ResultsViewModel _resultsViewModel;
        private readonly NavigationStore _navigationStore;
        private CancellationTokenStore _cancellationTokenStore;

        public App()
        {
            _navigationStore = new NavigationStore();
            _navigationStore.CurrentViewModel = CreateFileInputViewModel();
            _resultsViewModel = new ResultsViewModel(_navigationStore);
            //_cancellationTokenStore = new CancellationTokenStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore, _resultsViewModel)
            };

            MainWindow.Show();

            base.OnStartup(e);
        }

        private FileInputViewModel CreateFileInputViewModel() => new FileInputViewModel(_navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);
        private FolderInputViewModel CreateFolderInputViewModel() => new FolderInputViewModel(_navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);
        private GenerateInputViewModel CreateGenerateInputViewModel() => new GenerateInputViewModel(_navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);

    }
}
