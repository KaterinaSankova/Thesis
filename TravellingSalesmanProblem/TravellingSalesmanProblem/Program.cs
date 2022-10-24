using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData"));

            string[] dataFiles = Directory.GetFiles(path, "*.tsp");

            string[] tourFiles = Directory.GetFiles(path, "*.opt.tour");

            Stopwatch stopwatch = new Stopwatch();

            int dataFile = 1;
            int tourFile = 0;

            Console.WriteLine(dataFiles[dataFile]);

            var input = new TSPDeserializer(dataFiles[dataFile]).DeserializeNodes();
            var output = new OptTourDeserializer(tourFiles[tourFile], input).DeserializeNodes();

            foreach (var node in output)
            {
                Console.WriteLine(node.id);
            }

            var length = new Graph(output).GetLength();

            Console.WriteLine($"Length from file = {length}");

            var nearestAddition = new NearestAddition();
            var doubleTree = new DoubleTree();
            var christofides = new Christofides();

            var result = new Graph(nearestAddition.FindShortestPath(new Graph(input)));

            double algoLen = result.GetLength();

            Console.WriteLine($"Length from NearestAddition algorithm = {algoLen}");

            Console.WriteLine($"{length <= 2 * algoLen}");


            result = new Graph(doubleTree.FindShortestPath(new Graph(input)));

            algoLen = result.GetLength();

            Console.WriteLine($"Length from DoubleTree algorithm = {algoLen}");

            Console.WriteLine($"{length <= 2 * algoLen}");


            result = new Graph(christofides.FindShortestPath(new Graph(input)));

            algoLen = result.GetLength();

            Console.WriteLine($"Length from Christofides algorithm = {algoLen}");

            Console.WriteLine($"{length <= 3 / 2 * algoLen}");

            //stopwatch.Start();
            //var matrix = new TSPLIBDeserializer(dataFiles[0]).DeserializeToEdges2();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(dataFiles[0]).DeserializeToEdges3();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //Console.WriteLine(dataFiles[1]);

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(dataFiles[1]).DeserializeToEdges2();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(dataFiles[1]).DeserializeToEdges3();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            ////Console.WriteLine(dataFiles[2]);
            //   ///
            //   Console.WriteLine(dataFiles[3]);

            //   stopwatch.Start();
            //   var matrix = new TSPLIBDeserializer(dataFiles[3]).DeserializeNodes();
            //   stopwatch.Stop();
            //   Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //   foreach (var node in matrix)
            //       Console.WriteLine(node);

            //   stopwatch.Start();
            //   var NearestAddition = new NearestAddition();
            //   var edge = NearestAddition.FindShortestEdge(matrix, matrix);
            //   stopwatch.Stop();
            //   Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");
            //   Console.WriteLine($"Result: {edge}, distance: {Edge.Distance(edge.Item1, edge.Item2)}");

            //   List<Node> result = NearestAddition.FindShortestPath(matrix);

            //   foreach (var node in result)
            //   {
            //       Console.WriteLine(node);
            //   }

            //   matrix = new TSPLIBDeserializer(dataFiles[3]).DeserializeNodes();
            //   var Prims = new Prims();

            //   var spanningTree = Prims.FindSpanningTree(matrix);

            //   Console.WriteLine("************\nPRIMS\n************");
            //   foreach (var e in spanningTree)
            //{
            //       Console.WriteLine($"{e.Item1} {e.Item2}");
            //}

            //var oddNodes = new DoubleTree().FindShortestPath(new Graph(dataFiles[3]));

            //Console.WriteLine("***ODD NODES***");

            //for (int i = 0; i < oddNodes.Count; i++)
            //    Console.WriteLine($"{oddNodes[i]}\t");

            //for (int i = 0; i < matrix.Count; i++)
            //{
            //    for (int j = 0; j < matrix.Count; j++)
            //    {
            //        Console.Write($"{matrix[i][j]}\t" );
            //    }
            //    Console.Write($"\n");
            //}



            Console.ReadKey();
        }
    }
}
