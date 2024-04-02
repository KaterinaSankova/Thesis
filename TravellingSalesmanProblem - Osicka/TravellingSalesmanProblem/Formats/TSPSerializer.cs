using System.Text;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Formats
{
    public static class TSPSerializer
    {
        private static TSPLib GetTspLibFormatData(Graph graph, string name)
        {
            var tsp = new TSPLib();
            tsp.Name = name;
            tsp.Type = Algorithms.Enums.TSPLib.Type.TSP;
            tsp.Dimension = graph.nodes.Count;
            tsp.WeightType = Algorithms.Enums.TSPLib.EdgeWeightType.EUC2D;
            StringBuilder nodesSection = new StringBuilder("");
            foreach (var node in graph.nodes)
            {
                nodesSection.AppendLine($"\t{node.Id}\t{node.X}\t{node.Y}");
            }
            nodesSection.Append("EOF");
            tsp.NodeCoordSection = nodesSection.ToString();
            return tsp;
        }

        public static void SerializeGraph(Graph graph, string folderPath, string name)
        {
            var tspLib = GetTspLibFormatData(graph, name);
            tspLib.Serialize(folderPath);
        }
    }
}
