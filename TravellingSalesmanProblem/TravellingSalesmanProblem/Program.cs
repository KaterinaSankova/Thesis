using TravellingSalesmanProblem.Testing;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            var test = new TSPLibTest();
            test.TestTPSLib();

        //    var test = new AlgorithmsTest();

        //    test.TestAlgorithm();

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
