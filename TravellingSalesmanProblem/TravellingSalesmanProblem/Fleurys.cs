namespace TravellingSalesmanProblem
{
    public class Fleurys
    {
        public List<Node> FindEulerCircuit(List<Node> nodes, List<(Node, Node)> edges)
        {
            List<Node> result = new List<Node>();

            if (new Graph(nodes).OddDegreeNodes(nodes, edges).Count != 0)
                throw new Exception("Euler circuit does not exist");

            List<(Node, Node)> unvisitedEdges = edges.ToList();

            Node node = unvisitedEdges.First().Item1;

            List<(Node, Node)> outgoingEdges = node.OutgoingEdges(unvisitedEdges);

            (Node, Node) currentEdge;

            while (outgoingEdges.Count > 0)
            {
                if (outgoingEdges.Count == 1)
                {
                    currentEdge = outgoingEdges.First();

                    result.Add(node);
                    node = currentEdge.Item1 == node ? currentEdge.Item2 : currentEdge.Item1;
                    unvisitedEdges.Remove(outgoingEdges.First());
                }
                else
                {
                    currentEdge = outgoingEdges.First();
                    if (currentEdge.IsBridge)
                        currentEdge = outgoingEdges.Last();

                    result.Add(node);
                    node = currentEdge.Item1 == node ? currentEdge.Item2 : currentEdge.Item1;
                    unvisitedEdges.Remove(outgoingEdges.First());
                }
            }

            return result;
        }
    }
}