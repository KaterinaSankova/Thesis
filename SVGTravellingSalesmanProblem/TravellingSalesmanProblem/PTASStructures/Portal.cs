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
        public SquareSide? side;
        public bool isCorner = false;
        public SquareSide? secondSide;
        public bool isCenter = false;

        public Portal(int id, double x, double y, SquareSide side) : base(id, x, y) => this.side = side;

        public Portal(int id, double x, double y, bool isCorner) : base(id, x, y)
        {
            this.isCorner = isCorner;
        }

        public Portal(int id, double x, double y, SquareSide side, SquareSide secondSide) : base(id, x, y)
        {
            this.side = side;
            isCorner= true;
            this.secondSide = secondSide;
        }
    }
}
