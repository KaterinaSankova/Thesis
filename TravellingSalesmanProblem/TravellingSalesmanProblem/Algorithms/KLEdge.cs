using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class KLEdge : Edge
    {
        KLEdgeState state;

        KLEdge(Node node1, Node node2) : base(node1, node2)
        {
            state = KLEdgeState.Normal;
        }
    }
}
