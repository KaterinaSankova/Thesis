using System.Reflection.PortableExecutable;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Formats
{
    public static class TourDeserializer
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

        private static List<Node> DeserializeNodes(string nodeSection, Graph graph) //null
        {
            var nodes = new List<Node>();

            Node? currNode;

            foreach (var line in nodeSection.Replace("\r\n", "\n").Trim().Split('\n'))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                currNode = LineToNode(line.TrimStart(' '), graph);

                if (currNode != null)
                    nodes.Add(currNode);
                else
                    break;
            }

            return nodes;
        }

        public static Path DeserializePath(string path, Graph graph)
        {
            try
            {
                var tsp = new TSPLib(path);
                var nodes = DeserializeNodes(tsp.TourSection, graph);
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
