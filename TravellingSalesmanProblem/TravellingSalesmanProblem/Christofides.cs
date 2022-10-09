namespace TravellingSalesmanProblem
{
    public class Christofides
    {
        private IPrims prims = new Prims();

        public List<Node> FindShortestPath(List<Node> nodes)
        {
            //Console.WriteLine("***ALL NODES***");

            //for (int i = 0; i < nodes.Count; i++)
            //    Console.WriteLine($"{nodes[i]}\t");

            List<Node> path = new List<Node>();

            List<(Node, Node)> minimalSpanningTree = prims.FindSpanningTree(nodes);

            //Console.WriteLine("***SPANNING TREE***");

            //for (int i = 0; i < minimalSpanningTree.Count; i++)
            //    Console.WriteLine($"{minimalSpanningTree[i]}\t");

            List<Node> oddDegreeNodes = nodes.Where(x => minimalSpanningTree.Where(edge => edge.Item1.Equals(x) || edge.Item2.Equals(x)).ToList().Count() % 2 == 1).ToList();

            return oddDegreeNodes;
        }
    }
}