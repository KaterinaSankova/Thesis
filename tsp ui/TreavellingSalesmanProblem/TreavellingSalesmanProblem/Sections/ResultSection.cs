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
            var messageBlock = new TextBlock();
            messageBlock.Text = message;
            messageBlock.SetValue(Grid.RowProperty, 0);
            messageBlock.SetValue(Grid.ColumnProperty, 0);
            messageBlock.Foreground = Brushes.Red;
            this.Children.Add(messageBlock);
            mode = ResultMode.ErrorMessage;
        }

        public void DisplayGraphs(List<(Graph Graph, Path Path)> inputs)
        {
            GridExtentions.RestoreGrid(this);

            numberOfGraphs = inputs.Count;

            double width = ((Grid)this.Parent).ColumnDefinitions[(int)this.GetValue(Grid.ColumnProperty)].ActualWidth;
            double height = ((Grid)this.Parent).RowDefinitions[(int)this.GetValue(Grid.RowProperty)].ActualHeight;

            var stackPanel = new StackPanel();
            stackPanel.SetValue(Grid.RowProperty, 0);
            stackPanel.SetValue(Grid.ColumnProperty, 0);
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.Margin = new Thickness(0);
            for (int i = 0; i < inputs.Count; i++)
            {
                var graphCanvas = new GraphCanvas(inputs[i].Graph, inputs[i].Path, width / numberOfGraphs, height);
                graphCanvas.Margin = new Thickness(0, 0, width / numberOfGraphs, 0);
                stackPanel.Children.Add(graphCanvas);
            }

            this.Children.Add(stackPanel);

            mode = ResultMode.Graph;
        }

        public void RedrawGraphs()
        {
            if (mode == ResultMode.Graph)
            {

                double width = ((Grid)this.Parent).ColumnDefinitions[(int)this.GetValue(Grid.ColumnProperty)].ActualWidth;
                double height = ((Grid)this.Parent).RowDefinitions[(int)this.GetValue(Grid.RowProperty)].ActualHeight;

                foreach (var graphCanvas in ((StackPanel)this.Children[0]).Children)
                {
                    ((GraphCanvas)graphCanvas).Redraw(width / numberOfGraphs, height);
                }
            }
        }
    }
}
