using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Enum;
using TSP.Models;

namespace TSP.ViewModels
{
    public class GenerateInputViewModel : InputViewModel
    {
        private int _numberOfSamples = 1;
        private int _numberOfCities = 20;
        private int _lowestX = -20;
        private int _lowestY = -20;
        private int _highestX = 20;
        private int _highestY = 20;

        public int NumberOfSamples
        {
            get
            {
                return _numberOfSamples;
            }
            set
            {
                _numberOfSamples = value;
                OnPropertyChanged(nameof(NumberOfSamples));
            }
        }

        public int NumberOfCities
        {
            get
            {
                return _numberOfCities;
            }
            set
            {
                _numberOfCities = value;
                OnPropertyChanged(nameof(NumberOfCities));
            }
        }

        public int LowestX
        {
            get
            {
                return _lowestX;
            }
            set
            {
                _lowestX = value;
                OnPropertyChanged(nameof(LowestX));
            }
        }

        public int LowestY
        {
            get
            {
                return _lowestY;
            }
            set
            {
                _lowestY = value;
                OnPropertyChanged($"{nameof(LowestY)}");
            }
        }

        public int HighestX
        {
            get
            {
                return _highestX;
            }
            set
            {
                _highestX = value;
                OnPropertyChanged(nameof(HighestX));
            }
        }

        public int HighestY
        {
            get
            {
                return _highestY;
            }
            set
            {
                _highestY = value;
                OnPropertyChanged($"{nameof(HighestY)}");
            }
        }

        public GenerateInputViewModel(ResultsModel results) : base(results) { }
    }
}

