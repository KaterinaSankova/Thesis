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

        private State state = new State();

        public MainWindow()
        {
            InitializeComponent();
            InitializeActionScreen();

            this.StateChanged += (sender, e) => resultSection.RedrawGraphs();
            this.SizeChanged += (sender, e) => resultSection.RedrawGraphs();
        }

        private void InitializeActionScreen()
        {
            GridExtentions.RestoreGrid(Grid);
            CreateActionScreenGrid();

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
            List<string> additionalMessages = new();
            if (inputSection.InputMode == InputMode.File)
            {
                if (inputSection.SourcePath == "" || inputSection.SourcePath == null)
                    resultSection.WriteErrorMessage("No file is selected.");
                else
                {
                    var nodes = TSPDeserializer.DeserializeNodes(inputSection.SourcePath);
                    var graph = new Graph(nodes);
                    var algorithms = algorithmsSection.SelectedAlgorithms;
                    Path? resultPath = null;
                    if (algorithms.Count == 0)
                        resultSection.WriteErrorMessage("No algortihms selected.");
                    else
                    {
                        if (inputSection.ResultPath != "")
                        {
                            try
                            {
                                resultPath = OptTourDeserializer.DeserializeNodes(inputSection.ResultPath, graph);
                            }
                            catch (System.Exception)
                            {
                                additionalMessages.Add($"File with results on path '{inputSection.ResultPath}' could not been deserialized.");
                            }
                        }

                        var results = new List<Result>();
                        List<Task<Result>> tasks = new();
                        foreach (var algorithm in algorithms)
                        {
                            tasks.Add(Task<Result>.Factory.StartNew(() => new Result(algorithm, graph, stopWatchSection.IsChecked, resultPath)));
                        }
                        foreach (var t in tasks)
                            results.Add(t.Result);

                        resultSection.DisplayGraphs(results, additionalMessages);
                    }
                }
            }
            else if (inputSection.InputMode == InputMode.Folder)
                if (inputSection.SourcePath == "")
                    resultSection.WriteErrorMessage("No folder is selected.");

        }

        private void CreateActionScreenGrid()
        {
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

    }
}
