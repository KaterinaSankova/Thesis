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

        public List<Edge> OutgoingEdges(List<Edge> edges) //refactor 
        {
            List<Edge> outgoingEdges = new();
            foreach (var edge in edges)
            {
                if (edge.node1 == this)
                    outgoingEdges.Add(edge);
                else if (edge.node2 == this)
                    outgoingEdges.Add(edge);
            }
            return outgoingEdges;
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
            //return $"{id}:[{x}, {y}]";
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
    }
}
