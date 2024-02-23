using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class NearestAddition : ITspAlgorithm<Path>
    {
        public Path FindShortestPath(Graph graph)
        {
            Edge firstEdge = graph.ShortestEdge();
            Edge shortestEdge;
            List<Node> path = new();
            List<Node> remainingCities = graph.nodes.ToList();

            path.Add(firstEdge.node1);
            path.Add(firstEdge.node2);
            path.Add(firstEdge.node1);

            remainingCities.Remove(firstEdge.node1);
            remainingCities.Remove(firstEdge.node2);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(path, remainingCities);
                path.Insert(path.IndexOf(shortestEdge.node1) + 1, shortestEdge.node2);
                remainingCities.Remove(shortestEdge.node2);
            }
            return new Path(path);
        }
    }
}
