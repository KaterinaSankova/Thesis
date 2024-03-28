using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TSP.Models;
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
            if ((((ResultsViewModel)this.DataContext).AlgoResults.Count < 5) && (((ResultsViewModel)this.DataContext).AlgoResults.Where(r => r.Graph == null).ToList().Count == 0))
                DisplayGraphs();
            else
                DisplayLargeResult();
        }

        private void DisplayGraphs()
        {
            ResultsTable.Visibility = Visibility.Hidden;
            Graphs.Visibility = Visibility.Visible;
            GraphLabels.Visibility = Visibility.Visible;
            Graphs.Children.Clear();
            GraphLabels.Children.Clear();
            GridExtentions.RestoreGrid(GraphLabels);

            var results = ((ResultsViewModel)this.DataContext).AlgoResults;

            double width = this.ActualWidth - 40;
            double height = this.ActualHeight - 80 ;

            for (int i = 0; i < results.Count; i++)
            {
                var algoResults = results[i];
                if (algoResults.Path.Count == 0)
                    break;

                var graphCanvas = new GraphCanvas(algoResults.Graph, algoResults.Path, width / results.Count, height - 84);
                graphCanvas.Margin = new Thickness(0, 0, width / results.Count, 0);
                Graphs.Children.Add(graphCanvas);

                GridExtentions.AddColumnToGrid(GraphLabels, 1, GridUnitType.Star);

                var info = new TextBlock();
                info.Text = $"Name: {algoResults.Name}\nAverage length: {algoResults.AverageLength}\nRatio: {algoResults.Ratio}\nAverage uration: {algoResults.AverageDuration}";
                info.SetValue(Grid.RowProperty, 0);
                info.SetValue(Grid.ColumnProperty, i);
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

        private void DisplayLargeResult()
        {
            Graphs.Visibility = Visibility.Hidden;
            GraphLabels.Visibility = Visibility.Hidden;
            ResultsTable.Visibility = Visibility.Visible;
        }
    }
}
