using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Elements;
using TSP.Extentions;
using TSP.Models;
using TSP.Stores;
using TSP.ViewModels;

namespace TSP.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
            this.SizeChanged += (sender, e) => RedrawGraphs();
        }

        private void StartCalculations(object sender, RoutedEventArgs e)
        {
            var dataContext = (ResultsViewModel)this.DataContext;
            dataContext.PropertyChanged += OnCalculationsChange;
            dataContext.Message = "";
            try
            {
                int resultsNumber = ((InputViewModel)dataContext.NavigationStore.CurrentViewModel).NumberOfSelectedAlgorithms;
                if (dataContext.NavigationStore.GetInputMode() == Enum.InputMode.Folder)
                {
                    string[] sourceFiles;
                    string sourceFolderPath = ((FolderInputViewModel)dataContext.NavigationStore.CurrentViewModel).SourceFolderPath;
                    try
                    {
                        sourceFiles = Directory.GetFiles(((FolderInputViewModel)dataContext.NavigationStore.CurrentViewModel).SourceFolderPath, "*.tsp");
                    }
                    catch (Exception)
                    {
                        throw new Exception($"No files could have been retrieved from path '{sourceFolderPath}'.");
                    }
                    resultsNumber *= sourceFiles.Length;
                }
                else if (dataContext.NavigationStore.GetInputMode() == Enum.InputMode.Generate)
                {
                    if (!((GenerateInputViewModel)dataContext.NavigationStore.CurrentViewModel).AverageResults)
                    {
                        resultsNumber *= ((GenerateInputViewModel)dataContext.NavigationStore.CurrentViewModel).NumberOfSamples;
                    }
                }

                if (resultsNumber > 5)
                {
                    PrepareResultTable();
                }
                else
                {
                    PrepareGraphs();
                }

                dataContext.StartCalculations.Execute(this);
            }
            catch (Exception exception)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;

                MessageBox.Show(exception.Message, "Error", button, icon, MessageBoxResult.Yes);
            }
        }

        private void OnCalculationsChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ResultsViewModel.CalculationsFinished))
            {
                if (this.Dispatcher.Invoke(() => ((ResultsViewModel)this.DataContext).CalculationsFinished))
                {
                    this.Dispatcher.Invoke(() => DisplayResults());
                }
            }
        }

        public void DisplayResults()
        {
            if (((ResultsViewModel)this.DataContext).AlgoResults.Count < 6)
            {
                if (((ResultsViewModel)this.DataContext).AlgoResults.Where(r => r.Graph == null).ToList().Count == 0)
                    DisplayGraphs();
                else
                    PrepareResultTable();
            }
        }

        private void DisplayGraphs()
        {
            PrepareGraphs();

            var results = ((ResultsViewModel)this.DataContext).AlgoResults.Where(r => r.Path.Count > 0).ToList();

            double width = this.ActualWidth - 40;
            double height = this.ActualHeight - 80 ;

            for (int i = 0; i < results.Count; i++)
            {
                var algoResults = results[i];

                var graphCanvas = new GraphCanvas(algoResults.Graph, algoResults.Path, width / results.Count, height - 84);
                graphCanvas.Margin = new Thickness(0, 0, width / results.Count, 0);
                Graphs.Children.Add(graphCanvas);

                GridExtentions.AddColumnToGrid(GraphLabels, 1, GridUnitType.Star);

                var info = new TextBox();
                info.Text = $"Name: {algoResults.Name}\nAlgorithm: {algoResults.Algorithm}\nLength: {algoResults.AverageLength}\nRatio: {algoResults.Ratio}\nDuration: {algoResults.AverageDuration}";
                info.SetValue(Grid.RowProperty, 0);
                info.SetValue(Grid.ColumnProperty, i);
                info.IsReadOnly = true;
                info.BorderThickness = new Thickness(0);
                info.Margin = new Thickness(15, 0, 0, 0);
                GraphLabels.Children.Add(info);
            }
        }

        private void RedrawGraphs()
        {
            var results = ((ResultsViewModel)this.DataContext).AlgoResults;
            var numberOfGraphs = results.Count;

            if (Graphs.Children.Count == 0)
                return;

            double width = this.ActualWidth - 40;
            double height = Grid.RowDefinitions[2].ActualHeight;

            foreach (var graph in Graphs.Children)
            {
                ((GraphCanvas)graph).Redraw(width / numberOfGraphs, height);
            }
        }

        private void PrepareGraphs()
        {
            Graphs.Children.Clear();
            GraphLabels.Children.Clear();
            GridExtentions.RestoreGrid(GraphLabels);

            if (GraphLabels.Visibility != Visibility.Visible)
            {
                Graphs.Visibility = Visibility.Visible;
                GraphLabels.Visibility = Visibility.Visible;
                ResultsTable.Visibility = Visibility.Hidden;
            }
        }

        private void PrepareResultTable()
        {
            var nav = ((ResultsViewModel)this.DataContext).NavigationStore;

            if (nav.GetInputMode() == Enum.InputMode.Generate && ((GenerateInputViewModel)nav.CurrentViewModel).AverageResults)
            {
                AverageLengthColumn.Width = 100;
                AverageDurationColumn.Width = 175;
                ShortestDurationColumn.Header = "Shortest duration";
                BestLengthColumn.Header = "Shortest length";
            }
            else
            {
                AverageLengthColumn.Width = 0;
                AverageDurationColumn.Width = 0;
                ShortestDurationColumn.Header = "Duration";
                BestLengthColumn.Header = "Length";
            }

            if (ResultsTable.Visibility != Visibility.Visible)
            {
                ResultsTable.Visibility = Visibility.Visible;
                Graphs.Visibility = Visibility.Hidden;
                GraphLabels.Visibility = Visibility.Hidden;
            }
        }
    }
}
