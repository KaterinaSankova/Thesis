using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TSP
{
    public class Result
    {
        public TSPAlgorithms Algorithm;
        public Graph Graph;
        public bool Stopwatch;
        public TimeSpan? Time;
        public Path? Path;
        public Path? ResultPath;

        private readonly NearestAddition nearestAddition = new();
        private readonly DoubleTree doubleTree = new();
        private readonly Christofides christofides = new();
        private readonly KernighanLin kernighanLin = new();
        private readonly Stopwatch sw = new();

        public Result(TSPAlgorithms algo, Graph graph,  bool stopwatch, Path? resultPath)
        {
            Algorithm = algo;
            Graph = graph;
            Stopwatch = stopwatch;
            ResultPath = resultPath;

            if (stopwatch)
                sw.Start();
            switch (algo)
            {
                case TSPAlgorithms.NearestAddition:
                    Path = nearestAddition.FindShortestPath(Graph);
                    break;
                case TSPAlgorithms.DoubleTree:
                    Path = doubleTree.FindShortestPath(Graph);
                    break;
                case TSPAlgorithms.Christofides:
                    Path = christofides.FindShortestPath(Graph);
                    break;
                case TSPAlgorithms.KernighanLin:
                    Path = kernighanLin.FindShortestPath(Graph);
                    break;
                default:
                    break;
            }
            if (stopwatch)
            {
                sw.Stop();
                Time = sw.Elapsed;
            }
        }
    }
}
