using TravellingSalesmanProblem.Algorithms;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Testing;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            var a = new Node(0, 1.9, -9.36);
            var b = new Node(1, -2.8645, 225.56454); 
            var f = 10.2545;
            var l1 = a.Distance(b);


          //  Console.WriteLine($"a: {a}\nb:{b}\nf: {f}\nl1: {l1}");

            a.x *= f;
            a.y *= f;
            b.x *= f;
            b.y *= f;


            var l2 = b.Distance(a);

           // Console.WriteLine($"a': {a}\nb':{b}\nl2: {l2}\nl1*f: {l1*f}\n");

            var graph = new Graph(new List<Node> {
                new Node(0, -1, -2),
                new Node(1, -0.7, 0.3),
                new Node(2, 2, 2.7),
                new Node(3, 4, -2),
                new Node(4, 4, 3)
            });

            var nit = new NiceInstanceTransformer();
            nit.TransformToNiceInstance(graph, 1);

            //  var test = new Noice();
            // test.Test();
            /*
            var test = new Noice();
            test.Test();*/

            /*
            var test = new TSPLibTest();
            test.TestTPSLib();
            */

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
