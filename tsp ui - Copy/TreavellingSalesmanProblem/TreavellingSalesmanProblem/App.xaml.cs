using System.Windows;
using TSP;
using TSP.Models;
using TSP.ViewModels;

namespace TreavellingSalesmanProblem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ResultsModel _resultsData;

        public App()
        {
            _resultsData = new ResultsModel();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_resultsData)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

    }
}
