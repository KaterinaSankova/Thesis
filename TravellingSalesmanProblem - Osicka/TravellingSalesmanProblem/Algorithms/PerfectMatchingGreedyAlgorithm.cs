using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class PerfectMatchingGreedyAlgorithm
    {
        public List<Edge> FindPerfectMatching(Graph graph)
        {
            List<Edge> perfectMatching = new();

            Edge shortestEdge;

            while (!graph.IsEmpty)
            {
                shortestEdge = graph.ShortestEdge();
                perfectMatching.Add(shortestEdge);
                graph.nodes.Remove(shortestEdge.node1);
                graph.nodes.Remove(shortestEdge.node2);
            }

            return perfectMatching;
        }
    }
}
