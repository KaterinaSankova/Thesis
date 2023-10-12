using System.Collections.Generic;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem
{
    public interface IPrims
    {
        List<Edge> FindSpanningTree(Graph graph);
    }
}