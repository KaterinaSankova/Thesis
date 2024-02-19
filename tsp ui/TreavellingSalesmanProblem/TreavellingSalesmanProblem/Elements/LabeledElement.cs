using System.Windows;
using System.Windows.Controls;

namespace TSP.Elements
{
    public class LabeledElement : StackPanel
    {
        public UIElement Element { get { return this.Children[1]; } }

        public LabeledElement(string label, UIElement control)
        {
            this.Orientation = Orientation.Horizontal;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Center;

            var labelElement = new ActionScreenLabel(label);
            labelElement.Margin = new Thickness(0, 0, 5, 0);
            this.Children.Add(labelElement);
            this.Children.Add(control);
        }
        public LabeledElement(string label, UIElement control, int row, int column) : this(label, control)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }
    }
}
