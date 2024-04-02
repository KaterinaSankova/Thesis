using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly ResultsViewModel results;
        private readonly CancellationTokenStore cancellationToken;

        private readonly NearestAddition nearestAdditionAlgo = new();
        private readonly DoubleTree doubleTreeAlgo = new();
        private readonly Christofides christofidesAlgo = new();
        private readonly KernighanLin kernighanLinAlgo = new();
        private readonly KeringhanLinReducedBacktracking kernignahLinRbAlgo = new();
        private readonly Random rand = new();

        //common
        private InputMode inputMode;
        private string outputFolderPath = "";
        private bool stopwatch;
        private readonly List<TSPAlgorithm> algorithmList = new();

        //file
        private string sourceFilePath = "";
        private string resultFilePath = "";

        //folder
        private string sourceFolderPath = "";
        private string resultFolderPath = "";
        private bool ignoreNotFoundResultFiles;

        //generate
        private int numberOfSamples;
        private int numberOfCities;
        private bool averageResults;
        private double highestX;
        private double lowestX;
        private double highestY;
        private double lowestY;

        public ViewModelBase InputViewModel => results.NavigationStore.CurrentViewModel;

        public StartCalculationsCommand(ResultsViewModel results, CancellationTokenStore cancellationToken)
        {
            this.results = results;
            this.cancellationToken = cancellationToken;
            SetupState();
        }

        private void SetupState()
        {
            inputMode = results.NavigationStore.GetInputMode();

            outputFolderPath = ((InputViewModel)InputViewModel).OutputFolderPath;
            stopwatch = ((InputViewModel)InputViewModel).Stopwatch;

            algorithmList.Clear();
            if (((InputViewModel)InputViewModel).NearestAddition)
                algorithmList.Add(TSPAlgorithm.NearestAddition);
            if (((InputViewModel)InputViewModel).DoubleTree)
                algorithmList.Add(TSPAlgorithm.DoubleTree);
            if (((InputViewModel)InputViewModel).Christofides)
                algorithmList.Add(TSPAlgorithm.Christofides);
            if (((InputViewModel)InputViewModel).KernighanLin)
                algorithmList.Add(TSPAlgorithm.KernighanLin);
            if (((InputViewModel)InputViewModel).KernighanLinRb)
                algorithmList.Add(TSPAlgorithm.KernighanLinRb);

            switch (inputMode)
            {
                case InputMode.File:
                    sourceFilePath = ((FileInputViewModel)InputViewModel).SourceFilePath;
                    resultFilePath = ((FileInputViewModel)InputViewModel).ResultFilePath;
                    break;
                case InputMode.Folder:
                    sourceFolderPath = ((FolderInputViewModel)InputViewModel).SourceFolderPath;
                    resultFolderPath = ((FolderInputViewModel)InputViewModel).ResultFolderPath;
                    ignoreNotFoundResultFiles = ((FolderInputViewModel)InputViewModel).IgnoreNotFoundResultFiles;
                    break;
                case InputMode.Generate:
                    numberOfSamples = ((GenerateInputViewModel)InputViewModel).NumberOfSamples;
                    numberOfCities = ((GenerateInputViewModel)InputViewModel).NumberOfCities;
                    averageResults = ((GenerateInputViewModel)InputViewModel).AverageResults;
                    highestX = ((GenerateInputViewModel)InputViewModel).HighestX;
                    lowestX = ((GenerateInputViewModel)InputViewModel).LowestX;
                    highestY = ((GenerateInputViewModel)InputViewModel).HighestY;
                    lowestY = ((GenerateInputViewModel)InputViewModel).LowestY;
                    break;
            }
            
            results.AlgoResults = new();
        }

        private void StartCalculations()
        {
            SetupState();
            switch (inputMode)
            {
                case InputMode.File:
                    StartCalculationsForFile(sourceFilePath, resultFilePath);
                    break;
                case InputMode.Folder:
                    string[] sourceFiles = GetSourceFiles();

                    int numberOfAllFiles = sourceFiles.Length;
                    int numberOfCurrentFile = 0;
                    foreach (var sourceFile in sourceFiles)
                    {
                        if (cancellationToken.Token.IsCancellationRequested)
                            return;
                        if (!StartCalculationsForFileFromFolder(sourceFile, numberOfCurrentFile, numberOfAllFiles))
                        {
                            results.RemoveLastMessage();
                            return;
                        }
                        numberOfCurrentFile++;
                    }

                    if (sourceFiles.Length > 0)
                        results.WriteMessage($"Completed {numberOfAllFiles}/{numberOfAllFiles}");

                    break;
                case InputMode.Generate:
                    List<AlgorithmResultModel> naResults = new(), dtResults = new(), chResults = new(), klResults = new(), klRbResults = new();
                    for (int i = 0; i < numberOfSamples; i++)
                    {
                        if (cancellationToken.Token.IsCancellationRequested)
                            break;

                        string sampleName = $"sample{i + 1}";

                        Graph graph = GenerateGraph();

                        if (!string.IsNullOrEmpty(outputFolderPath))
                            TSPSerializer.SerializeGraph(graph, outputFolderPath, sampleName);

                        foreach (var algo in algorithmList)
                            StartCalculationsForGeneratedSample(graph, sampleName, naResults, algo);
                    }
                    if (averageResults)
                    {
                        foreach (var algo in algorithmList)
                            AddAverageResult(naResults, algo);
                    }
                    break;
                default:
                    break;
            }
        }

        private bool StartCalculationsForFile(string sourceFilePath, string resultFilePath)
        {
            foreach (var algo in algorithmList)
            {
                try
                {
                    if (!StartAlgorithmCalculationsForFile(sourceFilePath, resultFilePath, algo))
                        return false;
                }
                catch (Exception exception)
                {
                    string message = $"{exception.Message}\nWould you like to continue?";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(message, "Error while processing file", button, icon, MessageBoxResult.Yes);

                    return result == MessageBoxResult.Yes;
                }
            }
            return true;
        }

        private string[] GetSourceFiles()
        {
            string[] sourceFiles;
            try
            {
                sourceFiles = Directory.GetFiles(sourceFolderPath, "*.tsp");
            }
            catch (Exception)
            {
                throw new Exception($"No files could have been retrieved from path '{sourceFolderPath}'.");
            }

            if (sourceFiles.Length == 0)
            {
                results.Message = $"No TSP files found in folder {sourceFolderPath}";
            }
            return sourceFiles;
        }

        private bool StartCalculationsForFileFromFolder(string sourceFile, int numberOfCurrentFile, int numberOfAllFiles)
        {
            string resultFile = "";
            if (!string.IsNullOrEmpty(resultFolderPath))
                resultFile = $"{resultFolderPath}\\{System.IO.Path.GetFileName(sourceFile)[..^4]}.opt.tour";

            results.WriteMessage($"Completed {numberOfCurrentFile}/{numberOfAllFiles}");

            return StartCalculationsForFile(sourceFile, resultFile);
        }

        private Graph GenerateGraph()
        {
            Graph graph = new();
            for (int i = 0; i < numberOfCities; i++)
            {
                var x = Math.Round(rand.NextDouble() * (highestX - lowestX) + lowestX, 2);
                var y = Math.Round(rand.NextDouble() * (highestY - lowestY) + lowestY, 2);
                graph.nodes.Add(new Node(i, x, y));
            }
            return graph;
        }

        private void StartCalculationsForGeneratedSample(Graph graph, string sampleName, List<AlgorithmResultModel> results, TSPAlgorithm algo)
        {

            this.results.AddMessage($"\nProcessing {sampleName} with {TSPAlgorithmExtentions.ToFullString(algo)}");

            var result = StartAlgorithmCalculations(sampleName, graph, algo);
            if (result == null)
            {
                this.results.RemoveLastMessage();
                return;
            }

            if (averageResults)
                results.Add(result);
            else
                App.Current.Dispatcher.Invoke(() => this.results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

            if (!string.IsNullOrEmpty(outputFolderPath))
                TourSerializer.SerializePath(result.Path, outputFolderPath, $"{sampleName}_{TSPAlgorithmExtentions.ToAbbreviationString(algo)}");

            this.results.RemoveLastMessage();
        }

        private void AddAverageResult(List<AlgorithmResultModel> results, TSPAlgorithm algo)
        {
            TimeSpan? averageTime = null, bestTime = null;

            if (stopwatch)
            {
                averageTime = TimeSpan.FromMicroseconds(results.Select(r => ((TimeSpan)r.BestTime).TotalMicroseconds).Average());
                bestTime = results.Select(r => r.BestTime).Min();
            }

            AlgorithmResultModel avgResult = new(
                $"sample_{TSPAlgorithmExtentions.ToAbbreviationString(algo)}",
                algo,
                stopwatch,
                results.Select(r => r.Path.Length).Min(),
                results.Select(r => r.Path.Length).Average(),
                null,
                null,
                averageTime,
                bestTime,
                null
            );

            App.Current.Dispatcher.Invoke(() => this.results.AlgoResults.Add(new AlgorithmResultViewModel(avgResult)));
        }

        private bool StartAlgorithmCalculationsForFile(string sourceFilePath, string resultFilePath, TSPAlgorithm algo)
        {
            results.AddMessage($"\nProcessing file {System.IO.Path.GetFileName(sourceFilePath)} with {TSPAlgorithmExtentions.ToFullString(algo)}");

            var graph = TSPDeserializer.DeserializeGraph(sourceFilePath);

            var fileName = System.IO.Path.GetFileName(sourceFilePath);
            fileName = fileName[0..fileName.IndexOf('.')];
            var result = StartAlgorithmCalculations(fileName, graph, algo);

            if (result == null)
            {
                results.RemoveLastMessage();
                return false;
            }

            if (!File.Exists(resultFilePath))
            {
                if (!ignoreNotFoundResultFiles && !string.IsNullOrEmpty(resultFilePath))
                {
                    string message = $"Result file '{resultFilePath}' could not been found.\nWould you like to continue?";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult msbxRslt;

                    msbxRslt = MessageBox.Show(message, "Error while processing file", button, icon, MessageBoxResult.Yes);

                    if (msbxRslt == MessageBoxResult.No)
                    {
                        results.RemoveLastMessage();
                        return false;
                    }
                }
            }
            else
            {
                result.ResultPath = TourDeserializer.DeserializePath(resultFilePath, graph);
            }
            App.Current.Dispatcher.Invoke(() => results.AlgoResults.Add(new AlgorithmResultViewModel(result)));

            if (!string.IsNullOrEmpty(outputFolderPath))
                SaveCalculationsResult(result, algo);

            results.RemoveLastMessage();
            return true;
        }

        private AlgorithmResultModel? StartAlgorithmCalculations(string name, Graph graph, TSPAlgorithm algo)
        {
            if (cancellationToken.Token.IsCancellationRequested)
                return null;

            Stopwatch sw = new();
            TimeSpan? time = null;
            if (stopwatch)
                sw.Start();

            if (stopwatch)
                sw.Start();

            Path path = GetAlgorithm(algo).FindShortestPath(graph);

            if (stopwatch)
            {
                sw.Stop();
                time = sw.Elapsed;
            }

            return new AlgorithmResultModel(name, algo, stopwatch, path.Length, path.Length, graph, path, time, time);
        }

        private void SaveCalculationsResult(AlgorithmResultModel result, TSPAlgorithm algo)
        {
            if (result.Path != null)
                TourSerializer.SerializePath(result.Path, outputFolderPath, $"{result.Name}_{TSPAlgorithmExtentions.ToAbbreviationString(algo)}");
        }

        public ITspAlgorithm GetAlgorithm(TSPAlgorithm algo)
        {
            switch (algo)
            {
                case TSPAlgorithm.NearestAddition:
                    return nearestAdditionAlgo;
                case TSPAlgorithm.DoubleTree:
                    return doubleTreeAlgo;
                case TSPAlgorithm.Christofides:
                    return christofidesAlgo;
                case TSPAlgorithm.KernighanLin:
                    return kernighanLinAlgo;
                case TSPAlgorithm.KernighanLinRb:
                    return kernignahLinRbAlgo;
                default:
                    throw new Exception("Unknown algorithm type");
            }
        }

        public override Task ExecuteAsync(object parameter)
        {
            var t = Task.Factory.StartNew(() =>
            {
                results.CalculationsFinished = false;
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
                cancellationToken.RestoreToken();
                results.CalculationsFinished = true;
            });
            return t;
        }
    }
}
