using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Formats
{
    public static class OptTourDeserializer
    {
        private static Node? LineToNode(string line, Graph graph)
        {
            int id;

            if (!int.TryParse(line, out id))
                throw new Exception($"Invalid line '{line}'");

            if (id == -1)
                return null; //some files end with EOF some with -1 as an id of last node - in this case LineToNode returns null

            var nodesWithId = graph.nodes.Where((node) => node.id == id).ToList();

            if (nodesWithId.Count == 0)
                throw new Exception($"No node with ID {id} was found in input graph");
            else
                return nodesWithId.First();
        }

        private static List<Node> DeserializeNodes(StreamReader reader, Graph graph) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            Node? currNode;
            while (line != "EOF" && !string.IsNullOrEmpty(line))
            {
                currNode = LineToNode(line.TrimStart(' '), graph);

                if (currNode != null)
                    nodes.Add(currNode);
                else
                    break; 
                line = reader.ReadLine();
            }

            return nodes;
        }

        public static Path DeserializePath(string path, Graph graph)
        {
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? item; //nullable type
            do
            {
                item = reader.ReadLine();
            } while (item != "TOUR_SECTION");

            try
            {
                var nodes = DeserializeNodes(reader, graph);
                if (nodes.Count != graph.nodes.Count)
                    throw new Exception("Number of nodes in input graph and tour file differ.");
                return new Path(nodes);
            }
            catch (Exception e)
            {
                throw new Exception($"File '{System.IO.Path.GetFileName(path)}' could not been deserialized: {e.Message}");
            }
        }
    }
}
