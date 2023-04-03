using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public  class PortalPairing
    {
        public Portal enterPortal;
        public Portal exitPortal;
        public InternalSquare? square;

        public PortalPairing(Portal enterPortal, Portal exitPortal, InternalSquare square)
        {
            this.enterPortal = enterPortal;
            this.exitPortal = exitPortal;
            this.square = square;
        }
        public PortalPairing(Portal enterPortal, Portal exitPortal)
        {
            this.enterPortal = enterPortal;
            this.exitPortal = exitPortal;
        }
    }
}
