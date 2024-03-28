using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
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
using Windows.Storage;
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
                List<AlgorithmResultModel> naResults = new(), dtResults = new(), chResults = new(), klResults = new(), klRbResults = new();
                AlgorithmResultModel avgResult;
                TimeSpan? averageTime = null, bestTime = null;
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

                        if (input.AverageResults)
                            naResults.Add(result);
                        else
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

                        if (input.AverageResults)
                            dtResults.Add(result);
                        else
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
                        _results.Message += $"\nProcessing {sampleName} with Chistofides' algorithm";

                        var result = StartCalculations($"{sampleName}", graph, TSPAlgorithms.Christofides);

                        if (input.AverageResults)
                            chResults.Add(result);
                        else
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

                        if (input.AverageResults)
                            klResults.Add(result);
                        else
                            App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_KL");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
                    if (input.KernighanLinRb)
                    {
                        _results.Message += $"\nProcessing {sampleName} with Kernighan - Lin reduced backtracking algorithm";

                        var result = StartCalculations($"{sampleName}", graph, TSPAlgorithms.KernighanLinRb);

                        if (input.AverageResults)
                            klRbResults.Add(result);
                        else
                            App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

                        if (!string.IsNullOrEmpty(input.OutputFolderPath))
                            TourSerializer.SerializePath(result.Path, input.OutputFolderPath, $"{sampleName}_KLrb");

                        if (_results.Message.ToCharArray().Count(c => c == '\n') > 0)
                            _results.Message = _results.Message[.._results.Message.IndexOf('\n')];
                        else
                            _results.Message = "";
                    }
                }
                if (input.AverageResults)
                {
                    if (input.NearestAddition)
                    {
                        if (input.Stopwatch)
                        {
                            averageTime = TimeSpan.FromMicroseconds(naResults.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                            bestTime = naResults.Select(r => r.BestTime).Min();
                        }

                        avgResult = new(
                            "sample_NA",
                            TSPAlgorithms.NearestAddition,
                            input.Stopwatch,
                            naResults.Select(r => r.Path.Length).Min(),
                            naResults.Select(r => r.Path.Length).Average(),
                            null,
                            null,
                            averageTime,
                            bestTime,
                            null
                        );

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
                    }
                    if (input.DoubleTree)
                    {
                        if (input.Stopwatch)
                        {
                            averageTime = TimeSpan.FromMicroseconds(dtResults.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                            bestTime = dtResults.Select(r => r.BestTime).Min();
                        }

                        avgResult = new(
                            "sample_DT",
                            TSPAlgorithms.DoubleTree,
                            input.Stopwatch,
                            dtResults.Select(r => r.Path.Length).Min(),
                            dtResults.Select(r => r.Path.Length).Average(),
                            null,
                            null,
                            averageTime,
                            bestTime,
                            null
                        );

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
                    }
                    if (input.Christofides)
                    {
                        if (input.Stopwatch)
                        {
                            averageTime = TimeSpan.FromMicroseconds(chResults.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                            bestTime = chResults.Select(r => r.BestTime).Min();
                        }

                        avgResult = new(
                            "sample_C",
                            TSPAlgorithms.Christofides,
                            input.Stopwatch,
                            chResults.Select(r => r.Path.Length).Min(),
                            chResults.Select(r => r.Path.Length).Average(),
                            null,
                            null,
                            averageTime,
                            bestTime,
                            null
                        );

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
                    }
                    if (input.KernighanLin)
                    {
                        if (input.Stopwatch)
                        {
                            averageTime = TimeSpan.FromMicroseconds(klResults.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                            bestTime = klResults.Select(r => r.BestTime).Min();
                        }

                        avgResult = new(
                            "sample_KL",
                            TSPAlgorithms.KernighanLin,
                            input.Stopwatch,
                            klResults.Select(r => r.Path.Length).Min(),
                            klResults.Select(r => r.Path.Length).Average(),
                            null,
                            null,
                            averageTime,
                            bestTime,
                            null
                        );

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
                    }
                    if (input.KernighanLinRb)
                    {
                        if (input.Stopwatch)
                        {
                            averageTime = TimeSpan.FromMicroseconds(klRbResults.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                            bestTime = klRbResults.Select(r => r.BestTime).Min();
                        }

                        avgResult = new(
                            "sample_KLrb",
                            TSPAlgorithms.KernighanLinRb,
                            input.Stopwatch,
                            klRbResults.Select(r => r.Path.Length).Min(),
                            klRbResults.Select(r => r.Path.Length).Average(),
                            null,
                            null,
                            averageTime,
                            bestTime,
                            null
                        );

                        App.Current.Dispatcher.Invoke(() => _results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
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
            if (input.KernighanLinRb)
            {
                _results.Message += $"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with Kernighan - Lin reduced backtracking algorithm";
                StartAlgorithmCalculations(sourceFilePath, resultFilePath, TSPAlgorithms.KernighanLinRb, input.OutputFolderPath);
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
                        case TSPAlgorithms.KernighanLinRb:
                            outputName += "_KLrb";
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
                case TSPAlgorithms.KernighanLinRb:
                    return StartAlgorithmCalculations(name, graph, new KeringhanLinReducedBacktracking(), TSPAlgorithms.KernighanLinRb);
                default:
                    return null;
            }            
        }

        private AlgorithmResultModel StartAlgorithmCalculations(string name, Graph graph, ITspAlgorithm algo, TSPAlgorithms algoType)
        {
            var input = (InputViewModel)InputViewModel;

            Stopwatch sw = new();
            TimeSpan? averageTime = null;
            TimeSpan? bestTime = null;
            if (input.Stopwatch)
                sw.Start();

            List<Path> paths = new ();
            List<TimeSpan> times = new();
            for (int i = 0; i < input.NumberOfCalculations; i++)
            {
                if (input.Stopwatch)
                    sw.Start();

                paths.Add(algo.FindShortestPath(graph));

                if (input.Stopwatch)
                {
                    sw.Stop();
                    times.Add(sw.Elapsed);
                }
            }

            if (input.Stopwatch)
            {
                averageTime = TimeSpan.FromMicroseconds(times.Select(t => ((TimeSpan)t).TotalMicroseconds).Average());
                bestTime = times.Min();
            }

            return new AlgorithmResultModel(name, algoType, input.Stopwatch, paths.Min(p => p.Length), paths.Average(p => p.Length), graph, paths.MinBy(p => p.Length), averageTime, bestTime);
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
