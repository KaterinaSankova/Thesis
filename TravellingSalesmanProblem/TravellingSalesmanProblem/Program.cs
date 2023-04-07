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
            for (int i = 0; i < 10; i++)
            {
                nodes.Add(new Node(i, rand.Next(20), rand.Next(20)));
            }

            var kl = new KeringhanLin();
            kl.FindShortestPath(new Graph(nodes));

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
