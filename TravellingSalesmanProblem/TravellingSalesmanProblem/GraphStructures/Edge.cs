namespace TravellingSalesmanProblem.GraphStructures
{
    public class Edge
    {
        public Node node1;
        public Node node2;

        public Edge(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }
        
        //chce to konstruktor bez argumentů a pak přidat do metod check, jestli nejsou nody null

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

        public bool Contains(Node node) => node1.Equals(node) || node2.Equals(node);

        public double Length() => node1.Distance(node2); //property

        public bool IsBridge(Graph graph, List<Edge> edges)
        {
            var edgesWithoutSelf = edges.ToList();
            edgesWithoutSelf.Remove(this);

            return graph.DepthFirstSearch(edges, node1).Count != graph.DepthFirstSearch(edgesWithoutSelf, node1).Count;
        }

        public bool SharesNode(Edge edge) => edge.Contains(this.node1) || edge.Equals(this.node2);

        public Node GetOtherNode(Node node)
        {
            if (node1 == node) return node2;
            if (node2 == node) return node1;
            else throw new ArgumentException($"Edge {this} does not contain node {node}");
        }
    }
}
