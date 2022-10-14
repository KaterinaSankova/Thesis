using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class Christofides
    {
        private readonly IPrims prims = new Prims();

        public List<Node> FindShortestPath(Graph graph)
        {
            Console.WriteLine("***ALL NODES***");

            for (int i = 0; i < graph.Size; i++)
                Console.WriteLine($"{graph.nodes[i]}\t");

            List<Node> path = new List<Node>();

            List<Edge> minimalSpanningTree = prims.FindSpanningTree(graph);

            Console.WriteLine("***SPANNING TREE***");

            for (int i = 0; i < minimalSpanningTree.Count; i++)
                Console.WriteLine($"{minimalSpanningTree[i]}\t");

            List<Node> oddDegreeNodes = graph.OddDegreeNodes(minimalSpanningTree);

            return oddDegreeNodes;
        }
    }
}