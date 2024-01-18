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
            Stopwatch stopWatch = new();

            Random rand = new();
            Graph graph = new();
            for (int i = 0; i < 500; i++)
            {
                graph.nodes.Add(new Node(i, rand.Next(100), rand.Next(100)));
            }

            TimeSpan ts;
            GraphStructures.Path path;

            Console.WriteLine("Nearest addition");
            stopWatch.Restart();
            path = new GraphStructures.Path(new NearestAddition().FindShortestPath(graph));
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();

            Console.WriteLine("Double tree");
            stopWatch.Restart();
            path = new GraphStructures.Path(new DoubleTree().FindShortestPath(graph));
            stopWatch.Stop();
            Console.WriteLine(path.Length);
            ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds / 10}ms");
            Console.WriteLine();


            Console.WriteLine("Christofides");
            stopWatch.Restart();
            path = new GraphStructures.Path(new Christofides().FindShortestPath(graph));
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
