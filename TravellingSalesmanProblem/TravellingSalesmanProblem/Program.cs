using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData"));

            string[] files = Directory.GetFiles(path, "*.tsp");

            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine(files[0]);

            //stopwatch.Start();
            //var matrix = new TSPLIBDeserializer(files[0]).DeserializeToEdges2();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(files[0]).DeserializeToEdges3();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //Console.WriteLine(files[1]);

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(files[1]).DeserializeToEdges2();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            //stopwatch.Start();
            //matrix = new TSPLIBDeserializer(files[1]).DeserializeToEdges3();
            //stopwatch.Stop();
            //Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

            ////Console.WriteLine(files[2]);

            stopwatch.Start();
            var matrix = new TSPLIBDeserializer(files[2]).DeserializeToEdges2();
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed.Hours}H {stopwatch.Elapsed.Minutes}M {stopwatch.Elapsed.Seconds}S {stopwatch.Elapsed.Milliseconds}MS");

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
