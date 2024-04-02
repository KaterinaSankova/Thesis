using System;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Models;

namespace TSP.ViewModels
{
    public class AlgorithmResultViewModel : ViewModelBase
    {
        private readonly AlgorithmResultModel result;

        public AlgorithmResultViewModel(AlgorithmResultModel result)
        {
            this.result = result;
        }

        public string Name => result.Name;

        public string Algorithm => TSPAlgorithmExtentions.ToAbbreviationString(result.Algorithm);

        public double BestLength => Math.Round(result.BestPathLength, 2);

        public double AverageLength => Math.Round(result.AveragePathLength, 2);

        public string Ratio => result.ResultPath != null ? Math.Round(result.AveragePathLength / result.ResultPath.Length, 2).ToString() : "UNK";

        public string AverageDuration
        {
            get
            {
                if (result.Stopwatch && result.AverageTime != null)
                {
                    var ts = (TimeSpan)result.AverageTime;
                    string timeString = "";
                    if (ts.Hours > 0)
                        timeString += $"{ts.Hours}h ";
                    if (ts.Minutes > 0 || ts.Hours > 0)
                        timeString += $"{ts.Minutes}m ";
                    if (ts.Seconds > 0 || ts.Minutes > 0 || ts.Hours > 0)
                        timeString += $"{ts.Seconds}s ";
                    if (ts.Hours == 0)
                        timeString += $"{Math.Round((double)ts.Milliseconds + (double)ts.Microseconds / 1000, 2)}ms";

                    return timeString;
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
                if (result.Stopwatch && result.BestTime != null)
                {
                    var ts = (TimeSpan)result.BestTime;
                    string timeString = "";
                    if (ts.Hours > 0)
                        timeString += $"{ts.Hours}h ";
                    if (ts.Minutes > 0 || ts.Hours > 0)
                        timeString += $"{ts.Minutes}m ";
                    if (ts.Seconds > 0 || ts.Minutes > 0 || ts.Hours > 0)
                        timeString += $"{ts.Seconds}s ";
                    if (ts.Hours == 0)
                        timeString += $"{Math.Round((double)ts.Milliseconds + (double)ts.Microseconds / 1000, 2)}ms";
                    return timeString;
                }
                else
                {
                    return "UNK";
                }
            }
        }

        public bool Stopwatch => result.Stopwatch;

        public Graph? Graph => result.Graph;

        public Path? Path => result.Path;
    }
}
