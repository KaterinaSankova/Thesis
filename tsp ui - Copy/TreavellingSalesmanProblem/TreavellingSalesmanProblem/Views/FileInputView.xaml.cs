using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TSP.Views
{
    /// <summary>
    /// Interaction logic for FileInputView.xaml
    /// </summary>
    public partial class FileInputView : UserControl
    {
        public FileInputView()
        {
            InitializeComponent();
        }

        private void SearchForSourceFile(object sender, RoutedEventArgs e) => SearchForFile(SourceFileTextBlock, "tsp");
        private void SearchForResultFile(object sender, RoutedEventArgs e) => SearchForFile(ResultFileTextBlock, "opt.tour");
        private void SearchForOutputFolder(object sender, RoutedEventArgs e) => SearchForFolder(OutputFolderTextBlock);

        private void SearchForFile(TextBox filePathTextBlock, string extention)
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
                filePathTextBlock.Text = dialog.FileName;
            }
        }

        private void SearchForFolder(TextBox folderPathTextBlock)
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
