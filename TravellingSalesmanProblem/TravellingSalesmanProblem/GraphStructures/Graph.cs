using System.Collections.Generic;
using System.Drawing;
using TravellingSalesmanProblem.Algorithms;
using TravellingSalesmanProblem.Formats;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph //add dimension property //osetrit grafy s 0 nebo 1 nodem
    {
        public readonly List<Node> nodes;
        public List<Edge>? edges;

        public int Size => nodes.Count;

        public bool IsEmpty => nodes.Count == 0;

        public Graph(List<Node> input)
        {
            nodes = input;
        }

        public Graph(string path)
        {
            nodes = new TSPDeserializer(path).DeserializeNodes();
        }

        public Edge ShortestEdge(List<Node>? fromNodes = null, List<Node>? toNodes = null)
        {
            fromNodes ??= nodes;
            toNodes ??= nodes;

            double minDistance = double.MaxValue;
            double currDistance;
            Edge minEdge = new (fromNodes.First(), toNodes.First());

            foreach (Node fromNode in fromNodes)
            {
                foreach (Node toNode in toNodes)
                {
                    currDistance = fromNode.Distance(toNode);
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        minEdge = new(fromNode, toNode);
                    }
                }
            }

            return minEdge;
        }

        public List<Node> OddDegreeNodes(List<Edge> edges)
        {
            Dictionary<int, int> nodeDegres = new();
            int degree;

            foreach (Edge edge in edges)
            {
                if (!nodeDegres.TryGetValue(edge.node1.id, out degree))
                    nodeDegres.Add(edge.node1.id, 1);
                else
                    nodeDegres[edge.node1.id]++;

                if (!nodeDegres.TryGetValue(edge.node2.id, out degree))
                    nodeDegres.Add(edge.node2.id, 1);
                else
                    nodeDegres[edge.node2.id]++;
            }

            return nodes.Where(n => nodeDegres[n.id] % 2 == 1) .ToList();
        }

        public static List<Node> DepthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> visited = new();
            Stack<Node> stack = new();

            stack.Push(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();

                if(!visited.Contains(node))
                {
                    visited.Add(node);

                    var connectedNodes = node.ConnectedNodes(edges);
                    foreach (Node adjacentNode in connectedNodes)
                        if (!visited.Contains(adjacentNode))
                            stack.Push(adjacentNode);
                }

            }

            return visited;
        }

        public static List<Node> BreadthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> visited = new List<Node>();
            Queue<Node> queue = new Queue<Node>();

            queue.Enqueue(node);
            visited.Add(node);

            while (queue.Count > 0)
            {
                node = queue.Dequeue();

                var connectedNodes = node.ConnectedNodes(edges);

                foreach (Node adjacentNode in connectedNodes)
                    if (!visited.Contains(adjacentNode))
                    {
                        visited.Add(adjacentNode);
                        queue.Enqueue(adjacentNode);
                    }
            }

            return visited;
        }

        public double GetLength()
        {
            double length = 0;

            for (int i = 0; i < nodes.Count - 1; i++)
                for (int j = 1; j < nodes.Count; j++)
                    length += nodes[i].Distance(nodes[j]);

            return length;
        }
    }
}
