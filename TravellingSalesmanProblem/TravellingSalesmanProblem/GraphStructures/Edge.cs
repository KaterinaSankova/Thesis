using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Edge
    {
        public Node node1;
        public Node node2;

        public Edge(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public override string ToString()
        {
            return $"({node1}, {node2})";
        }

        public bool Contains(Node node) => node1.Equals(node) || node2.Equals(node);

        public double Distance() => Math.Sqrt(Math.Pow(node2.x - node1.x, 2) + Math.Pow(node2.y - node1.y, 2));

        public bool IsBridge(Graph graph, List<Edge> edges) => graph.DepthFirstSearch(edges, node1) == graph.DepthFirstSearch(edges.Where(edge => edge != this).ToList(), node1);
    }
}
