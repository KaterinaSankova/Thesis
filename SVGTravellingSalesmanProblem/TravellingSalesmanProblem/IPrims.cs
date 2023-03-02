using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem
{
    public interface IPrims
    {
        List<Edge> FindSpanningTree(Graph graph);
    }
}