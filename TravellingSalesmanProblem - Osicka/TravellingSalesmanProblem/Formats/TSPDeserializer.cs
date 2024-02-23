using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Formats
{
    public static class TSPDeserializer
    {
        private static Node LineToNode(string line) //tryparse
        {
            double x, y;
            int id;
            string[] coordinates;

            if (!int.TryParse(line[0..line.IndexOf(' ')], out id))
                throw new Exception($"Invalid line '{line}'");

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            if(!double.TryParse(line[0..line.IndexOf(' ')], out x))
                throw new Exception($"Invalid line '{line}'");

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            if(!double.TryParse(line, out y))
                throw new Exception($"Invalid line '{line}'");

            return new Node(id, x, y);
        }

        private static List<Node> DeserializeNodes(StreamReader reader) //null
        {
            var nodes = new List<Node>();

            string? line = reader.ReadLine();
            while (line != "EOF" && !string.IsNullOrEmpty(line))
            {
                nodes.Add(LineToNode(line.TrimStart(' '))); //in some files spaces are in the beggining of lines for alignment
                line = reader.ReadLine();
            }

            return nodes;
        }

        public static Graph DeserializeGraph(string path)
        {
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? item; //nullable type
            do
            {
                item = reader.ReadLine();
            } while (item != "NODE_COORD_SECTION");

            try
            {
                return new Graph(DeserializeNodes(reader));
            }
            catch (Exception e)
            {
                throw new Exception($"File '{System.IO.Path.GetFileName(path)}' could not been deserialized: {e.Message}");
            }
        }
    }
}
