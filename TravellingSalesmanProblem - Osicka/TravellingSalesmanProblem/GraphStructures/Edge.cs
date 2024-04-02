namespace TravellingSalesmanProblem.GraphStructures
{
    public class Edge
    {
        public Node Node1;
        public Node Node2;

        private double _length = -1;
        public double Length {
            get {
                if (_length == -1)
                    _length = Node1.Distance(Node2);
                return _length;
            }
        }

        public Edge(Node node1, Node node2)
        {
            this.Node1 = node1;
            this.Node2 = node2;
        }

        public bool Contains(Node node) => Node1.Equals(node) || Node2.Equals(node);

        public bool SharesNode(Edge edge) => edge.Contains(this.Node1) || edge.Equals(this.Node2);

        public Node GetOtherNode(Node node)
        {
            if (Node1 == node) return Node2;
            if (Node2 == node) return Node1;
            else throw new ArgumentException($"Edge {this} does not contain node {node}");
        }

        public override string ToString() => $"({Node1}, {Node2})";

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Edge e = (Edge)obj;
                return (e.Node1 == Node1) && (e.Node2 == Node2) || (e.Node1 == Node2) && (e.Node2 == Node1);
            }
        }

        public override int GetHashCode() =>  Node1.GetHashCode() + Node2.GetHashCode();
    }
}
