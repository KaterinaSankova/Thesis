using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TSP.Elements
{
    public class RadioButtonGroup : StackPanel
    {
        public RadioButtonGroup(List<(string Label, RoutedEventHandler Hadler)> buttonPropetries, string groupName, int indexOfCheckedButton = 0)
        {
            this.Orientation = Orientation.Horizontal;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Center;

            for (int i = 0; i < buttonPropetries.Count; i++)
            {
                var radioButton = new RadioButton();
                radioButton.GroupName = groupName;
                radioButton.Content = buttonPropetries[i].Label;
                radioButton.IsChecked = true;
                radioButton.Margin = new Thickness(0, 0, 10, 0);
                radioButton.Checked += buttonPropetries[i].Hadler;
                this.Children.Add(radioButton);
            }

            ((RadioButton)this.Children[0]).IsChecked = true;
        }

        public RadioButtonGroup(List<(string Label, RoutedEventHandler Hadler)> buttonPropetries, string groupName, int row, int column, int indexOfCheckedButton = 0) : this(buttonPropetries, groupName, indexOfCheckedButton)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }
    }
}
