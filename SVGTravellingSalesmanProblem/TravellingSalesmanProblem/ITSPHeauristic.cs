using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem
{
    public interface ITSPHeauristic
    {
        public bool FindTour(string sourcePath, string destinationPath);
    }
}
