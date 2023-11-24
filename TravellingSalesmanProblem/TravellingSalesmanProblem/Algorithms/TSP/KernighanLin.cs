using System.Diagnostics;
using System.Xml.Linq;
using TravellingSalesmanProblem.Diagnostics;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KernighanLin
    {
        private Graph? graph;
        private KernighanLinPath? path;

        private List<Edge>? brokenEdges;
        private List<Edge>? addedEdges;

        private double improvement;
        private double partialSum;

        private Random rand = new Random();

        //Avoiding checkout time
        private List<(Node Node, List<KernighanLinPath> Paths)> checkedOutPathsForNodes = new();

        //Reduction
        private KernighanLinPath[] locallyOptimalPaths = new KernighanLinPath[5]; //constants
        private List<Edge> goodEdges = new();

        private KernighanLinPath? shortestPath;

        private Node? startingNode;
        private Node? enclosingNode;

        //Diagnostics
        private readonly KeringhanLinDiagnoser diagnoser = new();
        private readonly Stopwatch findShortestPathStopWatch = new Stopwatch();
        
        public KernighanLinPath FindShortestPath(Graph inputGraph)
        {
            findShortestPathStopWatch.Start();

            if (inputGraph.nodes.Count < 4)
                return new KernighanLinPath(inputGraph.nodes.ToList().Append(inputGraph.nodes.First()).ToList());

            graph = inputGraph;
            GeneratePath();
            SetupInitialState();

            bool continueImproving = FindShortestPath(path);
            while (continueImproving)
            {
                continueImproving = FindShortestPath(shortestPath);
            }

            diagnoser.FindShortestPath.AddRecord(findShortestPathStopWatch.Elapsed);
            //diagnoser.Print();
            return shortestPath;
        }

        private bool FindShortestPath(KernighanLinPath inputPath)
        {
            findShortestPathStopWatch.Stop();
            diagnoser.FindShortestPath.AddRecord(findShortestPathStopWatch.Elapsed);
            findShortestPathStopWatch.Restart();

            path = inputPath;
            SetupEdges();
            improvement = 0;
            partialSum = 0;
            //PrintState();
            return ImprovePath();
        }

        private void GeneratePath() => path = new KernighanLinPath(graph.nodes.OrderBy(_ => rand.Next()).ToList());

        private void SetupInitialState()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            improvement = 0;
            partialSum = 0;

            checkedOutPathsForNodes = new();

            locallyOptimalPaths = new KernighanLinPath[5]; //constants
            goodEdges = new List<Edge>();

            shortestPath = path.ToPath();

            stopwatch.Stop();
            diagnoser.SetupInitialState.AddRecord(stopwatch.Elapsed);
        }

        private void SetupEdges()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            brokenEdges = new List<Edge>();
            addedEdges = new List<Edge>();

            stopwatch.Stop();
            diagnoser.SetupEdges.AddRecord(stopwatch.Elapsed);
        }

        private bool ImprovePath()
        {
            int i = 0;
            KernighanLinPath currentPath;
            var stopwatch = new Stopwatch();

            foreach (var node1 in graph.nodes.OrderBy(x => rand.Next()).ToList())
            {
                stopwatch.Restart();

                if (IsNodeCheckedOut(node1))
                    continue;

                startingNode = node1;
                currentPath = path.ToPath();
                RestoreState(currentPath, i);

                stopwatch.Stop();
                diagnoser.ImprovePath.Node1.AddRecord(stopwatch.Elapsed);

                foreach (var node2 in FindNode2(node1))
                {
                    i = 1;
                    currentPath = path.ToPath();
                    RestoreState(currentPath, i);

                    Edge brokenEdge1 = new Edge(node1, node2);

                    enclosingNode = node2;
                    //PrintImprovementState(node1, node2, 1);
                    //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                    //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    //currentPath.SetDirection(node1, node2);
                    //currentPath.CurrentIndex = currentPath.IndexOf(node2);
                    currentPath.SetStartingPoint(node2, node1, Direction.Backwards);

                    List<(Node Node3, Node Node4, Node AlternativeNode4, double Value)> nextPairValues = new();
                    Node investigatedNode3 = null;
                    Node investigatedNode4 = null;
                    var stoppingNode = currentPath.PeekBefore(2);
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
                    diagnoser.ImprovePath.BrokenEdge1.AddRecord(stopwatch.Elapsed);

                    foreach (var nextPair in orderedNextPairs)
                    {
                        stopwatch.Restart();

                        i = 1;
                        currentPath = path.ToPath();
                        RestoreState(currentPath, i);
                        enclosingNode = node2;

                        //currentPath.SetDirection(startingNode, enclosingNode);

                        var addedEdge1 = new Edge(node2, nextPair.Node3);

                        //PrintImprovementState(nextPair.Node3, nextPair.Node4, 3);
                        //Console.WriteLine($"\tLATEST PATH: {currentPath}");
                        //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                        if (!UpdatePartialSum(brokenEdge1.Length, addedEdge1.Length))
                            break;
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);

                        stopwatch.Stop();
                        diagnoser.ImprovePath.AddedEdge1.AddRecord(stopwatch.Elapsed);

                        bool pathImproved = ImprovePathFromBrokenEdge2(nextPair.Node3, nextPair.Node4, currentPath);
                        if (pathImproved)
                        {
                            UpdateLocalOptimum(shortestPath);
                            AddCheckOutNodes(node1, path);
                            NonsequentialExchange(path);
                            return true;
                        }
                        if (nextPair.AlternativeNode4 != node1)
                            pathImproved = ImprovePathFromAlternativeBrokenEdge2(node2, nextPair.Node3, nextPair.AlternativeNode4, currentPath);
                        if (pathImproved)
                        {
                            UpdateLocalOptimum(shortestPath);
                            AddCheckOutNodes(node1, path);
                            NonsequentialExchange(path);
                            return true;
                        }
                    }

                }

                AddCheckOutNodes(node1, path);
            }

            return false;
        }

        private void AddCheckOutNodes(Node node, KernighanLinPath path)
        {
            var nodes = checkedOutPathsForNodes.Where(r => r.Node.Equals(node)).ToList();
            if (nodes.Count != 0)
                nodes.First().Paths.Add(path);
            else
                checkedOutPathsForNodes.Add((node, new List<KernighanLinPath>() { path }));
        }

        private bool ImprovePathFromBrokenEdge2(Node node3, Node node4, KernighanLinPath latestPath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            latestPath = latestPath.ToPath();
            //Console.WriteLine("[+] IMPROVE PATH FROM BROKEN EDGE 2");
            var brokenEdge2 = new Edge(node3, node4);
            int i = 2;

            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();

            latestPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);

            reconnectEdgesStopwatch.Stop();
            diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);
            //Console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //Console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            UpdateShortestPath(latestPath);
            enclosingNode = node4;

            RestoreState(latestPath, i);

            //latestPath.SetDirection(startingNode, enclosingNode);
            //latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);
            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);

            var stoppingNode = latestPath.PeekBefore(2);

            List<(Node Node5, Node Node6, double Value)> pairValues = new();
            double pairImprovement;
            Node investigatedNode5 = latestPath.Next(2);
            Node investigatedNode6 = null;
            while (investigatedNode5 != stoppingNode)
            {
                if (brokenEdges.Contains(new(node4, investigatedNode5)))
                    continue;
                investigatedNode6 = latestPath.PeekPrev();
                if (!addedEdges.Contains(new(investigatedNode5, investigatedNode6)))
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
            diagnoser.ImprovePath.BrokenEdge2.AddRecord(stopwatch.Elapsed);

            foreach (var pair in orderedPairValues)
            {
                stopwatch.Restart();

                i = 2;
                RestoreState(latestPath, i);

                var currentPath = latestPath.ToPath();
                enclosingNode = node4;

                var addedEdge2 = new Edge(node4, pair.Node5);
                var brokenEdge3 = new Edge(pair.Node5, pair.Node6);

                //PrintImprovementState(pair.Node5, pair.Node6, 5);
                //Console.WriteLine($"\tLATEST PATH: {currentPath}");
                //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(brokenEdge2.Length, addedEdge2.Length))
                    break;

                brokenEdges.Insert(i - 1, brokenEdge2);
                addedEdges.Insert(i - 1, addedEdge2);

                reconnectEdgesStopwatch.Restart();

                currentPath.ReconnectEdges(startingNode, enclosingNode, pair.Node5, pair.Node6);

                reconnectEdgesStopwatch.Stop();
                diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);

                //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                UpdateShortestPath(currentPath);
                //if (UpdateLocalOptimum(currentPath, i))
                //    NonsequentialExchange(currentPath, i);
                enclosingNode = pair.Node6;

                stopwatch.Stop();
                diagnoser.ImprovePath.AddedEdge2.AddRecord(stopwatch.Elapsed);

                ImprovePathFromNode(pair.Node6, brokenEdge3, currentPath, i + 1);

                if (improvement > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ImprovePathFromAlternativeBrokenEdge2(Node node2, Node node3, Node node4, KernighanLinPath latestPath)
        {
            //Console.WriteLine("[+] IMPROVE PATH FROM ALTERNATIVE BROKEN EDGE 2");
            int i = 2;
            RestoreState(latestPath, i);
            //Console.WriteLine("[+] IMPROVE PATH FROM ALTERNATIVE BROKEN EDGE 2");

            AlterBrokenEdge2(node2, node3, node4, latestPath, i);

            if (improvement > 0)
            {
                return true;
            }

            return false;
        }

        private bool AlterBrokenEdge2(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            bool pathImproved = AlterBrokenEdge2Option1(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

            stopwatch.Stop();
            diagnoser.ImprovePath.AlternativeBrokenEdge2Option1.AddRecord(stopwatch.Elapsed);
            if (!pathImproved)
            {
                //Console.WriteLine("\tIMPROVEMENT ");

                stopwatch.Start();

                pathImproved = AlterBrokenEdge2Option2(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

                stopwatch.Stop();
                diagnoser.ImprovePath.AlternativeBrokenEdge2Option2.AddRecord(stopwatch.Elapsed);

                return pathImproved;
            }
            return true;
        }

        private bool AlterBrokenEdge2Option1(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            //Console.WriteLine("[+] ALTER BROKEN EDGE 2 OPTION 1");
            var pair1BrokenEdge = new Edge(pair1Node1, pair1Node2);

            //Console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //Console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            //latestPath.SetDirection(pair1Node2, pair1Node1);
            //latestPath.CurrentIndex = latestPath.IndexOf(pair1Node1);
            latestPath.SetStartingPoint(pair1Node1, pair1Node2, Direction.Backwards);

            List<(Node Node1, Node Node2, double Value)> pair2Node1Values = new();
            Node investigatedPair2Node1 = latestPath.Next(2);
            Node investigatedPair2Node2 = null;
            while (investigatedPair2Node1 != enclosingNode)
            {
                investigatedPair2Node2 = latestPath.PeekPrev();
                if (brokenEdges.Contains(new(pair1Node2, investigatedPair2Node1)))
                    continue;
                pair2Node1Values.Add((investigatedPair2Node1, investigatedPair2Node2, pair1Node2.Distance(investigatedPair2Node1)));

                investigatedPair2Node1 = latestPath.Next();
            }
            var orderedPair2Node1s = pair2Node1Values.OrderBy(v => v.Value).ToList();

            int index = -1;
            Node pair2Node1, pair2Node2;
            Edge pair1AddedEdge, pair2BrokenEdge;
            List<(Node Pair2Node1, Node Pair2Node2, double Gain)> nextExchangePairs = new();
            for (int j = 0; j < 5;)
            {
                index++;
                if (index == orderedPair2Node1s.Count)
                    break;

                pair2Node1 = orderedPair2Node1s[index].Node1;
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);

                pair2Node2 = orderedPair2Node1s[index].Node2;
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                if (addedEdges.Contains(pair2BrokenEdge))
                    continue;

                nextExchangePairs.Add((pair2Node1, pair2Node2, pair2BrokenEdge.Length - pair1AddedEdge.Length));
                j++;
            }

            nextExchangePairs = nextExchangePairs.OrderByDescending(p => p.Gain).ToList();

            
            if (nextExchangePairs.Count == 0)
            {
                //latestPath.SetDirection(pair1Node2, pair1Node1);
                //latestPath.CurrentIndex = latestPath.IndexOf(pair1Node1);
                latestPath.SetStartingPoint(pair1Node1, pair1Node2, Direction.Backwards);

                pair2Node1Values = new();
                investigatedPair2Node1 = latestPath.Next();
                investigatedPair2Node2 = null;
                while (latestPath.PeekNext() != enclosingNode)
                {
                    if (brokenEdges.Contains(new(pair1Node2, investigatedPair2Node1)))
                        continue;
                    investigatedPair2Node2 = latestPath.PeekNext();
                    pair2Node1Values.Add((investigatedPair2Node1, investigatedPair2Node2, pair1Node2.Distance(investigatedPair2Node1)));

                    investigatedPair2Node1 = latestPath.Next();
                }
                orderedPair2Node1s = pair2Node1Values.OrderBy(v => v.Value).ToList();

                index = -1;
                nextExchangePairs = new();
                for (int j = 0; j < 5;)
                {
                    index++;
                    if (index == orderedPair2Node1s.Count)
                        break;

                    pair2Node1 = orderedPair2Node1s[index].Node1;
                    pair1AddedEdge = new Edge(pair1Node2, pair2Node1);

                    pair2Node2 = orderedPair2Node1s[index].Node2;
                    pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                    if (addedEdges.Contains(pair2BrokenEdge))
                        continue;

                    nextExchangePairs.Add((pair2Node1, pair2Node2, pair2BrokenEdge.Length - pair1AddedEdge.Length));
                    j++;
                }

                nextExchangePairs = nextExchangePairs.OrderByDescending(p => p.Gain).ToList();
            }

            if (nextExchangePairs.Count > 0)
            {
                var currentPath = latestPath.ToPath();
                pair2Node1 = nextExchangePairs.First().Pair2Node1;
                pair2Node2 = nextExchangePairs.First().Pair2Node2;
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                //PrintImprovementState(pair2Node1, pair2Node2, 5);
                //Console.WriteLine($"\tLATEST PATH: {currentPath}");
                //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(pair1BrokenEdge.Length, pair1AddedEdge.Length))
                    return false;

                brokenEdges.Insert(i - 1, pair1BrokenEdge);
                addedEdges.Insert(i - 1, pair1AddedEdge);

                var reconnectEdgesStopwatch = new Stopwatch();
                reconnectEdgesStopwatch.Start();
                currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2);
                reconnectEdgesStopwatch.Stop();
                diagnoser.ReconnectEdgesAlternativeBrokenEdge2Option1.AddRecord(reconnectEdgesStopwatch.Elapsed);

                //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                UpdateShortestPath(currentPath);
                //if (UpdateLocalOptimum(currentPath, i))
                //    NonsequentialExchange(currentPath, i);
                this.enclosingNode = pair2Node2;

                ImprovePathFromNode(pair2Node2, pair2BrokenEdge, currentPath, i + 1);
            }

            return true;
        }

        private bool AlterBrokenEdge2Option2(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            //Console.WriteLine("[+] ALTER BTOKEN EDGE 2 OPTION 2");
            var pair1BrokenEdge = new Edge(pair1Node1, pair1Node2);

            //Console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //Console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            //latestPath.SetDirection(pair1Node1, pair1Node2);
            //latestPath.CurrentIndex = latestPath.IndexOf(pair1Node2);
            latestPath.SetStartingPoint(pair1Node2, pair1Node1, Direction.Backwards);

            List<(Node Pair2Node1, Node Pair2Node2, double Value)> pair2Values = new();
            Node investigatedPair2Node1 = latestPath.Next();
            Node investigatedPair2Node2 = null;
            while (investigatedPair2Node1 != startingNode)
            {
                if (brokenEdges.Contains(new(pair1Node2, investigatedPair2Node1)))
                    continue;
                investigatedPair2Node2 = latestPath.PeekPrev();
                pair2Values.Add((investigatedPair2Node1, investigatedPair2Node2, pair1Node2.Distance(investigatedPair2Node1)));
                investigatedPair2Node1 = latestPath.Next();
            }
            var orderedPair2Node1s = pair2Values.OrderBy(v => v.Value).ToList();

            int index = -1;
            Node pair2Node1, pair2Node2;
            Edge pair1AddedEdge, pair2BrokenEdge;

            (Node Pair2Node1, Node Pair2Node2, double Gain) nextExchangePair = (null, null, double.MinValue);
            double gain;
            for (int j = 0; j < 5;)
            {
                index++;
                if (index == orderedPair2Node1s.Count)
                    break;

                pair2Node1 = orderedPair2Node1s[index].Pair2Node1;
                pair2Node2 = orderedPair2Node1s[index].Pair2Node2;

                if (pair2Node2 == pair1Node2 || addedEdges.Contains(new(pair2Node1, pair2Node2)))
                    continue;

                gain = pair2Node1.Distance(pair2Node2) - pair1Node2.Distance(pair2Node2);

                if (gain > nextExchangePair.Gain)
                    nextExchangePair = (pair2Node1, pair2Node2, gain);

                j++;
            }

            if (nextExchangePair.Pair2Node1 != null)
            {
                i++;
                var currentPath = latestPath.ToPath();
                pair2Node1 = nextExchangePair.Pair2Node1;
                pair2Node2 = nextExchangePair.Pair2Node2;
                pair1AddedEdge = new Edge(pair1Node2, pair2Node1);
                pair2BrokenEdge = new Edge(pair2Node1, pair2Node2);

                //PrintImprovementState(pair2Node1, pair2Node2, 5);
                //Console.WriteLine($"\tLATEST PATH: {currentPath}");
                //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(pair1BrokenEdge.Length, pair1AddedEdge.Length))
                    return false;

                //latestPath.SetDirection(pair1Node2, pair1Node1);
                //latestPath.CurrentIndex = latestPath.IndexOf(pair1Node1);
                latestPath.SetStartingPoint(pair1Node1, pair1Node2, Direction.Backwards);

                List<(Node Node, double Value)> pair3Node1Values = new List<(Node Node, double Value)>();
                Node investigatedpair3Node1 = latestPath.Next();
                while (investigatedpair3Node1 != enclosingNode)
                {
                    if (brokenEdges.Contains(new(pair1Node2, investigatedpair3Node1)))
                        continue;
                    pair3Node1Values.Add((investigatedpair3Node1, pair2Node2.Distance(investigatedpair3Node1)));
                    investigatedpair3Node1 = latestPath.Next();
                }
                var orderedPair3Node1s = pair3Node1Values.OrderBy(v => v.Value).Select(v => v.Node).ToList();

                index = -1;
                Node pair3Node1, pair3Node2, alternativepair3Node2;
                Edge pair2AddedEdge, pair3BrokenEdge, alternativePair3BrokenEdge;
                (Node Pair3Node1, Node Pair3Node2, double Gain) nextNextExchangePair = (null, null, double.MinValue);
                var nextNextExchangePairs = new List<(Node Pair3Node1, Node Pair3Node2, double Gain)>();
                for (int j = 0; j < 5;)
                {
                    index++;
                    if (index == orderedPair3Node1s.Count)
                        break;

                    pair3Node1 = orderedPair3Node1s[index];
                    pair3Node2 = latestPath.PeekPrev(pair3Node1);
                    pair3BrokenEdge = new Edge(pair3Node1, pair3Node2);
                    alternativepair3Node2 = latestPath.PeekNext(pair3Node1);
                    alternativePair3BrokenEdge = new Edge(pair3Node1, pair3Node2);

                    if (pair3Node2 == pair1Node1 || addedEdges.Contains(pair3BrokenEdge))
                    {
                        if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                            continue;

                        gain = alternativePair3BrokenEdge.Length - pair2Node2.Distance(pair3Node1);
                        if (nextNextExchangePair.Gain < gain)
                            nextNextExchangePair = (pair3Node1, alternativepair3Node2, gain);
                    }
                    else if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                    {
                        gain = pair3BrokenEdge.Length - pair2Node2.Distance(pair3Node1);
                        if (nextNextExchangePair.Gain < gain)
                            nextNextExchangePair = (pair3Node1, pair3Node2, gain);
                    }
                    else
                    {
                        if (pair3BrokenEdge.Length <= alternativePair3BrokenEdge.Length)
                        {
                            gain = pair3BrokenEdge.Length - pair2Node2.Distance(pair3Node1);
                            if (nextNextExchangePair.Gain < gain)
                                nextNextExchangePair = (pair3Node1, pair3Node2, gain);
                        }
                        else
                        {
                            gain = alternativePair3BrokenEdge.Length - pair2Node2.Distance(pair3Node1);
                            if (nextNextExchangePair.Gain < gain)
                                nextNextExchangePair = (pair3Node1, alternativepair3Node2, gain);
                        }
                    }

                    j++;
                }

                if (nextNextExchangePair.Pair3Node1 != null)
                {
                    currentPath = latestPath.ToPath();
                    pair3Node1 = nextNextExchangePair.Pair3Node1;
                    pair3Node2 = nextNextExchangePair.Pair3Node2;
                    pair2AddedEdge = new Edge(pair2Node2, pair3Node1);
                    pair3BrokenEdge = new Edge(pair3Node1, pair3Node2);

                    //PrintImprovementState(pair3Node1, pair3Node2, 7);
                    //Console.WriteLine($"\tLATEST PATH: {currentPath}");
                    //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    if (!UpdatePartialSum(pair2BrokenEdge.Length, pair2AddedEdge.Length))
                        return false;

                    brokenEdges.Insert(i - 2, pair1BrokenEdge);
                    addedEdges.Insert(i - 2, pair1AddedEdge);

                    brokenEdges.Insert(i - 1, pair2BrokenEdge);
                    addedEdges.Insert(i - 1, pair2AddedEdge);

                    var reconnectEdgesStopwatch = new Stopwatch();
                    reconnectEdgesStopwatch.Start();
                    currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2, pair3Node1, pair3Node2);
                    reconnectEdgesStopwatch.Stop();
                    diagnoser.ReconnectEdgesAlternativeBrokenEdge2Option2.AddRecord(reconnectEdgesStopwatch.Elapsed);
                    //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                    //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    UpdateShortestPath(currentPath);
                    //if (UpdateLocalOptimum(currentPath, i))
                    //    NonsequentialExchange(currentPath, i);
                    this.enclosingNode = pair3Node2;

                    ImprovePathFromNode(pair3Node2, pair3BrokenEdge, currentPath, i + 1);
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        private void ImprovePathFromNode(Node fromNode, Edge lastBrokenEdge, KernighanLinPath latestPath, int i)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //latestPath.SetDirection(startingNode, enclosingNode);
            //latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);
            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);

            (Node Node, Node NextNode, double Value) nextPair = (null, null, double.MaxValue);
            List<(Node Node, Node NextNode, double Value)> nodesValues = new List<(Node Node, Node nextNode, double Value)>();
            var stoppingNode = latestPath.PeekBefore(2);
            Node investigatedNodes = latestPath.Next();
            Node investigatedNextNode = null;
            double nextPairImprovement;
            while (investigatedNodes != stoppingNode)
            {
                investigatedNodes = latestPath.Next();
                if (brokenEdges.Contains(new(fromNode, investigatedNodes)))
                    continue;
                if (goodEdges.Contains(new(fromNode, investigatedNodes)))
                    continue;
                investigatedNextNode = latestPath.PeekPrev();
                if (!addedEdges.Contains(new(investigatedNodes, investigatedNextNode)))
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

            //PrintImprovementState(nextPair.Node, nextPair.NextNode, 2*i+1);
            //Console.WriteLine($"\tLATEST PATH: {currentPath}");
            //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            if (!UpdatePartialSum(lastBrokenEdge.Length, addedEdge.Length))
                return;

            brokenEdges.Add(lastBrokenEdge);
            addedEdges.Add(addedEdge);

            //Console.WriteLine("[+] RECONNECTING EDGES");
            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();
            currentPath.ReconnectEdges(startingNode, enclosingNode, nextPair.Node, nextPair.NextNode);
            reconnectEdgesStopwatch.Stop();
            diagnoser.ReconnectEdges.AddRecord(reconnectEdgesStopwatch.Elapsed);
            //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
            //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            UpdateShortestPath(currentPath);
            //if (UpdateLocalOptimum(currentPath, i))
            //    NonsequentialExchange(currentPath, i);

            enclosingNode = nextPair.NextNode;

            stopwatch.Stop();
            diagnoser.ImprovePath.FromNode.AddRecord(stopwatch.Elapsed);

            ImprovePathFromNode(nextPair.NextNode, nextBrokenEdge, currentPath, i+1);
        }

        private bool NonsequentialExchange(KernighanLinPath latestPath)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Console.WriteLine("[+] NONSEQUENTIAL EXCHANGE");
            //Console.WriteLine($"\tCURRENT PATH: {latestPath}");
            //Console.WriteLine($"\tLENGTH: {latestPath.Length}\n");

            KernighanLinPath currentPath = latestPath.ToPath();
            if (currentPath.Count < 8)
                return false;

            currentPath.SetDirection(currentPath[0], currentPath[1]);
            currentPath.CurrentIndex = 0;
            /*
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
                //Console.WriteLine("\tNo edges for nonsequentail exchange were found.");
                stopwatch.Stop();
                diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
                return false;
            }
            
            var edgesToExchange = orderedExchangeableEdges.First();
            */

            if (brokenEdges.Count < 4)
                return false;

            (Edge[] BrokenEdges, Edge[] AddedEdges) edgesToExchange = (brokenEdges.Take(4).ToArray(), addedEdges.Take(4).ToArray());
            double improvement = edgesToExchange.BrokenEdges.Sum(x => x.Length) - edgesToExchange.AddedEdges.Sum(x => x.Length);

            if (!UpdatePartialSum(improvement))
            {
                //Console.WriteLine("\tNo nonsequentail exchange improving path was found.");
                stopwatch.Stop();
                diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
                return false;
            }

            var reconnectEdgesStopwatch = new Stopwatch();
            reconnectEdgesStopwatch.Start();
            currentPath.ReconnectEdges(edgesToExchange, improvement);
            reconnectEdgesStopwatch.Stop();
            diagnoser.ReconnectEdgesNonsequentialExchange.AddRecord(reconnectEdgesStopwatch.Elapsed);

            //Console.WriteLine($"\tCURRENT PATH: {currentPath}");
            //Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            UpdateShortestPath(currentPath);

            stopwatch.Stop();
            diagnoser.NonsequentialExchange.AddRecord(stopwatch.Elapsed);
            return true;
        }

        private bool IsNodeCheckedOut(Node node)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var paths = checkedOutPathsForNodes.Where(r => r.Node == node).ToList();

            if (paths.Count == 0)
                return false;

            foreach (var checkoutPath in paths)
            {
                if (path.Equals(checkoutPath))
                    return true;
            }

            stopwatch.Stop();
            diagnoser.GetFruitlessNodesForPath.AddRecord(stopwatch.Elapsed);
            return false;
        }

        private Node[] FindNode2(Node node)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int nodeIndex = path.IndexOf(node);
            var nodes2 = new Node[2] { path.PeekPrev(nodeIndex), path.PeekNext(nodeIndex)};

            if (node.Distance(nodes2[1]) > node.Distance(nodes2[0]))
                (nodes2[0], nodes2[1]) = (nodes2[1], nodes2[0]);

            stopwatch.Stop();
            diagnoser.FindNode2.AddRecord(stopwatch.Elapsed);

            return nodes2;
        }

        private void RestoreState(KernighanLinPath currentPath, int i)
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
            diagnoser.RestoreState.AddRecord(stopwatch.Elapsed);
        }

        private bool UpdatePartialSum(double brokenEdgeLegth, double addedEdgeLength) => UpdatePartialSum(brokenEdgeLegth - addedEdgeLength);

        private bool UpdatePartialSum(double improvement)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Console.WriteLine("[+] PARTIAL SUM UPDATE");
            //Console.WriteLine($"\tPARTIAL SUM: {partialSum}");
            //Console.WriteLine($"\tIMPROVEMENT: {improvement}");
            double currentPartialSum = partialSum + improvement;
            //Console.WriteLine($"\tCURRENT PARTIAL SUM: {currentPartialSum}");
            if (currentPartialSum > 0)
            {
                //Console.WriteLine("\tUpdating partial sum\n");
                partialSum = currentPartialSum;
                return true;
            }
            //Console.WriteLine("\tNot updating partial sum\n");

            stopwatch.Stop();
            diagnoser.UpdatePartialSum.AddRecord(stopwatch.Elapsed);
            return false;
        }

        private void UpdateLocalOptimum(KernighanLinPath currentPath)
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
                //Console.WriteLine($"\tAdding current path to list of locally optimal paths\n");
                locallyOptimalPaths[count] = currentPath;
                //UpdateShortestPath(currentPath);
                return;
            }
            if (index == -1)
                return;

            //Console.WriteLine("\tLOCALLY OPTIMAL PATHS");
            foreach (var path in locallyOptimalPaths)
            {
                if (path != null)
                {
                    //Console.WriteLine($"\t\tPath: {path}");
                    //Console.WriteLine($"\t\tLength: {path.Length}");
                }
            }

            if (currentPath.Length < highestOptimalPathLength)
            {
                //Console.WriteLine($"\tAdding current path to list of locally optimal paths");
                locallyOptimalPaths[index] = currentPath;
                UpdateGoodEdges(currentPath, count, index);
            }

            stopwatch.Stop();
            diagnoser.UpdateLocallyOptimalPaths.AddRecord(stopwatch.Elapsed);
        }

        private void UpdateGoodEdges(KernighanLinPath currentPath, int count, int index)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var getEdgesStopwatch = new Stopwatch();
            getEdgesStopwatch.Start();
            var pathEdges = currentPath.Edges;
            getEdgesStopwatch.Stop();
            diagnoser.GetEdges.AddRecord(getEdgesStopwatch.Elapsed);

            var newGoodEdges = pathEdges;
            //Console.WriteLine($"\tUpdating good edges");
            for (int j = 0; j < count; j++)
            {
                if (j == index)
                    continue;

                getEdgesStopwatch.Restart();
                var optimalPathEdges = locallyOptimalPaths[j].Edges;
                getEdgesStopwatch.Stop();
                diagnoser.GetEdges.AddRecord(getEdgesStopwatch.Elapsed);

                newGoodEdges = newGoodEdges.Intersect(optimalPathEdges).ToList();
            }
            goodEdges = newGoodEdges;
            //PrintEdges("\tGOOD EDGES", goodEdges);
            
            stopwatch.Stop();
            diagnoser.UpdateGoodEdges.AddRecord(stopwatch.Elapsed);
        }

        private void UpdateShortestPath(KernighanLinPath currentPath)
        {
            double currentImprovement = path.Length - currentPath.Length;
            if (currentImprovement > improvement)
            {
                //Console.WriteLine($"\tCurrent path is local optimum\n");
                improvement = currentImprovement;
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Console.WriteLine("[+] UPDATE SHORTEST PATH");
            //Console.WriteLine($"\tSHORTEST PATH: {shortestPath}");
            //Console.WriteLine($"\tSHORTEST PATH LENGTH: {shortestPath.Length}");
            if (currentPath.Length < shortestPath.Length)
            {
                //Console.WriteLine("\t*Updating shortest path*");
                shortestPath = currentPath;
                //Console.WriteLine($"\tNEW SHORTEST PATH: {shortestPath}");
                //Console.WriteLine($"\tNEW SHORTEST PATH LENGTH: {shortestPath.Length}");
            }
            //Console.WriteLine();

            stopwatch.Stop();
            diagnoser.UpdateShortestPath.AddRecord(stopwatch.Elapsed);
        }
        
        private void PrintState()
        {
            Console.WriteLine("######################### CURRENT STATE #########################");
            Console.WriteLine($"PATH: {path}");
            Console.WriteLine($"PATH LENGTH: {path.Length}");

            Console.WriteLine($"IMPROVEMENT: {improvement}");
            PrintEdges("BROKEN EDGES", brokenEdges);
            PrintEdges("ADDED EDGES", addedEdges);

            Console.WriteLine("FRUITLESS NODES");
            //foreach (var pair in fruitlessNodesForPaths)
            //{
            //    Console.WriteLine($"Path: {pair.Path}");

            //    Console.WriteLine("\nFruitless nodes: ");
            //    foreach (var node in pair.Nodes)
            //        Console.Write($"{node} ");
            //}

            Console.WriteLine("LOCALLY OPTIMAL PATHS");
            foreach (var path in locallyOptimalPaths)
            {
                if (path != null)
                {
                    Console.WriteLine($"Path: {path}");
                    Console.WriteLine($"Length: {path.Length}");
                }
            }
            PrintEdges("GOOD EDGES", goodEdges);
            Console.WriteLine($"SHORTEST PATH: {shortestPath}");
            Console.WriteLine($"PATH LENGTH: {shortestPath.Length}");
        }

        private void PrintEdgeState()
        {
            PrintEdges("\tBROKEN EDGES", brokenEdges);
            PrintEdges("\tADDED EDGES", addedEdges);
        }

        private void PrintEdges(string name, List<Edge> edges)
        {
            Console.Write($"{name}: ");
            foreach (var edge in edges)
                Console.Write($"{edge} ");
            Console.WriteLine();
        }

        private void PrintImprovementState(Node node1, Node node2, int i)
        {
            Console.WriteLine("[*] CURRENT STATE");
            Console.WriteLine($"\tNODE{i}: {node1}");
            Console.WriteLine($"\tNODE{i+1}: {node2}");
            PrintEdgeState();
            Console.WriteLine($"\tSTARTING NODE: {startingNode}");
            Console.WriteLine($"\tENCLOSING NODE: {enclosingNode}");
        }
    }
}
