using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Testing;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            Random rand = new Random();
            List<Node> nodes = new List<Node>();
            //for (int i = 0; i < 10; i++)
            //{
            //    nodes.Add(new Node(i, rand.Next(20), rand.Next(20)));
            //    Console.WriteLine(nodes[i]);
            //    Console.WriteLine($"Node node{i} = new Node({nodes[i].id}, {nodes[i].x}, {nodes[i].y});");
            //    Console.WriteLine($"nodes.Add(node{i});");
            //}
            //var n1 = new Node(0, 1, 1);
            //var n2 = new Node(1, 2, 2);
            //var a = new Edge(n1, n2);
            //var b = new Edge(n2, n1);
            //Console.WriteLine(a.Equals(b));

            Node node0 = new Node(0, 8, 6);
            nodes.Add(node0);
            Node node1 = new Node(1, 1, 11);
            nodes.Add(node1);
            Node node2 = new Node(2, 8, 16);
            nodes.Add(node2);
            Node node3 = new Node(3, 1, 14);
            nodes.Add(node3);
            Node node4 = new Node(4, 20, 18);
            nodes.Add(node4);
            Node node5 = new Node(5, 20, 6);
            nodes.Add(node5);
            Node node6 = new Node(6, 14, 1);
            nodes.Add(node6);
            Node node7 = new Node(7, 14, 8);
            nodes.Add(node7);
            Node node8 = new Node(8, 16, 16);
            nodes.Add(node8);
            Node node9 = new Node(9, 4, 18);
            nodes.Add(node9);
            Console.WriteLine();

            var saved = Console.Out;
            FileStream filestream = new FileStream("D:\\Documents\\Výška\\Thesis\\log.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);

            var path = KernighanLin.FindShortestPath(new Graph(nodes));

            Console.SetOut(saved);
            Console.WriteLine("End");

            //  List<Node> square = new List<Node>() {new Node(0, -1, -1) , new Node(1, -1, 1) , new Node(2, 1, -1) , new Node(3, 1, 1) };

            //    var at = new AlgorithmsTest();
            //    at.TestAlgorithm();
            // kl.FindShortestPath(new Graph(square));

            /*  var test = new TSPLibTest();
              test.TestTPSLib();*/

            // var test = new AlgorithmsTest();

            // test.TestAlgorithm();

            //string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData\BigTest"));

            //string[] dataFiles = Directory.GetFiles(path, "*.tsp");

            //string[] tourFiles = Directory.GetFiles(path, "*.opt.tour");

            //foreach (var item in dataFiles.Concat(tourFiles))
            //{
            //    Console.WriteLine(item);
            //    var lib = new TSPLib(item);

            //    try
            //    {
            //        var nodes = lib.DeserializeToNodes();
            //        if (nodes == null)
            //            Console.WriteLine("No coordinates to deserialize");
            //        else
            //        {
            //            foreach (var node in nodes.Take(10))
            //                Console.Write($"{node}; ");
            //            Console.WriteLine();
            //        }

            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine($"Invalid type {lib.Type} or edge weight type {lib.WeightType}");
            //    }
            //}

            Console.ReadKey();
        }
    }
}
