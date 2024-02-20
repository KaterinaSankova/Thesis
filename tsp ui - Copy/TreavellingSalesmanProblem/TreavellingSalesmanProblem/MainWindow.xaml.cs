using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;
using TSP.Enum;
using TSP.Sections;

namespace TSP
{
    public partial class MainWindow : Window
    {
        private InputSection inputSection;
        private OutputSection outputSection;
        private AlgorithmsSection algorithmsSection;
        private StopWatchSection stopWatchSection;
        private ResultSection resultSection = new ResultSection();

        private State state = new State();

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
