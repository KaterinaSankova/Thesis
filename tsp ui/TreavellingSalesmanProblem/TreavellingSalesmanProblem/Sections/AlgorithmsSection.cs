using System.Collections.Generic;
using System.Windows.Controls;
using TravellingSalesmanProblem.Algorithms.Enums;
using TSP.Elements;

namespace TSP.Sections
{
    public class AlgorithmsSection : LabeledElement
    {
        public List<TSPAlgorithms> SelectedAlgorithms
        {
            get
            {
                List<TSPAlgorithms> algorithms = new List<TSPAlgorithms>();

                var checkBox = (CheckBoxGroup)this.Children[1];

                for (int i = 0; i < checkBox.Count; i++)
                {
                    if ((bool)((CheckBox)checkBox.Children[i]).IsChecked)
                    {
                        algorithms.Add((TSPAlgorithms)i);
                    }
                }

                return algorithms;
            }
        }

        public AlgorithmsSection() : base("Algorithms:", new System.Windows.UIElement())
        {
            List<string> checkBoxProperties = [
                ("Nearest addition"),
                ("Double tree"),
                ("Christofides'"),
                ("Kernighan-Lin")
            ];
            var inputRadioButtonGroup = new CheckBoxGroup(checkBoxProperties);
            this.Children.RemoveAt(1);
            this.Children.Add(inputRadioButtonGroup);
        }

        public AlgorithmsSection(int row, int column) : this()
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }
    }
}
