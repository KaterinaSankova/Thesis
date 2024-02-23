using TravellingSalesmanProblem.GraphStructures;
using TravellingSalesmanProblem.Interfaces;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KernighanLin : ITspAlgorithm<KernighanLinPath>
    {
        private Graph graph = new();
        private KernighanLinPath path = new();

        private List<Edge> brokenEdges = new();
        private List<Edge> addedEdges = new();

        private double improvement = 0;
        private double partialSum = 0;

        private readonly Random rand = new();

        //Avoiding checkout time
        private List<(Node Node, List<KernighanLinPath> Paths)> checkedOutPathsForNodes = new();

        //Reduction
        private KernighanLinPath[] locallyOptimalPaths = new KernighanLinPath[5];
        private List<Edge> goodEdges = new();

        private KernighanLinPath shortestPath = new();

        private Node? startingNode;
        private Node? enclosingNode;
        
        public KernighanLinPath FindShortestPath(Graph graph)
        {
            if (graph.nodes.Count < 4)
                return new KernighanLinPath(graph.nodes.ToList().Append(graph.nodes.First()).ToList());

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
            startingNode = null;
            enclosingNode = null;

            checkedOutPathsForNodes = new();

            locallyOptimalPaths = new KernighanLinPath[5];
            goodEdges = new();

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
                if (IsNodeCheckedOut(node1))
                    continue;

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

                    List<(Node Node3, Node Node4, Node AlternativeNode4, double Value)> nextPairValues = new();
                    Node? investigatedNode3 = null, investigatedNode4;
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

                    foreach (var (Node3, Node4, AlternativeNode4, Value) in orderedNextPairs)
                    {
                        i = 1;
                        currentPath = path.ToPath();
                        RestoreState(i);
                        enclosingNode = node2;

                        Edge addedEdge1 = new(node2, Node3);

                        if (!UpdatePartialSum(brokenEdge1.Length, addedEdge1.Length))
                            break;
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);

                        bool pathImproved = ImprovePathFromBrokenEdge2(Node3, Node4, currentPath);
                        if (pathImproved)
                        {
                            UpdateLocalOptimum(shortestPath);
                            AddCheckOutNodes(node1, path);
                            //NonsequentialExchange(path);
                            return true;
                        }
                        if (AlternativeNode4 != node1)
                            pathImproved = ImprovePathFromAlternativeBrokenEdge2(node2, Node3, AlternativeNode4, currentPath);
                        if (pathImproved)
                        {
                            UpdateLocalOptimum(shortestPath);
                            AddCheckOutNodes(node1, path);
                            //NonsequentialExchange(path);
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
            latestPath = latestPath.ToPath();
            Edge brokenEdge2 = new(node3, node4);
            int i = 2;

            latestPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);

            UpdateShortestPath(latestPath);
            enclosingNode = node4;

            RestoreState(i);

            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);

            var stoppingNode = latestPath.PeekBefore(2);

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
                    pairImprovement = investigatedNode5.Distance(investigatedNode6) - enclosingNode.Distance(investigatedNode5);
                    if (pairImprovement > 0)
                    {
                        pairValues.Add((investigatedNode5, investigatedNode6, pairImprovement));
                    }
                }                    
                investigatedNode5 = latestPath.Next();
            }
            var orderedPairValues = pairValues.OrderByDescending(v => v.Value).ToList();

            foreach (var pair in orderedPairValues)
            {
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
                UpdateLocalOptimum(currentPath);

                enclosingNode = pair.Node6;

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
            int i = 2;
            RestoreState(i);

            AlterBrokenEdge2(node2, node3, node4, latestPath, i);

            if (improvement > 0)
            {
                return true;
            }

            return false;
        }

        private bool AlterBrokenEdge2(Node enclosingNode, Node pair1Node1, Node pair1Node2, KernighanLinPath latestPath, int i)
        {
            bool pathImproved = AlterBrokenEdge2Option1(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

            if (!pathImproved)
            {
                pathImproved = AlterBrokenEdge2Option2(enclosingNode, pair1Node1, pair1Node2, latestPath, i);

                return pathImproved;
            }
            return true;
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
                UpdateLocalOptimum(currentPath);

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

            if (nextExchangePair.Pair2Node1 != null)
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
                    pair3BrokenEdge = new(pair3Node1, pair3Node2);
                    alternativepair3Node2 = latestPath.PeekNext(pair3Node1);
                    alternativePair3BrokenEdge = new(pair3Node1, pair3Node2);

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
                    UpdateLocalOptimum(currentPath);

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
            latestPath.SetStartingPoint(enclosingNode, startingNode, Direction.Backwards);
            
            (Node? Node, Node? NextNode, double Value) nextPair = (null, null, double.MaxValue);
            List<(Node Node, Node NextNode, double Value)> nodesValues = new();
            var stoppingNode = latestPath.PeekBefore(2);
            Node investigatedNodes = latestPath.Next();
            Node? investigatedNextNode = null;
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

            Edge addedEdge = new(fromNode, nextPair.Node);
            Edge nextBrokenEdge = new(nextPair.Node, nextPair.NextNode);

            if (!UpdatePartialSum(lastBrokenEdge.Length, addedEdge.Length))
                return;

            brokenEdges.Add(lastBrokenEdge);
            addedEdges.Add(addedEdge);

            currentPath.ReconnectEdges(startingNode, enclosingNode, nextPair.Node, nextPair.NextNode);
            
            UpdateShortestPath(currentPath);
            UpdateLocalOptimum(currentPath);

            enclosingNode = nextPair.NextNode;

            ImprovePathFromNode(nextPair.NextNode, nextBrokenEdge, currentPath, i+1);
        }

        private bool NonsequentialExchange(KernighanLinPath latestPath)
        {
            KernighanLinPath currentPath = latestPath.ToPath();
            if (currentPath.Count < 8)
                return false;

            currentPath.SetDirection(currentPath[0], currentPath[1]);
            currentPath.CurrentIndex = 0;
            
            List<(Edge[] BrokenEdges, Edge[] AddedEdges, double Improvement)> exchangableEdges = new();
            double brokenEdgesLengthSum, addedEdgesLengthSum, improvement;
            for (int j = 0; j < 2; j++)
            {
                Edge brokenEdge1 = new(currentPath[j], currentPath.PeekNext(j));
                if (addedEdges.Contains(brokenEdge1))
                    continue;
                if (goodEdges.Contains(brokenEdge1))
                    continue;

                for (int k = j + 2; k < currentPath.Count - 4; k++)
                {
                    Edge brokenEdge4 = new(currentPath[k], currentPath.PeekNext(k));
                    if (addedEdges.Contains(brokenEdge4))
                        continue;
                    if (goodEdges.Contains(brokenEdge4))
                        continue;

                    for (int l = k + 2; l < currentPath.Count - 2; l++)
                    {
                        Edge brokenEdge2 = new(currentPath[l], currentPath.PeekNext(l));
                        if (addedEdges.Contains(brokenEdge2))
                            continue;
                        if (goodEdges.Contains(brokenEdge2))
                            continue;

                        for (int m = l + 2; m < currentPath.Count; m++)
                        {
                            if (currentPath.PeekNext(m) == currentPath[j])
                                continue;

                            Edge brokenEdge3 = new(currentPath[m], currentPath.PeekNext(m));
                            if (addedEdges.Contains(brokenEdge3))
                                continue;
                            if (goodEdges.Contains(brokenEdge3))
                                continue;

                            Edge addedEdge1 = new(brokenEdge1.node1, brokenEdge2.node2);
                            if (brokenEdges.Contains(addedEdge1))
                                continue;
                            Edge addedEdge2 = new(brokenEdge2.node1, brokenEdge1.node2);
                            if (brokenEdges.Contains(addedEdge2))
                                continue;
                            Edge addedEdge3 = new(brokenEdge3.node1, brokenEdge4.node2);
                            if (brokenEdges.Contains(addedEdge3))
                                continue;
                            Edge addedEdge4 = new(brokenEdge3.node1, brokenEdge3.node2);
                            if (brokenEdges.Contains(addedEdge4))
                                continue;

                            brokenEdgesLengthSum = brokenEdge1.Length + brokenEdge2.Length + brokenEdge3.Length + brokenEdge4.Length;
                            addedEdgesLengthSum = addedEdge1.Length + addedEdge2.Length + addedEdge3.Length + addedEdge4.Length;

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
                return false;
            }
            
            var edgesToExchange = orderedExchangeableEdges.First();

            currentPath.ReconnectEdges(edgesToExchange);

            UpdateShortestPath(currentPath);

            return true;
        }

        private bool IsNodeCheckedOut(Node node)
        {
            var paths = checkedOutPathsForNodes.Where(r => r.Node == node).ToList();

            if (paths.Count == 0)
                return false;

            foreach (var checkoutPath in paths)
            {
                if (path.Equals(checkoutPath))
                    return true;
            }

            return false;
        }

        private Node[] FindNode2(Node node)
        {
            int nodeIndex = path.IndexOf(node);
            var nodes2 = new Node[2] { path.PeekPrev(nodeIndex), path.PeekNext(nodeIndex)};

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

        private void UpdateLocalOptimum(KernighanLinPath currentPath)
        {
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
                locallyOptimalPaths[count] = currentPath;
                return;
            }
            if (index == -1)
                return;

            if (currentPath.Length < highestOptimalPathLength)
            {
                locallyOptimalPaths[index] = currentPath;
                UpdateGoodEdges(currentPath, count, index);
            }
        }

        private void UpdateGoodEdges(KernighanLinPath currentPath, int count, int index)
        {
            var pathEdges = currentPath.Edges;

            var newGoodEdges = pathEdges;
            for (int j = 0; j < count; j++)
            {
                if (j == index)
                    continue;

                var optimalPathEdges = locallyOptimalPaths[j].Edges;

                newGoodEdges = newGoodEdges.Intersect(optimalPathEdges).ToList();
            }
            goodEdges = newGoodEdges;
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