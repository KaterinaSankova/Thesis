using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TSP.Commands;
using TSP.Enum;
using TSP.Models;
using TSP.Stores;

namespace TSP.ViewModels
{
    public class GenerateInputViewModel : InputViewModel
    {
        private int _numberOfSamples = 1;
        private int _numberOfCities = 20;
        private bool _averageResults = false;
        private double _lowestX = -20;
        private double _lowestY = -20;
        private double _highestX = 20;
        private double _highestY = 20;

        private int highestPermitedNumberOfSamples = 1000;
        private int highestPermitedNumberOfCities = 20000;
        private int lowestPermitedCoordinate = -50000;
        private int highestPermitedCoordinate = 50000;

        public int NumberOfSamples
        {
            get
            {
                return _numberOfSamples;
            }
            set
            {
                if (value < 1)
                    _numberOfSamples = 1;
                else if (value > highestPermitedNumberOfSamples)
                    _numberOfCities = highestPermitedNumberOfSamples;
                else
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
                if (value < 0)
                    _numberOfCities = 0;
                else if (value > highestPermitedNumberOfCities)
                    _numberOfCities = highestPermitedNumberOfCities;
                else
                    _numberOfCities = value;
                OnPropertyChanged(nameof(NumberOfCities));
            }
        }

        public bool AverageResults
        {
            get
            {
                return _averageResults;
            }
            set
            {
                _averageResults = value;
                OnPropertyChanged(nameof(AverageResults));
            }
        }

        public double LowestX
        {
            get
            {
                return _lowestX;
            }
            set
            {
                if (value < lowestPermitedCoordinate)
                    _lowestX = lowestPermitedCoordinate;
                else if (value > HighestX)
                    _lowestX = Math.Max(HighestX - 1, lowestPermitedCoordinate);
                else
                    _lowestX = value;

                OnPropertyChanged(nameof(LowestX));
            }
        }

        public double LowestY
        {
            get
            {
                return _lowestY;
            }
            set
            {
                if (value < lowestPermitedCoordinate)
                    _lowestY = lowestPermitedCoordinate;
                else if (value > HighestY)
                    _lowestY = Math.Max(HighestY - 1, lowestPermitedCoordinate);
                else
                    _lowestY = value;
                OnPropertyChanged($"{nameof(LowestY)}");
            }
        }

        public double HighestX
        {
            get
            {
                return _highestX;
            }
            set
            {
                if (value > highestPermitedCoordinate)
                    _highestX = highestPermitedCoordinate;
                else if (value < LowestX)
                    _highestX = Math.Min(LowestX + 1, highestPermitedCoordinate);
                else
                    _highestX = value;
                OnPropertyChanged(nameof(HighestX));
            }
        }

        public double HighestY
        {
            get
            {
                return _highestY;
            }
            set
            {
                if (value > highestPermitedCoordinate)
                    _highestY = highestPermitedCoordinate;
                else if (value < LowestY)
                    _highestY = Math.Min(LowestY + 1, highestPermitedCoordinate);
                else
                    _highestY = value;
                OnPropertyChanged($"{nameof(HighestY)}");
            }
        }

        public GenerateInputViewModel(NavigationStore navigationStore, Func<FileInputViewModel> createFileInputViewModel, Func<FolderInputViewModel> createFolderInputViewModel, Func<GenerateInputViewModel> createGenerateInputViewModel) : base(navigationStore, createFileInputViewModel, createFolderInputViewModel, createGenerateInputViewModel){ }
    }
}

