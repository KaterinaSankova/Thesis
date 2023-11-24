using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class Fleurys
    {
        public static List<Node> FindEulerCircuit(Graph graph, List<Edge> edges)
        {
            List<Node> result = new();

            if (graph.OddDegreeNodes(edges).Count != 0)
                throw new Exception("Euler circuit does not exist");

            List<Edge> unvisitedEdges = edges.ToList();

            Node currentNode = graph.nodes.First();

            List<Edge> outgoingEdges = currentNode.OutgoingEdges(unvisitedEdges);

            Edge currentEdge;

            while (outgoingEdges.Count > 0)
            {
                currentEdge = outgoingEdges.First();
                if (outgoingEdges.Count >= 1)
                    if (currentEdge.IsBridge(graph, unvisitedEdges))
                        currentEdge = outgoingEdges.Last();

                result.Add(currentNode);
                currentNode = currentEdge.node1 == currentNode ? currentEdge.node2 : currentEdge.node1;
                unvisitedEdges.Remove(currentEdge);

                outgoingEdges = currentNode.OutgoingEdges(unvisitedEdges);
            }

            return result;
        }
    }
}