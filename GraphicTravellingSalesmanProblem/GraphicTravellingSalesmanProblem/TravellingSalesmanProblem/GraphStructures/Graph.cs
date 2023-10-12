using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravellingSalesmanProblem.Formats;
using static System.Net.Mime.MediaTypeNames;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph //add dimension property //osetrit grafy s 0 nebo 1 nodem
    {
        public List<Node> nodes;
        public int Size
        {
            get { return nodes.Count; }
        }

        public bool IsEmpty
        {
            get { return nodes.Count == 0; }
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
                    currDistance = currentEdge.Length();
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        minEdge = currentEdge;
                        // //Console.WriteLine($"{edge}: {minDistance}");
                    }
                }
            return minEdge;
        }

        public List<Node> OddDegreeNodes(List<Edge> edges) => nodes.Where(x => x.IsOdd(edges)).ToList();

        public List<Node> DepthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> visited = new List<Node>();
            Stack<Node> stack = new Stack<Node>();

            stack.Push(node);
            visited.Add(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();

                var connectedNodes = node.ConnectedNodes(edges);

                foreach (Node adjacentNode in connectedNodes)
                    if (!visited.Contains(adjacentNode))
                    {
                        stack.Push(adjacentNode);
                        visited.Add(adjacentNode);
                    }
            }

            return visited;
        }

        public double GetLength(List<Edge> edges) //property?
        {
            double length = 0;

            foreach (var edge in edges)
                length += edge.Length();

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

        public void MoveGraphInDirections(double x, double y) => nodes = nodes.Select((node) => node.Move(node.x + x, node.y + y)).ToList();

        public void Scale(double factor) => nodes = nodes.Select((node) => node.Move(node.x * factor, node.y * factor)).ToList();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var node in nodes)
                sb.Append(node + "\t");
            return sb.Append('\n').ToString();
        }
    }
}
