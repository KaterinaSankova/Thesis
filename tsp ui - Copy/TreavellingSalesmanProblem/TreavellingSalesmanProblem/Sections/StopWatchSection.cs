using System.Windows.Controls;
using TSP.Elements;

namespace TSP.Sections
{
    public class StopWatchSection : LabeledElement
    {
        public bool IsChecked { get { return (bool)((CheckBox)this.Children[1]).IsChecked; } }

        public StopWatchSection() : base("Stopwatch:", new CheckBox())
        {
            ((CheckBox)this.Children[1]).IsChecked = false;
            ((CheckBox)this.Children[1]).VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }

        public StopWatchSection(int row, int column) : base("Stopwatch:", new CheckBox(), row, column)
        {
            ((CheckBox)this.Children[1]).IsChecked = false;
            ((CheckBox)this.Children[1]).VerticalAlignment = System.Windows.VerticalAlignment.Center;
        }
    }
}
