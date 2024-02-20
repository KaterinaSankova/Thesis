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
    public class State
    {
        public InputMode Mode;
        public string? SourceFilePath;
        public string? SourceFolderPath;
        //generate
        public string? ResultFilePath;
        public string? ResultFolderPath;

        //output
        public string? OutputFolderPath;

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
    }
}
