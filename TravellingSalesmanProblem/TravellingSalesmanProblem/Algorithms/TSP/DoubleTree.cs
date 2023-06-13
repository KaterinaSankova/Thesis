using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree
    {
        private readonly IPrims prims = new Prims();

        public List<Node> FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = prims.FindSpanningTree(graph);

            List<Edge>  multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = Fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return path;
        }
    }
}
