using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Formats
{
    public static class OptTourDeserializer
    {
        private static Node LineToNode(string line, Graph graph)
        {
            int id;

            int.TryParse(line, out id);
            var nodesWithId = graph.nodes.Where((node) => node.id == id).ToList();

            if (nodesWithId.Count == 0)
                return null;
            else
                return nodesWithId.First();
        }

        private static Path DeserializeToNodes(StreamReader reader, Graph graph) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            Node currNode;
            while (line != "EOF")
            {
                currNode = LineToNode(line.TrimStart(' '), graph);

                if (currNode != null)
                    nodes.Add(currNode);
                else
                    break; //some files end with EOF some with -1 as an id of last node - in this case LineToNode returns null
                line = reader.ReadLine();
            }

            nodes.Add(nodes.First());

            return new Path(nodes);
        }

        public static Path DeserializeNodes(string path, Graph graph)
        {
            List<Node> nodes = new List<Node>();
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? item; //nullable type
            do
            {
                item = reader.ReadLine();
            } while (item != "TOUR_SECTION");

            return DeserializeToNodes(reader, graph);
        }
    }
}
