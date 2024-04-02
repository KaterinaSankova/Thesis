using System.Threading;

namespace TSP.Stores
{
    public class CancellationTokenStore
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CancellationToken Token { get {  return _cancellationTokenSource.Token; } }

        public void Cancel() => _cancellationTokenSource.Cancel();

        public void RestoreToken() => _cancellationTokenSource = new CancellationTokenSource();
    }
}
