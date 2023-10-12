namespace SVGTravellingSalesmanProblem.GraphStructures
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


        public override string ToString()
        {
           // return $"{id}:[{x}, {y}]";
            return $"{id}";
        }

        public override bool Equals(object? obj)
        {
            /// if () idk, checknout aby objekt bl Node

            Node node = obj as Node;

            return node.id == id;
        }

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashId = id.GetHashCode();


            //Calculate the hash code for the product.
            return hashId;
        }

        public List<Edge> OutgoingEdges(List<Edge> edges) //refactor 
        {
            return edges.Where((edge) => edge.Contains(this)).ToList();
        }

        public List<Node> ConnectedNodes(List<Edge> edges)
        {
            return OutgoingEdges(edges).Select((edge) => this == edge.node1 ? edge.node2 : edge.node1).ToList();
        }

        public bool IsOdd(List<Edge> edges) => OutgoingEdges(edges).Count % 2 == 1;


        public double Distance(Node node) => Math.Sqrt(Math.Pow(this.x - node.x, 2) + Math.Pow(this.y - node.y, 2));

        public Node Move(double x, double y)
        {
            this.x = x;
            this.y = y;

            return this;
        }
    }
}
