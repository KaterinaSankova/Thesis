using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
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
            return $"{{{node1}, {node2}}}";
        }
    }
}
