namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph
    {
        public readonly List<Node> nodes;
        public List<Edge>? edges;

        public int Size => nodes.Count;

        public bool IsEmpty => nodes.Count == 0;
        
        public Graph()
        {
            nodes = new();
        }

        public Graph(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public Graph(List<Node> nodes, List<Edge> edges)
        {
            this.nodes = nodes;
            this.edges = edges;
        }

        public Edge ShortestEdge(List<Node>? fromNodes = null, List<Node>? toNodes = null)
        {
            fromNodes ??= nodes;
            toNodes ??= nodes;
            if (fromNodes.Count == 0 || toNodes.Count == 0)
                throw new Exception("Cannot search for shortest path when no fromNodes or toNodes are provided.");

            double minDistance = double.MaxValue;
            double currDistance;
            Edge minEdge = new (fromNodes.First(), toNodes.First());

            foreach (Node fromNode in fromNodes)
            {
                foreach (Node toNode in toNodes)
                {
                    currDistance = fromNode.Distance(toNode);
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        minEdge = new(fromNode, toNode);
                    }
                }
            }

            return minEdge;
        }

        public List<Node> OddDegreeNodes(List<Edge>? edges = null)
        {
            if (edges == null)
            {
                if (this.edges != null)
                    edges = this.edges;
                else
                    return new();
            }

            Dictionary<int, int> nodeDegres = new();

            foreach (Edge edge in edges)
            {
                if (!nodeDegres.TryGetValue(edge.Node1.Id, out int degree))
                    nodeDegres.Add(edge.Node1.Id, 1);
                else
                    nodeDegres[edge.Node1.Id]++;

                if (!nodeDegres.TryGetValue(edge.Node2.Id, out degree))
                    nodeDegres.Add(edge.Node2.Id, 1);
                else
                    nodeDegres[edge.Node2.Id]++;
            }

            return nodes.Where(n => nodeDegres[n.Id] % 2 == 1) .ToList();
        }

        public List<Edge> OutgoingEdges(Node node, List<Edge>? edges = null)
        {
            if (edges == null)
            {
                if (this.edges != null)
                    edges = this.edges;
                else
                    return new();
            }

            List<Edge> outgoingEdges = new();
            foreach (var edge in edges)
            {
                if (edge.Node1 == node)
                    outgoingEdges.Add(edge);
                else if (edge.Node2 == node)
                    outgoingEdges.Add(edge);
            }
            return outgoingEdges;
        }

        public bool IsBridge(Edge edge, List<Edge>? edges = null)
        {
            if (edges == null)
            {
                if (this.edges != null)
                    edges = this.edges;
                else
                    return new();
            }

            var edgesWithoutSelf = edges.ToList();
            edgesWithoutSelf.Remove(edge);

            return DepthFirstSearch(edge.Node1, edges).Count != DepthFirstSearch(edge.Node1, edgesWithoutSelf).Count;
        }

        public List<Node> DepthFirstSearch(Node node, List<Edge>? edges = null)
        {
            if (edges == null)
            {
                if (this.edges != null)
                    edges = this.edges;
                else
                    return new();
            }
            List<Node> visited = new();
            Stack<Node> stack = new();

            stack.Push(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();

                if(!visited.Contains(node))
                {
                    visited.Add(node);

                    var connectedNodes = node.ConnectedNodes(edges);
                    foreach (Node adjacentNode in connectedNodes)
                        if (!visited.Contains(adjacentNode))
                            stack.Push(adjacentNode);
                }

            }

            return visited;
        }

        public (double MaxX, double MaxY, double MinX, double MinY) GetExtremeCoordinatesValues()
        {
            double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;

            foreach (Node node in nodes)
            {
                if (node.X > maxX) maxX = node.X;
                if (node.X < minX) minX = node.X;

                if (node.Y > maxY) maxY = node.Y;
                if (node.Y < minY) minY = node.Y;
            }

            return (maxX, maxY, minX, minY);
        }
    }
}
