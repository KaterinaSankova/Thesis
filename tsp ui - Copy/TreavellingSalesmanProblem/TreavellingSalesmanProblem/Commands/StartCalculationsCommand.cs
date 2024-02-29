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
using System.Xml.Linq;
using TravellingSalesmanProblem.Algorithms.Enums;
using TravellingSalesmanProblem.Algorithms.TSP;
using TravellingSalesmanProblem.Formats;
using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using TreavellingSalesmanProblem;
using TSP.Enum;
using TSP.Models;
using TSP.Stores;
using TSP.ViewModels;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TSP.Commands
{
    public class StartCalculationsCommand : AsyncCommandBase
    {
        private ResultsViewModel _results;
        private Random rand = new();

        public ViewModelBase InputViewModel => _results.NavigationStore.CurrentViewModel;

        public StartCalculationsCommand(ResultsViewModel results)
        {
            this._results = results;
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

                    _results.Message = $"Completed {numberOfCurrentFile}/{numberOfAllFiles}";

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

                _results.Message = $"Completed {numberOfAllFiles}/{numberOfAllFiles}";
            }
            else
            {
                var input = (GenerateInputViewModel)InputViewModel;
                for (int i = 0; i < input.NumberOfSamples; i++)
                {
                    string sampleName = $"sample{i + 1}";

                    Graph graph = new();
                    for (int j = 0; j < input.NumberOfCities; j++)
                    {
                        var x = Math.Round(rand.NextDouble() * (input.HighestX - input.LowestX) + input.LowestX, 2);
                        var y = Math.Round(rand.NextDouble() * (input.HighestY - input.LowestY) + input.LowestY, 2);
                        graph.nodes.Add(new Node(j, x, y));
                    }

                    if (!string.IsNullOrEmpty(input.OutputFolderPath))
                        TSPSerializer.SerializeGraph(graph, input.OutputFolderPath, sampleName);

                    if (input.NearestAddition)
                    {
                        _results.Message += $"\nProcessing {sampleName} with nearest addition algorithm";

                        var result = StartCalculations(sampleName, graph, TSPAlgorithms.NearestAddition);

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_NA");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
                    if (input.DoubleTree)
                    {
                        _results.Message += $"\nProcessing {sampleName} with double tree algorithm";

                        var result = StartCalculations(sampleName, graph, TSPAlgorithms.DoubleTree);

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_DT");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
                    if (input.Christofides)
                    {
                        _results.Message += $"\nProcessing{sampleName} with Chistofides' algorithm";

                        var result = StartCalculations($"{sampleName}", graph, TSPAlgorithms.Christofides);

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_C");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
                    if (input.KernighanLin)
                    {
                        _results.Message += $"\nProcessing {sampleName} with Kernighan - Lin algorithm";

                        var result = StartCalculations($"{sampleName}", graph, TSPAlgorithms.KernighanLin);

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_KL");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
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
            var input = (InputViewModel)InputViewModel;
            if (input.NearestAddition)
            {
                _results.Message += $"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with nearest addition algorithm";
                StartAlgorithmCalculations(sourceFilePath, resultFilePath, TSPAlgorithms.NearestAddition, input.OutputFolderPath);
                if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                    _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                else
                    _results.Message = "";
            }
            if (input.DoubleTree)
            {
                _results.Message += $"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with double tree algorithm";
                StartAlgorithmCalculations(sourceFilePath, resultFilePath, TSPAlgorithms.DoubleTree, input.OutputFolderPath);
                if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                    _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                else
                    _results.Message = "";
            }
            if (input.Christofides)
            {
                _results.Message += $"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with Christofides' algorithm";
                StartAlgorithmCalculations(sourceFilePath, resultFilePath, TSPAlgorithms.Christofides, input.OutputFolderPath);
                if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                    _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                else
                    _results.Message = "";
            }
            if (input.KernighanLin)
            {
                _results.Message += $"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with Kernighan - Lin algorithm";
                StartAlgorithmCalculations(sourceFilePath, resultFilePath, TSPAlgorithms.KernighanLin, input.OutputFolderPath);
                if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                    _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                else
                    _results.Message = "";
            }
        }

        private void StartAlgorithmCalculations(string sourceFilePath, string resultFilePath, TSPAlgorithms algo, string outputFolder)
        {
            var graph = TSPDeserializer.DeserializeGraph(sourceFilePath);
            var result = StartCalculations(System.IO.Path.GetFileName(sourceFilePath), graph, algo);
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
                        {
                            if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                                _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                            else
                                _results.Message = "";
                            return;
                        }
                    }
                }
                else
                {
                    result.ResultPath = TourDeserializer.DeserializePath(resultFilePath, graph);
                }
                App.Current.Dispatcher.Invoke(() =>_results.AlgoResults.Add(new AlgorithmResultViewModel(result)));


                if (!string.IsNullOrEmpty(outputFolder))
                {
                    string outputName = result.Name;
                    switch (algo)
                    {
                        case TSPAlgorithms.NearestAddition:
                            outputName += "_NA";
                            break;
                        case TSPAlgorithms.DoubleTree:
                            outputName += "_DA";
                            break;
                        case TSPAlgorithms.Christofides:
                            outputName += "_C";
                            break;
                        case TSPAlgorithms.KernighanLin:
                            outputName += "_KL";
                            break;
                        default:
                            throw new Exception("Invalid algorithm type.");
                    }
                    TourSerializer.SerializePath(result.Path, outputFolder, outputName);
                }
            }
        }

        private AlgorithmResultModel? StartCalculations(string fileName, Graph graph, TSPAlgorithms algo)
        {
            string name = fileName;
            if (fileName.Contains('.'))
                name = fileName[0..fileName.IndexOf('.')];
            switch (algo)
            {
                case TSPAlgorithms.NearestAddition:
                    return StartAlgorithmCalculations(name, graph, new NearestAddition(), TSPAlgorithms.NearestAddition);
                case TSPAlgorithms.DoubleTree:
                    return StartAlgorithmCalculations(name, graph, new DoubleTree(), TSPAlgorithms.DoubleTree);
                case TSPAlgorithms.Christofides:
                    return StartAlgorithmCalculations(name, graph, new Christofides(), TSPAlgorithms.Christofides);
                case TSPAlgorithms.KernighanLin:
                    return StartAlgorithmCalculations(name, graph, new KernighanLin(), TSPAlgorithms.KernighanLin);
                default:
                    return null;
            }            
        }

        private AlgorithmResultModel StartAlgorithmCalculations(string name, Graph graph, ITspAlgorithm algo, TSPAlgorithms algoType)
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

            return new AlgorithmResultModel(name, algoType, graph, path, input.Stopwatch, ts);
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
