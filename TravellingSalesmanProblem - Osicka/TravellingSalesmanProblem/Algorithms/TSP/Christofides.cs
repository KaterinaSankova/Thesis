using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class Christofides
    {
        private readonly PerfectMatchingGreedyAlgorithm perfectMatchingAlgorithm = new();

        public List<Node> FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);
            var oddDegreeNodes = graph.OddDegreeNodes(minimalSpanningTree);

            List<Edge> perfectMatching = PerfectMatching.FindMinimalPerfectMatching(new Graph(oddDegreeNodes));

            List<Node> path = Fleurys.FindEulerCircuit(graph, minimalSpanningTree.Concat(perfectMatching).ToList()).Distinct().ToList();

            path.Add(path[0]);

            return path;
        }
    }
}