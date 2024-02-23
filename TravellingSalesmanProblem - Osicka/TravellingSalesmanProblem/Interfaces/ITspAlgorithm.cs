using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Interfaces
{
    public interface ITspAlgorithm<T> where T: Path
    {
        public T FindShortestPath(Graph graph);
    }
}
