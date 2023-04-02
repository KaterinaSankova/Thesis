using SVGTravellingSalesmanProblem.Formats.TSPLib;
using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem.Algorithms.TSP
{
    public class DoubleTree : ITSPHeauristic
    {
        private readonly IPrims prims = new Prims();
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
            List<Edge> minimalSpanningTree = prims.FindSpanningTree(graph);

            List<Edge>  multiGraph = minimalSpanningTree.Concat(minimalSpanningTree).ToList(); //refactor

            List<Node> eulerCircuit = fleurys.FindEulerCircuit(graph, multiGraph);

            List<Node> path = eulerCircuit.Distinct().ToList(); //shortcutting

            path.Add(path[0]);

            return path;
        }
    }
}
