using SVGTravellingSalesmanProblem.GraphStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class Portal
    {
        int id;
        public List<SquareSide> sides = new List<SquareSide>(); //??

        public Portal(int id, params SquareSide[] sides)
        {
            this.id= id;
            this.sides = sides.ToList();
        }

        public Portal(int id, List<SquareSide> sides)
        {
            this.id = id;
            this.sides = sides;
        }

        public Portal Copy()
        {
            var p = new Portal(id, sides);
            return p;
        }

        public void CrossSide()
        {
            if (this.sides.Count == 1)
                sides[0].crosses--;
        }
        public void CrossSideTo(Portal portal)
        {
            if (this.sides.Count == 1)
                sides[0].crosses--;
        }
    }
}
