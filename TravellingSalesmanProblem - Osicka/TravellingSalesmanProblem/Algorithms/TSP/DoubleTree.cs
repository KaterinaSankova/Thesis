using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree : ITspAlgorithm
    {
        public Path FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);

            List<Edge> multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = Fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            return new Path(path);
        }
    }
}
