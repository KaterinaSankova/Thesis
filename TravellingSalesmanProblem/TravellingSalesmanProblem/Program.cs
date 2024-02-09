using System.Diagnostics;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Testing;

namespace TravellingSalesmanProblem
{
    internal class Program
    { 
        static void Main()
        {
            Stopwatch stopWatch = new Stopwatch();

            Random rand = new Random();
            rand.Next(20);
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < 1000; i++)
            {
                nodes.Add(new Node(i, rand.Next(1000), rand.Next(1000)));
            }

            var saved = Console.Out;
            FileStream filestream = new FileStream(".\\..\\..\\..\\..\\..\\log.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;

            GraphStructures.Path path;
            TimeSpan ts;
            string elapsedTime;

            Console.SetOut(streamwriter);
            stopWatch.Restart();
            path = new KernighanLin().FindShortestPath(new Graph(nodes));
            Console.SetOut(saved);
            //Console.WriteLine(path);
            Console.WriteLine(path.Length);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms";
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine("End KernighanLin\n");

            Console.SetOut(streamwriter);
            stopWatch.Restart();
            path = new GraphStructures.Path(new Christofides().FindShortestPath(new Graph(nodes)));
            Console.SetOut(saved);
            //Console.WriteLine(path);
            Console.WriteLine(path.Length);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms";
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine("End Christofides\n");

            stopWatch.Restart();
            path = new GraphStructures.Path(new NearestAddition().FindShortestPath(new Graph(nodes)));
            //Console.WriteLine(path);
            Console.WriteLine(path.Length);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms";
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine("End NearestAddition\n");

            stopWatch.Restart();
            path = new GraphStructures.Path(new DoubleTree().FindShortestPath(new Graph(nodes)));
            //Console.WriteLine(path);
            Console.WriteLine(path.Length);
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms";
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine("End DoubleTree\n");




            //  List<Node> square = new List<Node>() {new Node(0, -1, -1) , new Node(1, -1, 1) , new Node(2, 1, -1) , new Node(3, 1, 1) };

            //    var at = new AlgorithmsTest();
            //    at.TestAlgorithm();
            // kl.FindShortestPath(new Graph(square));

            /*  var test = new TSPLibTest();
            //  test.TestTPSLib();*/

            //var test = new AlgorithmsTest();

            //test.TestAlgorithm();

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
