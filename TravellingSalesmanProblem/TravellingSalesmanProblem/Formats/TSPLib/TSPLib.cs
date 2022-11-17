using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms;
using TravellingSalesmanProblem.Algorithms.Enums.TSPLib;
using TravellingSalesmanProblem.GraphStructures;
using Type = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.Type;
using EdgeWeightType = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.EdgeWeightType;

namespace TravellingSalesmanProblem
{
    public class TSPLib
    {
        //the file format
        public string Name;
        public Type Type;
        public string? Comment;
        public int Dimension;
        public EdgeWeightType? WeightType;

        //public int? Capacity; //Specifies the truck capacity in a CVRP ... mam ji tam vubec mit?
        //public EdgeWeightFormat? WeightFormat;  //jenom, pokud jsou edge weights dany explicitne
        //public EdgeDataFormat? DataFormat; //asi taky ne? if graph is not complete...
        //public NodeCoordType? CoordType;    //spis ne? 
        //public DisplayDataType? DisplayDataType;   //spis ne? 

        //the data part
        public List<Node> NodeCoordSection;
        public List<int> TourSection;

        //public string DepotSection; //asi ne?
        //public string DemandSection; //ne
        //public string EdgeDataSection;//ne?
        //public string FixedEdgesSection;    //nope
        //public string DisplayDataSection;   //asi ne?
        //public string EdgeWeightSection; //asi ne?

        //helpers
        private EnumRecognizer EnumRecognizer = new EnumRecognizer();

        public TSPLib(string path)
        {
            List<Node> nodes = new List<Node>();
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string line;

            while ((line = reader.ReadLine()) != null && line != "EOF") //EOF is optional
                RedirectToSection(line, reader);
        }

        public void RedirectToSection(string line, StreamReader reader) //refactor
        {
            string section;
            string? content = null;

            if(line.Contains(':'))
            {
                section = line[0..line.IndexOf(":")].Trim(' ');
                content = line.Substring(line.IndexOf(':') + 1).Trim(' ');
            }
            else
                section = line.Trim(' ');

            DeserializeSection(section, content, reader);
        }
        public void DeserializeSection(string section, string? content, StreamReader reader) //refactor
        {
            switch (section)
            {
                case "NAME":
                    Name = content;
                    return;
                case "TYPE":
                    Type = EnumRecognizer.RecognizeType(content);
                    CheckType();
                    return;
                case "COMMENT":
                    Comment = content;
                    return;
                case "DIMENSION":
                    int.TryParse(content, out Dimension);
                    return;
                //case "CAPACITY":
                //    int capacity;
                //    int.TryParse(content, out capacity);
                //    Capacity = capacity;
                //    return;
                case "EDGE_WEIGHT_TYPE":
                    WeightType = EnumRecognizer.RecognizeEdgeWeightType(content);
                    CheckEdgeWeightType();
                    return;
                //case "EDGE_WEIGHT_FORMAT":
                //    WeightFormat = EnumRecognizer.RecognizeEdgeWeightFormat(content);
                //    return;
                //case "EDGE_DATA_FORMAT":
                //    DataFormat = EnumRecognizer.RecognizeEdgeDataFormat(content);
                //    return;
                //case "DISPLAY_DATA_TYPE":
                //    DisplayDataType = EnumRecognizer.RecognizeDisplayDataType(content);
                //    return;
                case "NODE_COORD_SECTION":
                    NodeCoordSection = DeserializeToNodes(reader);
                    return;
                default:
                    break;
            }
            return;
        }

        public void CheckType()
        {
            if (Type != Type.TSP && Type != Type.TOUR)
                throw new Exception(); //neco jako, ze invalid problem type nebo tak
        }
        
        public void CheckEdgeWeightType()
        {
            if (WeightType != EdgeWeightType.EUC2D)
                throw new Exception(); //neco jako, ze invalid problem type nebo tak
            
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
            while (line != "EOF" && line != null) //EOF is optional
            {
                nodes.Add(LineToNode(line.TrimStart(' '))); //in some files spaces are in the beggining of lines for alignment
                line = reader.ReadLine();
            }

            // Console.WriteLine($"Dimensions: {nodes.Count}");

            return nodes;
        }

        public List<Node> DeserializeNodes(string path)
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

        public override string ToString()
        {
            return $"Name: {Name}\nType: {Type}\nComment: {Comment}\nDimension: {Dimension}\n" +
                $"EdgeWeightType: {WeightType}\n";
        }
    }
}
