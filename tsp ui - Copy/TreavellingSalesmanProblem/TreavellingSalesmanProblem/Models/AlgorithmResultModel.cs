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
        public TSPAlgorithms Algorithm { get; set; }
        public Graph Graph { get; set; }
        public bool Stopwatch { get; set; }
        public TimeSpan? Duration { get; set; }
        public Path Path { get; set; }
        public Path? ResultPath { get; set; }

        public AlgorithmResultModel(string name, TSPAlgorithms algo, Graph graph, Path path, bool stopwatch, TimeSpan? time = null, Path? resultPath = null)
        {
            Name = name;
            Algorithm = algo;
            Graph = graph;
            Stopwatch = stopwatch;
            Duration = time;
            Path = path;
            ResultPath = resultPath;
        }
    }
}
