using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Models;

namespace TSP.ViewModels
{
    public class AlgorithmResultViewModel : ViewModelBase
    {
        private readonly AlgorithmResultModel _result;

        public AlgorithmResultViewModel(AlgorithmResultModel result)
        {
            _result = result;
        }

        public string Name => _result.Name;
        public string Algorithm
        {
            get
            {
                switch (_result.Algorithm)
                {
                    case TSPAlgorithms.NearestAddition:
                        return "NA";
                    case TSPAlgorithms.DoubleTree:
                        return "DT";
                    case TSPAlgorithms.Christofides:
                        return "C";
                    case TSPAlgorithms.KernighanLin:
                        return "KL";
                    default:
                        return "UNK";
                }
            }
        }
        public double Length => Math.Round(_result.Path.Length, 2);
        public string Ratio
        {
            get
            {
                if (_result.ResultPath != null)
                    return Math.Round(_result.Path.Length / _result.ResultPath.Length, 2).ToString();
                else
                    return "UNK";
            }
        }
        public string Duration
        {
            get
            {
                if (_result.Stopwatch)
                {
                    var ts = (TimeSpan)_result.Duration;
                    return $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms";
                }
                else
                {
                    return "UNK";
                }
            }
        }
        public bool Stopwatch => _result.Stopwatch;

        public Graph Graph => _result.Graph;

        public Path Path => _result.Path;
    }
}
