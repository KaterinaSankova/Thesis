using System.Collections.Generic;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree
    {
        private readonly IPrims prims = new Prims();
        private readonly Fleurys fleurys = new Fleurys(); //interface

        public List<Node> FindShortestPath(Graph graph)
        {
            List<Edge> minimalSpanningTree = prims.FindSpanningTree(graph);

            List<Edge>  multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return path;
        }
    }
}
