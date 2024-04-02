using System.Windows;
using System.Windows.Controls;

namespace TSP.Extentions
{
    static class GridExtentions
    {
        public static void AddRowToGrid(Grid grid, double height, GridUnitType unitType = GridUnitType.Pixel)
        {
            var rd = new RowDefinition();
            rd.Height = new GridLength(height, unitType);
            grid.RowDefinitions.Add(rd);
        }

        public static void AddColumnToGrid(Grid grid, double weight, GridUnitType unitType = GridUnitType.Pixel)
        {
            var cd = new ColumnDefinition();
            cd.Width = new GridLength(weight, unitType);
            grid.ColumnDefinitions.Add(cd);
        }

        public static void AddColumnToGrid(Grid grid)
        {
            var cd = new ColumnDefinition();
            cd.Width = GridLength.Auto;
            grid.ColumnDefinitions.Add(cd);
        }

        public static void AddRowToGrid(Grid grid)
        {
            var rd = new RowDefinition();
            rd.Height = GridLength.Auto;
            grid.RowDefinitions.Add(rd);
        }

        public static void RestoreGrid(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }
    }
}
