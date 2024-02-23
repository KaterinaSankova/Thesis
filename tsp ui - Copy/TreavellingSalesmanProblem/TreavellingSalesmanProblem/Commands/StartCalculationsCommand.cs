using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using TreavellingSalesmanProblem;
using TSP.Enum;
using TSP.Models;
using TSP.Sections;
using TSP.Stores;
using TSP.ViewModels;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TSP.Commands
{
    public class StartCalculationsCommand : AsyncCommandBase
    {
        private ResultsViewModel _results;

        public ViewModelBase InputViewModel => _results.NavigationStore.CurrentViewModel;

        public StartCalculationsCommand(ResultsViewModel results)
        {
            this._results = results;

            _results.NavigationStore.PropertyChanged += OnViewModelPropertyChanged;
            _results.NavigationStore.CurrentViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void StartCalculations()
        {

            _results.AlgoResults = new();
            InputMode mode = GetInputMode();
            if (mode == InputMode.File)
            {
                var input = (FileInputViewModel)InputViewModel;
                StartCalculations(input.SourceFilePath, input.ResultFilePath);
            }
            else if (mode == InputMode.Folder)
            {
                var input = (FolderInputViewModel)InputViewModel;
                string[] sourceFiles;
                try
                {
                    sourceFiles = Directory.GetFiles(input.SourceFolderPath, "*.tsp");
                }
                catch (Exception)
                {
                    throw new Exception($"No files could have been retrieved from path '{input.SourceFolderPath}'.");
                }
                if (sourceFiles.Length == 0)
                {
                    _results.Message = $"No TSP files found in folder {input.SourceFolderPath}";
                    return;
                }
                int numberOfAllFiles = sourceFiles.Length;
                int numberOfCurrentFile = 0;
                foreach (var sourceFile in sourceFiles)
                {
                    string resultFile = $"{input.ResultFolderPath}\\{System.IO.Path.GetFileName(sourceFile)[..^4]}.opt.tour";

                    int numberOfLines = _results.Message.ToCharArray().Count(c => c == '\n');
                    if (numberOfLines >= 1)
                        _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                    _results.Message += $"\nCompleted {numberOfCurrentFile}/{numberOfAllFiles}\n";

                    try
                    {
                        StartCalculations(sourceFile, resultFile);
                    }
                    catch (Exception exception)
                    {
                        string message = $"{exception.Message}\nWould you like to continue?";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBoxResult result;

                        result = MessageBox.Show(message, "Error while processing file", button, icon, MessageBoxResult.Yes);

                        if (result == MessageBoxResult.No)
                            return;
                    }

                    numberOfCurrentFile++;
                }
            }
        }


        private InputMode GetInputMode()
        {
            switch (InputViewModel.GetType().Name)
            {
                case nameof(FileInputViewModel):
                    return InputMode.File;
                case nameof(FolderInputViewModel):
                    return InputMode.Folder;
                case nameof(GenerateInputViewModel):
                    return InputMode.Generate;
                default:
                    throw new Exception("Invalid view model");
            }
        }

        private void StartCalculations(string sourceFilePath, string resultFilePath)
        {
            _results.Message += $"Processing file {System.IO.Path.GetFileName(sourceFilePath)}";
            var graph = TSPDeserializer.DeserializeGraph(sourceFilePath);
            var result = StartCalculations(System.IO.Path.GetFileName(sourceFilePath), graph);
            if (result == null)
            {
                _results.Message = "No algorithms were selected.";
                return;
            }
            else
            {
                if (!File.Exists(resultFilePath))
                {
                    if (!((InputViewModel)InputViewModel).IgnoreNotFoundResultFiles && !string.IsNullOrEmpty(resultFilePath))
                    {
                        string message = $"Result file '{resultFilePath}' could not been found.\nWould you like to continue?";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        MessageBoxResult msbxRslt;

                        msbxRslt = MessageBox.Show(message, "Error while processing file", button, icon, MessageBoxResult.Yes);

                        if (msbxRslt == MessageBoxResult.No)
                            return;
                    }
                }
                else
                {
                    result.ResultPath = OptTourDeserializer.DeserializePath(resultFilePath, graph);
                }
                App.Current.Dispatcher.Invoke(() =>_results.AlgoResults.Add(new AlgorithmResultViewModel(result)));
            }
            if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
            else
                _results.Message = "";
        }


        private AlgorithmResultModel? StartCalculations(string name, Graph graph)
        {
            var input = (InputViewModel)InputViewModel;
            if (input.NearestAddition)
                return StartAlgorithmCalculations(name, graph, new NearestAddition(), TSPAlgorithms.NearestAddition);
            if (input.DoubleTree)
                return StartAlgorithmCalculations(name, graph, new DoubleTree(), TSPAlgorithms.DoubleTree);
            if (input.Christofides)
                return StartAlgorithmCalculations(name, graph, new Christofides(), TSPAlgorithms.Christofides);
            if (input.KernighanLin)        
                return StartAlgorithmCalculations(name, graph, new KernighanLin(), TSPAlgorithms.KernighanLin);
            return null;
        }

        private AlgorithmResultModel StartAlgorithmCalculations<T>(string name, Graph graph, ITspAlgorithm<T> algo, TSPAlgorithms algoType) where T : Path
        {
            var input = (InputViewModel)InputViewModel;

            Stopwatch sw = new();
            TimeSpan? ts = null;
            if (input.Stopwatch)
                sw.Start();

            Path path = algo.FindShortestPath(graph);

            if (input.Stopwatch)
            {
                sw.Stop();
                ts = sw.Elapsed;
            }

            return new AlgorithmResultModel(name ,algoType, graph, path, input.Stopwatch, ts);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(FileInputViewModel.SourceFilePath)) || (e.PropertyName == nameof(FolderInputViewModel.SourceFolderPath)))
            {
                OnCanExecutedChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if ((InputViewModel.GetType().Name == nameof(FileInputViewModel)) && string.IsNullOrEmpty(((FileInputViewModel)InputViewModel).SourceFilePath))
            {
                return false && base.CanExecute(parameter);
            }
            if ((InputViewModel.GetType().Name == nameof(FolderInputViewModel)) && string.IsNullOrEmpty(((FolderInputViewModel)InputViewModel).SourceFolderPath))
            {
                return false && base.CanExecute(parameter);
            }
            return base.CanExecute(parameter);
        }        

        public override Task ExecuteAsync(object parameter)
        {
            var t = Task.Factory.StartNew(() =>
            {
                _results.CalculationsFinished = false;
                try
                {
                    StartCalculations();
                }
                catch (Exception exception)
                {
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;

                    MessageBox.Show(exception.Message, "Error", button, icon, MessageBoxResult.Yes);
                }
                _results.CalculationsFinished = true;
            });
            return t;
        }
    }
}
