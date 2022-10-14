using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Node
    {
        public int id;
        public double x;
        public double y;

        public Node(int id, double x, double y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
        }


        public override string ToString()
        {
            return $"{id}:[{x}, {y}]";
        }

        public override bool Equals(object obj)
        {
            /// if () idk, checknout aby objekt bl Node

            Node node = obj as Node;

            return node.x == x && node.y == y;
        }

        public List<Edge> OutgoingEdges(List<Edge> edges) //refactor 
        {
            return edges.Where(edge => edge.Contains(this)).ToList();
        }

        public List<Node> ConnectedNodes(List<Edge> edges)
        {
            return OutgoingEdges(edges).Select(edge => this == edge.node1 ? edge.node1 : edge.node2).ToList();
        }

        public bool IsOdd(List<Edge> edges) => OutgoingEdges(edges).Count % 2 == 1;
    }
}
