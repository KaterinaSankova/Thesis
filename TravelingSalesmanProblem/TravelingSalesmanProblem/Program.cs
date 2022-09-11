using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelingSalesmanProblem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\TestingData"));

            Console.WriteLine($"Directory:\n{path}\nFiles:");

            string[] files = Directory.GetFiles(path, "*.tsp");

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            using var reader = new StringReader(files[0]);
            string? item;
            do
            {
                item = reader.ReadLine();
                Console.WriteLine(item);
            } while (item != null);

            Console.ReadKey();
        }
    }
}
