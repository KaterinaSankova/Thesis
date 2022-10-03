using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem
{
    public class Prims : AlgorithmBase
    {
        public List<(Node, Node)> FindSpanningTree(List<Node> nodes)
        {
            (Node, Node) firstPair = FindShortestEdge(nodes, nodes);
            (Node, Node) shortestEdge;
            List<Node> includedCities = new List<Node>();
            List<Node> remainingCities = nodes;
            List<Tuple<Node, Node>> path = new List<Tuple<Node, Node>>;

            includedCities.Add(firstPair.Item1);
            includedCities.Add(firstPair.Item2);

            remainingCities.Remove(firstPair.Item1);
            remainingCities.Remove(firstPair.Item2);

            path.Add(firstPair);

            while (remainingCities.Count > 0)
            {
                shortestEdge = FindShortestEdge(includedCities, remainingCities);
                path.Add(shortestEdge);
                includedCities.Add(shortestEdge.Item2);
                remainingCities.Remove(shortestEdge.Item2);
            }
            return path;
        }
    }
}
