﻿using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.Formats.TSPLib;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Testing
{
    public class AlgorithmsTest
    {
        string dataFile;
        string tourFile;
        TSPAlgorithms algorithm;

        //public Test(string dataFile, string tourFile, TSPAlgorithms algorithm) //check .xxx
        //{
        //    this.dataFile = dataFile;
        //    this.tourFile = tourFile;
        //    this.algorithm = algorithm;
        //}

        private float GetApproximationFactor(TSPAlgorithms algorithm) //move?
        {
            switch (algorithm)
            {
                case TSPAlgorithms.Christofides:
                    return 1.5f;
                case TSPAlgorithms.DoubleTree:
                    return 2;
                case TSPAlgorithms.NearestAddition:
                    return 2;
                default:
                    return -1;
            }
        }

        public bool TestAlgorithm()
        {
            string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData\BigTest\GoodFormat\"));

            string[] dataFiles = Directory.GetFiles(path, "*.tsp");

            string[] tourFiles = Directory.GetFiles(path, "*.opt.tour");

            int count = 0;
            int positive = 0;
            int negative = 0;

            foreach (var file in dataFiles)
            {
                string tourFile = string.Concat(file[0..^4], ".opt.tour");
                if (tourFiles.Contains(tourFile))
                {
                    Console.WriteLine($"{file}");
                    if (ExcuteTest(file, tourFile))
                    {
                        positive++;
                    }
                    else
                    {
                        negative++;
                    }
                    
                    count++;
                }
            }

            Console.WriteLine($"{count} tests tested");
            Console.WriteLine($"{positive} tests passed");
            Console.WriteLine($"{negative} tests failed");

            return false;
        }

        public bool ExcuteTest(string dataFile, string tourFile)
        {
            List<Node> input;

            try
            {
                input = new TSPLib(dataFile).DeserializeToNodes();
            }
            catch (Exception)
            {
                Console.WriteLine();
                return false;
            }
            var output = new OptTourDeserializer(tourFile, input).DeserializeNodes();

            if (input.Count + 1 != output.Count)
                return false;

            var length = new Graph(output).GetLength();

            bool pass = false;

            Console.WriteLine($"Length from file = {length}");

            var nearestAddition = new NearestAddition();
            var doubleTree = new DoubleTree();
            var christofides = new Christofides();
            var kernighanLin = new KernighanLin();

            var rnd = new Random();
            var inputGraph = new Graph(input.ToList().OrderBy( x => rnd.Next()).ToList());

            var result = new Graph(nearestAddition.FindShortestPath(inputGraph));

            double algoLen = result.GetLength();

            Console.Write($"NearestAddition algorithm: {algoLen}, ");

            Console.Write($"factor: {algoLen / length}, ");

            Console.WriteLine($"{length <= 2 * algoLen}");


            pass = length <= 2 * algoLen && CheckResult(input, result.nodes);


            result = new Graph(doubleTree.FindShortestPath(inputGraph));

            algoLen = result.GetLength();

            Console.Write($"DoubleTree algorithm: {algoLen}, ");

            Console.Write($"factor: {algoLen / length}, ");

            Console.WriteLine($"{length <= 2 * algoLen}");

            pass = pass && length <= 2 * algoLen && CheckResult(input, result.nodes);


            result = new Graph(christofides.FindShortestPath(inputGraph));

            algoLen = result.GetLength();

            Console.Write($"Christofides algorith: {algoLen}, ");

            Console.Write($"factor: {algoLen / length}, ");

            Console.WriteLine($"{length <= 1.5f * algoLen}\n\n\n");

            pass = pass && length <= 1.5f * algoLen && CheckResult(input, result.nodes);


            result = new Graph(kernighanLin.FindShortestPath(inputGraph).ToList());

            algoLen = result.GetLength();

            Console.Write($"Kernighan-Lin algorith: {algoLen}, ");

            Console.WriteLine($"factor: {algoLen / length}");

            Console.WriteLine($"result: {CheckResult(input, result.nodes)}\n\n\n");

            return pass;
        }

        public bool CheckResult(List<Node> inputNodes, List<Node> path)
        {
            var nodes = path.ToList();

            foreach (var node in inputNodes)
                if (!nodes.Remove(node))
                    return false;

            if (nodes.Count != 1)
                return false;
            else if (nodes.First() != path.First())
                return false;
            else
                return true;
        }
    }
}
