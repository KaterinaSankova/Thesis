using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Elements;
using TSP.Enum;

namespace TSP.Sections
{
    public class ResultSection : Grid
    {
        public ResultMode mode;
        private int numberOfGraphs;
        private int graphIndex;

        public ResultSection() { }

        public ResultSection(int row, int column)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
            mode = ResultMode.None;
        }

        public void WriteErrorMessage(string message)
        {
            GridExtentions.RestoreGrid(this);
            GridExtentions.AddRowToGrid(this, 45);
            var messageBlock = new TextBlock();
            messageBlock.Text = message;
            messageBlock.SetValue(Grid.RowProperty, 0);
            messageBlock.SetValue(Grid.ColumnProperty, 0);
            messageBlock.Foreground = Brushes.Red;
            this.Children.Add(messageBlock);
            mode = ResultMode.ErrorMessage;
        }

        //public void DisplayGraphs(List<Result> results, List<string> additionalMessages)
        //{
        //    GridExtentions.RestoreGrid(this);
        //    graphIndex = 0;

        //    for (int i = 0; i < additionalMessages.Count; i++)
        //    {
        //        GridExtentions.AddRowToGrid(this, 45);
        //        var messageBlock = new TextBlock();
        //        messageBlock.Text = additionalMessages[i];
        //        messageBlock.SetValue(Grid.RowProperty, 0);
        //        messageBlock.SetValue(Grid.ColumnProperty, 0);
        //        messageBlock.Foreground = Brushes.Red;
        //        this.Children.Add(messageBlock);
        //        graphIndex++;
        //    }

        //    GridExtentions.AddRowToGrid(this, 1, GridUnitType.Star);

        //    numberOfGraphs = results.Count;

        //    double width = ((Grid)this.Parent).ColumnDefinitions[(int)this.GetValue(Grid.ColumnProperty)].ActualWidth;
        //    double height = ((Grid)this.Parent).RowDefinitions[(int)this.GetValue(Grid.RowProperty)].ActualHeight;

        //    var stackPanel = new StackPanel();
        //    stackPanel.SetValue(Grid.RowProperty, graphIndex);
        //    stackPanel.SetValue(Grid.ColumnProperty, 0);
        //    stackPanel.Orientation = Orientation.Horizontal;
        //    stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
        //    stackPanel.Margin = new Thickness(0);
        //    for (int i = 0; i < results.Count; i++)
        //    {
        //        var graphCanvas = new GraphCanvas(results[i].Graph, results[i].Path, width / numberOfGraphs, height);
        //        graphCanvas.Margin = new Thickness(0, 0, width / numberOfGraphs, 0);
        //        stackPanel.Children.Add(graphCanvas);
        //    }

        //    this.Children.Add(stackPanel);

        //    mode = ResultMode.Graph;
        //}

        public void RedrawGraphs()
        {
            if (mode == ResultMode.Graph)
            {

                double width = ((Grid)this.Parent).ColumnDefinitions[(int)this.GetValue(Grid.ColumnProperty)].ActualWidth;
                double height = ((Grid)this.Parent).RowDefinitions[(int)this.GetValue(Grid.RowProperty)].ActualHeight;

                foreach (var graphCanvas in ((StackPanel)this.Children[graphIndex]).Children)
                {
                    ((GraphCanvas)graphCanvas).Redraw(width / numberOfGraphs, height);
                }
            }
        }
    }
}
