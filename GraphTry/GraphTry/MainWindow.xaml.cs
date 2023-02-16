using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TravellingSalesmanProblem.GraphStructures;

namespace GraphTry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Line xAxisLine, yAxisLine;
        private double xAxisStart = 100, yAxisStart = 100, interval = 0.5, lowestX = -1, lowestY = -5, highestX = 7, highestY = 9;
        private Polyline chartPolyline;

        private Point origin;
        private List<Node> values;


        public MainWindow()
        {
            InitializeComponent();

            values = new List<Node>()
            {
                //new Value(0,0),
                //new Value(100,100),
                //new Value(200,200),
                //new Value(300,300),
                //new Value(400,200),
                //new Value(500,500),
                //new Value(600,500),
                //new Value(700,500),
                //new Value(800,500),
                //new Value(900,600),
                //new Value(1000,200),
                //new Value(1100,100),
                //new Value(1200,400),

                //new Value(0,0),
                //new Value(100,200),
                //new Value(200,100),
                //new Value(300,200),
                //new Value(400,300),
                //new Value(500,400),
                //new Value(600,500),
                //new Value(700,400),
                //new Value(800,500),
                //new Value(900,600),
                //new Value(1000,300),
                //new Value(1100,100),
                //new Value(1200,400),

                //new Node(0, -1, -2),
                //new Node(1, -0.7, 0.3),
                //new Node(2, 2, 2.7),
                //new Node(3, 4, -2),
                //new Node(4, 4, 3)

                new Node(0, 100, 100),
                new Node(1, 200, 100),
                new Node(2, 100, 200),
                new Node(3, 300, 500),
                new Node(4, 400, 300)

            };

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
                    if (lowestX < 0) highestXValue = 0 - highestXValue;

                    int highestYValue = (int)Math.Ceiling(Math.Abs(highestY) / interval);
                    if (lowestY < 0) highestYValue = 0 - highestYValue;

                    int numberOfVerticalLines = Math.Abs(highestXValue - lowestXValue);

                    int numberOfHorizontalLines = Math.Abs(highestYValue - lowestYValue);

                    // axis lines
                    xAxisLine = new Line()
                    {
                        X1 = xAxisStart,
                        Y1 = this.ActualHeight - yAxisStart + lowestYValue * interval,
                        X2 = this.ActualWidth - xAxisStart,
                        Y2 = this.ActualHeight - yAxisStart + lowestYValue * interval,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                    };
                    yAxisLine = new Line()
                    {
                        X1 = xAxisStart - lowestXValue * interval,
                        Y1 = yAxisStart - 50,
                        X2 = xAxisStart - lowestXValue * interval,
                        Y2 = this.ActualHeight - yAxisStart,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5,
                    };

                    // y axis lines
                    double trueHorizontalSpacing = Math.Abs(xAxisLine.X1 - xAxisLine.X2) / numberOfVerticalLines;

                    double xValue = lowestXValue * interval;
                    double xPoint = xAxisLine.X1;

                    while (xPoint < xAxisLine.X2)
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


                    var yTextBlock0 = new TextBlock() { Text = $"{0}" };
                    chartCanvas.Children.Add(yTextBlock0);
                    Canvas.SetLeft(yTextBlock0, origin.X - 20);
                    Canvas.SetTop(yTextBlock0, origin.Y - 10);

                    // x axis lines
                    double trueVerticalSpacing = Math.Abs(yAxisLine.Y1 - yAxisLine.Y2) / numberOfHorizontalLines;

                    var yValue = highestYValue * interval;
                    double yPoint = yAxisLine.Y1;
                    while (yPoint < yAxisLine.Y2)
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

                    origin = new Point(yAxisLine.X1, xAxisLine.Y1);

                    var xTextBlock0 = new TextBlock() { Text = $"{0}", FontSize=15 };
                    chartCanvas.Children.Add(xTextBlock0);
                    Canvas.SetLeft(xTextBlock0, origin.X + 6); // 6 = thickness os je 5, aby to bylo vidět
                    Canvas.SetTop(xTextBlock0, origin.Y + 6);


                    // polyline
                    chartPolyline = new Polyline()
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(68, 114, 196)),
                        StrokeThickness = 10,
                    };
                    chartCanvas.Children.Add(chartPolyline);

                    // showing where are the connections points
                    foreach (var val in values)
                    {
                        Ellipse oEllipse = new Ellipse()
                        {
                            Fill = Brushes.Red,
                            Width = 10,
                            Height = 10,
                            Opacity = 1,
                        };

                        chartCanvas.Children.Add(oEllipse);
                        Canvas.SetLeft(oEllipse, origin.X + val.x - 5);
                        Canvas.SetTop(oEllipse, origin.Y - val.y - 5);
                    }

                    foreach (var val1 in values)
                    {
                        foreach (var val2 in values)
                        {
                            var line = new Line()
                            {
                                X1 = origin.X + val1.x - 5,
                                Y1 = origin.Y - val1.y - 5,
                                X2 = origin.X + val2.x - 5,
                                Y2 = origin.Y - val2.y - 5,
                                Stroke = Brushes.AliceBlue,
                                StrokeThickness = 5,
                                Opacity = 1,
                            };

                            chartCanvas.Children.Add(line);
                        }
                    }

                    foreach (var val in values)
                    {
                        var xTextBlock1 = new TextBlock() { Text = $"{val}", FontSize = 15 };
                        chartCanvas.Children.Add(xTextBlock1);
                        Canvas.SetLeft(xTextBlock1, origin.X + val.x);
                        Canvas.SetTop(xTextBlock1, origin.Y - val.y);
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
