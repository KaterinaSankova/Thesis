using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Models;

namespace TSP.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; }

        public MainViewModel(ResultsModel results)
        {
            CurrentViewModel = new FileInputViewModel(results);
        }
    }
}
