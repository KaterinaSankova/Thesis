using SVGTravellingSalesmanProblem.GraphStructures;
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
        public bool isInternal;

        public SquareSide(List<Portal> portals, int crosses, bool isInternal)
        {
            this.portals = portals;
            this.crosses = crosses;
            this.isInternal = isInternal;
        }

        public SquareSide(int crosses, bool isInternal) {
            this.crosses = crosses;
            this.isInternal = isInternal;
        }
        public SquareSide(int crosses)
        {
            this.crosses = crosses;
        }

        public SquareSide Copy()
        {
            var newSide = new SquareSide(portals.Select((p) => p.Copy()).ToList(), crosses, isInternal);
            foreach (var p in newSide.portals)
                for (int i = 0; i < p.sides.Count(); i++)
                    if (p.sides[i] == this) { p.sides[i] = newSide; break; }
            return newSide;
        }

        public SquareSide ShallowCopy()
        {
            return new SquareSide(portals, crosses, isInternal);
        }

        public override bool Equals(object? obj)
        {
            /// if () idk, checknout aby objekt bl Node

            SquareSide side = obj as SquareSide;

            return side.portals == this.portals;
        }
        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashId = portals.GetHashCode();


            //Calculate the hash code for the product.
            return hashId;
        }
    }
}
