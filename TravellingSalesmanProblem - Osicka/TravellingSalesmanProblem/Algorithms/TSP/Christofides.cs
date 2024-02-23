using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class Christofides : ITspAlgorithm<Path>
    {
        private readonly PerfectMatchingGreedyAlgorithm perfectMatchingAlgorithm = new();

        public Path FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);
            var oddDegreeNodes = graph.OddDegreeNodes(minimalSpanningTree);

            List<Edge> perfectMatching = PerfectMatching.FindMinimalPerfectMatching(new Graph(oddDegreeNodes));

            List<Node> path = Fleurys.FindEulerCircuit(graph, minimalSpanningTree.Concat(perfectMatching).ToList()).Distinct().ToList();

            path.Add(path[0]);

            return new Path(path);
        }
    }
}