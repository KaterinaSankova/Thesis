using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class SquareSide
    {
        public List<Portal> portals = new List<Portal>();
        public int crosses;
        public bool copied = false;

        public SquareSide(List<Portal> portals, int crosses)
        {
            this.portals = portals;
            this.crosses = crosses;
        }

        public SquareSide(int crosses) { this.crosses = crosses; }

        public SquareSide Copy()
        {
            var newSide = new SquareSide(portals.Select((p) => p.Copy()).ToList(), crosses);
            foreach (var p in newSide.portals)
            {
                if(!p.isCenter)
                {
                    if(p.hasTwoSides)
                    {
                        if (p.secondSide == this)
                            p.secondSide = newSide;
                    }
                    if(p.side == this)
                        p.side = newSide;
                }
            }
            newSide.copied= true;

            return newSide;
        }
    }
}
