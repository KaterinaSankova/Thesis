using SVGTravellingSalesmanProblem.GraphStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class Portal : Node
    {
        public int crosses = 0;

        public Portal(int id, double x, double y) : base(id, x, y) { }
    }
}
