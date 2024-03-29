using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree : ITspAlgorithm
    {
        public Path FindShortestPath(Graph graph)
        {
            if (graph.IsEmpty)
                return new Path();
            if (graph.Size == 1)
                return new Path(new List<Node>() { graph.nodes.First() });

            List<Node> path = new();

            List<Edge> minimalSpanningTree = Prims.FindSpanningTree(graph);

            List<Edge> multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList();


            Node currNode = graph.nodes[new Random().Next(graph.nodes.Count)];
            Node? nextNode = null;
            List<Node> outgoingNodes = currNode.ConnectedNodes(multiGraph);
            int index = 0;
            while (outgoingNodes.Count > 0)
            {
                while (nextNode == null && index < outgoingNodes.Count)
                {
                    if (outgoingNodes.Count(n => n == outgoingNodes[index]) == 1)
                    {
                        index++;
                        continue;
                    }
                    else
                    {
                        nextNode = outgoingNodes[index];
                        index++;
                    }
                }
                if (nextNode == null)
                {
                    nextNode = outgoingNodes.First();
                }
                path.Add(currNode);
                multiGraph.Remove(new Edge(currNode, nextNode));
                currNode = nextNode;
                outgoingNodes = currNode.ConnectedNodes(multiGraph);
                index = 0;
                nextNode = null;
            }

            path = path.Distinct().ToList(); //shortcutting

            return new Path(path);
        }
    }
}
