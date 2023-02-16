using System.Collections.Generic;
using System.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class Prims : IPrims
    {
        public List<Edge> FindSpanningTree(Graph graph)
        {
            Edge firstEdge = graph.ShortestEdge();
            Edge shortestEdge;
            List<Node> includedCities = new List<Node>();
            List<Node> remainingCities = graph.nodes.ToList();
            List<Edge> path = new List<Edge> { };

            includedCities.Add(firstEdge.node1);
            includedCities.Add(firstEdge.node2);

            remainingCities.Remove(firstEdge.node1);
            remainingCities.Remove(firstEdge.node2);

            path.Add(firstEdge);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(includedCities, remainingCities);
                path.Add(shortestEdge);
                includedCities.Add(shortestEdge.node2);
                remainingCities.Remove(shortestEdge.node2);
            }
            return path;
        }
    }
}
