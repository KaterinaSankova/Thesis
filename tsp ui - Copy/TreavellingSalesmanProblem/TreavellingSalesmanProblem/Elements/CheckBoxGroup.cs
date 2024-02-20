using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TSP.Elements
{
    public class CheckBoxGroup : StackPanel
    {
        public int Count { get { return Children.Count; } }

        public CheckBoxGroup(List<string> checkBoxPropetries, int indexOfCheckedButton = 0)
        {
            this.Orientation = Orientation.Horizontal;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Center;

            for (int i = 0; i < checkBoxPropetries.Count; i++)
            {
                var checkBox = new CheckBox();
                checkBox.Content = checkBoxPropetries[i];
                checkBox.IsChecked = true;
                checkBox.Margin = new Thickness(0, 0, 10, 0);
                checkBox.IsChecked = false;
                this.Children.Add(checkBox);
            }

            ((CheckBox)this.Children[0]).IsChecked = true;
        }

        public CheckBoxGroup(List<string> checkBoxPropetries, int row, int column, int indexOfCheckedButton = 0) : this(checkBoxPropetries, indexOfCheckedButton)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }
    }
}
