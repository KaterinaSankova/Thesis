namespace TravellingSalesmanProblem.GraphStructures
{
    public class Edge
    {
        public Node node1;
        public Node node2;
        private double _length = -1;
        public double Length {
            get {
                if (_length == -1)
                    _length = node1.Distance(node2);
                return _length;
            }
        }

        public Edge(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public bool Contains(Node node) => node1.Equals(node) || node2.Equals(node);

        public bool SharesNode(Edge edge) => edge.Contains(this.node1) || edge.Equals(this.node2);

        public Node GetOtherNode(Node node)
        {
            if (node1 == node) return node2;
            if (node2 == node) return node1;
            else throw new ArgumentException($"Edge {this} does not contain node {node}");
        }

        public override string ToString() => $"({node1}, {node2})";

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Edge e = (Edge)obj;
                return (e.node1 == node1) && (e.node2 == node2) || (e.node1 == node2) && (e.node2 == node1);
            }
        }

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashNode1 = node1.GetHashCode();

            //Get hash code for the Code field.
            int hashNode2 = node2.GetHashCode();

            //Calculate the hash code for the product.
            return hashNode1 + hashNode2;
        }
    }
}
