using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class Christofides : ITspAlgorithm
    {
        public Path FindShortestPath(Graph graph)
        {
            if (graph.IsEmpty)
                return new Path();
            if (graph.Size == 1)
                return new Path(new List<Node>() { graph.nodes.First() });

            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);
            var oddDegreeNodes = graph.OddDegreeNodes(minimalSpanningTree);

            List<Edge> perfectMatching = PerfectMatching.FindMinimalPerfectMatching(new Graph(oddDegreeNodes));

            List<Node> path = Fleurys.FindEulerCircuit(graph, minimalSpanningTree.Concat(perfectMatching).ToList()).Distinct().ToList();

            return new Path(path);
        }
    }
}