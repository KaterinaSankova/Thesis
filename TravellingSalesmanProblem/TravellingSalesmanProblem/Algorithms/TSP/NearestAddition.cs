using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class NearestAddition
    {
        public List<Node> FindShortestPath(Graph graph)
        {
            Edge firstEdge = graph.ShortestEdge();
            Edge shortestEdge;
            List<Node> path = new List<Node>();
            List<Node> remainingCities = graph.nodes.ToList();

            path.Add(firstEdge.node1);
            path.Add(firstEdge.node2);

            remainingCities.Remove(firstEdge.node1);
            remainingCities.Remove(firstEdge.node2);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(path, remainingCities);
                path.Insert(path.IndexOf(shortestEdge.node1) + 1, shortestEdge.node2);
                remainingCities.Remove(shortestEdge.node2);
            }
            return path;
        }
    }
}
