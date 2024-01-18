using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree
    {
        public List<Node> FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);

            List<Edge>  multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = Fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return path;
        }
    }
}
