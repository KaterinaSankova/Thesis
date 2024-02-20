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
    /// Interaction logic for FolderInputView.xaml
    /// </summary>
    public partial class FolderInputView : UserControl
    {
        public FolderInputView()
        {
            InitializeComponent();
        }
        private void SearchForSourceFolder(object sender, RoutedEventArgs e) => SearchForFolder(SourceFolderTextBlock);
        private void SearchForResultFolder(object sender, RoutedEventArgs e) => SearchForFolder(ResultFolderTextBlock);
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
                // Get the selected folder
                var fileName = dialog.FolderName;
                if (fileName.Length > 50)
                {
                    fileName = fileName.Substring(0, 50);
                    fileName = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);
                    fileName += "...";
                }
                folderPathTextBlock.Text = fileName;
            }
        }
    }
}
