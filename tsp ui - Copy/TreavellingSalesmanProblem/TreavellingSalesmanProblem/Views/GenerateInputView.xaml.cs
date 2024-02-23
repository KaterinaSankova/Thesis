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
            if (!double.TryParse(((TextBox)sender).Text, out _))
                ((TextBox)sender).Text = "0";
        }
    }
}
