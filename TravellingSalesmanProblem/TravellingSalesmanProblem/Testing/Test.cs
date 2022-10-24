using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.Testing
{
    public class Test
    {
        string dataFile;
        string tourFile;
        TSPAlgorithms algorithm;

        public Test(string dataFile, string tourFile, TSPAlgorithms algorithm) //check .xxx
        {
            this.dataFile = dataFile;
            this.tourFile = tourFile;
            this.algorithm = algorithm;
        }

        private float GetApproximationFactor(TSPAlgorithms algorithm) //move?
        {
            switch (algorithm)
            {
                case TSPAlgorithms.Christofides:
                    return 3 / 2;
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
            return false;
        }
    }
}
