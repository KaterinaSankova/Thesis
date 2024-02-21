using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Enum;
using TSP.Models;
using TSP.Sections;
using TSP.ViewModels;

namespace TSP
{
    public partial class MainWindow : Window
    {
        public MainWindow(ResultsModel resultData)
        {
            InitializeComponent();
            var inputView = new Frame();
            inputView.DataContext = new FileInputViewModel(resultData);
            inputView.Source = new System.Uri(System.IO.Path.Combine(Environment.CurrentDirectory, ".\\Views\\FileInputView.xaml"));

            Grid.Children.Add(inputView);



        //< Frame Source = ".\Views\FileInputView.xaml" DataContext = "{Binding CurrentViewModel}" Grid.Row = "0" ></ Frame >
        //< Frame Source = ".\Views\ResultsView.xaml" Grid.Row = "0" ></ Frame >
        }
    }
}
