using System.Collections.Generic;
using TravellingSalesmanProblem.Formats;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph //add dimension property //osetrit grafy s 0 nebo 1 nodem
    {
        public readonly List<Node> nodes; 
        public int Size
        {
            get { return nodes.Count; }
        }

        public Graph(List<Node> input) //bude potřeba?
        {
            nodes = input;
        }

        public Graph(string path)
        {
            nodes = new TSPDeserializer(path).DeserializeNodes();
        }

        public Edge ShortestEdge(List<Node> fromNodes = null, List<Node> toNodes = null) //hledam jako debil //node s- null + overeni
        {
            if (fromNodes == null)
                fromNodes = nodes;
            if (toNodes == null)
                toNodes = nodes;

            double minDistance = double.MaxValue;
            double currDistance;
            Edge minEdge = new Edge(fromNodes.First(), toNodes.First());
            Edge currentEdge;

            foreach (Node fromNode in fromNodes)
                foreach (Node toNode in toNodes)
                {
                    currentEdge = new Edge(fromNode, toNode);
                    currDistance = currentEdge.Distance();
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        minEdge = currentEdge;
                        // Console.WriteLine($"{edge}: {minDistance}");
                    }
                }
            return minEdge;
        }

        public List<Node> OddDegreeNodes(List<Edge> edges) => nodes.Where(x => x.IsOdd(edges)).ToList();

        public List<Node> DepthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> sequence = new List<Node>();
            List<Node> visited = new List<Node>();
            Stack<Node> stack = new Stack<Node>();

            stack.Push(node);
            while (stack.Count > 0)
            {
                node = stack.Pop();

                if (!visited.Contains(node))
                {
                    sequence.Add(node);
                    visited.Add(node);
                }

                foreach (Node adjacentNode in node.ConnectedNodes(edges))
                    if (!visited.Contains(adjacentNode))
                        visited.Add(adjacentNode);
            }

            return sequence;
        }

        public double GetLength(List<Edge> edges) //property?
        {
            double length = 0;

            foreach (var edge in edges)
                length += edge.Distance();

            return length;
        }

        public double GetLength()
        {
            double length = 0;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                for (int j = 1; j < nodes.Count; j++)
                {
                    length += nodes[i].Distance(nodes[j]);
                }
            }

            return length;
        }
    }
}
