namespace TravellingSalesmanProblem.GraphStructures
{
    public class Node
    {
        public int id;
        public double x;
        public double y;

        public Node(int id, double x, double y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
        }

        public List<Node> ConnectedNodes(List<Edge> edges)
        {
            List<Node> connectedNodes = new();
            foreach (var edge in edges)
            {
                if (edge.node1 == this)
                    connectedNodes.Add(edge.node2);
                if (edge.node2 == this)
                    connectedNodes.Add(edge.node1);
            }
            return connectedNodes;
        }

        public double Distance(Node node) => Math.Sqrt(Math.Pow(this.x - node.x, 2) + Math.Pow(this.y - node.y, 2));

        public override string ToString()
        {
            return $"{id}:[{x}, {y}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                Node node = (Node)obj;
                return node.id == id;
            }
        }

        public override int GetHashCode()
        {
            int hashId = id.GetHashCode();
            return hashId;
        }
    }
}
