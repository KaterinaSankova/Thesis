using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TSP.Elements
{

    public class FilePicker : StackPanel, INotifyPropertyChanged
    {
        public string FilePath { get { return fileNameField.Text; } }

        string extention;
        TextBlock fileNameField;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public FilePicker(string extention)
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
            browseFilesButton.Content = "Browse files";
            browseFilesButton.Width = 90;
            browseFilesButton.Margin = new Thickness(10, 0, 10, 0);
            browseFilesButton.Click += SearchForFile;
            this.Children.Add(browseFilesButton);

            this.extention = extention;
        }

        public FilePicker(string extention, int row, int column) : this(extention)
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }


        private void SearchForFile(object sender, RoutedEventArgs routedEventArgs)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = ""; // Default file name
            dialog.DefaultExt = ".tsp"; // Default file extension
            dialog.Filter = $"{extention} documents (.{extention})|*.{extention}"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                var fileName = dialog.FileName;
                if (fileName.Length > 58)
                {
                    fileName = fileName.Substring(0, 58);
                    fileName = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);
                    fileName += "...";
                }
                fileNameField.Text = fileName;
                NotifyPropertyChanged();
            }
        }
    }
}
