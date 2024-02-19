using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TSP.Elements
{
    public class FolderPicker : StackPanel
    {
        public string FolderPath { get { return fileNameField.Text; } }

        TextBlock fileNameField;

        public FolderPicker()
        {
            this.Orientation = Orientation.Horizontal;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Center;

            fileNameField = new TextBlock();
            fileNameField.Text = "";
            fileNameField.Width = 330;
            fileNameField.Background = Brushes.LightGray;
            this.Children.Add(fileNameField);

            var browseFilesButton = new Button();
            browseFilesButton.Content = "Browse folders";
            browseFilesButton.Width = 90;
            browseFilesButton.Margin = new Thickness(10, 0, 10, 0);
            browseFilesButton.Click += SearchForFolder;
            this.Children.Add(browseFilesButton);
        }

        public FolderPicker(int row, int column) : this()
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }

        private void SearchForFolder(object sender, RoutedEventArgs routedEventArgs)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                var fileName = dialog.FolderName;
                if (fileName.Length > 58)
                {
                    fileName = fileName.Substring(0, 58);
                    fileName = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);
                    fileName += "...";
                }
                fileNameField.Text = fileName;
            }
        }
    }
}
