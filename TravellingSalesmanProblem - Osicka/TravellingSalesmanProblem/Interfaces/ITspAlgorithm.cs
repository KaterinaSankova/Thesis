using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Interfaces
{
    public interface ITspAlgorithm
    {
        public Path FindShortestPath(Graph graph);
    }
}
