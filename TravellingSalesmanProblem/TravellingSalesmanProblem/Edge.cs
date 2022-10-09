using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
{
    public static class Edge
    {
        public static double Distance(Node node1, Node node2) => Math.Sqrt(Math.Pow(node2.x - node1.x, 2) + Math.Pow(node2.y - node1.y, 2));

        public bool IsBridge() { }
    }
}
