﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            foreach (var item in dataFiles.Concat(tourFiles))
            {
                Console.WriteLine(item);
                var lib = new TSPLib(item);
                Console.WriteLine(lib);

                try
                {
                    var nodes = lib.DeserializeToNodes();
                    if (nodes == null)
                        Console.WriteLine("No coordinates to deserialize");
                    else
                    {
                        foreach (var node in nodes.Take(10))
                            Console.Write($"{node}; ");
                        Console.WriteLine();
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine($"Invalid type {lib.Type} or edge weight type {lib.WeightType}");
                }
            }

            Console.ReadKey();
        }
    }
}
