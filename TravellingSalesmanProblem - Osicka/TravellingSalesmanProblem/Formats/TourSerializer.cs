using System.Text;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Formats
{
    public static class TourSerializer
    {
        private static TSPLib GetTspLibFormatData(Path path, string name) //null
        {
            var tsp = new TSPLib();
            tsp.Name = name;
            tsp.Type = Algorithms.Enums.TSPLib.Type.TOUR;
            tsp.Dimension = path.Count;
            StringBuilder tourSection = new StringBuilder("");
            foreach (var node in path.ToList())
            {
                tourSection.AppendLine(node.id.ToString());
            }
            tourSection.Append("-1");
            tsp.TourSection = tourSection.ToString();
            return tsp;
        }

        public static void SerializePath(Path path, string folderPath, string name)
        {
            var tspLib = GetTspLibFormatData(path, name);
            tspLib.Serialize(folderPath);
        }
    }
}
