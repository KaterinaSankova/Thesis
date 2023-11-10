using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSPAlgorithms
{
    public static class KernighanLin
    {
        private static Graph graph;
        private static Path path;

        private static List<Edge> brokenEdges;
        private static List<Edge> addedEdges;

        private static double improvement;
        private static double partialSum;

        private static Random rand = new Random();

        //Avoiding checkout time
        private static List<(Path Path, List<Node> Nodes)> fruitlessNodesForPaths = new();

        //Reduction
        private static Path[] locallyOptimalPaths = new Path[5]; //constants
        private static List<Edge> goodEdges = new();

        private static Path shortestPath;

        private static Node startingNode;
        private static Node enclosingNode;

        //Diagnostics
        //private static KeringhanLin//diagnoser //diagnoser = new();
        private static Stopwatch findShortestPathStopWatch = new Stopwatch();

        public static Path FindShortestPath(Graph inputGraph)
        {
            findShortestPathStopWatch.Start();

            if (inputGraph.nodes.Count < 4)
                return new Path(inputGraph.nodes.ToList().Append(inputGraph.nodes.First()).ToList());

            graph = inputGraph;
            GeneratePath();
            SetupInitialState();

            bool continueImproving = FindShortestPath(path);
            while (continueImproving)
            {
                continueImproving = FindShortestPath(shortestPath);
            }

            //diagnoser.FindShortestPath.AddRecord(findShortestPathStopWatch.Elapsed);
            //diagnoser.//print();
            return shortestPath;
        }

        private static bool FindShortestPath(Path inputPath)
        {
            findShortestPathStopWatch.Stop();
            //diagnoser.FindShortestPath.AddRecord(findShortestPathStopWatch.Elapsed);
            findShortestPathStopWatch.Restart();

            path = inputPath;
            SetupEdges();
            improvement = 0;
            partialSum = 0;
            //printState();
            return ImprovePath();
        }

        private static void GeneratePath() => path = new Path(graph.nodes.OrderBy(_ => rand.Next()).ToList());

        private static void SetupInitialState()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            improvement = 0;
            partialSum = 0;

            fruitlessNodesForPaths = new List<(Path Path, List<Node> Nodes)>();

            locallyOptimalPaths = new Path[5]; //constants
            goodEdges = new List<Edge>();

            shortestPath = path.ToPath();

            stopwatch.Stop();
            //diagnoser.SetupInitialState.AddRecord(stopwatch.Elapsed);
        }

        private static void SetupEdges()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            path.GetEdges();
            brokenEdges = new List<Edge>();
            addedEdges = new List<Edge>();

            stopwatch.Stop();
            //diagnoser.SetupEdges.AddRecord(stopwatch.Elapsed);
        }

        private static bool ImprovePath()
        {
            int i = 0;
            Path currentPath;
            var stopwatch = new Stopwatch();

            var fruitlessNodes = GetFruitlessNodesForPath();
            foreach (var node1 in graph.nodes)
            {
                stopwatch.Restart();

                if (fruitlessNodes.Contains(node1))
                    continue;
                startingNode = node1;
                currentPath = path.ToPath();
                RestoreState(currentPath, i);

                stopwatch.Stop();
                //diagnoser.ImprovePath.Node1.AddRecord(stopwatch.Elapsed);

                foreach (var node2 in FindNode2(node1))
                {
                    i = 1;
                    currentPath = path.ToPath();
                    RestoreState(currentPath, i);

                    Edge brokenEdge1 = new Edge(node1, node2);

                    enclosingNode = node2;
                    //printImprovementState(node1, node2, 1);
                    //console.WriteLine($"\tCURRENT PATH: {currentPath}");
                    //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    currentPath.SetDirection(node1, node2);
                    currentPath.CurrentIndex = currentPath.IndexOf(node2);

                    List<(Node Node3, Node Node4, Node AlternativeNode4, double Value)> nextPairValues = new();
                    Node investigatedNode3 = null;
                    Node investigatedNode4 = null;
                    var stoppingNode = currentPath.PeekAfter(2);
                    currentPath.Next();
                    while (investigatedNode3 != stoppingNode)
                    {
                        investigatedNode3 = currentPath.Next();
                        investigatedNode4 = currentPath.PeekPrev();
                        nextPairValues.Add(
                            (investigatedNode3, investigatedNode4, currentPath.PeekNext(), investigatedNode3.Distance(investigatedNode4) - node2.Distance(investigatedNode3))
                            );
                    }
                    var orderedNextPairs = nextPairValues.OrderByDescending(v => v.Value).ToList();

                    stopwatch.Stop();
                    //diagnoser.ImprovePath.BrokenEdge1.AddRecord(stopwatch.Elapsed);

                    foreach (var nextPair in orderedNextPairs)
                    {
                        stopwatch.Restart();

                        i = 1;
                        currentPath = path.ToPath();
                        RestoreState(currentPath, i);
                        enclosingNode = node2;

                        currentPath.SetDirection(startingNode, enclosingNode);

                        var addedEdge1 = new Edge(node2, nextPair.Node3);

                        //printImprovementState(nextPair.Node3, nextPair.Node4, 3);
                        //console.WriteLine($"\tLATEST PATH: {currentPath}");
                        //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                        if (!UpdatePartialSum(brokenEdge1.Length(), addedEdge1.Length()))
                            break;
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);

                        stopwatch.Stop();
                        //diagnoser.ImprovePath.AddedEdge1.AddRecord(stopwatch.Elapsed);

                        bool pathImproved = ImprovePathFromBrokenEdge2(nextPair.Node3, nextPair.Node4, currentPath);
                        if (pathImproved)
                            return true;
                        if (nextPair.AlternativeNode4 != node1)
                            pathImproved = ImprovePathFromAlternativeBrokenEdge2(node2, nextPair.Node3, nextPair.AlternativeNode4, currentPath);
                        if (pathImproved)
                            return true;
                    }

                }
            }

            return false;
        }

        private static bool ImprovePathFromBrokenEdge2(Node node3, Node node4, Path latestPath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            latestPath = latestPath.ToPath();
            //console.WriteLine("[+] IMPROVE PATH FROM BROKEN EDGE 2");
            var brokenEdge2 = new Edge(node3, node4);
            int i = 2;

            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();

            latestPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);

            reconnectEdgesStopwatch.Stop();
            //diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);
            //console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            UpdateLocalOptimum(latestPath, i);
            enclosingNode = node4;

            RestoreState(latestPath, i);

            latestPath.SetDirection(startingNode, enclosingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);

            var stoppingNode = latestPath.PeekAfter(2);

            List<(Node Node5, Node Node6, double Value)> pairValues = new();
            double pairImprovement;
            Node investigatedNode5 = latestPath.Next(2);
            Node investigatedNode6 = null;
            while (investigatedNode5 != stoppingNode)
            {
                if (brokenEdges.Contains(new Edge(node4, investigatedNode5)))
                    continue;
                investigatedNode6 = latestPath.PeekPrev();
                if (!addedEdges.Contains(new Edge(investigatedNode5, investigatedNode6)))
                {
                    pairImprovement = investigatedNode5.Distance(investigatedNode6) - enclosingNode.Distance(investigatedNode5);
                    if (pairImprovement > 0)
                    {
                        pairValues.Add((investigatedNode5, investigatedNode6, pairImprovement));
                    }
                }
                investigatedNode5 = latestPath.Next();
            }
            var orderedPairValues = pairValues.OrderByDescending(v => v.Value).ToList();

            stopwatch.Stop();
            //diagnoser.ImprovePath.BrokenEdge2.AddRecord(stopwatch.Elapsed);

            foreach (var pair in orderedPairValues)
            {
                stopwatch.Restart();

                i = 2;
                RestoreState(latestPath, i);

                var currentPath = latestPath.ToPath();
                enclosingNode = node4;

                var addedEdge2 = new Edge(node4, pair.Node5);
                var brokenEdge3 = new Edge(pair.Node5, pair.Node6);

                //printImprovementState(pair.Node5, pair.Node6, 5);
                //console.WriteLine($"\tLATEST PATH: {currentPath}");
                //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(brokenEdge2.Length(), addedEdge2.Length()))
                    break;

                brokenEdges.Insert(i - 1, brokenEdge2);
                addedEdges.Insert(i - 1, addedEdge2);

                reconnectEdgesStopwatch.Restart();

                currentPath.ReconnectEdges(startingNode, enclosingNode, pair.Node5, pair.Node6);

                reconnectEdgesStopwatch.Stop();
                //diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);

                //console.WriteLine($"\tCURRENT PATH: {currentPath}");
                //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                UpdateLocalOptimum(currentPath, i);
                //if (UpdateLocalOptimum(currentPath, i))
                //    NonsequentialExchange(currentPath, i);
                enclosingNode = pair.Node6;

                stopwatch.Stop();
                //diagnoser.ImprovePath.AddedEdge2.AddRecord(stopwatch.Elapsed);

                ImprovePathFromNode(pair.Node6, brokenEdge3, currentPath, i + 1);

                if (improvement == 0)
                {
                    var paths = fruitlessNodesForPaths.Where(r => r.Path.Equals(path)).ToList();
                    if (paths.Count != 0)
                        paths.First().Nodes.Add(startingNode);
                    else
                        fruitlessNodesForPaths.Add((path.ToPath(), new List<Node>() { startingNode }));
                }
                if (improvement > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ImprovePathFromAlternativeBrokenEdge2(Node node2, Node node3, Node node4, Path latestPath)
        {
            //console.WriteLine("[+] IMPROVE PATH FROM ALTERNATIVE BROKEN EDGE 2");
            int i = 2;
            RestoreState(latestPath, i);
            //console.WriteLine("[+] IMPROVE PATH FROM ALTERNATIVE BROKEN EDGE 2");

            AlterBrokenEdge2(node2, node3, node4, latestPath, i);

            if (improvement == 0)
            {
                var paths = fruitlessNodesForPaths.Where(r => r.Path.Equals(path)).ToList();
                if (paths.Count != 0)
                    paths.First().Nodes.Add(startingNode);
                else
                    fruitlessNodesForPaths.Add((path.ToPath(), new List<Node>() { startingNode }));
            }
            if (improvement > 0)
            {
                return true;
            }

            return false;
        }

        private static bool AlterBrokenEdge2(Node enclosingNode, Node pair1Node1, Node pair1Node2, Path latestPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            bool pathImproved = AlterBrokenEdge2Option1(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

            stopwatch.Stop();
            //diagnoser.ImprovePath.AlternativeBrokenEdge2Option1.AddRecord(stopwatch.Elapsed);
            if (!pathImproved)
            {
                //console.WriteLine("\tIMPROVEMENT ");

                stopwatch.Start();

                pathImproved = AlterBrokenEdge2Option2(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

                stopwatch.Stop();
                //diagnoser.ImprovePath.AlternativeBrokenEdge2Option2.AddRecord(stopwatch.Elapsed);

                return pathImproved;
            }
            return true;
        }

        private static bool AlterBrokenEdge2Option1(Node enclosingNode, Node pair1Node1, Node pair1Node2, Path latestPath, int i)
        {
            //console.WriteLine("[+] ALTER BROKEN EDGE 2 OPTION 1");
            var pair1BrokenEdge = new Edge(pair1Node1, pair1Node2);

            //console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            latestPath.SetDirection(pair1Node2, pair1Node1);
            latestPath.CurrentIndex = latestPath.IndexOf(pair1Node1);
            //latestPath.Next();

            List<(Node Node, double Value)> pair2Node1Values = new List<(Node Node, double Value)>();
            Node investigatedPair2Node1 = latestPath.Next();
            while (investigatedPair2Node1 != enclosingNode)
            {
                if (brokenEdges.Contains(new Edge(pair1Node2, investigatedPair2Node1)))
                    continue;
                pair2Node1Values.Add((investigatedPair2Node1, new Edge(pair1Node2, investigatedPair2Node1).Length()));
                investigatedPair2Node1 = latestPath.Next();
            }
            var orderedPair2Node1s = pair2Node1Values.OrderBy(v => v.Value).Select(v => v.Node).ToList();

            int index = -1;
            Node pair2Node1, pair2Node2;
            Edge pair1AddedEdge, pair2BrokenEdge;
            List<(Node Pair2Node1, Node Pair2Node2, double Gain)> nextExchangePairs = new();
            for (int j = 0; j < 5;)
            {
                index++;
                if (index == orderedPair2Node1s.Count)
                    break;

                pair2Node1 = orderedPair2Node1s[index];
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);

                pair2Node2 = latestPath.PeekPrev(pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                if (pair2Node2 == pair1Node1 || addedEdges.Contains(pair2BrokenEdge))
                {
                    pair2Node2 = latestPath.PeekNext(pair2Node1);
                    pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);
                    if (addedEdges.Contains(pair2BrokenEdge))
                        continue;
                }

                nextExchangePairs.Add((pair2Node1, pair2Node2, pair2BrokenEdge.Length() - pair1AddedEdge.Length()));
                j++;
            }

            nextExchangePairs = nextExchangePairs.OrderByDescending(p => p.Gain).ToList();

            if (nextExchangePairs.Count > 0)
            {
                var currentPath = latestPath.ToPath();
                pair2Node1 = nextExchangePairs.First().Pair2Node1;
                pair2Node2 = nextExchangePairs.First().Pair2Node2;
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                //printImprovementState(pair2Node1, pair2Node2, 5);
                //console.WriteLine($"\tLATEST PATH: {currentPath}");
                //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(pair1BrokenEdge.Length(), pair1AddedEdge.Length()))
                    return false;

                brokenEdges.Insert(i - 1, pair1BrokenEdge);
                addedEdges.Insert(i - 1, pair1AddedEdge);

                var reconnectEdgesStopwatch = new Stopwatch();
                reconnectEdgesStopwatch.Start();
                currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2);
                reconnectEdgesStopwatch.Stop();
                //diagnoser.ReconnectEdgesAlternativeBrokenEdge2Option1.AddRecord(reconnectEdgesStopwatch.Elapsed);

                //console.WriteLine($"\tCURRENT PATH: {currentPath}");
                //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                UpdateLocalOptimum(currentPath, i);
                //if (UpdateLocalOptimum(currentPath, i))
                //    NonsequentialExchange(currentPath, i);
                KernighanLin.enclosingNode = pair2Node2;

                ImprovePathFromNode(pair2Node2, pair2BrokenEdge, currentPath, i + 1);
            }

            return true;
        }

        private static bool AlterBrokenEdge2Option2(Node enclosingNode, Node pair1Node1, Node pair1Node2, Path latestPath, int i)
        {
            //console.WriteLine("[+] ALTER BTOKEN EDGE 2 OPTION 2");
            var pair1BrokenEdge = new Edge(pair1Node1, pair1Node2);

            //console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            latestPath.SetDirection(pair1Node1, pair1Node2);
            latestPath.CurrentIndex = latestPath.IndexOf(pair1Node2);

            List<(Node Node, double Value)> pair2Node1Values = new List<(Node Node, double Value)>();
            Node investigatedPair2Node1 = latestPath.Next();
            while (investigatedPair2Node1 != startingNode)
            {
                if (brokenEdges.Contains(new Edge(pair1Node2, investigatedPair2Node1)))
                    continue;
                pair2Node1Values.Add((investigatedPair2Node1, new Edge(pair1Node2, investigatedPair2Node1).Length()));
                investigatedPair2Node1 = latestPath.Next();
            }
            var orderedPair2Node1s = pair2Node1Values.OrderBy(v => v.Value).Select(v => v.Node).ToList();

            int index = -1;
            Node pair2Node1, pair2Node2;
            Edge pair1AddedEdge, pair2BrokenEdge;
            List<(Node Pair2Node1, Node Pair2Node2, double Gain)> nextExchangePairs = new();
            for (int j = 0; j < 5;)
            {
                index++;
                if (index == orderedPair2Node1s.Count)
                    break;

                pair2Node1 = orderedPair2Node1s[index];
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);

                pair2Node2 = latestPath.PeekPrev(pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                if (pair2Node2 == pair1Node2 || addedEdges.Contains(pair2BrokenEdge))
                    continue;

                nextExchangePairs.Add((pair2Node1, pair2Node2, pair2BrokenEdge.Length() - pair1AddedEdge.Length()));
                j++;
            }

            nextExchangePairs = nextExchangePairs.OrderByDescending(p => p.Gain).ToList();
            if (nextExchangePairs.Count > 0)
            {
                i++;
                var currentPath = latestPath.ToPath();
                pair2Node1 = nextExchangePairs.First().Pair2Node1;
                pair2Node2 = nextExchangePairs.First().Pair2Node2;
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                //printImprovementState(pair2Node1, pair2Node2, 5);
                //console.WriteLine($"\tLATEST PATH: {currentPath}");
                //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(pair1BrokenEdge.Length(), pair1AddedEdge.Length()))
                    return false;

                latestPath.SetDirection(pair1Node2, pair1Node1);
                latestPath.CurrentIndex = latestPath.IndexOf(pair1Node1);

                List<(Node Node, double Value)> pair3Node1Values = new List<(Node Node, double Value)>();
                Node investigatedpair3Node1 = latestPath.Next();
                while (investigatedpair3Node1 != enclosingNode)
                {
                    if (brokenEdges.Contains(new Edge(pair1Node2, investigatedpair3Node1)))
                        continue;
                    pair3Node1Values.Add((investigatedpair3Node1, new Edge(pair2Node2, investigatedpair3Node1).Length()));
                    investigatedpair3Node1 = latestPath.Next();
                }
                var orderedPair3Node1s = pair3Node1Values.OrderBy(v => v.Value).Select(v => v.Node).ToList();

                index = -1;
                Node pair3Node1, pair3Node2, alternativepair3Node2;
                Edge pair2AddedEdge, pair3BrokenEdge, alternativePair3BrokenEdge;
                var nextNextExchangePairs = new List<(Node Pair3Node1, Node Pair3Node2, double Gain)>();
                double gain;
                for (int j = 0; j < 5;)
                {
                    index++;
                    if (index == orderedPair3Node1s.Count)
                        break;

                    pair3Node1 = orderedPair3Node1s[index];
                    pair2AddedEdge = new Edge(pair2Node2, pair3Node1);

                    pair3Node2 = latestPath.PeekPrev(pair3Node1);
                    pair3BrokenEdge = new Edge(pair3Node1, pair3Node2);
                    alternativepair3Node2 = latestPath.PeekNext(pair3Node1);
                    alternativePair3BrokenEdge = new Edge(pair3Node1, pair3Node2);

                    if (pair3Node2 == pair1Node1 || addedEdges.Contains(pair3BrokenEdge))
                    {
                        if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                            continue;

                        nextNextExchangePairs.Add((pair3Node1, alternativepair3Node2, alternativePair3BrokenEdge.Length() - pair2AddedEdge.Length()));
                    }
                    else if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                    {
                        nextNextExchangePairs.Add((pair3Node1, pair3Node2, pair3BrokenEdge.Length() - pair2AddedEdge.Length()));
                    }
                    else
                    {
                        if (pair3BrokenEdge.Length() <= alternativePair3BrokenEdge.Length())
                            nextNextExchangePairs.Add((pair3Node1, pair3Node2, pair3BrokenEdge.Length() - pair2AddedEdge.Length()));
                        else
                            nextNextExchangePairs.Add((pair3Node1, alternativepair3Node2, alternativePair3BrokenEdge.Length() - pair2AddedEdge.Length()));
                    }

                    j++;
                }

                nextNextExchangePairs = nextNextExchangePairs.OrderByDescending(p => p.Gain).ToList();
                if (nextNextExchangePairs.Count > 0)
                {
                    currentPath = latestPath.ToPath();
                    pair3Node1 = nextNextExchangePairs.First().Pair3Node1;
                    pair3Node2 = nextNextExchangePairs.First().Pair3Node2;
                    pair2AddedEdge = new Edge(pair2Node2, pair3Node1);
                    pair3BrokenEdge = new Edge(pair3Node1, pair3Node2);

                    //printImprovementState(pair3Node1, pair3Node2, 7);
                    //console.WriteLine($"\tLATEST PATH: {currentPath}");
                    //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    if (!UpdatePartialSum(pair2BrokenEdge.Length(), pair2AddedEdge.Length()))
                        return false;

                    brokenEdges.Insert(i - 2, pair1BrokenEdge);
                    addedEdges.Insert(i - 2, pair1AddedEdge);

                    brokenEdges.Insert(i - 1, pair2BrokenEdge);
                    addedEdges.Insert(i - 1, pair2AddedEdge);

                    var reconnectEdgesStopwatch = new Stopwatch();
                    reconnectEdgesStopwatch.Start();
                    currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2, pair3Node1, pair3Node2);
                    reconnectEdgesStopwatch.Stop();
                    //diagnoser.ReconnectEdgesAlternativeBrokenEdge2Option2.AddRecord(reconnectEdgesStopwatch.Elapsed);
                    //console.WriteLine($"\tCURRENT PATH: {currentPath}");
                    //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    UpdateLocalOptimum(currentPath, i);
                    //if (UpdateLocalOptimum(currentPath, i))
                    //    NonsequentialExchange(currentPath, i);
                    KernighanLin.enclosingNode = pair3Node2;

                    ImprovePathFromNode(pair3Node2, pair3BrokenEdge, currentPath, i + 1);
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        private static void ImprovePathFromNode(Node fromNode, Edge lastBrokenEdge, Path latestPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            latestPath.SetDirection(startingNode, enclosingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);

            (Node Node, Node NextNode, double Value) nextPair = (null, null, double.MaxValue);
            List<(Node Node, Node NextNode, double Value)> nodesValues = new List<(Node Node, Node nextNode, double Value)>();
            var stoppingNode = latestPath.PeekAfter(2);
            Node investigatedNodes = latestPath.Next();
            Node investigatedNextNode = null;
            double nextPairImprovement;
            while (investigatedNodes != stoppingNode)
            {
                investigatedNodes = latestPath.Next();
                if (brokenEdges.Contains(new Edge(fromNode, investigatedNodes)))
                    continue;
                if (goodEdges.Contains(new Edge(fromNode, investigatedNodes)))
                    continue;
                investigatedNextNode = latestPath.PeekPrev();
                if (!addedEdges.Contains(new Edge(investigatedNodes, investigatedNextNode)))
                {
                    nextPairImprovement = investigatedNodes.Distance(investigatedNextNode) - enclosingNode.Distance(investigatedNodes);
                    if (nextPair.Value > nextPairImprovement)
                    {
                        nextPair = (investigatedNodes, investigatedNextNode, nextPairImprovement);
                    }
                }
            }

            if (nextPair.Node == null)
                return;

            var currentPath = latestPath.ToPath();
            enclosingNode = fromNode;

            var addedEdge = new Edge(fromNode, nextPair.Node);
            var nextBrokenEdge = new Edge(nextPair.Node, nextPair.NextNode);

            //printImprovementState(nextPair.Node, nextPair.NextNode, 2 * i + 1);
            //console.WriteLine($"\tLATEST PATH: {currentPath}");
            //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            if (!UpdatePartialSum(lastBrokenEdge.Length(), addedEdge.Length()))
                return;

            brokenEdges.Add(lastBrokenEdge);
            addedEdges.Add(addedEdge);

            //console.WriteLine("[+] RECONNECTING EDGES");
            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();
            currentPath.ReconnectEdges(startingNode, enclosingNode, nextPair.Node, nextPair.NextNode);
            reconnectEdgesStopwatch.Stop();
            //diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);
            //console.WriteLine($"\tCURRENT PATH: {currentPath}");
            //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            UpdateLocalOptimum(currentPath, i);
            //if (UpdateLocalOptimum(currentPath, i))
            //    NonsequentialExchange(currentPath, i);

            enclosingNode = nextPair.NextNode;

            stopwatch.Stop();
            //diagnoser.ImprovePath.FromNode.AddRecord(stopwatch.Elapsed);

            ImprovePathFromNode(nextPair.NextNode, nextBrokenEdge, currentPath, i + 1);
        }

        private static bool NonsequentialExchange(Path latestPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //console.WriteLine("[+] NONSEQUENTIAL EXCHANGE");
            //console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            Path currentPath = latestPath.ToPath();
            if (currentPath.Count < 8)
                return false;

            currentPath.SetDirection(currentPath[0], currentPath[1]);
            currentPath.CurrentIndex = 0;

            List<(Edge[] BrokenEdges, Edge[] AddedEdges, double Improvement)> exchangableEdges = new();
            double brokenEdgesLengthSum, addedEdgesLengthSum, improvement;
            for (int j = 0; j < 2; j++)
            {
                Edge brokenEdge1 = new Edge(currentPath[j], currentPath.PeekNext(j));
                if (addedEdges.Contains(brokenEdge1))
                    continue;
                if (goodEdges.Contains(brokenEdge1))
                    continue;

                for (int k = j + 2; k < currentPath.Count - 4; k++)
                {
                    Edge brokenEdge4 = new Edge(currentPath[k], currentPath.PeekNext(k));
                    if (addedEdges.Contains(brokenEdge4))
                        continue;
                    if (goodEdges.Contains(brokenEdge4))
                        continue;

                    for (int l = k + 2; l < currentPath.Count - 2; l++)
                    {
                        Edge brokenEdge2 = new Edge(currentPath[l], currentPath.PeekNext(l));
                        if (addedEdges.Contains(brokenEdge2))
                            continue;
                        if (goodEdges.Contains(brokenEdge2))
                            continue;

                        for (int m = l + 2; m < currentPath.Count; m++)
                        {
                            if (currentPath.PeekNext(m) == currentPath[j])
                                continue;

                            Edge brokenEdge3 = new Edge(currentPath[m], currentPath.PeekNext(m));
                            if (addedEdges.Contains(brokenEdge3))
                                continue;
                            if (goodEdges.Contains(brokenEdge3))
                                continue;

                            Edge addedEdge1 = new Edge(brokenEdge1.node1, brokenEdge2.node2);
                            if (brokenEdges.Contains(addedEdge1))
                                continue;
                            Edge addedEdge2 = new Edge(brokenEdge2.node1, brokenEdge1.node2);
                            if (brokenEdges.Contains(addedEdge2))
                                continue;
                            Edge addedEdge3 = new Edge(brokenEdge3.node1, brokenEdge4.node2);
                            if (brokenEdges.Contains(addedEdge3))
                                continue;
                            Edge addedEdge4 = new Edge(brokenEdge3.node1, brokenEdge3.node2);
                            if (brokenEdges.Contains(addedEdge4))
                                continue;

                            brokenEdgesLengthSum = brokenEdge1.Length() + brokenEdge2.Length() + brokenEdge3.Length() + brokenEdge4.Length();
                            addedEdgesLengthSum = addedEdge1.Length() + addedEdge2.Length() + addedEdge3.Length() + addedEdge4.Length();

                            if (brokenEdgesLengthSum - addedEdgesLengthSum + partialSum <= 0)
                                continue;

                            exchangableEdges.Add((
                                new Edge[4] { brokenEdge1, brokenEdge2, brokenEdge3, brokenEdge4 },
                                new Edge[4] { addedEdge1, addedEdge2, addedEdge3, addedEdge4 },
                                brokenEdgesLengthSum - addedEdgesLengthSum
                            ));
                        }
                    }
                }
            }

            var orderedExchangeableEdges = exchangableEdges.OrderByDescending(e => e.Improvement).ToList();
            if (orderedExchangeableEdges.Count == 0)
            {
                //console.WriteLine("\tNo edges for nonsequentail exchange were found.");
                stopwatch.Stop();
                //diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
                return false;
            }
            var edgesToExchange = orderedExchangeableEdges.First();

            if (!UpdatePartialSum(edgesToExchange.Improvement))
            {
                //console.WriteLine("\tNo nonsequentail exchange improving path was found.");
                stopwatch.Stop();
                //diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
                return false;
            }

            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();
            currentPath.ReconnectEdges(edgesToExchange);
            reconnectEdgesStopwatch.Stop();
            //diagnoser.ReconnectEdgesNonsequentialExchange.AddRecord(reconnectEdgesStopwatch.Elapsed);

            //console.WriteLine($"\tCURRENT PATH: {currentPath}");
            //console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            UpdateLocalOptimum(currentPath, i);

            stopwatch.Stop();
            //diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
            return true;
        }

        private static List<Node> GetFruitlessNodesForPath()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var record in fruitlessNodesForPaths)
            {
                if (path.Equals(record.Path))
                    return record.Nodes;
            }

            stopwatch.Stop();
            //diagnoser.GetFruitlessNodesForPath.AddRecord(stopwatch.Elapsed);
            return new List<Node>();
        }

        private static Node[] FindNode2(Node node)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int nodeIndex = path.IndexOf(node);
            var nodes2 = new Node[2] { path.PeekPrev(nodeIndex), path.PeekNext(nodeIndex) };

            if (new Edge(node, nodes2[1]).Length() > new Edge(node, nodes2[0]).Length())
                (nodes2[0], nodes2[1]) = (nodes2[1], nodes2[0]);

            stopwatch.Stop();
            //diagnoser.FindNode2.AddRecord(stopwatch.Elapsed);

            return nodes2;
        }

        private static void RestoreState(Path currentPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            partialSum = 0;

            if (i == 2)
            {
                if (brokenEdges.Count > 0)
                {
                    brokenEdges = brokenEdges.Take(1).ToList();
                    addedEdges = brokenEdges.Take(1).ToList();
                }
            }
            else
            {
                brokenEdges.Clear();
                addedEdges.Clear();
            }
            stopwatch.Stop();
            //diagnoser.RestoreState.AddRecord(stopwatch.Elapsed);
        }

        private static bool UpdatePartialSum(double brokenEdgeLegth, double addedEdgeLength) => UpdatePartialSum(brokenEdgeLegth - addedEdgeLength);

        private static bool UpdatePartialSum(double improvement)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //console.WriteLine("[+] PARTIAL SUM UPDATE");
            //console.WriteLine($"\tPARTIAL SUM: {partialSum}");
            //console.WriteLine($"\tIMPROVEMENT: {improvement}");
            double currentPartialSum = partialSum + improvement;
            //console.WriteLine($"\tCURRENT PARTIAL SUM: {currentPartialSum}");
            if (currentPartialSum > 0)
            {
                //console.WriteLine("\tUpdating partial sum\n");
                partialSum = currentPartialSum;
                return true;
            }
            //console.WriteLine("\tNot updating partial sum\n");

            stopwatch.Stop();
            //diagnoser.UpdatePartialSum.AddRecord(stopwatch.Elapsed);
            return false;
        }

        private static bool UpdateLocalOptimum(Path currentPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //console.WriteLine("[+] LOCAL OPTIMUM UPDATE");

            double currentImprovement = path.Length - currentPath.Length;
            //console.WriteLine($"\tIMPROVEMENT: {improvement}");
            //console.WriteLine($"\tCURRENT IMPROVEMENT: {currentImprovement}");
            if (currentImprovement > improvement)
            {
                //console.WriteLine($"\tCurrent path is local optimum\n");
                improvement = currentImprovement;

                UpdateLocallyOptimalPaths(currentPath);

                UpdateShortestPath(currentPath);

                //console.WriteLine();

                stopwatch.Stop();
                //diagnoser.UpdateLocalOptimum.AddRecord(stopwatch.Elapsed);
                return true;
            }
            else
            {
                //console.WriteLine($"\tCurrent path is NOT local optimum\n");

                stopwatch.Stop();
                //diagnoser.UpdateLocalOptimum.AddRecord(stopwatch.Elapsed);
                return false;
            }
        }

        private static void UpdateLocallyOptimalPaths(Path currentPath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            double highestOptimalPathLength = -1;
            int index = -1;
            int count = 5;
            for (int j = 0; j < 5; j++)
            {
                if (locallyOptimalPaths[j] == null)
                {
                    index = -1;
                    count = j;
                    break;
                }
                if (locallyOptimalPaths[j].Length > highestOptimalPathLength)
                {
                    highestOptimalPathLength = locallyOptimalPaths[j].Length;
                    index = j;
                }
            }
            if (count < 5)
            {
                //console.WriteLine($"\tAdding current path to list of locally optimal paths\n");
                locallyOptimalPaths[count] = currentPath;
                UpdateShortestPath(currentPath);
                return;
            }
            if (index == -1)
                return;

            //console.WriteLine("\tLOCALLY OPTIMAL PATHS");
            foreach (var path in locallyOptimalPaths)
            {
                if (path != null)
                {
                    //console.WriteLine($"\t\tPath: {path}");
                    //console.WriteLine($"\t\tLength: {path.Length}");
                }
            }

            if (currentPath.Length < highestOptimalPathLength)
            {
                //console.WriteLine($"\tAdding current path to list of locally optimal paths");
                locallyOptimalPaths[index] = currentPath;
                UpdateGoodEdges(currentPath, count, index);
            }

            stopwatch.Stop();
            //diagnoser.UpdateLocallyOptimalPaths.AddRecord(stopwatch.Elapsed);
        }

        private static void UpdateGoodEdges(Path currentPath, int count, int index)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var getEdgesStopwatch = new Stopwatch();
            getEdgesStopwatch.Start();
            var pathEdges = currentPath.GetEdges();
            getEdgesStopwatch.Stop();
            //diagnoser.GetEdges.AddRecord(getEdgesStopwatch.Elapsed);

            var newGoodEdges = pathEdges;
            //console.WriteLine($"\tUpdating good edges");
            for (int j = 0; j < count; j++)
            {
                if (j == index)
                    continue;

                getEdgesStopwatch.Restart();
                var optimalPathEdges = locallyOptimalPaths[j].GetEdges();
                getEdgesStopwatch.Stop();
                //diagnoser.GetEdges.AddRecord(getEdgesStopwatch.Elapsed);

                newGoodEdges = newGoodEdges.Intersect(optimalPathEdges).ToList();
            }
            goodEdges = newGoodEdges;
            //printEdges("\tGOOD EDGES", goodEdges);

            stopwatch.Stop();
            //diagnoser.UpdateGoodEdges.AddRecord(stopwatch.Elapsed);
        }

        private static void UpdateShortestPath(Path currentPath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //console.WriteLine("[+] UPDATE SHORTEST PATH");
            //console.WriteLine($"\tSHORTEST PATH: {shortestPath}");
            //console.WriteLine($"\tSHORTEST PATH LENGTH: {shortestPath.Length}");
            if (currentPath.Length < shortestPath.Length)
            {
                //console.WriteLine("\t*Updating shortest path*");
                shortestPath = currentPath;
                //console.WriteLine($"\tNEW SHORTEST PATH: {shortestPath}");
                //console.WriteLine($"\tNEW SHORTEST PATH LENGTH: {shortestPath.Length}");
            }
            //console.WriteLine();

            stopwatch.Stop();
            //diagnoser.UpdateShortestPath.AddRecord(stopwatch.Elapsed);
        }
        
    }
}
