using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KeringhanLinReducedBacktracking : ITspAlgorithm
    {
        private readonly KernighanLin kernighanLin = new(true);

        public Path FindShortestPath(Graph graph) => kernighanLin.FindShortestPath(graph);
    }
}