namespace TravellingSalesmanProblem
{
    public class TSPLIBDeserializer
    {
        string path;

        public TSPLIBDeserializer(string path)
        {
            this.path = path;
        }

        private Node LineToNode(string line) //tryparse
        {
            double x, y;
            string[] coordinates;

            line = line.Substring(line.IndexOf(' ') + 1);

            coordinates = line.Split(' ');
            double.TryParse(coordinates[0], out x);
            double.TryParse(coordinates[1], out y);

            return new Node(x, y);
        }

        private List<Node> DeserializeToNodes(StreamReader reader) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            while (line != "EOF")
            {
                nodes.Add(LineToNode(line));
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
