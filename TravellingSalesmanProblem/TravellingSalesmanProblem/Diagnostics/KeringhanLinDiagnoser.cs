using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Diagnostics
{
    public class KeringhanLinDiagnoser
    {
        public MethodInfo FindShortestPath = new("FindShortestPath");
        public MethodInfo SetupInitialState = new("SetupInitialState");
        public MethodInfo SetupEdges = new("SetupEdges");
        public ImprovePathMathodInfo ImprovePath = new("ImprovePath");
        public MethodInfo NonsequentialExchange = new("NonsequentialExchange");
        public MethodInfo GetFruitlessNodesForPath = new("GetFruitlessNodesForPath");
        public MethodInfo FindNode2 = new("FindNode2");
        public MethodInfo RestoreState = new("RestoreState");
        public MethodInfo UpdatePartialSum = new("UpdatePartialSum");
        public MethodInfo UpdateLocalOptimum = new("UpdateLocalOptimum");
        public MethodInfo UpdateLocallyOptimalPaths = new("UpdateLocallyOptimalPaths");
        public MethodInfo UpdateGoodEdges = new("UpdateGoodEdges");
        public MethodInfo UpdateShortestPath = new("UpdateShortestPath");
        public MethodInfo GetEdges = new("GetEdges");
        public MethodInfo ReconnectEdges = new("ReconnectEdges");
        public MethodInfo ReconnectEdgesAlternativeBrokenEdge2Option1 = new("ReconnectEdgesAlternativeBrokenEdge2Option1");
        public MethodInfo ReconnectEdgesAlternativeBrokenEdge2Option2 = new("ReconnectEdgesAlternativeBrokenEdge2Option2");
        public MethodInfo ReconnectEdgesNonsequentialExchange = new("ReconnectEdgesNonsequentialExchange");

        public void Print()
        {
            var t = this.GetType();
            var p = t.GetFields();
            foreach (var property in p)
            {
                var a = property.GetValue(this).GetType();
                if (property.GetValue(this).GetType().Name == "ImprovePathMathodInfo")
                {
                    ((ImprovePathMathodInfo)property.GetValue(this)).Print();
                }
                else
                {
                    ((MethodInfo)property.GetValue(this)).Print();
                }
            }


            //FindShortestPath.Print();
            //SetupInitialState.Print();
            //SetupEdges.Print();
            //ImprovePath.Print();
            //NonsequentialExchange.Print();
            //GetFruitlessNodesForPath.Print();
            //FindNode2.Print();
            //RestoreState.Print();
            //UpdatePartialSum.Print();
            //UpdateLocalOptimum.Print();
            //UpdateLocallyOptimalPaths.Print();
            //UpdateGoodEdges.Print();
            //UpdateShortestPath.Print();
        }
    }
}
