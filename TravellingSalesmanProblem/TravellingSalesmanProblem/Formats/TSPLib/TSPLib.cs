using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.Enums.TSPLib;
using TravellingSalesmanProblem.GraphStructures;
using Type = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.Type;
using EdgeWeightType = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.EdgeWeightType;
using System.Text;

namespace TravellingSalesmanProblem.Formats.TSPLib
{
    public class TSPLib
    {
        //the file format
        public string Name;
        public Type Type;
        public string? Comment;
        public int Dimension;
        public EdgeWeightType WeightType;

        public int? Capacity; //Specifies the truck capacity in a CVRP ... mam ji tam vubec mit?
        public EdgeWeightFormat? WeightFormat;  //jenom, pokud jsou edge weights dany explicitne
        public EdgeDataFormat? DataFormat; //asi taky ne? if graph is not complete...
        public NodeCoordType? CoordType;    //spis ne? 
        public DisplayDataType? DisplayDataType;   //spis ne? 

        //the data part
        public string? NodeCoordSection;
        public string? TourSection;
        public string? DepotSection; //asi ne?
        public string? DemandSection; //ne
        public string? EdgeDataSection;//ne?
        public string? FixedEdgesSection;    //nope
        public string? DisplayDataSection;   //asi ne?
        public string? EdgeWeightSection; //asi ne?

        //helpers
        private readonly EnumRecognizer EnumRecognizer = new EnumRecognizer();

        public TSPLib(string path)
        {
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? line;

            while ((line = reader.ReadLine()) != null && line != "EOF") //EOF is optional
                RedirectToSection(line, reader);

            Name ??= Path.GetFileName(path)[0..^4]; //if there is no Name, it will set it to the name of the file

            return;
        }

        public void RedirectToSection(string line, StreamReader reader) //refactor
        {
            string section;
            string? content = null;

            if(line.Contains(':'))
            {
                section = line[0..line.IndexOf(":")].Trim(' ');
                content = line[(line.IndexOf(':') + 1)..].Trim(' ');
            }
            else
                section = line.Trim(' ');

            DeserializeSection(section, content, reader);
        }

        public void DeserializeSection(string section, string? content, StreamReader reader) //refactor
        {
            switch (section)
            {
                case "NAME": { Name = GetName(content); return; }  
                case "TYPE": { Type = GetType(content); return; }
                case "COMMENT": { Comment = content; return; }
                case "DIMENSION": { Dimension = GetDimensions(content); return; }
                case "CAPACITY": { Capacity = GetCapacity(content); return; }
                case "EDGE_WEIGHT_TYPE": { WeightType = GetEdgeWeghtType(content); return; }
                case "EDGE_WEIGHT_FORMAT": { WeightFormat = GetEdgeWeghtFormat(content); return; }
                case "EDGE_DATA_FORMAT": { DataFormat = GetEdgeDataFormat(content); return; }
                case "DISPLAY_DATA_TYPE": { DisplayDataType = GetDisplayDataType(content); return; }
                case "NODE_COORD_SECTION": { NodeCoordSection = GetSection(reader); return; }                   
                case "DEPOT_SECTION": { DepotSection = GetSection(reader, "-1"); return; }
                case "DEMAND_SECTION": { DemandSection = GetSection(reader); return; }
                case "EDGE_DATA_SECTION": { EdgeDataSection = GetSection(reader, "-1"); return; }
                case "FIXED_EDGES_SECTION": { FixedEdgesSection = GetSection(reader, "-1"); return; }
                case "DISPLAY_DATA_SECTION": { DisplayDataSection = GetSection(reader); return; }                  
                case "TOUR_SECTION": { TourSection = GetSection(reader, "-1"); return; }
                case "EDGE_WEIGHT_SECTION": { EdgeWeightSection = GetSection(reader); return; }
                default: { return; }    //throw?           
            }
        }

        private static string GetName(string? content) =>
            content ?? throw new Exception("Name cannot be null");

        private Type GetType(string? content) =>
            content != null ? EnumRecognizer.RecognizeType(content) : throw new Exception("Name cannot be null");

        private static int GetDimensions(string? content) =>
            int.TryParse(content, out int dimensions) ? dimensions : throw new Exception($"Invalid dimension value: {content}");

        private static int GetCapacity(string? content) =>
            int.TryParse(content, out int capacity) ? capacity : throw new Exception($"Invalid capacity value: {content}");

        private EdgeWeightType GetEdgeWeghtType(string? content) =>
            content != null ? EnumRecognizer.RecognizeEdgeWeightType(content) : throw new Exception($"Invalid edge weight type value: {content}");

        private EdgeWeightFormat GetEdgeWeghtFormat(string? content) =>
            content != null ? EnumRecognizer.RecognizeEdgeWeightFormat(content) : throw new Exception($"Invalid edge weight format value: {content}");

        private EdgeDataFormat GetEdgeDataFormat(string? content) =>
            content != null ? EnumRecognizer.RecognizeEdgeDataFormat(content) : throw new Exception($"Invalid edge data format value: {content}");

        private DisplayDataType GetDisplayDataType(string? content) =>
            content != null ? EnumRecognizer.RecognizeDisplayDataType(content) : throw new Exception($"Invalid display data format value: {content}");

        private static string GetSection(StreamReader reader, string? terminator = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            string? line = reader.ReadLine();

            while (line != null && line != "EOF" && (terminator == null || line != terminator))
            {
                stringBuilder.AppendLine(line);
                line = reader.ReadLine();
            }

            return stringBuilder.ToString();
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
            int.TryParse(line[0..line.IndexOf(' ')], out int id);

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            double.TryParse(line[0..line.IndexOf(' ')], out double x);

            line = line.Substring(line.IndexOf(' ') + 1).TrimStart(' ');

            double.TryParse(line, out double y);

            return new Node(id, x, y);
        }

        public List<Node>? DeserializeToNodes()
        {
            CheckType();
            CheckEdgeWeightType();

            if (NodeCoordSection == null)
                return null;

            List<Node> nodes = new List<Node>();

            foreach (string line in NodeCoordSection.Split('\n'))
                if (line != String.Empty)
                    nodes.Add(LineToNode(line.TrimStart(' ')));
            
            return nodes;
        }

        public override string ToString()
        {
            return GetType().GetFields()
                .Select(field => (field.Name, Value: field.GetValue(this)))
                .Aggregate(new StringBuilder(),
                (stringBuilder, info) =>
                {
                    if (info.Value == null)
                        return stringBuilder;
                    else if (info.Name.ToString().EndsWith("Section"))
                        return stringBuilder.AppendLine($"{info.Name}:\n{info.Value}");
                    else
                        return stringBuilder.AppendLine($"{info.Name}: {info.Value}");
                },
                stringBuilder => stringBuilder.ToString());
        }

        public 
    }
}