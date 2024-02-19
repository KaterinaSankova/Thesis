using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Enum;
using TSP.Sections;

namespace TSP
{
    public partial class MainWindow : Window
    {
        private InputSection inputSection;
        private OutputSection outputSection;
        private AlgorithmsSection algorithmsSection;
        private StopWatchSection stopWatchSection;
        private ResultSection resultSection = new ResultSection();

        private readonly NearestAddition nearestAddition = new();
        private readonly DoubleTree doubleTree = new();
        private readonly Christofides christofides = new();
        private readonly KernighanLin kernighanLin = new();

        public MainWindow()
        {
            InitializeComponent();
            DrawStartScreen();

            this.StateChanged += (sender, e) => resultSection.RedrawGraphs();
            this.SizeChanged += (sender, e) => resultSection.RedrawGraphs();
        }

        public void DrawStartScreen()
        {
            GridExtentions.RestoreGrid(Grid);
            CreateStartScreenGrid();

            Label name = new Label();   //nazev
            name.Content = "Travelling Salesman Problem";
            name.SetValue(Grid.RowProperty, 1);
            name.SetValue(Grid.ColumnProperty, 1);
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.VerticalAlignment = VerticalAlignment.Center;
            name.FontSize = 30;
            name.FontWeight = FontWeights.Bold;
            name.Foreground = (Brush)new BrushConverter().ConvertFrom("#ecc400");
            Grid.Children.Add(name);

            CreateStartScreenButton("Find shortest path", 3, 1, DrawFindShortestPathScreen);
            CreateStartScreenButton("Compare algorithms", 5, 1, DrawCompareAlgorithmsScreen);
        }

        public void CreateStartScreenGrid()
        {
            GridExtentions.AddRowToGrid(Grid, 1, GridUnitType.Star);
            GridExtentions.AddRowToGrid(Grid, 100);
            GridExtentions.AddRowToGrid(Grid, 70);
            GridExtentions.AddRowToGrid(Grid, 50);
            GridExtentions.AddRowToGrid(Grid, 50);
            GridExtentions.AddRowToGrid(Grid, 50);
            GridExtentions.AddRowToGrid(Grid, 1, GridUnitType.Star);

            GridExtentions.AddColumnToGrid(Grid, 1, GridUnitType.Star);
            GridExtentions.AddColumnToGrid(Grid, 500);
            GridExtentions.AddColumnToGrid(Grid, 1, GridUnitType.Star);
        }

        private void CreateStartScreenButton(string text, int row, int column, RoutedEventHandler handler)
        {
            Button findShortestPathButton = new Button();
            findShortestPathButton.Content = text;
            findShortestPathButton.SetValue(Grid.RowProperty, row);
            findShortestPathButton.SetValue(Grid.ColumnProperty, column);
            findShortestPathButton.Height = 50;
            findShortestPathButton.Width = 200;
            findShortestPathButton.Click += handler;
            findShortestPathButton.FontSize = 16;
            findShortestPathButton.Foreground = Brushes.White;
            findShortestPathButton.Background = (Brush)new BrushConverter().ConvertFrom("#ecc400");
            findShortestPathButton.BorderThickness = new Thickness(0);
            findShortestPathButton.VerticalAlignment = VerticalAlignment.Top;
            Grid.Children.Add(findShortestPathButton);
        }

        private void DrawFindShortestPathScreen(object sender, RoutedEventArgs e)
        {
            InitializeActionScreen();
        }

        private void DrawCompareAlgorithmsScreen(object sender, RoutedEventArgs e)
        {
            InitializeActionScreen();
        }

