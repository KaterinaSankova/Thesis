using SVGTravellingSalesmanProblem.Formats.TSPLib;
using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem.Algorithms.TSP
{
    public class NearestAddition : ITSPHeauristic
    {
        public bool FindTour(string sourcePath, string destinationPath)
        {
            var tspLib = new TSPLib(sourcePath);
            var nodes = tspLib.DeserializeToNodes();
            var tour = FindTour(new Graph(nodes));
            return true;
        }

        public List<Node> FindTour(Graph graph)
        {
            Edge firstEdge = graph.ShortestEdge();
            Edge shortestEdge;
            List<Node> path = new List<Node>();
            List<Node> remainingCities = graph.nodes.ToList();

            path.Add(firstEdge.node1);
            path.Add(firstEdge.node2);
            path.Add(firstEdge.node1);

            remainingCities.Remove(firstEdge.node1);
            remainingCities.Remove(firstEdge.node2);

            while (remainingCities.Count > 0)
            {
                shortestEdge = graph.ShortestEdge(path, remainingCities);
                path.Insert(path.IndexOf(shortestEdge.node1) + 1, shortestEdge.node2);
                remainingCities.Remove(shortestEdge.node2);
            }
            return path;
        }
    }
}
