using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Formats
{
    public static class TSPDeserializer
    {
        public static Graph DeserializeGraph(string path)
        {
            try
            {
                var tsp = new TSPLib(path);
                var nodes = tsp.DeserializeToNodes();
                return new Graph(nodes);
            }
            catch (Exception e)
            {
                throw new Exception($"File '{System.IO.Path.GetFileName(path)}' could not been deserialized: {e.Message}");
            }
        }
    }
}
