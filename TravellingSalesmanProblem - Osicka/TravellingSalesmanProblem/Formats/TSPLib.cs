using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.Enums.TSPLib;
using TravellingSalesmanProblem.GraphStructures;
using Type = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.Type;
using EdgeWeightType = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.EdgeWeightType;
using System.Text;
using System.Globalization;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace TravellingSalesmanProblem.Formats
{
    public class TSPLib
    {
        //the file format
        public string? Name;
        public Type? Type;
        public string? Comment;
        public int? Dimension;
        public EdgeWeightType? WeightType;

        public int? Capacity; //Specifies the truck capacity in a CVRP
        public EdgeWeightFormat? WeightFormat;  //if edge weights are explicitly given
        public EdgeDataFormat? DataFormat; //graph is not complete
        public NodeCoordType? CoordType;
        public DisplayDataType? DisplayDataType;

        //the data part
        public string? NodeCoordSection;
        public string? TourSection;
        public string? DepotSection;
        public string? DemandSection;
        public string? EdgeDataSection;
        public string? FixedEdgesSection;
        public string? DisplayDataSection;
        public string? EdgeWeightSection;

        public TSPLib() { }

        public TSPLib(string path)
        {
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? line;

            while ((line = reader.ReadLine()) != null && line != "EOF") //EOF is optional
                RedirectToSection(line, reader);

            Name ??= System.IO.Path.GetFileName(path)[0..^4]; //if there is no Name, it will set it to the name of the file

            return;
        }

        private void RedirectToSection(string line, StreamReader reader)
        {
            string section;
            string? content = null;

            if (line.Contains(':'))
            {
                section = line[0..line.IndexOf(":")].Trim(' ');
                content = line[(line.IndexOf(':') + 1)..].Trim(' ');
            }
            else
                section = line.Trim(' ');

            DeserializeSection(section, content, reader);
        }

        private void DeserializeSection(string section, string? content, StreamReader reader)
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
                default: { return; }
            }
        }

        private static string GetName(string? content) =>
            content ?? throw new Exception("Name cannot be null");

        private Type GetType(string? content) =>
            content != null ? EnumRecognizer.RecognizeType(content) : throw new Exception("Type cannot be null");

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
            StringBuilder stringBuilder = new StringBuilder("");

            string? line = reader.ReadLine();

            while (!string.IsNullOrEmpty(line) && line.Trim() != "EOF" && (terminator == null || line.Trim() != terminator))
            {
                stringBuilder.AppendLine(line);
                line = reader.ReadLine();
            }

            return stringBuilder.ToString();
        }

        private void CheckType()
        {
            if (Type != Algorithms.Enums.TSPLib.Type.TSP && Type != Algorithms.Enums.TSPLib.Type.TOUR)
                throw new Exception($"Type '{Type}' is not supported");
        }

        private void CheckEdgeWeightType()
        {
            if (WeightType != EdgeWeightType.EUC2D)
                throw new Exception($"Edge weigth type '{WeightType}' is not supported");
        }

        private Node LineToNode(string line)
        {
            double x, y;
            int id;
            string coordLine = line.Replace(',', '.').Trim();

            if (!int.TryParse(coordLine[0..coordLine.IndexOfAny(new char[] { ' ', '\t'})], out id))
                throw new Exception($"Invalid line '{line.Trim()}'");

            coordLine = coordLine.Substring(coordLine.IndexOfAny(new char[] { ' ', '\t' }) + 1).TrimStart(' ');

            if (!double.TryParse(coordLine[0..coordLine.IndexOfAny(new char[] { ' ', '\t' })], NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                throw new Exception($"Invalid line '{line.Trim()}'");

            coordLine = coordLine.Substring(coordLine.IndexOfAny(new char[] { ' ', '\t' }) + 1).TrimStart(' ');

            if (!double.TryParse(coordLine, NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                throw new Exception($"Invalid line '{line.Trim()}'");

            return new Node(id, x, y);
        }

        public List<Node> DeserializeToNodes()
        {
            CheckType();
            CheckEdgeWeightType();

            if (NodeCoordSection == null)
                return null;

            List<Node> nodes = new List<Node>();

            foreach (string line in NodeCoordSection.Replace("\r\n", "\n").Split('\n'))
                if (line != string.Empty)
                    nodes.Add(LineToNode(line.TrimStart(' ')));

            return nodes;
        }

        public void Serialize(string folderPath)
        {
            while (!Directory.Exists(folderPath))
                throw new Exception($"Folder '{folderPath}' was not found.");

            if (string.IsNullOrEmpty(Name))
                throw new Exception("'Name' cannot be empty");

            if (Type == null)
                throw new Exception("'Type' cannot be empty");
            CheckType();

            if (Type == Algorithms.Enums.TSPLib.Type.TOUR)
            {
                string path = System.IO.Path.Combine(folderPath, $"{Name}.tour");
                int i = 2;
                while (File.Exists(path))
                {
                    path = System.IO.Path.Combine(folderPath, $"{Name}({i}).tour");
                    i++;
                }

                var sb = new StringBuilder("");

                sb.AppendLine($"NAME : {Name}");
                if (Comment != null)
                    sb.AppendLine($"COMMENT : {Comment}");
                sb.AppendLine("TYPE : TOUR");
                if (Dimension == null)
                    throw new Exception("'Dimension' cannot be empty");
                sb.AppendLine($"DIMENSION : {Dimension}");
                sb.AppendLine("TOUR_SECTION");
                if (TourSection == null)
                    throw new Exception("'TOUR_SECTION' cannot be empty");
                sb.Append($"{TourSection}");

                FileStream fs = File.Create(path);
                if (!File.Exists(path))
                {
                    throw new Exception($"File '{path}' could not be created.");
                }

                using var writer = new StreamWriter(fs);
                writer.Write(sb.ToString());
            }
            else if (Type == Algorithms.Enums.TSPLib.Type.TSP)
            {
                string path = System.IO.Path.Combine(folderPath, $"{Name}.tsp");
                int i = 2;
                while (File.Exists(path))
                {
                    path = System.IO.Path.Combine(folderPath, $"{Name}({i}).tsp");
                    i++;
                }

                var sb = new StringBuilder("");

                sb.AppendLine($"NAME : {Name}");
                if (Comment != null)
                    sb.AppendLine($"COMMENT : {Comment}");
                sb.AppendLine("TYPE : TSP");
                if (Dimension == null)
                    throw new Exception("'Dimension' cannot be empty");
                sb.AppendLine($"DIMENSION : {Dimension}");
                if (WeightType == null)
                    throw new Exception("'WeightType' cannot be empty");
                CheckEdgeWeightType();
                sb.AppendLine($"EDGE_WEIGHT_TYPE : EUC_2D");
                sb.AppendLine("NODE_COORD_SECTION");
                if (NodeCoordSection == null)
                    throw new Exception("'NodeCoordSection' cannot be empty");
                sb.Append($"{NodeCoordSection}");

                FileStream fs = File.Create(path);
                if (!File.Exists(path))
                {
                    throw new Exception($"File '{path}' could not be created.");
                }

                using var writer = new StreamWriter(fs);
                writer.Write(sb.ToString());
            }
        }


        public List<string> NonNullProperties()
        {
            return GetType().GetFields()
                .Where((field) => field.GetValue(this) != null)
                .Select((info) => info.Name.ToString()).ToList();
        }

        public List<(string, string?)> NonNullPropertiesWithValues()
        {
            return GetType().GetFields()
                .Where((field) => field.GetValue(this) != null)
                .Select((info) => (info.Name.ToString(), info.GetValue(this).ToString())).ToList();
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

    }
}