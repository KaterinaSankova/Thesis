using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public static class Prims
    {
        public static List<Edge> FindSpanningTree(Graph graph) 
        {
            Edge? firstEdge = graph.ShortestEdge();
            if (firstEdge == null)
                return new();

            Edge shortestEdge;
            List<Node> includedCities = new();
            List<Node> remainingCities = graph.nodes.ToList();

            List<Edge> path = new();

            includedCities.Add(firstEdge.Node1);
            includedCities.Add(firstEdge.Node2);

            remainingCities.Remove(firstEdge.Node1);
            remainingCities.Remove(firstEdge.Node2);

            path.Add(firstEdge);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(includedCities, remainingCities);
                path.Add(shortestEdge);
                includedCities.Add(shortestEdge.Node2);
                remainingCities.Remove(shortestEdge.Node2);
            }
            return path;
        }
    }
}
