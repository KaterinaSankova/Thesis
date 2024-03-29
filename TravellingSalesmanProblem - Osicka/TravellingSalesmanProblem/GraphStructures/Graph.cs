using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using TravellingSalesmanProblem.Algorithms;
using TravellingSalesmanProblem.Formats;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph //osetrit grafy s 0 nebo 1 nodem
    {
        public readonly List<Node> nodes;
        public List<Edge>? edges;

        public int Size => nodes.Count;

        public bool IsEmpty => nodes.Count == 0;
        
        public Graph()
        {
            nodes = new();
        }

        public Graph(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public Graph(List<Node> nodes, List<Edge> edges)
        {
            this.nodes = nodes;
            this.edges = edges;
        }

        public Edge? ShortestEdge(List<Node>? fromNodes = null, List<Node>? toNodes = null)
        {
            fromNodes ??= nodes;
            toNodes ??= nodes;
            if (fromNodes.Count == 0 || toNodes.Count == 0)
                return null;

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

        public List<Node> OddDegreeNodes(List<Edge> edges = null)
        {
            if (edges == null)
                edges = this.edges;

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

        public List<Edge> OutgoingEdges(Node node)
        {
            List<Edge> outgoingEdges = new();
            foreach (var edge in edges)
            {
                if (edge.node1 == node)
                    outgoingEdges.Add(edge);
                else if (edge.node2 == node)
                    outgoingEdges.Add(edge);
            }
            return outgoingEdges;
        }

        public bool IsBridge(Edge edge)
        {
            var edgesWithoutSelf = edges.ToList();
            edgesWithoutSelf.Remove(edge);

            return Graph.DepthFirstSearch(edges, edge.node1).Count != Graph.DepthFirstSearch(edgesWithoutSelf, edge.node1).Count;
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

        public (double MaxX, double MaxY, double MinX, double MinY) GetExtremeCoordinatesValues()
        {
            double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;

            foreach (Node node in nodes)
            {
                if (node.x > maxX) maxX = node.x;
                if (node.x < minX) minX = node.x;

                if (node.y > maxY) maxY = node.y;
                if (node.y < minY) minY = node.y;
            }

            return (maxX, maxY, minX, minY);

        }
    }
}
