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
        private ResultsViewModel resultsViewModel;
        private readonly NavigationStore navigationStore = new();
        private CancellationTokenStore cancellationTokenStore = new();

        public App()
        {
            navigationStore.CurrentViewModel = CreateFileInputViewModel();
            resultsViewModel = new ResultsViewModel(navigationStore, cancellationTokenStore);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(resultsViewModel, navigationStore)
            };

            MainWindow.Show();

            base.OnStartup(e);
        }

        private FileInputViewModel CreateFileInputViewModel() => new FileInputViewModel(navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);
        private FolderInputViewModel CreateFolderInputViewModel() => new FolderInputViewModel(navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);
        private GenerateInputViewModel CreateGenerateInputViewModel() => new GenerateInputViewModel(navigationStore, CreateFileInputViewModel, CreateFolderInputViewModel, CreateGenerateInputViewModel);

    }
}
