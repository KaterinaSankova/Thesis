using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace TSP.Views
{
    /// <summary>
    /// Interaction logic for GenerateInputView.xaml
    /// </summary>
    public partial class GenerateInputView : UserControl
    {
        public GenerateInputView()
        {
            InitializeComponent();
        }

        private void ValidateIntegerInput(object sender, RoutedEventArgs e)
        {
        }
        private void ValidateDoubleInput(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(((TextBox)sender).Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                ((TextBox)sender).Text = "0";
        }

        private void SearchForOutputFolder(object sender, RoutedEventArgs e) => SearchForFolder(OutputFolderTextBlock);

        private void SearchForFolder(TextBlock folderPathTextBlock)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                folderPathTextBlock.Text = dialog.FolderName;
            }
        }
    }
}
