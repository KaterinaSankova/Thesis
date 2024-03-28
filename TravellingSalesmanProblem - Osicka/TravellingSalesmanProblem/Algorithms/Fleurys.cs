using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class Fleurys
    {
        public static List<Node> FindEulerCircuit(Graph inputGraph, List<Edge> edges)
        {
            List<Node> result = new();
            var graph = new Graph(inputGraph.nodes, edges);

            if (graph.OddDegreeNodes(edges).Count != 0)
                throw new Exception("Euler circuit does not exist");

            List<Edge> unvisitedEdges = edges.ToList();

            Node? currentNode = graph.nodes.OrderBy(i => new Random().Next()).FirstOrDefault();

            if (currentNode == null)
                return new();

            List<Edge> outgoingEdges = graph.OutgoingEdges(currentNode);

            Edge currentEdge;

            while (outgoingEdges.Count > 0)
            {
                currentEdge = outgoingEdges.First();
                if (outgoingEdges.Count >= 1)
                    if (graph.IsBridge(currentEdge))
                        currentEdge = outgoingEdges.Last();

                result.Add(currentNode);
                currentNode = currentEdge.node1 == currentNode ? currentEdge.node2 : currentEdge.node1;
                graph.edges.Remove(currentEdge);

                outgoingEdges = graph.OutgoingEdges(currentNode);
            }

            return result;
        }
    }
}