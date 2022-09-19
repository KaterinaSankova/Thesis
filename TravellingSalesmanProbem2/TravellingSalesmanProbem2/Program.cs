using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
{
    internal class Program
    {
        static void Main()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\TestingData"));

            string[] files = Directory.GetFiles(path, "*.tsp");

            Console.WriteLine(files[0]);

            new TSPLIBDeserializer(files[0]).DeserializeEdges();

            Console.ReadKey();
        }
    }
}
