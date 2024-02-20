using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Algorithms.Enums;
using TSP.Enum;

namespace TSP
{
    public class State : INotifyPropertyChanged
    {
        public InputMode Mode;
        public string? SourceFilePath;
        public string? SourceFolderPath;
        //generate
        public string? ResultFilePath;
        public string? ResultFolderPath;

        //output
        private string? _outputFolderPath;
        public string? OutputFolderPath
        {
            get
            {
                return _outputFolderPath;
            }
            set
            {
                _outputFolderPath = value;
                OnPropertyChanged("OutputFolderPath");
            }
        }

        //algorithms
        public List<TSPAlgorithms> SelectedAlgorithms;

        //stopwatch
        public bool Stopwatch;

        public State()
        {
            Mode = InputMode.File;
            SelectedAlgorithms = [TSPAlgorithms.NearestAddition];
            Stopwatch = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
