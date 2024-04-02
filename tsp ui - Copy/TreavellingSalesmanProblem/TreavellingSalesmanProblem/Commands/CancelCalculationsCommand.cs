using System.ComponentModel;
using System.Threading.Tasks;
using TSP.Stores;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class CancelCalculationsCommand : AsyncCommandBase
    {
        private readonly ResultsViewModel results;
        private readonly CancellationTokenStore cancellationToken;

        public CancelCalculationsCommand(ResultsViewModel results, CancellationTokenStore cancellationToken)
        {
            this.results = results;
            this.cancellationToken = cancellationToken;
            results.PropertyChanged += OnResultsChange;
        }

        private void OnResultsChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ResultsViewModel.CalculationsStarted))
            {
                OnCanExecutedChanged();
                OnCalculationsStarted();
            }
        }

        private void OnCalculationsStarted()
        {
            if (results.CalculationsStarted)
            {
                cancellationToken.RestoreToken();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return (results.CalculationsStarted && !cancellationToken.Token.IsCancellationRequested && base.CanExecute(parameter));
        }

        public override Task ExecuteAsync(object parameter)
        {
            return Task.Factory.StartNew(cancellationToken.Cancel);
        }
    }
}
