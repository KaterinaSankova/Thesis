using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
{
    public class Node
    {
        public double x;
        public double y;

        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"[{x}, {y}]";
        }

        public override bool Equals(object obj)
        {
            /// if () idk, checknout aby objekt bl Node

            Node node = obj as Node;

            return node.x == x && node.y == y;
        }

        public List<(Node, Node)> OutgoingEdges(List<(Node, Node)> edges) //refactor
        {
            return edges.Where(edge => edge.Item1.Equals(x) || edge.Item2.Equals(x)).ToList();
        }

        public bool IsOdd(List<(Node, Node)> edges) => OutgoingEdges(edges).Count % 2 == 1;
    }
}
