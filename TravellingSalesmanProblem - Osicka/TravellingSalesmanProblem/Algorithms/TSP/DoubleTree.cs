using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree : ITspAlgorithm<Path>
    {
        public Path FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);

            List<Edge>  multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = Fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return new Path(path);
        }
    }
}