        private void InitializeActionScreen()
        {
            GridExtentions.RestoreGrid(Grid);
            CreateActionScreenGrid();

            CreateHomeButton();

            inputSection = new InputSection(1, 1);
            Grid.Children.Add(inputSection);
            outputSection = new OutputSection(2, 1);
            Grid.Children.Add(outputSection);
            algorithmsSection = new AlgorithmsSection(3, 1);
            Grid.Children.Add(algorithmsSection);
            stopWatchSection = new StopWatchSection(4, 1);
            Grid.Children.Add(stopWatchSection);

            var startButton = new Button();
            startButton.Content = "START";
            startButton.SetValue(Grid.RowProperty, 5);
            startButton.SetValue(Grid.ColumnProperty, 1);
            startButton.Height = 40;
            startButton.Width = 100;
            startButton.HorizontalAlignment = HorizontalAlignment.Left;
            startButton.VerticalAlignment = VerticalAlignment.Center;
            startButton.Click += StartButtonClick;
            Grid.Children.Add(startButton);

            resultSection = new ResultSection(6, 1);
            Grid.Children.Add(resultSection);

        }
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (inputSection.InputMode == InputMode.File)
            {
                if (inputSection.SourcePath == "")
                    resultSection.WriteErrorMessage("No file is selected.");
                else
                {
                    var nodes = TSPDeserializer.DeserializeNodes(inputSection.SourcePath);
                    var graph = new Graph(nodes);
                    var algorithms = algorithmsSection.SelectedAlgorithms;
                    if (algorithms.Count == 0)
                        resultSection.WriteErrorMessage("No algortihms selected.");
                    else
                    {
                        var paths = new List<(Graph, Path)>();
                        List<Task<Path>> tasks = new();
                        foreach (var algorithm in algorithms)
                        {
                            switch (algorithm)
                            {
                                case TSPAlgorithms.NearestAddition:
                                    tasks.Add(Task<Path>.Factory.StartNew(() => nearestAddition.FindShortestPath(graph)));
                                    break;
                                case TSPAlgorithms.DoubleTree:
                                    tasks.Add(Task<Path>.Factory.StartNew(() => doubleTree.FindShortestPath(graph)));
                                    break;
                                case TSPAlgorithms.Christofides:
                                    tasks.Add(Task<Path>.Factory.StartNew(() => christofides.FindShortestPath(graph)));
                                    break;
                                case TSPAlgorithms.KernighanLin:
                                    tasks.Add(Task<Path>.Factory.StartNew(() => kernighanLin.FindShortestPath(graph)));
                                    break;
                                default:
                                    break;
                            }
                        }
                        foreach (var t in tasks)
                            paths.Add((graph, t.Result));

                        resultSection.DisplayGraphs(paths);
                    }
                }
            }
            else if (inputSection.InputMode == InputMode.Folder)
                if (inputSection.SourcePath == "")
                    resultSection.WriteErrorMessage("No folder is selected.");

        }

        private void CreateActionScreenGrid()
        {
            //homeButton homeButton
            GridExtentions.AddRowToGrid(Grid, 45);

            //input
            GridExtentions.AddRowToGrid(Grid, 115);

            //output
            GridExtentions.AddRowToGrid(Grid, 45);

            //algorithms
            GridExtentions.AddRowToGrid(Grid, 45);

            //stopwatch
            GridExtentions.AddRowToGrid(Grid, 45);

            //startButton
            GridExtentions.AddRowToGrid(Grid, 45);

            //result
            GridExtentions.AddRowToGrid(Grid, 1, GridUnitType.Star);

            //oter edge
            GridExtentions.AddRowToGrid(Grid, 45);

            GridExtentions.AddColumnToGrid(Grid, 45);
            GridExtentions.AddColumnToGrid(Grid, 1, GridUnitType.Star);
            GridExtentions.AddColumnToGrid(Grid, 45);
        }

        private void CreateHomeButton()
        {
            var homeButton = new Button();
            homeButton.Content = "🏠";
            homeButton.SetValue(Grid.RowProperty, 0);
            homeButton.SetValue(Grid.ColumnProperty, Grid.ColumnDefinitions.Count - 1);
            homeButton.Height = 40;
            homeButton.Width = 45;
            homeButton.FontSize = 20;
            homeButton.Foreground = Brushes.White;
            homeButton.Background = (Brush)new BrushConverter().ConvertFrom("#bfafa0");
            homeButton.BorderThickness = new Thickness(0);
            homeButton.BorderThickness = new Thickness(0);
            homeButton.Click += GoHome;
            Grid.Children.Add(homeButton);
        }

        private void GoHome(object sender, RoutedEventArgs e) => DrawStartScreen();

    }
}
