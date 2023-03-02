using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem.Formats
{
    public class TSPDeserializer
    {
        string path;

        public TSPDeserializer(string path)
        {
            this.path = path;
        }

        private Node LineToNode(string line) //tryparse
        {
            double x, y;
            int id;
            string[] coordinates;

            int.TryParse(line[0..line.IndexOf(' ')], out id);

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            double.TryParse(line[0..line.IndexOf(' ')], out x);

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            double.TryParse(line, out y);

            return new Node(id, x, y);
        }

        private List<Node> DeserializeToNodes(StreamReader reader) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            while (line != "EOF" && line != null)
            {
                nodes.Add(LineToNode(line.TrimStart(' '))); //in some files spaces are in the beggining of lines for alignment
                line = reader.ReadLine();
            }

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
            } while (item != "NODE_COORD_SECTION");

            return DeserializeToNodes(reader);
        }
    }
}
