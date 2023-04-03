using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class InternalSquare
    {
        public SquareSide leftSide;
        public SquareSide rightSide;
        public SquareSide topSide;
        public SquareSide bottomSide;

        public InternalSquare(SquareSide leftSide, SquareSide rightSide, SquareSide topSide, SquareSide bottomSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            this.topSide = topSide;
            this.bottomSide = bottomSide;
        }

        public InternalSquare Copy() => new InternalSquare(leftSide.Copy(), rightSide.Copy(), topSide.Copy(), bottomSide.Copy());

        public List<SquareSide> GetSquareSides() => new List<SquareSide>() { leftSide, rightSide, topSide, bottomSide };  //ref

        public List<Portal> GetPortals() => GetSquareSides().SelectMany((side) => side.portals).ToList();

        public List<SquareSide> GetInternalSides() => GetSquareSides().Where((side) => side.isInternal).ToList();

        public List<SquareSide> GetOutsideSides() => GetSquareSides().Where((side) => !side.isInternal).ToList();

        public bool ContainsPortal(Portal portal)
        {
            foreach (var portalSide in portal.sides)
                foreach (var squareSide in GetSquareSides())
                    if (portalSide == squareSide) return true;
            return false;
        }

        public bool ContainsPortals(Portal firstPortal, Portal secondPortal) => ContainsPortal(firstPortal) && ContainsPortal(secondPortal);

        public List<Portal> ReachableInternalPortals(Portal portal)
        {
            var reachablePortals = new List<Portal>();

            if (ContainsPortal(portal))
                reachablePortals = GetInternalSides().Where((side) => side.crosses > 0).SelectMany((side) => side.portals).Distinct().ToList();

            return reachablePortals;
        }
    }
}
