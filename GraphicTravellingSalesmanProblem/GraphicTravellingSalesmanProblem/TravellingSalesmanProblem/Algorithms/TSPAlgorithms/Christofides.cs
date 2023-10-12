using System.Collections.Generic;
using System.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSPAlgorithms
{
    public class Christofides
    {
        private readonly IPrims prims = new Prims();
        private readonly PerfectMatchingGreedyAlgorithm perfectMatchingAlgorithm = new PerfectMatchingGreedyAlgorithm();
        private readonly Fleurys fleurys = new Fleurys(); //interface

        public List<Node> FindShortestPath(Graph graph)
        {
            List<Node> path = new List<Node>();

            List<Edge> minimalSpanningTree = prims.FindSpanningTree(graph);

            List<Edge> perfectMatching = perfectMatchingAlgorithm.FindPerfectMatching(new Graph(graph.OddDegreeNodes(minimalSpanningTree)));

            path = fleurys.FindEulerCircuit(graph, minimalSpanningTree.Concat(perfectMatching).ToList());

            path = path.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return path;
        }
    }
}