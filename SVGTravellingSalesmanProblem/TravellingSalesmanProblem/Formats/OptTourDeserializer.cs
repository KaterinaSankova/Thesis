using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem.Formats
{
    public class OptTourDeserializer
    {
        string path;
        List<Node> nodes;

        public OptTourDeserializer(string path, List<Node> nodes)
        {
            this.path = path;
            this.nodes = nodes;
        }

        private Node LineToNode(string line)
        {
            int id;

            int.TryParse(line, out id);
            var nodesWithId = nodes.Where((node) => node.id == id).ToList();

            if (nodesWithId.Count == 0)
                return null;
            else
                return nodesWithId.First();
        }

        private List<Node> DeserializeToNodes(StreamReader reader) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            Node currNode;
            while (line != "EOF")
            {
                currNode = LineToNode(line.TrimStart(' '));

                if (currNode != null)
                    nodes.Add(currNode);
                else
                    break; //some files end with EOF some with -1 as an id of last node - in this case LineToNode returns null
                line = reader.ReadLine();
            }

            nodes.Add(nodes.First());

            return nodes;
        }

        public List<Node> DeserializeNodes()
        {
            List<Node> nodes = new List<Node>();
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? item; //nullable type
            do
            {
                item = reader.ReadLine();
            } while (item != "TOUR_SECTION");

            return DeserializeToNodes(reader);
        }
    }
}
