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

        public List<List<object>> DeserializeEdges()
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

        private double Distance(Node node1, Node node2) => Math.Sqrt(Math.Pow((node2.x - node1.x), 2) + Math.Pow((node2.y - node1.y), 2));

        private void Function (ref List<Node> nodes, ref List<List<object>> matrix, int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i % 100 == 0) Console.WriteLine($"{i}/{endIndex}");
                List<object> column = new List<object>();
                column.Add(nodes[i]);
                for (int j = 0; j <  nodes.Count; j++)
                {
                    if (i == j)
                    {
                        column.Add(-1);
                    }
                    else
                    {
                        column.Add(Distance(nodes[i], nodes[j]));
                    }
                }
                matrix.Add(column);
            }
        }

        public List<List<object>> DeserializeToEdges2(StreamReader reader = null)
        {
            var nodes = new List<Node>();
            var matrix = new List<List<object>>();

            nodes = DeserializeNodes();

            int numberOfThreads = Environment.ProcessorCount;

            Console.WriteLine($"Number of threads: {numberOfThreads}");

            int count = nodes.Count / 4;

            Console.WriteLine($"x1: {count / 4}; x2: {2 * count / 4}; x3: {3 * count / 4}; x4: {count}");

            Thread t1 = new Thread(() => Function(ref nodes, ref matrix, 0, count / 8));
            Thread t2 = new Thread(() => Function(ref nodes, ref matrix, count / 8 + 1, 2 * count / 8));
            Thread t3 = new Thread(() => Function(ref nodes, ref matrix, 2 * count / 8 + 1, 3 * count / 8));
            Thread t4 = new Thread(() => Function(ref nodes, ref matrix, 3 * count / 8 + 1, 4 * count / 8));
            Thread t5 = new Thread(() => Function(ref nodes, ref matrix, 4 * count / 8 + 1, 5 * count / 8));
            Thread t6 = new Thread(() => Function(ref nodes, ref matrix, 5 * count / 8 + 1, 6 * count / 8));
            Thread t7 = new Thread(() => Function(ref nodes, ref matrix, 6 * count / 4 + 1, 7 * count / 8));
            Thread t8 = new Thread(() => Function(ref nodes, ref matrix, 7 * count / 4 + 1, count));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();
            t7.Start();
            t8.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();
            t6.Join();
            t7.Join();
            t8.Join();

            return matrix;
        }

        public List<List<object>> DeserializeToEdges3(StreamReader reader = null)
        {
            var nodes = new List<Node>();
            var matrix = new List<List<object>>();

            nodes = DeserializeNodes();

            Function(ref nodes, ref matrix, 0, nodes.Count);

            return matrix;
        }
    }
}
