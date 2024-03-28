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
                    case TSPAlgorithms.KernighanLinRb:
                        return "KLrb";
                    default:
                        return "UNK";
                }
            }
        }
        public double BestLength => Math.Round(_result.BestPathLength, 2);
        public double AverageLength => Math.Round(_result.AveragePathLength, 2);

        public string Ratio
        {
            get
            {
                if (_result.ResultPath != null)
                    return Math.Round(_result.AveragePathLength / _result.ResultPath.Length, 2).ToString();
                else
                    return "UNK";
            }
        }

        public string AverageDuration
        {
            get
            {
                if (_result.Stopwatch)
                {
                    var ts = (TimeSpan)_result.AverageTime;
                    return $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds}ms {ts.Microseconds}μs {ts.Nanoseconds}ns";
                }
                else
                {
                    return "UNK";
                }
            }
        }
        public string BestDuration
        {
            get
            {
                if (_result.Stopwatch)
                {
                    var ts = (TimeSpan)_result.BestTime;
                    return $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds}ms {ts.Microseconds}μs {ts.Nanoseconds}ns";
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
