using System;
using TSP.Models;
using TSP.ViewModels;

namespace TSP.Commands
{
    public class StartCalculationsCommand : CommandBase
    {
        private InputViewModel _inputViewModel;
        private ResultsModel _results;

        public StartCalculationsCommand(InputViewModel viewModel, ResultsModel results)
        {
            this._inputViewModel = viewModel;
            this._results = results;
        }

        public override void Execute( object? parameter)
        {
            throw new NotImplementedException();
        }
    }
}
