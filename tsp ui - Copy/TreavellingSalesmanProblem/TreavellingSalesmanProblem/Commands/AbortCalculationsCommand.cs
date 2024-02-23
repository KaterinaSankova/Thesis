using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSP.Models;
using TSP.Stores;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class AbortCalculationsCommand : CommandBase
    {
        private CancellationTokenStore _cancellationTokenSource;

        public AbortCalculationsCommand(CancellationTokenStore cancellationTokenStore)
        {
            this._cancellationTokenSource = cancellationTokenStore;
        }

        public override void Execute(object parameter)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
