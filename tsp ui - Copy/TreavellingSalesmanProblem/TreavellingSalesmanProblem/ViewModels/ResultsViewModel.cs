﻿using System.Windows.Input;
using System;
using TSP.Commands;
using TSP.Models;
using TSP.Stores;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace TSP.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {
        private string _message;
        private ObservableCollection<AlgorithmResultViewModel> _algoResults;

        private NavigationStore _navigationStore;
        private bool _isInputValid = false;
        private bool _calculationsFinished = true;
        private bool _canStartCalculations = false;

        public ObservableCollection<AlgorithmResultViewModel> AlgoResults
        {
            get
            {
                return _algoResults;
            }
            set
            {
                _algoResults = value;
                OnPropertyChanged(nameof(AlgoResults));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsInputValid
        {
            get => _isInputValid;
            set
            {
                _isInputValid = value;
                CanStartCalculations = _isInputValid && CalculationsFinished;
                OnPropertyChanged(nameof(IsInputValid));
            }
        }

        public NavigationStore NavigationStore { get => _navigationStore; set => _navigationStore = value; }


        public bool CalculationsFinished
        {
            get
            {
                return _calculationsFinished;
            }
            set
            {
                _calculationsFinished = value;
                CanStartCalculations = IsInputValid && _calculationsFinished;
                OnPropertyChanged(nameof(CalculationsFinished));
            }
        }

        public bool CanStartCalculations
        {
            get
            {
                return _canStartCalculations;
            }
            set
            {
                _canStartCalculations = value;
                OnPropertyChanged(nameof(CanStartCalculations));
            }
        }

        public ICommand StartCalculations { get; }

        public ICommand AbortCalculations { get; }

        public ResultsViewModel(NavigationStore navigationStore)
        {
            NavigationStore = navigationStore;
            StartCalculations = new StartCalculationsCommand(this);
            AlgoResults = new();

            _navigationStore.CurrentViewModel.PropertyChanged += OnInputChanged;
            navigationStore.CurrentViewModelChanged += OnInputChanged;
        }

        private void OnInputChanged(object? sender, PropertyChangedEventArgs e) => IsInputValid = ValidateInput();

        private bool ValidateInput()
        {
            if (NavigationStore.CurrentViewModel.GetType().Name == nameof(FileInputViewModel))
            {
                if (string.IsNullOrEmpty(((FileInputViewModel)NavigationStore.CurrentViewModel).SourceFilePath))
                    return false;
                else
                    return true;
            }
            else if (NavigationStore.CurrentViewModel.GetType().Name == nameof(FolderInputViewModel))
            {
                if (string.IsNullOrEmpty(((FolderInputViewModel)NavigationStore.CurrentViewModel).SourceFolderPath))
                    return false;
                else
                    return true;
            }
            else
            {
                return true;
            }
        }

        private void OnInputChanged()
        {
            _navigationStore.CurrentViewModel.PropertyChanged += OnInputChanged;
            IsInputValid = ValidateInput();
        }
    }
}