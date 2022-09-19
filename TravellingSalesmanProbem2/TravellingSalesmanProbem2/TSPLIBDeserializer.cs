using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        private static void AddEdges(List<Edge> edges, List<Node> allNodes, List<Node> nodes)
        {
            foreach (var newNode in nodes)
                foreach (var node in allNodes)
                    edges.Add(new Edge(node, newNode));
        }



        private List<Edge> DeserializeToEdges(StreamReader reader)
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();
            Node newNode;

            string? line = reader.ReadLine();
            while (line != "EOF")
            {
                newNode = LineToNode(line);

                nodes.Add(newNode);

                line = reader.ReadLine();
            }

            Stopwatch stopwatch = new Stopwatch();
            int threadCount = 4;
            int chunkSize = nodes.Count / threadCount;
            List<List<Node>> parts = nodes
                    .Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / chunkSize)
                    .Select(x => x.Select(v => v.Value).ToList())
                    .ToList();
            stopwatch.Start();

            Parallel.ForEach(parts.Cast<List<Node>>(),
            list =>
            {
                AddEdges(edges, nodes, list);
            });

            stopwatch.Stop();

            Console.WriteLine($"Threads is {stopwatch.Elapsed.Seconds} s");

            foreach (Edge edge in edges)
            {
                Console.WriteLine(edge);
            }

            return edges;
        }

        public List<Edge> DeserializeEdges()
        {
            List<Edge> edges = new List<Edge>();
            using var reader = new StreamReader(new FileStream(path, FileMode.Open));
            string? item; //nullable type
            do
            {
                item = reader.ReadLine();
            } while (item != "NODE_COORD_SECTION");

            return DeserializeToEdges2(reader);
        }

        private static void AddEdges2(ref List<Edge> edges, ref List<Node> nodes, ref bool end) //ne sam se sebou
        {
            int currentIndex = 0;

            while (currentIndex < nodes.Count - 1 || !end)
            {
                if (currentIndex % 100 == 0) Console.WriteLine($"Index: {currentIndex}");
                if (currentIndex >= nodes.Count - 1)
                    continue;
                for (int i = 0; i < currentIndex; i++)
                    edges.Add(new Edge(nodes[currentIndex], nodes[i]));
                currentIndex++;
            }
        }

        private static void AddEdges4(ref List<Edge> edges, ref List<Node> nodes, int startIndex, int endIndex) //ne sam se sebou
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i % 100 == 0) Console.WriteLine($"Index: {i}/{endIndex}");
                for (int j = 0; j < startIndex; j++)
                    edges.Add(new Edge(nodes[i], nodes[j]));
            }
        }

        private static int AddEdges3(ref List<Edge> edges, ref List<Node> nodes, ref bool end, ref int currentIndex) //ne sam se sebou
        {
            currentIndex = 0;
            Console.WriteLine($"Starting");

            while (currentIndex < nodes.Count - 1)
            {
                if (currentIndex % 100 == 0) Console.WriteLine($"Index: {currentIndex}");
                Console.WriteLine($"Index: {currentIndex}");
                if (currentIndex >= nodes.Count - 1)
                    continue;
                for (int i = 0; i < currentIndex; i++)
                    edges.Add(new Edge(nodes[currentIndex], nodes[i]));
                currentIndex++;
            }
            Console.WriteLine($"Ending with index: {currentIndex}");
            return currentIndex;
        }

        private List<Edge> DeserializeToEdges2(StreamReader reader)
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();
            Node newNode;
            bool end = false;
            int currentIndex = 0;

            string? line = reader.ReadLine();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var t = new Thread(() => AddEdges3(ref edges, ref nodes, ref end, ref currentIndex));

            t.Start();

            while (line != "EOF")
            {
                newNode = LineToNode(line);

                nodes.Add(newNode);

                line = reader.ReadLine();
            }

            end = true;

            t.Join();

            if (currentIndex < nodes.Count - 1)
            {
                Console.WriteLine($"Creating threads; currentIndex: {currentIndex}");
                int x1 = (int)Math.Sqrt(Math.Pow(nodes.Count, 2) + 3 * Math.Pow(currentIndex, 2)) / 2;
                int x2 = (int)Math.Sqrt((Math.Pow(nodes.Count, 2) + Math.Pow(currentIndex, 2)) / 2);
                int x3 = (int)Math.Sqrt(3 * Math.Pow(nodes.Count, 2) + Math.Pow(currentIndex, 2)) / 2;
                Console.WriteLine($"x1: {x1}; x2: {x2}; x3: {x3}; x4: {nodes.Count}");

                var t1 = new Thread(() => AddEdges4(ref edges, ref nodes, 0, nodes.Count-1));
                var t2 = new Thread(() => AddEdges4(ref edges, ref nodes, x1 + 1, x2));
                var t3 = new Thread(() => AddEdges4(ref edges, ref nodes, x2 + 1, x3));
                var t4 = new Thread(() => AddEdges4(ref edges, ref nodes, x3 + 1, nodes.Count));


                //t1.Start();
                //t2.Start();
                //t3.Start();
                //t4.Start();

                //t1.Join();
                //t2.Join();
                //t3.Join();
                //t4.Join();
            }

            stopwatch.Stop();

            Console.WriteLine($"Stopped at: {stopwatch.Elapsed.Hours}h {stopwatch.Elapsed.Minutes}m  {stopwatch.Elapsed.Seconds}s");

            //edges.Clear();
           // edges.OrderBy(e => e.node1);

            foreach (Edge edge in edges)
            {
                Console.WriteLine(edge);
            }

            return edges;
        }
    }
}
