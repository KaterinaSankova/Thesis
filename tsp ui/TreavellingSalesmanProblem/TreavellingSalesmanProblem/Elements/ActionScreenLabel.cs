using System.Windows;
using System.Windows.Controls;

namespace TSP.Elements
{
    public class ActionScreenLabel : Label
    {
        public ActionScreenLabel(string text)
        {
            this.Content = text;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Center;
        }

        public ActionScreenLabel(string text, int row, int column) : this(text)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }
    }
}
