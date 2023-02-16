using System;
using System.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms
{
    public class NiceInstanceTransformer
    {
        public Graph TransformToNiceInstance(Graph graph, float epsilon)
        {
            var boundingValues = graph.GetExtremeCoordinatesValues();
            double sideLengthOfABondingSquare = Math.Max(Math.Abs(boundingValues.MinX - boundingValues.MaxX), Math.Abs(boundingValues.MinY - boundingValues.MaxY));
            double spacing = epsilon * sideLengthOfABondingSquare / (2 * graph.nodes.Count());

            var grid = new Grid(spacing, (boundingValues.MinX, boundingValues.MinY));

            Console.WriteLine(graph.ToString());
            var transformedGraph = grid.SnapGraphNodesToGrid(graph);
            Console.WriteLine(graph.ToString());
            var factor = 8 * graph.nodes.Count / (epsilon * sideLengthOfABondingSquare);
            transformedGraph.Scale(factor);
            Console.WriteLine(graph.ToString());
            transformedGraph.MoveGraphInDirections(-boundingValues.MinX * factor, -boundingValues.MinY * factor);
            Console.WriteLine(graph.ToString());

            return transformedGraph;
        }


    }
}
