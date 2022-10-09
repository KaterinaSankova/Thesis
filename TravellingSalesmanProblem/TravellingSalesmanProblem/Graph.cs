namespace TravellingSalesmanProblem
{
    public class Graph
    {
        public List<Node> nodes;

        public Graph(List<Node> input)
        {
            nodes = input;
        }

        public (Node, Node) ShortestEdge(List<Node> fromNode = nodes, List<Node> toNodes = nodes) //hledam jako debil //node s- null + overeni
        {
            double minDistance = double.MaxValue;
            (Node, Node) edge = (fromNodes.First(), toNodes.First());
            double currDistance;

            foreach (Node fromNode in fromNodes)
                foreach (Node toNode in toNodes)
                {
                    currDistance = Edge.Distance(fromNode, toNode);
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        edge = (fromNode, toNode);
                        // Console.WriteLine($"{edge}: {minDistance}");
                    }
                }
            return edge;
        }

        public List<Node> OddDegreeNodes(List<Node> nodes, List<(Node, Node)> edges) => nodes.Where(x => x.IsOdd(edges)).ToList();
    }
}
