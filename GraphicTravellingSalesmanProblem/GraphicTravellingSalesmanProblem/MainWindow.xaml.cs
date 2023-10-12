using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TravellingSalesmanProblem.Algorithms.TSPAlgorithms;
using TravellingSalesmanProblem.GraphStructures;

namespace GraphicTravellingSalesmanProblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Line xAxisLine, yAxisLine;
        private double xAxisStart = 100, yAxisStart = 100, interval = 0.5, lowestX = -1, lowestY = -5, highestX = 5, highestY = 6;
        private Polyline chartPolyline;

        private Point origin;
        private List<Node> values;
        private List<Node> path;


        public MainWindow()
        {
            InitializeComponent();

            var rnd = new Random();

            Graph graph = new Graph( new List<Node>()
            {
                new Node(0, rnd.NextDouble() * rnd.Next(-20, 500), rnd.NextDouble() * rnd.Next(-20, 500)),
                new Node(1, rnd.NextDouble() * rnd.Next(-20, 500), rnd.NextDouble() * rnd.Next(-20, 500)),
                new Node(2, rnd.NextDouble() * rnd.Next(-20, 500), rnd.NextDouble() * rnd.Next(-20, 500)),
                new Node(3, rnd.NextDouble() * rnd.Next(-20, 500), rnd.NextDouble() * rnd.Next(-20, 500)),
                new Node(4, rnd.NextDouble() * rnd.Next(-20, 500), rnd.NextDouble() * rnd.Next(-20, 500))

            });

            values = graph.nodes;
            var bounds = graph.GetExtremeCoordinatesValues();
            lowestX = bounds.MinX;
            lowestY = bounds.MinY;
            highestX = bounds.MaxX;
            highestY = bounds.MaxY;

            Paint();
            
            bounds = graph.GetExtremeCoordinatesValues();
            lowestX = bounds.MinX;
            lowestY = bounds.MinY;
            highestX = bounds.MaxX;
            highestY = bounds.MaxY;

            path = KernighanLin.FindShortestPath(graph);

            interval = 4;

            Paint();

            this.StateChanged += (sender, e) => Paint();
            this.SizeChanged += (sender, e) => Paint();
        }


        public void Paint()
        {
            try
            {
                if (this.ActualWidth > 0 && this.ActualHeight > 0)
                {
                    chartCanvas.Children.Clear();

                    int lowestXValue = (int)Math.Ceiling(Math.Abs(lowestX) / interval);
                    if (lowestX < 0) lowestXValue = 0 - lowestXValue;

                    int lowestYValue = (int)Math.Ceiling(Math.Abs(lowestY) / interval);
                    if (lowestY < 0) lowestYValue = 0 - lowestYValue;

                    int highestXValue = (int)Math.Ceiling(Math.Abs(highestX) / interval);
                    if (highestX < 0) highestXValue = 0 - highestXValue;

                    int highestYValue = (int)Math.Ceiling(Math.Abs(highestY) / interval);
                    if (highestY < 0) highestYValue = 0 - highestYValue;

                    int numberOfVerticalLines = Math.Abs(highestXValue - lowestXValue);

                    int numberOfHorizontalLines = Math.Abs(highestYValue - lowestYValue);

                    (double X1, double X2, double Y1, double Y2) actualBounds = (xAxisStart, this.ActualWidth - xAxisStart, yAxisStart - 50, this.ActualHeight - yAxisStart);

                    double trueHorizontalSpacing = Math.Abs(actualBounds.X1 - actualBounds.X2) / numberOfVerticalLines;
                    double trueVerticalSpacing = Math.Abs(actualBounds.Y1 - actualBounds.Y2) / numberOfHorizontalLines;

                    // axis lines
                    xAxisLine = new Line()
                    {
                        X1 = actualBounds.X1,
                        Y1 = this.ActualHeight - yAxisStart + lowestYValue * trueVerticalSpacing,
                        X2 = actualBounds.X2,
                        Y2 = this.ActualHeight - yAxisStart + lowestYValue * trueVerticalSpacing,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                    };
                    yAxisLine = new Line()
                    {
                        X1 = xAxisStart - lowestXValue * trueHorizontalSpacing,
                        Y1 = actualBounds.Y1,
                        X2 = xAxisStart - lowestXValue * trueHorizontalSpacing,
                        Y2 = actualBounds.Y2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                    };

                    origin = new Point(yAxisLine.X1, xAxisLine.Y1);

                    var xTextBlock0 = new TextBlock() { Text = $"{0}", FontSize = 15 };
                    chartCanvas.Children.Add(xTextBlock0);
                    Canvas.SetLeft(xTextBlock0, origin.X + 6); // 6 = thickness os je 5, aby to bylo vidět
                    Canvas.SetTop(xTextBlock0, origin.Y + 6);

                    // y axis lines

                    double xValue = lowestXValue * interval;
                    double xPoint = xAxisLine.X1;

                    while (xValue <= highestXValue * interval)
                    {
                        var line = new Line()
                        {
                            X1 = xPoint,
                            Y1 = yAxisStart - 50,
                            X2 = xPoint,
                            Y2 = this.ActualHeight - yAxisStart,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 5,
                            Opacity = 1,
                        };

                        chartCanvas.Children.Add(line);

                        var textBlock = new TextBlock { Text = $"{xValue}", };

                        chartCanvas.Children.Add(textBlock);
                        Canvas.SetLeft(textBlock, xPoint - 12.5);
                        Canvas.SetTop(textBlock, line.Y2 + 5);

                        xPoint += trueHorizontalSpacing;
                        xValue += interval;
                    }

                    // x axis lines

                    var yValue = highestYValue * interval;
                    double yPoint = yAxisLine.Y1;
                    while (yValue >= lowestYValue * interval)
                    {
                        var line = new Line()
                        {
                            X1 = xAxisStart,
                            Y1 = yPoint,
                            X2 = this.ActualWidth - xAxisStart,
                            Y2 = yPoint,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 5,
                            Opacity = 1,
                        };

                        chartCanvas.Children.Add(line);

                        var textBlock = new TextBlock() { Text = $"{yValue}" };
                        chartCanvas.Children.Add(textBlock);
                        Canvas.SetLeft(textBlock, line.X1 - 30);
                        Canvas.SetTop(textBlock, yPoint - 10);

                        yPoint += trueVerticalSpacing;
                        yValue -= interval;
                    }



                    chartCanvas.Children.Add(xAxisLine);
                    chartCanvas.Children.Add(yAxisLine);


                    // showing where are the connections points
                    var ellipseRadius = 10;
                    foreach (var val in values)
                    {
                        Ellipse oEllipse = new Ellipse()
                        {
                            Fill = Brushes.Red,
                            Width = ellipseRadius,
                            Height = ellipseRadius,
                            Opacity = 1,
                        };

                        chartCanvas.Children.Add(oEllipse);
                        Canvas.SetLeft(oEllipse, origin.X + (val.x / interval) * trueHorizontalSpacing - ellipseRadius/2);
                        Canvas.SetTop(oEllipse, origin.Y - (val.y / interval) * trueVerticalSpacing - ellipseRadius/2);
                    }

                    for (int i = 0; i < path.Count; i++)
                    {
                        var node1 = path[i];
                        var node2 = path[(i + 1) % path.Count];

                        var line = new Line()
                        {
                            X1 = origin.X + (node1.x / interval) * trueHorizontalSpacing,
                            Y1 = origin.Y - (node1.y / interval) * trueVerticalSpacing,
                            X2 = origin.X + (node2.x / interval) * trueHorizontalSpacing,
                            Y2 = origin.Y - (node2.y / interval) * trueVerticalSpacing,
                            Stroke = Brushes.Blue,
                            StrokeThickness = 1,
                            Opacity = 1,
                        };

                        chartCanvas.Children.Add(line);
                    }

                    foreach (var val in values)
                    {
                        var xTextBlock1 = new TextBlock() { Text = $"{val}", FontSize = 15 };
                        chartCanvas.Children.Add(xTextBlock1);
                        Canvas.SetLeft(xTextBlock1, origin.X + (val.x / interval) * trueHorizontalSpacing);
                        Canvas.SetTop(xTextBlock1, origin.Y - (val.y / interval) * trueVerticalSpacing);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class Holder
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point Point { get; set; }

        public Holder()
        {
        }
    }

    public class Value
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Value(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
