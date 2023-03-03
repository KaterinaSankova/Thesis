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
        public bool hasTwoSides = false;
        public SquareSide? secondSide;
        public bool isCenter = false;
        public bool copied = false;

        public Portal(int id, double x, double y, SquareSide side) : base(id, x, y) => this.side = side;

        public Portal(int id, double x, double y, bool isCenter) : base(id, x, y)
        {
            this.isCenter = isCenter;
        }

        public Portal(int id, double x, double y, SquareSide side, SquareSide secondSide) : base(id, x, y)
        {
            this.side = side;
            hasTwoSides= true;
            this.secondSide = secondSide;
        }

        private Portal(int id, double x, double y, SquareSide? side, SquareSide? secondSide, bool isCenter, bool hasTwoSides) : base(id, x, y)
        {
            this.side = side;
            this.hasTwoSides = hasTwoSides;
            this.secondSide = secondSide;
            this.isCenter = isCenter;
        }

        public Portal Copy()
        {
            var p = new Portal(id, x, y, side, secondSide, isCenter, hasTwoSides);
            p.copied = true;
            return p;
        }
    }
}
