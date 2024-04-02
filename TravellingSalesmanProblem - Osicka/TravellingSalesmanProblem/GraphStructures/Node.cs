namespace TravellingSalesmanProblem.GraphStructures
{
    public class Node
    {
        public int Id;
        public double X;
        public double Y;

        public Node(int id, double x, double y)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
        }

        public List<Node> ConnectedNodes(List<Edge> edges)
        {
            List<Node> connectedNodes = new();
            foreach (var edge in edges)
            {
                if (edge.Node1 == this)
                    connectedNodes.Add(edge.Node2);
                if (edge.Node2 == this)
                    connectedNodes.Add(edge.Node1);
            }
            return connectedNodes;
        }

        public double Distance(Node node) => Math.Sqrt(Math.Pow(this.X - node.X, 2) + Math.Pow(this.Y - node.Y, 2));

        public override string ToString()
        {
            return $"{Id}:[{X}, {Y}]";
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
                return node.Id == Id;
            }
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
