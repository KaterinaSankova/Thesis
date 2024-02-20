using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravellingSalesmanProblem.Algorithms.Enums;

namespace TSP.Models
{
    public class InputModel : INotifyPropertyChanged
    {
        private bool _nearestAddition = true;
        private bool _doubleTree;
        private bool _christofides;
        private bool _kernighanLin;
        public bool Stopwatch;

        public bool NearestAddition
        {
            get
            {
                return _nearestAddition;
            }
            set
            {
                _nearestAddition = value;
                OnPropertyChanged(nameof(NearestAddition));
            }
        }

        public bool DoubleTree
        {
            get
            {
                return _doubleTree;
            }
            set
            {
                _doubleTree = value;
                OnPropertyChanged(nameof(DoubleTree));
            }
        }

        public bool Christofides {
            get
            {
                return _christofides;
            }
            set
            {
                _christofides = value;
                OnPropertyChanged(nameof(Christofides));
            }
        }

        public bool KernighanLin
        {
            get
            {
                return _kernighanLin;
            }
            set
            {
                _kernighanLin = value;
                OnPropertyChanged(nameof(KernighanLin));
            }
        }

        public InputModel() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
