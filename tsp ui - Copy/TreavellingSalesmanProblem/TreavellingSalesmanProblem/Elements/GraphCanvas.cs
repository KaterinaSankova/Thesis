using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TSP.Elements
{
    public class GraphCanvas : Canvas
    {
        private double lowestX;
        private double highestX;
        private double lowestY;
        private double highestY;
        private Graph graph;
        private Path path;

        public GraphCanvas(Graph graph, Path path, double width, double height)
        {
            this.graph = graph;
            this.path = path;

            var bounds = graph.GetExtremeCoordinatesValues();
            lowestX = bounds.MinX;
            lowestY = bounds.MinY;
            highestX = bounds.MaxX;
            highestY = bounds.MaxY;

            Redraw(width, height);
        }
        public GraphCanvas(Graph graph, Path path, double width, double height, int row, int column) : this(graph, path, width, height)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }

        public void Redraw(double width, double height)
        {
            int numberOfLines = 10;
            this.Margin = new Thickness(0, 0, width, 0);
            width -= 40;
            height -= 20;
            double intervalY = Math.Round(height / numberOfLines, 2);
            double intervalX = Math.Round(width / numberOfLines, 2);
            double xStep = (Math.Max(highestX, 0) - Math.Min(lowestX, 0)) / numberOfLines;
            double yStep = (Math.Max(highestY, 0) - Math.Min(lowestY, 0)) / numberOfLines;

            if (width > 0 && height > 0)
            {
                this.Children.Clear();

                var lowestPoint = GetRelativeCoordinates(lowestX, lowestY, width, height);
                var originPoint = GetRelativeCoordinates(0, 0, width, height);



                // y axis lines
                double xValue = 0 - xStep;
                double numberOfLinesOnTheLeft = originPoint.X / intervalX;
                int i = -1;
                int currentNumberOfLinesOnTheLeft = 0;
                while (currentNumberOfLinesOnTheLeft <= numberOfLinesOnTheLeft - 1)
                {
                    Point pt1 = new Point(originPoint.X + i * intervalX, Math.Max(lowestPoint.Y, originPoint.Y));
                    Point pt2 = new Point(originPoint.X + i * intervalX, 0);
                    var line = new Line()
                    {
                        X1 = pt1.X,
                        Y1 = pt1.Y,
                        X2 = pt2.X,
                        Y2 = pt2.Y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 3,
                        Opacity = 1,
                    };

                    this.Children.Add(line);

                    var textBlock = new TextBlock { Text = $"{Math.Round(xValue, 2)}", FontSize = 10 };

                    this.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, pt1.X - 12.5);
                    Canvas.SetTop(textBlock, originPoint.Y + 5);

                    xValue -= xStep;
                    currentNumberOfLinesOnTheLeft++;
                    i--;
                }

                xValue = 0 + xStep;
                i = 1;
                int currentNumberOfLinesOnTheRight = 0;
                while (currentNumberOfLinesOnTheRight <= (numberOfLines - numberOfLinesOnTheLeft - 1))
                {
                    Point pt1 = new Point(originPoint.X + i * intervalX, Math.Max(lowestPoint.Y, originPoint.Y));
                    Point pt2 = new Point(originPoint.X + i * intervalX, 0);
                    var line = new Line()
                    {
                        X1 = pt1.X,
                        Y1 = pt1.Y,
                        X2 = pt2.X,
                        Y2 = pt2.Y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 3,
                        Opacity = 1,
                    };

                    this.Children.Add(line);

                    var textBlock = new TextBlock { Text = $"{Math.Round(xValue, 2)}", FontSize = 10 };

                    this.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, pt1.X - 12.5);
                    Canvas.SetTop(textBlock, originPoint.Y + 5);

                    xValue += xStep;
                    currentNumberOfLinesOnTheRight++;
                    i++;
                }

                //x axis lines
                double yValue = 0 - yStep;
                double numberOfLinesOnTheTop = originPoint.Y / intervalY;
                i = 1;
                int currentNumberOfLinesOnTheBottom = 0;
                while (currentNumberOfLinesOnTheBottom <= (numberOfLines - numberOfLinesOnTheTop - 1))
                {
                    Point pt1 = new Point(Math.Min(lowestPoint.X, originPoint.X), originPoint.Y + i * intervalY);
                    Point pt2 = new Point(width, originPoint.Y + i * intervalY);
                    var line = new Line()
                    {
                        X1 = pt1.X,
                        Y1 = pt1.Y,
                        X2 = pt2.X,
                        Y2 = pt2.Y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 3,
                        Opacity = 1,
                    };

                    this.Children.Add(line);

                    var textBlock = new TextBlock { Text = $"{Math.Round(yValue, 2)}", FontSize = 10 };

                    this.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, originPoint.X + 5);
                    Canvas.SetTop(textBlock, pt2.Y - 5);

                    yValue -= yStep;
                    currentNumberOfLinesOnTheBottom++;
                    i++;
                }

                yValue = 0 + yStep;
                i = -1;
                int currentNumberOfLinesOnTheTop = 0;
                while (currentNumberOfLinesOnTheTop <= numberOfLinesOnTheTop - 1)
                {
                    Point pt1 = new Point(Math.Min(lowestPoint.X, originPoint.X), originPoint.Y + i * intervalY);
                    Point pt2 = new Point(width, originPoint.Y + i * intervalY);
                    var line = new Line()
                    {
                        X1 = pt1.X,
                        Y1 = pt1.Y,
                        X2 = pt2.X,
                        Y2 = pt2.Y,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 3,
                        Opacity = 1,
                    };

                    this.Children.Add(line);

                    var textBlock = new TextBlock { Text = $"{Math.Round(yValue, 2)}", FontSize = 10 };

                    this.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, originPoint.X + 5);
                    Canvas.SetTop(textBlock, pt2.Y - 5);

                    yValue += yStep;
                    currentNumberOfLinesOnTheTop++;
                    i--;
                }

                // axis lines
                var xAxisLine = new Line()
                {
                    X1 = 0,
                    Y1 = originPoint.Y,
                    X2 = width,
                    Y2 = originPoint.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3,
                };
                this.Children.Add(xAxisLine);

                var yAxisLine = new Line()
                {
                    X1 = originPoint.X,
                    Y1 = 0,
                    X2 = originPoint.X,
                    Y2 = height,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3,
                };
                this.Children.Add(yAxisLine);

                var xTextBlock0 = new TextBlock() { Text = $"{0}", FontSize = 15 };
                this.Children.Add(xTextBlock0);
                Canvas.SetLeft(xTextBlock0, originPoint.X + 6); // 6 = thickness os je 5, aby to bylo vidět
                Canvas.SetTop(xTextBlock0, originPoint.Y + 6);

                // showing where are the connections points
                var ellipseRadius = 2;
                foreach (var node in graph.nodes)
                {
                    Ellipse oEllipse = new Ellipse()
                    {
                        Fill = Brushes.DarkBlue,
                        Width = ellipseRadius * 2,
                        Height = ellipseRadius * 2,
                        Opacity = 0.5
                    };

                    var nodeCoords = GetRelativeCoordinates(node.x, node.y, width, height);
                    this.Children.Add(oEllipse);
                    Canvas.SetLeft(oEllipse, nodeCoords.X - ellipseRadius);
                    Canvas.SetTop(oEllipse, nodeCoords.Y - ellipseRadius);

                    if (graph.nodes.Count <= 20)
                    {
                        var xTextBlock1 = new TextBlock() { Text = $"{node}", FontSize = 8 };
                        this.Children.Add(xTextBlock1);
                        Canvas.SetLeft(xTextBlock1, nodeCoords.X + 5);
                        Canvas.SetTop(xTextBlock1, nodeCoords.Y + 5);
                    }
                }

                for (int j = 0; j < path.Count; j++)
                {
                    var node1 = path[j];
                    var node2 = path[(j + 1) % path.Count];
                    var node1Coords = GetRelativeCoordinates(node1.x, node1.y, width, height);
                    var node2Coords = GetRelativeCoordinates(node2.x, node2.y, width, height);

                    var line = new Line()
                    {
                        X1 = node1Coords.X,
                        Y1 = node1Coords.Y,
                        X2 = node2Coords.X,
                        Y2 = node2Coords.Y,
                        Stroke = Brushes.Blue,
                        StrokeThickness = 1,
                        Opacity = 0.5,
                    };

                    this.Children.Add(line);
                }

            }
        }

        private Point GetRelativeCoordinates(double x, double y, double width, double height)
        {
            double newX = x * (width / Math.Abs(Math.Min(lowestX, 0) - Math.Max(highestX, 0)));
            if (lowestX < 0)
                newX += Math.Abs(lowestX) * (width / Math.Abs(Math.Min(lowestX, 0) - Math.Max(highestX, 0)));

            double originY = lowestY;
            if (lowestY > 0)
                originY = 0;
            double a = Math.Abs(Math.Min(lowestY, 0) - Math.Max(highestY, 0));
            double newOriginY = height + (originY * (height / Math.Abs(Math.Min(lowestY, 0) - Math.Max(highestY, 0))));

            double newY = newOriginY - (y * height / Math.Abs(Math.Min(lowestY, 0) - Math.Max(highestY, 0)));

            return new Point(newX, newY);
        }
    }
}
