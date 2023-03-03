﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class SquareSide
    {
        public List<Portal> portals = new List<Portal>();
        public int crosses;

        public SquareSide(List<Portal> portals, int crosses)
        {
            this.portals = portals;
            this.crosses = crosses;
        }

        public SquareSide(int crosses) { this.crosses = crosses; }
    }
}