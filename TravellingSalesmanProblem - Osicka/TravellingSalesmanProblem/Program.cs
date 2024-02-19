using System.Diagnostics;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem
{
    internal class Program
    { 
        static void Main()
        {
            Stopwatch stopWatch = new();

            Random rand = new();
            Graph graph = new();
            for (int i = 0; i < 1000; i++)
            {
                graph.nodes.Add(new Node(i, rand.Next(20), rand.Next(20)));
            }

            TimeSpan ts;
            GraphStructures.Path path;

            Console.WriteLine("Nearest addition");
            stopWatch.Restart();
            path = new NearestAddition().FindShortestPath(graph);
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();

            Console.WriteLine("Double tree");
            stopWatch.Restart();
            path = new DoubleTree().FindShortestPath(graph);
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();


            Console.WriteLine("Christofides");
            stopWatch.Restart();
            path = new Christofides().FindShortestPath(graph);
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();

            Console.WriteLine("KernighanLin");
            stopWatch.Restart();
            path = new KernighanLin().FindShortestPath(graph);
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
