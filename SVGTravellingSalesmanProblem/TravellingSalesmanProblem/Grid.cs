using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem
{
    public class Grid
    {
        double spacing;
        (double X, double Y) intersectionPoint; //arbitrary point on an interesection of grid lines

        public Grid(double spacing, (double X, double Y) intersectionPoint)
        {
            this.spacing = spacing;
            this.intersectionPoint = intersectionPoint;
        }

        public Graph SnapGraphNodesToGrid(Graph graph)
        {
            this.spacing = spacing;
            var bounds = graph.GetExtremeCoordinatesValues();
            intersectionPoint = (bounds.MinX, bounds.MaxX);
            return new Graph(graph.nodes.Select((node) => SnapNodeToGrid(node)).ToList());
        }

        private Node SnapNodeToGrid(Node point)
        {
            Node relativePoint = new Node(point.id, point.x - intersectionPoint.X, point.y - intersectionPoint.Y);
            Node snappedPoint = point;

            int closestVerticleGridLine = (int)(relativePoint.x / spacing); //closest left verticle grid line to relativePoint 
            double middleX = (closestVerticleGridLine * spacing + (closestVerticleGridLine + 1) * spacing) / 2; //middle x value between 2 closest grid lines to relativePoint

            if (relativePoint.x <= middleX)
                snappedPoint.x = middleX - spacing / 2 + intersectionPoint.X;
            else
                snappedPoint.x = middleX + spacing / 2 + intersectionPoint.X;

            int closestHorizontalGridLine = (int)(relativePoint.y / spacing);
            double middleY = (closestHorizontalGridLine * spacing + (closestHorizontalGridLine + 1) * spacing) / 2;

            if (relativePoint.y <= middleY)
                snappedPoint.y = middleY - spacing / 2 + intersectionPoint.Y;
            else
                snappedPoint.y = middleY + spacing / 2 + intersectionPoint.Y;

            return snappedPoint;

        }
    }
}
