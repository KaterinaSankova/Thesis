using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KLN2
    {
        private Graph graph;

        private List<Node> path = new List<Node>();

        private List<Edge> availableEdges = new List<Edge>();
        private List<Edge> brokenEdges = new List<Edge>();
        private List<Edge> addedEdges = new List<Edge>();

        private Node startingNode;

        private Node enclosingNode;

        private List<Edge> nextEdgeToBreak = new List<Edge>();

        private double currentBestPathImprovement = 0;

        private double globalBestPathImprovement = 0;

        int k = 0;

        List<Node> bestPathFound = new List<Node>();

        Random rand = new Random();

        public List<Node> FindShortestPath(Graph graph, List<Node>? path = null)
        {
            availableEdges.Clear();
            brokenEdges.Clear();
            addedEdges.Clear();

            //@TODO pro 0-1 prvků
            this.graph = graph;
            if(path == null)
                this.path = GenerateRandomStartingTour();
            else
                this.path = path;

            Console.WriteLine();
            foreach (var node in this.path)
            {
                Console.WriteLine(node);
            }
            Console.WriteLine();

            SetAvailableEdgesFromPath();

            var bestPath = FindEdgesToExchange();

            Console.WriteLine();
            foreach (var node in this.bestPathFound)
            {
                Console.WriteLine(node);
            }
            Console.WriteLine();

            return new List<Node>();
        }

        private List<Node> GenerateRandomStartingTour() => graph.nodes.OrderBy(_ => rand.Next()).ToList();

        private void SetAvailableEdgesFromPath()
        {
            for (int j = 0; j < path.Count - 1; j++)
                availableEdges.Add(new Edge(path[j], path[j + 1]));
            availableEdges.Add(new Edge(path[path.Count - 1], path[0]));
        }

        private List<Node> FindEdgesToExchange()
        {
            int i;
            k = 0;
            globalBestPathImprovement = 0;
            var ranomizedNodes = path.OrderBy(_ => rand.Next()).ToList();
            //Node
            foreach (var firstNode in ranomizedNodes)
            {
                //Console.WriteLine($"\n\n******************NODE{firstNode}***********************");
                startingNode = firstNode;
                var possibleFirstEdges = availableEdges.Where(e => e.Contains(firstNode)).ToList();
                //x1
                foreach (var firstEdgeToBeBroken in possibleFirstEdges)
                {
                    i = 1;
                    //Console.WriteLine($"\n\n******************X1{firstEdgeToBeBroken}***********************");
                    RestoreState(i);
                    Node secondNode = firstEdgeToBeBroken.GetOtherNode(firstNode);

                    List<Node> nodesThatCanNotBeAThirdNode = new List<Node>();
                    foreach (var edge in availableEdges)
                    {
                        if (edge.node1 == secondNode)
                            nodesThatCanNotBeAThirdNode.Add(edge.node2);
                        if (edge.node2 == secondNode)
                            nodesThatCanNotBeAThirdNode.Add(edge.node1);
                    }
                    nodesThatCanNotBeAThirdNode.Add(secondNode);

                    var possibleThirdNodes = graph.nodes.OrderBy(node => node.Distance(secondNode)).ToList();
                    //y1
                    foreach (Node thirdNode in possibleThirdNodes) //we want y to be as small as possible
                    {
                        i = 1;
                        currentBestPathImprovement = 0;
                        globalBestPathImprovement = 0;
                        RestoreState(i);
                        if (nodesThatCanNotBeAThirdNode.Contains(thirdNode))
                            continue;

                        Edge firstEdgeToBeAdded = new Edge(secondNode, thirdNode);
                        if (!CheckGainCriterion(i, firstEdgeToBeBroken.Length(), firstEdgeToBeAdded.Length())) //partial sum isnt positive
                            break;

                        //x2
                        int numberOfPossibleSecondEdgesToBeBreak = 0;
                        List<Edge> possibleSecondEdgestToBreak = new List<Edge>();
                        foreach (var edge in availableEdges.Except(new List<Edge>() { firstEdgeToBeBroken }).ToList())
                        {
                            if (edge.Contains(thirdNode))
                            {
                                possibleSecondEdgestToBreak.Add(edge);
                                numberOfPossibleSecondEdgesToBeBreak++;
                            }
                            if (numberOfPossibleSecondEdgesToBeBreak == 2) //more that 2 edges can not be found - we are working with paths
                                break;
                        }
                        if (numberOfPossibleSecondEdgesToBeBreak == 0) continue; ///does not permit breaking xi+1
                        //Console.WriteLine($"\n\n******************Y1{firstEdgeToBeAdded}***********************");

                        brokenEdges.Insert(i - 1, firstEdgeToBeBroken);
                        addedEdges.Insert(i - 1, firstEdgeToBeAdded);
                        UpdateCurrentBestPathImprovement(firstNode, secondNode, firstEdgeToBeBroken, i);
                        UpdateGlobaBestPathImprovement(i, secondNode);
                        availableEdges.Remove(firstEdgeToBeBroken);
                        i = 2;
                        RestoreState(i);

                        Edge? secondEdgeToBeBroken = null;
                        Edge? alternativeSecondEdgeToBeBroken;

                        if (CheckIfTourIsClosable(possibleSecondEdgestToBreak.First(), possibleSecondEdgestToBreak.First().GetOtherNode(thirdNode)))
                        {
                            secondEdgeToBeBroken = possibleSecondEdgestToBreak.First();
                            alternativeSecondEdgeToBeBroken = possibleSecondEdgestToBreak.Last();
                        }
                        else if(CheckIfTourIsClosable(possibleSecondEdgestToBreak.Last(), possibleSecondEdgestToBreak.Last().GetOtherNode(thirdNode)))
                        {
                            secondEdgeToBeBroken = possibleSecondEdgestToBreak.Last();
                            alternativeSecondEdgeToBeBroken = possibleSecondEdgestToBreak.First();
                        }
                        
                        if(secondEdgeToBeBroken == null)
                        {
                            Console.WriteLine("Secind edge to be broken is not closable");
                            continue;
                        }
                        //Console.WriteLine($"\n\n******************X2{secondEdgeToBeBroken}***********************");

                        Node fourhNode = secondEdgeToBeBroken.GetOtherNode(thirdNode);

                        List<Node> nodesThatCanNotBeAFifthNode = new List<Node>();
                        foreach (var edge in availableEdges.Concat(brokenEdges).Concat(addedEdges).ToList())
                        {
                            if (edge.node1 == fourhNode)
                                nodesThatCanNotBeAFifthNode.Add(edge.node2);
                            if (edge.node2 == fourhNode)
                                nodesThatCanNotBeAFifthNode.Add(edge.node1);
                        }
                        nodesThatCanNotBeAFifthNode.Add(fourhNode);

                        var possibleFifthNodes = graph.nodes.OrderBy(node => node.Distance(fourhNode)).ToList();
                        //y2
                        foreach (var fifthNode in possibleFifthNodes) //we want y to be as small as possible
                        {
                            i = 2;
                            RestoreState(i);
                            if (nodesThatCanNotBeAFifthNode.Contains(fifthNode))
                                continue;

                            Edge secondEdgeToBeAdded = new Edge(fourhNode, fifthNode);

                            var gain = GetPartialSum(i, secondEdgeToBeBroken.Length(), secondEdgeToBeAdded.Length());
                            if (!CheckGainCriterion(i, secondEdgeToBeBroken.Length(), secondEdgeToBeAdded.Length())) //partial sum isnt positive
                                continue;

                            int numberOfPossibleThirdEdgesToBeBreak = 0;
                            List<Edge> possibleThirdEdgestToBreak = new List<Edge>();
                            foreach (var edge in availableEdges.Except(new List<Edge>() { secondEdgeToBeBroken }).ToList())
                            {
                                if (edge.Contains(fifthNode))
                                {
                                    possibleThirdEdgestToBreak.Add(edge);
                                    numberOfPossibleThirdEdgesToBeBreak++;
                                }
                                if (numberOfPossibleThirdEdgesToBeBreak == 2) //more that 2 edges can not be found - we are working with paths
                                    break;
                            }
                            Edge? nextEdgeToBeBroken = null;
                            foreach (var edge in possibleThirdEdgestToBreak)
                            {
                                if (CheckIfTourIsClosable(secondEdgeToBeBroken, secondEdgeToBeAdded, edge, edge.GetOtherNode(fifthNode))) nextEdgeToBeBroken = edge;
                            }
                            if (nextEdgeToBeBroken == null) continue; ///does not permit breaking xi+1
                            //Console.WriteLine($"\n\n******************Y2{secondEdgeToBeAdded}***********************");
                            Node nextOutgoingNodeFromNextEdgeToBeBroken = nextEdgeToBeBroken.GetOtherNode(fifthNode);

                            brokenEdges.Insert(i - 1, secondEdgeToBeBroken);
                            addedEdges.Insert(i - 1, secondEdgeToBeAdded);
                            availableEdges.Remove(secondEdgeToBeBroken);
                            UpdateCurrentBestPathImprovement(firstNode, fourhNode, secondEdgeToBeBroken, i);
                            UpdateGlobaBestPathImprovement(i, fourhNode);

                            //Console.WriteLine(k);
                            //Console.WriteLine(globalBestPathImprovement);
                            //Console.WriteLine(currentBestPathImprovement);
                            //Console.WriteLine($"\n\n\n1: {firstEdgeToBeBroken}\t\t{firstEdgeToBeAdded}\n2: {secondEdgeToBeBroken}\t\t{secondEdgeToBeAdded}");
                            Edge lastBrokenEdge = nextEdgeToBeBroken;
                            Node lastOutgoingNodeFromEdgeToBeBroken = nextOutgoingNodeFromNextEdgeToBeBroken;
                            //bool aaaaa = false;
                            while (GetPartialSum(i) > currentBestPathImprovement)
                            {
                                //if(i>3 && !aaaaa)
                                //{
                                //    Console.WriteLine("#################################################################################################################");
                                //    foreach (var node in path)
                                //    {
                                //        Console.WriteLine(node);
                                //    }
                                //    aaaaa = true;
                                //}
                                i++;
                                if(nextEdgeToBeBroken != null)
                                {
                                    Console.WriteLine("nextEdgeToBeBroken was null");
                                    lastBrokenEdge = nextEdgeToBeBroken;
                                }
                                lastOutgoingNodeFromEdgeToBeBroken = nextOutgoingNodeFromNextEdgeToBeBroken;



                                List<Node> nodesThatCanNotBeANextNode = new List<Node>();
                                foreach (var edge in availableEdges.Concat(brokenEdges).Concat(addedEdges).ToList())
                                {
                                    if (edge.node1 == nextOutgoingNodeFromNextEdgeToBeBroken)
                                        nodesThatCanNotBeANextNode.Add(edge.node2);
                                    if (edge.node2 == nextOutgoingNodeFromNextEdgeToBeBroken)
                                        nodesThatCanNotBeANextNode.Add(edge.node1);
                                }
                                nodesThatCanNotBeANextNode.Add(nextOutgoingNodeFromNextEdgeToBeBroken);

                                Edge? nextEdgeToBeAdded = null;
                                foreach (var nextOutgoingNodeFromNextEdgeToAdded in graph.nodes.OrderBy(node => node.Distance(nextOutgoingNodeFromNextEdgeToBeBroken)).ToList()) //we want y to be as small as possible
                                {
                                    if (nodesThatCanNotBeANextNode.Contains(nextOutgoingNodeFromNextEdgeToAdded))
                                        continue;

                                    nextEdgeToBeAdded = new Edge(nextOutgoingNodeFromNextEdgeToBeBroken, nextOutgoingNodeFromNextEdgeToAdded);
                                    gain = GetPartialSum(i, lastBrokenEdge.Length(), nextEdgeToBeAdded.Length());
                                    if (!CheckGainCriterion(i, lastBrokenEdge.Length(), nextEdgeToBeAdded.Length())) //partial sum isnt positive
                                        continue;

                                    int numberOfPossibleNextEdgesToBeBreak = 0;
                                    List<Edge> possibleNextEdgestToBreak = new List<Edge>();
                                    foreach (var edge in availableEdges.Except(new List<Edge>() { lastBrokenEdge }).ToList())
                                    {
                                        if (edge.Contains(nextOutgoingNodeFromNextEdgeToAdded))
                                        {
                                            possibleNextEdgestToBreak.Add(edge);
                                            numberOfPossibleNextEdgesToBeBreak++;
                                        }
                                        if (numberOfPossibleNextEdgesToBeBreak == 2) //more that 2 edges can not be found - we are working with paths
                                            break;
                                    }
                                    if (numberOfPossibleNextEdgesToBeBreak == 0) continue;
                                    nextEdgeToBeBroken = null;
                                    foreach (var edge in possibleNextEdgestToBreak)
                                    {
                                        if (CheckIfTourIsClosable(lastBrokenEdge, nextEdgeToBeAdded, edge, edge.GetOtherNode(nextOutgoingNodeFromNextEdgeToAdded))) nextEdgeToBeBroken = edge;
                                        break;
                                    }
                                    if (nextEdgeToBeBroken == null) continue; ///does not permit breaking xi+1
                                    nextOutgoingNodeFromNextEdgeToBeBroken = nextEdgeToBeBroken.GetOtherNode(nextOutgoingNodeFromNextEdgeToAdded);
                                    break;
                                }

                                if (nextEdgeToBeAdded == null) break;

                                brokenEdges.Insert(i - 1, lastBrokenEdge);
                                addedEdges.Insert(i - 1, nextEdgeToBeAdded);
                                availableEdges.Remove(lastBrokenEdge);
                                UpdateCurrentBestPathImprovement(firstNode, lastOutgoingNodeFromEdgeToBeBroken, lastBrokenEdge, i);
                                UpdateGlobaBestPathImprovement(i, lastOutgoingNodeFromEdgeToBeBroken);
                                //Console.WriteLine($"{i}: {lastBrokenEdge}\t\t{nextEdgeToBeAdded}\n");
                            }

                            if(currentBestPathImprovement > 0)
                            {

                                double pathLength = 0;
                                for (int j = 0; j < path.Count - 1; j++)
                                {
                                    pathLength += path[j].Distance(path[j + 1]);
                                }
                                pathLength += path[0].Distance(path[path.Count-1]);

                                double bestPathLength = 0;
                                for (int j = 0; j < bestPathFound.Count - 1; j++)
                                {
                                    bestPathLength += bestPathFound[j].Distance(bestPathFound[j + 1]);
                                }
                                bestPathLength += bestPathFound[0].Distance(bestPathFound[bestPathFound.Count - 1]);

                                double improvement = 0;
                                for (int j = 0; j < k - 1; j++)
                                {
                                    improvement += brokenEdges[j].Length() - addedEdges[j].Length();
                                }
                                improvement += brokenEdges[k - 1].Length() - startingNode.Distance(enclosingNode);

                                double brokenAddedDifference = 0;
                                for (int j = 0; j < k; j++)
                                {
                                    brokenAddedDifference += brokenEdges[j].Length() - addedEdges[j].Length();
                                }

                                Console.WriteLine($"pathLength: {pathLength}");
                                Console.WriteLine($"bestPathLength: {bestPathLength}");
                                Console.WriteLine($"improvement: {improvement}");
                                Console.WriteLine($"brokenAddedDifference: {brokenAddedDifference}");
                                Console.WriteLine($"currentBestPathImprovement: {currentBestPathImprovement}");
                                Console.WriteLine($"globalBestPathImprovement: {globalBestPathImprovement}");

                                return FindShortestPath(graph, bestPathFound);
                            }
                        }
                    }
                }
            }

            return bestPathFound;
        }

        private double GetPartialSum() => brokenEdges.Select(e => e.Length()).Sum() - addedEdges.Select(e => e.Length()).Sum();
        private double GetPartialSum(int i)
        {
            var brokenSum = brokenEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            var addedSum = addedEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            return brokenSum - addedSum;
        }

        private double GetPartialSum(int i, double xDistance, double yDistance) => GetPartialSum(i) + xDistance - yDistance;


        public bool CheckGainCriterion() => GetPartialSum() > 0;

        public bool CheckGainCriterion(int i) => GetPartialSum(i) > 0;

        public bool CheckGainCriterion(int i, double xDistance, double yDistance) => GetPartialSum(i-1, xDistance, yDistance) > 0;

        private bool CheckIfTourIsClosable(Edge edge, Node outgoingNodeFromEdge)
        {
            var tourEdges = availableEdges.Where(e => e != edge).Concat(addedEdges).Except(brokenEdges).Append(new Edge(outgoingNodeFromEdge, startingNode)).ToList();
            return graph.DepthFirstSearch(tourEdges, startingNode).Count() == graph.Size;
        }

        private bool CheckIfTourIsClosable(Edge lastBrokenEdge, Edge nextAddedEdge, Edge edge, Node outgoingNodeFromEdge)
        {
            var tourEdges = availableEdges.Where(e => e != edge && e!= lastBrokenEdge).Concat(addedEdges).Append(nextAddedEdge).Append(new Edge(startingNode, outgoingNodeFromEdge)).ToList();
            return graph.DepthFirstSearch(tourEdges, startingNode).Count() == graph.Size;
        }

        //Break and Add should be one function - 2 cycles vs 1
        private void Break(Edge edge)
        {
            availableEdges.Remove(edge);
            brokenEdges.Add(edge);
        }

        private void Add(Edge edge)
        {
            availableEdges.Remove(edge);
            addedEdges.Add(edge);
        }

        private void UpdateCurrentBestPathImprovement(Node startingNode, Node lastNode, Edge lastBrokenEdge, int i)
        {
            Edge enclosingEdge = new Edge(lastNode, startingNode);

            var enclosingEdgeLen = enclosingEdge.Length();
            var lastBrokenEdgeLen = lastBrokenEdge.Length();

            double improvement = GetPartialSum(i - 1) + (lastBrokenEdge.Length() - enclosingEdge.Length());
            if (improvement > currentBestPathImprovement)
            {
                currentBestPathImprovement = improvement;
                k = i;
            }
        }

        private void SetCurrentBestPathImprovement(Node startingNode, Node lastNode, Edge lastBrokenEdge, int i)
        {
            Edge enclosingEdge = new Edge(lastNode, startingNode);
            double improvement = GetPartialSum(i - 1) + (enclosingEdge.Length() - lastBrokenEdge.Length());
            currentBestPathImprovement = improvement;
            k = i;
        }

        private void UpdateGlobaBestPathImprovement(int i, Node enclosingNode)
        {
            if (currentBestPathImprovement > globalBestPathImprovement)
            {
                globalBestPathImprovement = currentBestPathImprovement;
                // bestPathFound = Fleurys.FindEulerCircuit(graph, availableEdges.Concat(addedEdges).ToList());
                k = i;
                this.enclosingNode = enclosingNode;
                bestPathFound = GetBestCurrentPath();
            }
        }
        private void SetGlobaBestPathImprovement()
        {
            globalBestPathImprovement = currentBestPathImprovement;
            // bestPathFound = Fleurys.FindEulerCircuit(graph, availableEdges.Concat(addedEdges).ToList());
        }


        private void RestoreState(int i)
        {
            for (int j = brokenEdges.Count - 1; j >= i-1 ; j--)
            {
                availableEdges.Add(brokenEdges[j]);
                brokenEdges.RemoveAt(j);
            }
            addedEdges = addedEdges.Take(i-1).ToList();
        }

        private List<Node> GetBestCurrentPath()
        {
            var edges = new List<Edge>();
            foreach (var edge in availableEdges)
            {
                if (brokenEdges.Take(k).Contains(edge))
                    continue;
                edges.Add(edge);
            }
            foreach (var edge in addedEdges.Take(k-1))
            {
                edges.Add(edge);
            }
            edges.Concat(brokenEdges.Skip(k).ToList());
            edges.Add(new Edge(startingNode, enclosingNode));

            return graph.DepthFirstSearch(edges, startingNode);
        }
    }
}
