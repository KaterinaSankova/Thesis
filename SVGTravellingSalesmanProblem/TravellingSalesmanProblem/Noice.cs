using SVGTravellingSalesmanProblem.GraphStructures;

namespace SVGTravellingSalesmanProblem
{
    public class Noice
    {
        (double MinX, double MaxX, double MinY, double MaxY) bounds = (-1, 4, -2, 3);
        double spacing = 0.5;

        public Graph SnapToGrid(Graph graph, double spacing)
        {
            spacing = spacing;
            bounds = graph.GetExtremeCoordinatesValues();
            return new Graph(graph.nodes.Select((node) => SnapToGrid(node)).ToList());
        }

        private Node SnapToGrid(Node point)
        {
            Node relativePoint = new Node(point.id, point.x - bounds.MinX, point.y - bounds.MinY);
            Node snappedPoint = point;

            int closestVerticleGridLine = (int)(relativePoint.x / spacing); //closest left verticle grid line to relativePoint 
            double middleX = (closestVerticleGridLine * spacing + (closestVerticleGridLine + 1) * spacing) / 2; //middle x value between 2 closest grid lines to relativePoint

            if (relativePoint.x <= middleX) 
                snappedPoint.x = middleX - spacing / 2 + bounds.MinX;
            else
                snappedPoint.x = middleX + spacing / 2 + bounds.MinX;

            int closestHorizontalGridLine = (int)(relativePoint.y / spacing);
            double middleY = (closestHorizontalGridLine * spacing + (closestHorizontalGridLine + 1) * spacing) / 2;

            if (relativePoint.y <= middleY)
                snappedPoint.y = middleY - spacing / 2 + bounds.MinY;
            else
                snappedPoint.y = middleY + spacing / 2 + bounds.MinY;

            /*
            int row, column;
            row = (int)(Math.Abs(bounds.MaxY - bounds.MinY) / spacing) - 1;
            column = closestVerticleGridLine;

            if (closestHorizontalGridLine > row) //points on the top side of the grid
                closestHorizontalGridLine--;
            row -= closestHorizontalGridLine;

            if (point.x == bounds.MaxX) //points on the right side o the grid
                column = (int)(Math.Abs(bounds.MaxX - bounds.MinX) / spacing) - 1;
            */

            return snappedPoint;

        }
        /*
        public void Test()
        {
            Console.WriteLine($"Spacing: {spacing}\nBounds:\n\t{bounds.MaxY}\n{bounds.MinX}\t\t{bounds.MaxX}\n\t{bounds.MinY}\n");
            var squareQuadrants = new List<(double x, double Y)> { (0.1, 0.1), (0.4, 0.2), (0.3, 0.3), (0.2, 0.4) };
            var corners = new List<(double x, double Y)> { //left bottom corners of squares in corners of grid
                    (bounds.MinX, bounds.MinY), //LB
                    (bounds.MaxX - spacing, bounds.MinY), //RB
                    (bounds.MaxX - spacing, bounds.MaxY - spacing), //RT
                    (bounds.MinX, bounds.MaxY - spacing) //LT
            };
            var squaresInGridQuadrants = new List<(double x, double Y)> { (-0.5, -1), (2.5, -1.5), (0, 1.5), (-1, 0) }; //left bottom corners of randomly chosen squares, 1 from each quadrant of a grid

            var squareCorners = new List<(double x, double Y)> { (0, 0), (spacing, 0), (spacing, spacing), (0, spacing) };
            var cornersResults = new List<(int row, int column)> {(9, 0), (9, 9), (0, 9), (0, 0)};
            var squaresInGridQuadrantsResults = new List<(int row, int column)> { (7, 1), (8, 7), (2, 2), (5, 0) };
            var extremes = new List<(double x, double Y)> { (spacing/2, 0), (spacing, spacing/2), (spacing/2, spacing), (0, spacing/2) };

            Console.WriteLine("\n######################    CORNERS  ######################");
            for (int i = 0; i < 4; i++)
            {
                var point = corners[i];
                //Console.WriteLine($"\n****Point: [{point.x};{point.Y}]***");

                Console.WriteLine("# Inside #");
                for (int j = 0; j < 4; j++)
                {
                    var squareCorner = squareCorners[j];
                    var deviation = squareQuadrants[j];
                    //Console.WriteLine($"Deviation: [{deviation.x};{deviation.Y}]");
                    Console.WriteLine($"\nPoint: [{point.x + deviation.x};{point.Y + deviation.Y}]");
                    Console.WriteLine($"Expected square index: [{cornersResults[i].row}][{cornersResults[i].column}]");
                    Console.WriteLine($"Expected square corner: [{point.x + squareCorner.x}; {point.Y + squareCorner.Y}]");
                    SnapNodeToGrid(new Node(0, point.x + deviation.x, point.Y + deviation.Y));
                }

                Console.WriteLine("# Sides #");
                for (int j = 0; j < 4; j++)
                {
                    var squareCorner = squareCorners[j];
                    var deviation = extremes[j];
                    //Console.WriteLine($"Deviation: [{deviation.x};{deviation.Y}]");
                    Console.WriteLine($"\nPoint: [{point.x + deviation.x};{point.Y + deviation.Y}]");
                    Console.WriteLine($"Expected square index: [{cornersResults[i].row}][{cornersResults[i].column}]");
                    Console.WriteLine($"Expected square corner: [{point.x + squareCorner.x}; {point.Y + squareCorner.Y}]");
                    SnapNodeToGrid(new Node(0, point.x + deviation.x, point.Y + deviation.Y));
                }
            }

            Console.WriteLine("\n######################     SQUARES IN GRID QUADRANTS   ######################");
            for (int i = 0; i < 4; i++)
            {
                var point = squaresInGridQuadrants[i];
                //Console.WriteLine($"\n****Point: [{point.x};{point.Y}]***");

                Console.WriteLine("# Inside #");
                for (int j = 0; j < 4; j++)
                {
                    var squareCorner = squareCorners[j];
                    var deviation = squareQuadrants[j];
                    //Console.WriteLine($"Deviation: [{deviation.x};{deviation.Y}]");
                    Console.WriteLine($"\nPoint: [{point.x + deviation.x};{point.Y + deviation.Y}]");
                    Console.WriteLine($"Expected square index: [{squaresInGridQuadrantsResults[i].row}][{squaresInGridQuadrantsResults[i].column}]");
                    Console.WriteLine($"Expected square corner: [{point.x + squareCorner.x}; {point.Y + squareCorner.Y}]");
                    SnapNodeToGrid(new Node(0, point.x + deviation.x, point.Y + deviation.Y));
                }

                Console.WriteLine("# Sides #");
                for (int j = 0; j < 4; j++)
                {
                    var squareCorner = squareCorners[j];
                    var deviation = extremes[j];
                    //Console.WriteLine($"Deviation: [{deviation.x};{deviation.Y}]");
                    Console.WriteLine($"\nPoint: [{point.x + deviation.x};{point.Y + deviation.Y}]");
                    Console.WriteLine($"Expected square index: [{squaresInGridQuadrantsResults[i].row}][{squaresInGridQuadrantsResults[i].column}]");
                    Console.WriteLine($"Expected square corner: [{point.x + squareCorner.x}; {point.Y + squareCorner.Y}]");
                    SnapNodeToGrid(new Node(0, point.x + deviation.x, point.Y + deviation.Y));
                }
            }
        }
        */
	}
}
