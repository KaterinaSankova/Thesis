using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Formats.TSPLib;

namespace TravellingSalesmanProblem.Testing
{
    public class TSPLibTest
    {
        public void TestTPSLib()
        {
            //var test = new Test();

            //test.TestAlgorithm();

            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData\BigTest"));

            string[] dataFiles = Directory.GetFiles(path, "*.tsp");

            string[] tourFiles = Directory.GetFiles(path, "*.opt.tour");

            List<string> properties = new List<String>();
            List<(string, string?)> vals = new List<(string, string?)>();

            List<string> eucProperties = new List<String>();
            List<(string, string?)> eucVals = new List<(string, string?)>();

            foreach (var item in dataFiles.Concat(tourFiles))
            {
               // Console.WriteLine(item);
                var lib = new TSPLib(item);
                //Console.WriteLine(lib);

                properties = properties.Concat(lib.NonNullProperties()).Distinct().ToList();
                vals = vals.Concat(lib.NonNullPropertiesWithValues()).Distinct().ToList();

                if(lib.WeightType == Algorithms.Enums.TSPLib.EdgeWeightType.EUC2D || lib.Type == Algorithms.Enums.TSPLib.Type.TOUR)
                {
                    eucProperties = eucProperties.Concat(lib.NonNullProperties()).Distinct().ToList();
                    eucVals = eucVals.Concat(lib.NonNullPropertiesWithValues()).Distinct().ToList();

                    Console.WriteLine();

                    foreach (var log in lib.NonNullPropertiesWithValues())
                    {
                        if (!log.Item1.ToString().EndsWith("Section"))
                        {
                            Console.WriteLine($"{log.Item1}: {log.Item2}");
                        }
                        else
                        {
                            Console.WriteLine($"{log.Item1}");
                        }

                    }
                }

                //try
                //{
                //    var nodes = lib.DeserializeToNodes();
                //    if (nodes == null)
                //        Console.WriteLine("No coordinates to deserialize");
                //    else
                //    {
                //        foreach (var node in nodes.Take(10))
                //            Console.Write($"{node}; ");
                //        Console.WriteLine();
                //    }

                //}
                //catch (Exception)
                //{
                //    Console.WriteLine($"Invalid type {lib.Type} or edge weight type {lib.WeightType}");
                //}
            }

            //foreach (var item in properties)
            //{
            //    Console.WriteLine(item);
            //}

            //Console.WriteLine();

            //foreach (var item in vals.OrderBy(item => item.Item1))
            //{
            //    if (!item.Item1.ToString().EndsWith("Section") && item.Item1 != "Name" && item.Item1 != "Dimension" && item.Item1 != "Comment")
            //    {
            //        Console.WriteLine(item);
            //    }
            //}

            //Console.WriteLine("\nEuc:");

            //foreach (var item in eucProperties)
            //{
            //    Console.WriteLine(item);
            //}

            //Console.WriteLine();

            //foreach (var item in eucVals.OrderBy(item => item.Item1))
            //{
            //    if (!item.Item1.ToString().EndsWith("Section") && item.Item1 != "Name" && item.Item1 != "Dimension" && item.Item1 != "Comment")
            //    {
            //        Console.WriteLine(item);
            //    }
            //}
        }
    }
}
