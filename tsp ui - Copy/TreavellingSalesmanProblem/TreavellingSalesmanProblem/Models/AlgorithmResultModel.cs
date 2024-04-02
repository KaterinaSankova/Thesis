using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.GraphStructures;

namespace TSP.Models
{
    public class AlgorithmResultModel
    {
        public string Name { get; set; }

        public TSPAlgorithm Algorithm { get; set; }

        public Graph? Graph { get; set; }

        public Path? Path { get; set; }

        public bool Stopwatch { get; set; }

        public double BestPathLength { get; set; }

        public double AveragePathLength { get; set; }

        public TimeSpan? AverageTime { get; set; }

        public TimeSpan? BestTime { get; set; }

        public Path? ResultPath { get; set; }


        public AlgorithmResultModel(string name, TSPAlgorithm algo, bool stopwatch, double bestPathLength, double averagePathLength, Graph? graph = null, Path? path = null, TimeSpan? averageTime = null, TimeSpan? bestTime = null, Path? resultPath = null)
        {
            Name = name;
            Algorithm = algo;
            Graph = graph;
            Path = path;
            Stopwatch = stopwatch;
            BestPathLength = bestPathLength;
            AveragePathLength = averagePathLength;
            AverageTime = averageTime;
            BestTime = bestTime;
            ResultPath = resultPath;
        }
    }
}
