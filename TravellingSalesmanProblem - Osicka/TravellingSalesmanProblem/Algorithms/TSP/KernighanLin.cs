using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KernighanLin : ITspAlgorithm
    {
        private readonly bool reducedBackTracking = false;

        private Graph graph = new();
        private KernighanLinPath path = new();

        private List<Edge> brokenEdges = new();
        private List<Edge> addedEdges = new();

        private double improvement = 0;
        private double partialSum = 0;

        private readonly Random rand = new();

        private KernighanLinPath shortestPath = new();

        private Node startingNode;
        private Node enclosingNode;

        public KernighanLin(bool reducedBackTracking = false)
        {
            this.reducedBackTracking = reducedBackTracking;
        }

        public Path FindShortestPath(Graph graph)
        {
            if (graph.nodes.Count < 4)
            {
                var pathNodes = graph.nodes.ToList();
                return new KernighanLinPath(pathNodes);
            }

            this.graph = graph;
            GeneratePath();
            SetupInitialState();

            bool continueImproving = FindShortestPath(path);
            while (continueImproving)
                continueImproving = FindShortestPath(shortestPath);

            return shortestPath;
        }

        private bool FindShortestPath(KernighanLinPath inputPath)
        {
            path = inputPath;
            SetupEdges();
            improvement = 0;
            partialSum = 0;

            return ImprovePath();
        }

        private void GeneratePath() => path = new KernighanLinPath(graph.nodes.OrderBy(_ => rand.Next()).ToList());

        private void SetupInitialState()
        {
            improvement = 0;
            partialSum = 0;
            SetupEdges();

            shortestPath = path.ToPath();
        }

        private void SetupEdges()
        {
            brokenEdges = new();
            addedEdges = new();
        }

        private bool ImprovePath()
        {
            int i = 0;
            KernighanLinPath currentPath;

            foreach (var node1 in graph.nodes.OrderBy(x => rand.Next()).ToList())
            {
                startingNode = node1;
                currentPath = path.ToPath();
                RestoreState(i);

                foreach (var node2 in FindNode2(node1))
                {
                    i = 1;
                    currentPath = path.ToPath();
                    RestoreState(i);

                    Edge brokenEdge1 = new(node1, node2);

                    enclosingNode = node2;
                    currentPath.SetStartingPoint(node2, node1, Direction.Backwards);

                    List<(Node Node3, Node Node4, Node AlternativeNode4, double Value)> orderedNextPairs = new();
                    if (!reducedBackTracking)
                    {
                        List<(Node Node3, Node Node4, Node AlternativeNode4, double Value)> nextPairValues = new();
                        Node? investigatedNode3 = null, investigatedNode4;
                        var stoppingNode = currentPath.PeekBefore(2);
                        currentPath.Next();
                        while (investigatedNode3 != stoppingNode)
                        {
                            investigatedNode3 = currentPath.Next();
                            investigatedNode4 = currentPath.PeekPrev();
                            nextPairValues.Add(
                                (investigatedNode3, investigatedNode4, currentPath.PeekNext(), brokenEdge1.Length - node2.Distance(investigatedNode3) + investigatedNode3.Distance(investigatedNode4))
                                );
                        }
                        orderedNextPairs = nextPairValues.OrderByDescending(v => v.Value).ToList();
                    }
                    else
                    {
                        var nextPairValues = new (Node Node3, Node Node4, Node AlternativeNode4, double Value)[2];
                        int index = 0;
                        double pairImprovement;
                        Node? investigatedNode3 = null, investigatedNode4;
                        var stoppingNode = currentPath.PeekBefore(2);
                        currentPath.Next();
                        while (investigatedNode3 != stoppingNode)
                        {
                            investigatedNode3 = currentPath.Next();
                            investigatedNode4 = currentPath.PeekPrev();
                            pairImprovement = brokenEdge1.Length - node2.Distance(investigatedNode3) + investigatedNode3.Distance(investigatedNode4);
                            if (index == 2)
                            {
                                if (nextPairValues[1].Value < pairImprovement)
                                {
                                    if (nextPairValues[0].Value < pairImprovement)
                                    {
                                        nextPairValues[0] = (investigatedNode3, investigatedNode4, currentPath.PeekNext(), pairImprovement);
                                    }
                                    else
                                    {
                                        nextPairValues[1] = (investigatedNode3, investigatedNode4, currentPath.PeekNext(), pairImprovement);
                                    }
                                }
                            }
                            else if (index == 1)
                            {
                                if (nextPairValues[0].Value < pairImprovement)
                                {
                                    nextPairValues[1] = nextPairValues[0];
                                    nextPairValues[0] = (investigatedNode3, investigatedNode4, currentPath.PeekNext(), pairImprovement);
                                }
                                else
                                {
                                    nextPairValues[1] = (investigatedNode3, investigatedNode4, currentPath.PeekNext(), pairImprovement);
                                }
                                index++;
                            }
                            else
                            {
                                nextPairValues[0] = (investigatedNode3, investigatedNode4, currentPath.PeekNext(), brokenEdge1.Length - node2.Distance(investigatedNode3) + investigatedNode3.Distance(investigatedNode4));
                                index++;
                            }
                        }
                        orderedNextPairs = nextPairValues.ToList();
                    }

                    foreach (var (Node3, Node4, AlternativeNode4, Value) in orderedNextPairs)
                    {
                        if (Node3 == null)
                            break;

                        i = 1;
                        currentPath = path.ToPath();
                        RestoreState(i);
                        enclosingNode = node2;

                        Edge addedEdge1 = new(node2, Node3);

                        if (!UpdatePartialSum(brokenEdge1.Length, addedEdge1.Length))
                            break;
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);

                        ImprovePathFromBrokenEdge2(Node3, Node4, currentPath);

                        if (improvement > 0)
                            return true;

                        if (AlternativeNode4 != node1)
                            ImprovePathFromAlternativeBrokenEdge2(node2, Node3, AlternativeNode4, currentPath);

                        if (improvement > 0)
                            return true;
                    }
                }
            }

            return false;
        }

        private void ImprovePathFromBrokenEdge2(Node node3, Node node4, KernighanLinPath latestPath)
        {
            latestPath = latestPath.ToPath();
            Edge brokenEdge2 = new(node3, node4);
            int i = 2;

            latestPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);

            UpdateShortestPath(latestPath);
            enclosingNode = node4;

            if (partialSum < improvement)
                return;

            RestoreState(i);

            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);

            var stoppingNode = latestPath.PeekBefore(2);

            List<(Node Node5, Node Node6, double Value)> orderedPairValues = new();
            if (!reducedBackTracking)
            {
                List<(Node Node5, Node Node6, double Value)> pairValues = new();
                double pairImprovement;
                Node investigatedNode5 = latestPath.Next(2);
                Node? investigatedNode6 = null;
                while (investigatedNode5 != stoppingNode)
                {
                    if (brokenEdges.Contains(new(node4, investigatedNode5)))
                        continue;
                    investigatedNode6 = latestPath.PeekPrev();
                    if (!addedEdges.Contains(new(investigatedNode5, investigatedNode6)))
                    {
                        pairImprovement = brokenEdge2.Length - node4.Distance(investigatedNode5) + investigatedNode5.Distance(investigatedNode6);
                        if (pairImprovement > 0)
                        {
                            pairValues.Add((investigatedNode5, investigatedNode6, pairImprovement));
                        }
                    }
                    investigatedNode5 = latestPath.Next();
                }
                orderedPairValues = pairValues.OrderByDescending(v => v.Value).ToList();
            }
            else
            {
                var pairValues = new (Node Node5, Node Node6, double Value)[2];
                int index = 0;
                double pairImprovement;
                Node investigatedNode5 = latestPath.Next(2);
                Node? investigatedNode6 = null;
                while (investigatedNode5 != stoppingNode)
                { 
                    if (brokenEdges.Contains(new(node4, investigatedNode5)))
                        continue;
                    investigatedNode6 = latestPath.PeekPrev();
                    if (!addedEdges.Contains(new(investigatedNode5, investigatedNode6)))
                    {
                        pairImprovement = brokenEdge2.Length - node4.Distance(investigatedNode5) + investigatedNode5.Distance(investigatedNode6);
                        if (pairImprovement > 0)
                        {
                            if (index == 2)
                            {
                                if (pairValues[1].Value < pairImprovement)
                                {
                                    if (pairValues[0].Value < pairImprovement)
                                    {
                                        pairValues[0] = (investigatedNode5, investigatedNode6, pairImprovement);
                                    }
                                    else
                                    {
                                        pairValues[1] = (investigatedNode5, investigatedNode6, pairImprovement);
                                    }
                                }
                            }
                            else if (index == 1)
                            {
                                if (pairValues[0].Value < pairImprovement)
                                {
                                    pairValues[1] = pairValues[0];
                                    pairValues[0] = (investigatedNode5, investigatedNode6, pairImprovement);
                                }
                                else
                                {
                                    pairValues[1] = (investigatedNode5, investigatedNode6, pairImprovement);
                                }
                                index++;
                            }
                            else
                            {
                                pairValues[0] = (investigatedNode5, investigatedNode6, pairImprovement);
                                index++;
                            }
                        }
                    }
                    investigatedNode5 = latestPath.Next();
                }

                orderedPairValues = pairValues.ToList();
            }

            foreach (var pair in orderedPairValues)
            {
                if (pair == (null, null, 0))
                    break;

                i = 2;
                RestoreState(i);

                var currentPath = latestPath.ToPath();
                enclosingNode = node4;

                Edge addedEdge2 = new(node4, pair.Node5);
                Edge brokenEdge3 = new(pair.Node5, pair.Node6);

                if (!UpdatePartialSum(brokenEdge2.Length, addedEdge2.Length))
                    break;

                brokenEdges.Insert(i - 1, brokenEdge2);
                addedEdges.Insert(i - 1, addedEdge2);

                currentPath.ReconnectEdges(startingNode, enclosingNode, pair.Node5, pair.Node6);

                UpdateShortestPath(currentPath);

                enclosingNode = pair.Node6;

                ImprovePathFromNode(pair.Node6, brokenEdge3, currentPath, i + 1);

                if (improvement > 0)
                    return;
            }
        }

        private void ImprovePathFromAlternativeBrokenEdge2(Node node2, Node node3, Node node4, KernighanLinPath latestPath)
        {
            int i = 2;
            RestoreState(i);

            bool pathImproved = AlterBrokenEdge2Option1(node2, node3, node4, latestPath, i);

            if (!pathImproved)
            {
                AlterBrokenEdge2Option2(node2, node3, node4, latestPath, i);
            }
        }

        private bool AlterBrokenEdge2Option1(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            Edge pair1BrokenEdge = new(pair1Node1, pair1Node2);

            latestPath.SetStartingPoint(pair1Node1, pair1Node2, Direction.Backwards);

            List<(Node Node1, Node Node2, double Value)> pair2Node1Values = new();
            Node investigatedPair2Node1 = latestPath.Next(2);
            Node? investigatedPair2Node2 = null;
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
                pair1AddedEdge = new(pair1Node2, pair2Node1);

                pair2Node2 = orderedPair2Node1s[index].Node2;
                pair2BrokenEdge = new(pair2Node1, pair2Node2);

                if (addedEdges.Contains(pair2BrokenEdge))
                    continue;

                nextExchangePairs.Add((pair2Node1, pair2Node2, pair2BrokenEdge.Length - pair1AddedEdge.Length));
                j++;
            }

            nextExchangePairs = nextExchangePairs.OrderByDescending(p => p.Gain).ToList();


            if (nextExchangePairs.Count == 0)
            {
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
                    pair1AddedEdge = new(pair1Node2, pair2Node1);

                    pair2Node2 = orderedPair2Node1s[index].Node2;
                    pair2BrokenEdge = new(pair2Node1, pair2Node2);

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
                pair1AddedEdge = new(pair1Node2, pair2Node1);
                pair2BrokenEdge = new(pair2Node1, pair2Node2);

                if (!UpdatePartialSum(pair1BrokenEdge.Length, pair1AddedEdge.Length))
                    return false;

                brokenEdges.Insert(i - 1, pair1BrokenEdge);
                addedEdges.Insert(i - 1, pair1AddedEdge);

                currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2);

                UpdateShortestPath(currentPath);

                if (partialSum < improvement)
                    return false;

                this.enclosingNode = pair2Node2;

                ImprovePathFromNode(pair2Node2, pair2BrokenEdge, currentPath, i + 1);
            }

            return true;
        }

        private bool AlterBrokenEdge2Option2(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            Edge pair1BrokenEdge = new(pair1Node1, pair1Node2);

            latestPath.SetStartingPoint(pair1Node2, pair1Node1, Direction.Backwards);

            List<(Node Pair2Node1, Node Pair2Node2, double Value)> pair2Values = new();
            Node investigatedPair2Node1 = latestPath.Next();
            Node? investigatedPair2Node2 = null;
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

            (Node? Pair2Node1, Node? Pair2Node2, double Gain) nextExchangePair = (null, null, double.MinValue);
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

            if (nextExchangePair.Pair2Node1 != null && nextExchangePair.Pair2Node2 != null)
            {
                i++;
                var currentPath = latestPath.ToPath();
                pair2Node1 = nextExchangePair.Pair2Node1;
                pair2Node2 = nextExchangePair.Pair2Node2;
                pair1AddedEdge = new(pair1Node2, pair2Node1);
                pair2BrokenEdge = new(pair2Node1, pair2Node2);

                if (!UpdatePartialSum(pair1BrokenEdge.Length, pair1AddedEdge.Length))
                    return false;
                latestPath.SetStartingPoint(pair1Node1, pair1Node2, Direction.Backwards);

                List<(Node Node, double Value)> pair3Node1Values = new();
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
                (Node? Pair3Node1, Node? Pair3Node2, double Gain) nextNextExchangePair = (null, null, double.MinValue);
                List<(Node Pair3Node1, Node Pair3Node2, double Gain)> nextNextExchangePairs = new();
                for (int j = 0; j < 5;)
                {
                    index++;
                    if (index == orderedPair3Node1s.Count)
                        break;

                    pair3Node1 = orderedPair3Node1s[index];
                    pair3Node2 = latestPath.PeekPrev(pair3Node1);

                    pair2AddedEdge = new(pair2Node2, pair3Node1);
                    pair3BrokenEdge = new(pair3Node1, pair3Node2);

                    alternativepair3Node2 = latestPath.PeekNext(pair3Node1);
                    alternativePair3BrokenEdge = new(pair3Node1, alternativepair3Node2);

                    if (pair3Node2 == pair1Node1 || addedEdges.Contains(pair3BrokenEdge))
                    {
                        if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                            continue;

                        gain = alternativePair3BrokenEdge.Length - pair2AddedEdge.Length;
                        if (nextNextExchangePair.Gain < gain)
                            nextNextExchangePair = (pair3Node1, alternativepair3Node2, gain);
                    }
                    else if (alternativepair3Node2 == enclosingNode || addedEdges.Contains(alternativePair3BrokenEdge))
                    {
                        gain = pair3BrokenEdge.Length - pair2AddedEdge.Length;
                        if (nextNextExchangePair.Gain < gain)
                            nextNextExchangePair = (pair3Node1, pair3Node2, gain);
                    }
                    else
                    {
                        if (pair3BrokenEdge.Length <= alternativePair3BrokenEdge.Length)
                        {
                            gain = pair3BrokenEdge.Length - pair2AddedEdge.Length;
                            if (nextNextExchangePair.Gain < gain)
                                nextNextExchangePair = (pair3Node1, pair3Node2, gain);
                        }
                        else
                        {
                            gain = alternativePair3BrokenEdge.Length - pair2AddedEdge.Length;
                            if (nextNextExchangePair.Gain < gain)
                                nextNextExchangePair = (pair3Node1, alternativepair3Node2, gain);
                        }
                    }

                    j++;
                }

                if (nextNextExchangePair.Pair3Node1 != null && nextNextExchangePair.Pair3Node2 != null)
                {
                    currentPath = latestPath.ToPath();
                    pair3Node1 = nextNextExchangePair.Pair3Node1;
                    pair3Node2 = nextNextExchangePair.Pair3Node2;
                    pair2AddedEdge = new(pair2Node2, pair3Node1);
                    pair3BrokenEdge = new(pair3Node1, pair3Node2);

                    if (!UpdatePartialSum(pair2BrokenEdge.Length, pair2AddedEdge.Length))
                        return false;

                    brokenEdges.Insert(i - 2, pair1BrokenEdge);
                    addedEdges.Insert(i - 2, pair1AddedEdge);

                    brokenEdges.Insert(i - 1, pair2BrokenEdge);
                    addedEdges.Insert(i - 1, pair2AddedEdge);

                    currentPath.ReconnectEdges(startingNode, enclosingNode, pair1Node1, pair1Node2, pair2Node1, pair2Node2, pair3Node1, pair3Node2);

                    UpdateShortestPath(currentPath);

                    if (partialSum < improvement)
                        return false;

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
            (bool KeepImproving, Node? FromNode, Edge? LastBrokenEdge, KernighanLinPath? Path) result = (true, fromNode, lastBrokenEdge, latestPath);

            while (result.KeepImproving)
            {
                result = ImprovePathFromNodeIter(result.FromNode, result.LastBrokenEdge, result.Path, i);
                i++;
            }
        }

        private (bool KeepImproving, Node? FromNode, Edge? LastBrokenEdge, KernighanLinPath? Path) ImprovePathFromNodeIter(Node fromNode, Edge lastBrokenEdge, KernighanLinPath latestPath, int i)
        {
            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);

            List<(Node Node, Node NextNode, double LastPairImprovement)> nextPairs = new();
            var stoppingNode = latestPath.PeekBefore(2);
            Node investigatedNode = latestPath.Next();
            Node? investigatedNextNode = null;
            double nextPairImprovement;
            while (investigatedNode != stoppingNode)
            {
                investigatedNode = latestPath.Next();
                if (brokenEdges.Contains(new(fromNode, investigatedNode)))
                    continue;

                investigatedNextNode = latestPath.PeekPrev();
                if (!addedEdges.Contains(new(investigatedNode, investigatedNextNode)))
                {
                    nextPairImprovement = lastBrokenEdge.Length - fromNode.Distance(investigatedNode);
                    if (nextPairImprovement > partialSum)
                    {
                        nextPairs.Add((investigatedNode, investigatedNextNode, nextPairImprovement));
                    }
                }
            }

            if (nextPairs.Count == 0)
                return (false, null, null, null);

            var nextPair = nextPairs.MaxBy(p => (p.Node.Distance(p.NextNode) - fromNode.Distance(p.Node)) + p.LastPairImprovement);

            var currentPath = latestPath.ToPath();
            enclosingNode = fromNode;

            Edge addedEdge = new(fromNode, nextPair.Node);
            Edge nextBrokenEdge = new(nextPair.Node, nextPair.NextNode);

            if (!UpdatePartialSum(lastBrokenEdge.Length, addedEdge.Length))
                return (false, null, null, null);

            brokenEdges.Add(lastBrokenEdge);
            addedEdges.Add(addedEdge);

            currentPath.ReconnectEdges(startingNode, enclosingNode, nextPair.Node, nextPair.NextNode);

            UpdateShortestPath(currentPath);

            enclosingNode = nextPair.NextNode;

            if (partialSum < improvement)
                return (false, null, null, null);

            return (true, nextPair.NextNode, nextBrokenEdge, currentPath);
        }

        private Node[] FindNode2(Node node)
        {
            int nodeIndex = path.IndexOf(node);
            var nodes2 = new Node[2] { path.PeekPrev(nodeIndex), path.PeekNext(nodeIndex) };

            if (node.Distance(nodes2[1]) > node.Distance(nodes2[0]))
                (nodes2[0], nodes2[1]) = (nodes2[1], nodes2[0]);

            return nodes2;
        }

        private void RestoreState(int i)
        {
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
        }

        private bool UpdatePartialSum(double brokenEdgeLegth, double addedEdgeLength) => UpdatePartialSum(brokenEdgeLegth - addedEdgeLength);

        private bool UpdatePartialSum(double improvement)
        {
            double currentPartialSum = partialSum + improvement;
            if (currentPartialSum > 0)
            {
                partialSum = currentPartialSum;
                return true;
            }
            return false;
        }

        private void UpdateShortestPath(KernighanLinPath currentPath)
        {
            double currentImprovement = path.Length - currentPath.Length;
            if (currentImprovement > improvement)
            {
                improvement = currentImprovement;
            }

            if (currentPath.Length < shortestPath.Length)
            {
                shortestPath = currentPath;
            }
        }
    }
}