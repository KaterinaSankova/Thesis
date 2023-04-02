using SVGTravellingSalesmanProblem.Formats.TSPLib;
using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem.Algorithms.TSP
{
    public class Christofides : ITSPHeauristic
    {
        private readonly IPrims prims = new Prims();
        private readonly PerfectMatchingGreedyAlgorithm perfectMatchingAlgorithm = new PerfectMatchingGreedyAlgorithm();
        private readonly Fleurys fleurys = new Fleurys(); //interface

        public bool FindTour(string sourcePath, string destinationPath)
        {
            var tspLib = new TSPLib(sourcePath);
            var nodes = tspLib.DeserializeToNodes();
            var tour = FindTour(new Graph(nodes));
            return true;
        }

        public List<Node> FindTour(Graph graph)
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