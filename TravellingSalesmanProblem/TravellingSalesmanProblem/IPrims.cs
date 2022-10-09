namespace TravellingSalesmanProblem
{
    public interface IPrims
    {
        List<(Node, Node)> FindSpanningTree(List<Node> nodes);
    }
}