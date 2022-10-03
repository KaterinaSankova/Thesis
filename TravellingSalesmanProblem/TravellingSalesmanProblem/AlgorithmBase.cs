using System;

namespace TravellingSalesmanProblem
{
	public class AlgorithmBase
	{
		public AlgorithmBase()
        {
        }

        public (Node, Node) FindShortestEdge(List<Node> fromNodes, List<Node> toNodes)
        {
            double minDistance = double.MaxValue;
            (Node, Node) edge = (fromNodes.First(), toNodes.First());
            double currDistance;

            foreach (Node fromNode in fromNodes)
                foreach (Node toNode in toNodes)
                {
                    currDistance = Edge.Distance(fromNode, toNode);
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        edge = (fromNode, toNode);
                        Console.WriteLine($"{edge}: {minDistance}");
                    }
                }
            return edge;
        }
    }
}