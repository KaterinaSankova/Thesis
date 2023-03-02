using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class SquareSide
    {
        List<Portal> portals;
        int crosses;

        public SquareSide(List<Portal> portals, int crosses)
        {
            this.portals = portals; 
            this.crosses = crosses;
        }
    }
}
