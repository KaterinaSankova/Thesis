using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class NearestAddition : ITspAlgorithm
    {
        public Path FindShortestPath(Graph graph)
        {
            if (graph.IsEmpty)
                return new Path();
            if (graph.Size == 1)
                return new Path(new List<Node>() { graph.nodes.First() });

            Edge? firstEdge = graph.ShortestEdge();
            if (firstEdge == null)
                return new Path();

            Edge shortestEdge;
            List<Node> path = new();
            List<Node> remainingCities = graph.nodes.ToList();

            path.Add(firstEdge.Node1);
            path.Add(firstEdge.Node2);
            path.Add(firstEdge.Node1);

            remainingCities.Remove(firstEdge.Node1);
            remainingCities.Remove(firstEdge.Node2);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(path, remainingCities);
                path.Insert(path.IndexOf(shortestEdge.Node1) + 1, shortestEdge.Node2);
                remainingCities.Remove(shortestEdge.Node2);
            }

            return new Path(path.SkipLast(1).ToList());
        }
    }
}
