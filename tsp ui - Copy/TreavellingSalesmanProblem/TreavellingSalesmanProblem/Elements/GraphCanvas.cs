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
        private readonly double offset = 20;

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

            var (MaxX, MaxY, MinX, MinY) = graph.GetExtremeCoordinatesValues();
            lowestX = MinX;
            lowestY = MinY;
            highestX = MaxX;
            highestY = MaxY;

            Redraw(width, height);
        }

        public void Redraw(double width, double height)
        {
            this.Margin = new Thickness(0, 0, width, 0);
            this.Children.Clear();

            width -= 40 + offset * 2;
            height -= 20 + offset * 2;

            int numberOfLines = (int)Math.Min(width, height) / 40;

            double intervalY = Math.Round(height / numberOfLines, 2);
            double intervalX = Math.Round(width / numberOfLines, 2);

            double xStep = (Math.Max(highestX, 0) - Math.Min(lowestX, 0)) / numberOfLines;
            double yStep = (Math.Max(highestY, 0) - Math.Min(lowestY, 0)) / numberOfLines;

            var lowestPoint = GetRelativeCoordinates(lowestX, lowestY, width, height);
            var originPoint = GetRelativeCoordinates(0, 0, width, height);

            // y axis lines
            double numberOfLinesOnTheLeft = Math.Max(originPoint.X / intervalX - 1, 0);
            int i = -1;
            int currentNumberOfLinesOnTheLeft = 0;
            while (currentNumberOfLinesOnTheLeft <= numberOfLinesOnTheLeft - 1)
            {
                var pt1 = new Point(originPoint.X + i * intervalX, Math.Max(lowestPoint.Y, originPoint.Y));
                var pt2 = new Point(originPoint.X + i * intervalX, 0 + offset);
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

                currentNumberOfLinesOnTheLeft++;
                i--;
            }

            i = 1;
            int currentNumberOfLinesOnTheRight = 0;
            while (currentNumberOfLinesOnTheRight <= (numberOfLines - numberOfLinesOnTheLeft - 2))
            {
                var pt1 = new Point(originPoint.X + i * intervalX, Math.Max(lowestPoint.Y, originPoint.Y));
                var pt2 = new Point(originPoint.X + i * intervalX, 0 + offset);
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
                currentNumberOfLinesOnTheRight++;
                i++;
            }

            //x axis lines
            double numberOfLinesOnTheTop = Math.Max(originPoint.Y / intervalY - 1, 0);
            i = 1;
            int currentNumberOfLinesOnTheBottom = 0;
            while (currentNumberOfLinesOnTheBottom <= (numberOfLines - numberOfLinesOnTheTop - 2))
            {
                var pt1 = new Point(Math.Min(lowestPoint.X, originPoint.X), originPoint.Y + i * intervalY);
                var pt2 = new Point(width + offset, originPoint.Y + i * intervalY);
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
                currentNumberOfLinesOnTheBottom++;
                i++;
            }

            i = -1;
            int currentNumberOfLinesOnTheTop = 0;
            while (currentNumberOfLinesOnTheTop <= numberOfLinesOnTheTop - 1)
            {
                var pt1 = new Point(Math.Min(lowestPoint.X, originPoint.X), originPoint.Y + i * intervalY);
                var pt2 = new Point(width + offset, originPoint.Y + i * intervalY);
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
                currentNumberOfLinesOnTheTop++;
                i--;
            }

            // main axis lines
            var xAxisLine = new Line()
            {
                X1 = 0 + offset,
                Y1 = originPoint.Y,
                X2 = width + offset,
                Y2 = originPoint.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
            };
            this.Children.Add(xAxisLine);

            var yAxisLine = new Line()
            {
                X1 = originPoint.X,
                Y1 = 0 + offset,
                X2 = originPoint.X,
                Y2 = height + offset,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
            };
            this.Children.Add(yAxisLine);

            // origin point
            var originLabel = new TextBlock() { Text = $"{0}", FontSize = 15 };
            this.Children.Add(originLabel);
            Canvas.SetLeft(originLabel, originPoint.X + 6);
            Canvas.SetTop(originLabel, originPoint.Y + 6);

            // path points
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

                var nodeCoords = GetRelativeCoordinates(node.X, node.Y, width, height);
                this.Children.Add(oEllipse);
                Canvas.SetLeft(oEllipse, nodeCoords.X - ellipseRadius);
                Canvas.SetTop(oEllipse, nodeCoords.Y - ellipseRadius);
            }

            // path edges
            for (int j = 0; j < path.Count; j++)
            {
                var node1 = path[j];
                var node2 = path[(j + 1) % path.Count];
                var node1Coords = GetRelativeCoordinates(node1.X, node1.Y, width, height);
                var node2Coords = GetRelativeCoordinates(node2.X, node2.Y, width, height);

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

            // y axis lines labels
            double xValue = 0 - xStep;
            numberOfLinesOnTheLeft = Math.Max(originPoint.X / intervalX - 1, 0);
            i = -1;
            currentNumberOfLinesOnTheLeft = 0;
            while (currentNumberOfLinesOnTheLeft <= numberOfLinesOnTheLeft - 1)
            {
                var textBlock = new TextBlock { Text = $"{Math.Round(xValue, 2)}", FontSize = 10 };

                this.Children.Add(textBlock);
                Canvas.SetLeft(textBlock, originPoint.X + i * intervalX - 12.5);
                Canvas.SetTop(textBlock, originPoint.Y + 5);

                xValue -= xStep;
                currentNumberOfLinesOnTheLeft++;
                i--;
            }

            xValue = 0 + xStep;
            i = 1;
            currentNumberOfLinesOnTheRight = 0;
            while (currentNumberOfLinesOnTheRight <= (numberOfLines - numberOfLinesOnTheLeft - 2))
            {
                var textBlock = new TextBlock { Text = $"{Math.Round(xValue, 2)}", FontSize = 10 };

                this.Children.Add(textBlock);
                Canvas.SetLeft(textBlock, originPoint.X + i * intervalX - 12.5);
                Canvas.SetTop(textBlock, originPoint.Y);

                xValue += xStep;
                currentNumberOfLinesOnTheRight++;
                i++;
            }

            //x axis lines labels
            double yValue = 0 - yStep;
            numberOfLinesOnTheTop = Math.Max(originPoint.Y / intervalY - 1, 0);
            i = 1;
            currentNumberOfLinesOnTheBottom = 0;
            while (currentNumberOfLinesOnTheBottom <= (numberOfLines - numberOfLinesOnTheTop - 2))
            {
                var textBlock = new TextBlock { Text = $"{Math.Round(yValue, 2)}", FontSize = 10 };

                this.Children.Add(textBlock);
                Canvas.SetLeft(textBlock, originPoint.X + 5);
                Canvas.SetTop(textBlock, originPoint.Y + i * intervalY - 5);

                yValue -= yStep;
                currentNumberOfLinesOnTheBottom++;
                i++;
            }

            yValue = 0 + yStep;
            i = -1;
            currentNumberOfLinesOnTheTop = 0;
            while (currentNumberOfLinesOnTheTop <= numberOfLinesOnTheTop - 1)
            {
                var textBlock = new TextBlock { Text = $"{Math.Round(yValue, 2)}", FontSize = 10 };

                this.Children.Add(textBlock);
                Canvas.SetLeft(textBlock, originPoint.X + 5);
                Canvas.SetTop(textBlock, originPoint.Y + i * intervalY - 5);

                yValue += yStep;
                currentNumberOfLinesOnTheTop++;
                i--;
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
            double newOriginY = height + (originY * (height / Math.Abs(Math.Min(lowestY, 0) - Math.Max(highestY, 0))));

            double newY = newOriginY - (y * height / Math.Abs(Math.Min(lowestY, 0) - Math.Max(highestY, 0)));

            return new Point(newX + offset, newY + offset);
        }
    }
}
