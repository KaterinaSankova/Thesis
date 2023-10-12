using System;
using System.Collections.Generic;
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

        private static List<Edge> originalEdges;
        private static List<Edge> brokenEdges;
        private static List<Edge> addedEdges;

        private static int k;
        private static double improvement;
        private static double partialSum;

        private static Random rand = new Random();

        //Avoiding checkout time
        private static List<(Path Path, List<Node> Nodes)> fruitlessNodesForPaths;

        //Reduction
        private static Path[] locallyOptimalPaths; //constants
        private static List<Edge> goodEdges;


        private static Path improvedPath;
        private static Path shortestPath;

        private static Node startingNode;
        private static Node enclosingNode;

        public static List<Node> FindShortestPath(Graph inputGraph)
        {
            if (inputGraph.nodes.Count < 4)
                return inputGraph.nodes.ToList().Append(inputGraph.nodes.First()).ToList();

            graph = inputGraph;
            GeneratePath();
            SetupInitialState();
            ImprovePath();
            return shortestPath.ToList();
        }

        private static List<Node> FindShortestPath(Path inputPath)
        {
            path = inputPath;
            SetupInitialState();
            ImprovePath();
            return shortestPath.ToList();
        }

        private static void GeneratePath() => path = new Path(graph.nodes.OrderBy(_ => rand.Next()).ToList());

        private static void SetupInitialState()
        {
            k = 0;
            improvement = 0;
            partialSum = 0;

            originalEdges = new List<Edge>();
            brokenEdges = new List<Edge>();
            addedEdges = new List<Edge>();

            fruitlessNodesForPaths = new List<(Path Path, List<Node> Nodes)> ();

            locallyOptimalPaths = new Path[5]; //constants
            goodEdges = new List<Edge>();

            improvedPath = path.ToPath();
            shortestPath = path.ToPath();

            for (int j = 0; j < path.Count - 1; j++)
                originalEdges.Add(new Edge(path[j], path[j + 1]));
            originalEdges.Add(new Edge(path[path.Count - 1], path[0]));
        }

        private static void ImprovePath()
        {
            int i = 0;
            k = 0;

            var fruitlessNodes = GetFruitlessNodesForPath();
            foreach (var node1 in graph.nodes)
            {
                //Console.WriteLine($"Node1: {node1}");
                if (fruitlessNodes.Contains(node1))
                    continue;
                startingNode = node1;

                foreach (var brokenEdge1 in FindAttachedOriginalEdges(node1))
                {
                    if (brokenEdge1 == null)
                        break;
                    i = 1;
                    RestoreState(i);
                    improvement = 0;
                    partialSum = 0;
                    var currentPath = path.ToPath();

                    var node2 = brokenEdge1.GetOtherNode(node1);
                    enclosingNode = node2;
                    //Console.WriteLine($"Node2: {node2}");

                    currentPath.SetDirection(startingNode, enclosingNode);
                    currentPath.CurrentIndex = currentPath.IndexOf(node2);
                    currentPath.Next();

                    List<(Node Node, double Value)> nodes3Values = new List<(Node Node, double Value)>();
                    Node investigatedNode3 = null;
                    Node investigatedNode4 = null;
                    var stoppingNode = currentPath.PeekPrev(node1);
                    //Console.WriteLine("Finding node3 values");
                    while (investigatedNode3 != stoppingNode)
                    {
                        investigatedNode3 = currentPath.Next();
                        investigatedNode4 = currentPath.PeekPrev();
                        nodes3Values.Add((investigatedNode3, new Edge(investigatedNode3, investigatedNode4).Length() - new Edge(node2, investigatedNode3).Length()));
                    }
                    //Console.WriteLine("Ordering node3 values");
                    var orderedNodes3 = nodes3Values.OrderByDescending(v => v.Value).Select(v => v.Node).ToList();

                    foreach (var node3 in orderedNodes3)
                    {
                        //Console.WriteLine($"Node3: {node3}");
                        i = 1;
                        RestoreState(i);
                        improvement = 0;
                        currentPath = path.ToPath();
                        enclosingNode = node2;
                        currentPath.SetDirection(startingNode, enclosingNode);

                        var addedEdge1 = new Edge(node2, node3);
                        var node4 = currentPath.PeekPrev(node3);
                        //Console.WriteLine($"Node4: {node4}");
                        var brokenEdge2 = new Edge(node3, node4);

                        //Console.WriteLine("Updating partial sum");
                        if (!UpdatePartialSum(brokenEdge1.Length(), addedEdge1.Length()))
                            break;
                        //Console.WriteLine("Breaking and adding first pair");
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);
                        originalEdges.Remove(brokenEdge1);

                        //Console.WriteLine("Reconnecting edges");
                        currentPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);
                        //Console.WriteLine("Updating local optimum");
                        UpdateLocalOptimum(i, brokenEdge2, node4, currentPath);
                        enclosingNode = node4;
                        improvedPath = currentPath;

                        //Console.WriteLine("Improving path from broken edge 2");
                        ImprovePathFromBrokenEdge2(brokenEdge2, node4, currentPath);
                        if (currentPath.PeekNext(node3) != node1)
                            ImprovePathFromAlternativeBrokenEdge2(new Edge(node3, currentPath.PeekNext(node3)));
                    }
                    
                }

                if (improvement == 0)
                {
                    var paths = fruitlessNodesForPaths.Where(r => r.Path.Equals(improvedPath)).ToList();
                    if (paths.Count != 0)
                        paths.First().Nodes.Add(startingNode);
                }
                if (improvement > 0)
                {
                    FindShortestPath(improvedPath);
                }
            }

        }

        private static void ImprovePathFromBrokenEdge2(Edge brokenEdge2, Node node4, Path latestPath)
        {
            int i = 2;
            RestoreState(i);
            improvement = 0;

            latestPath.SetDirection(startingNode, enclosingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);
            latestPath.Next();

            List<(Node Node, double Value)> nodes5Values = new List<(Node Node, double Value)>();
            Node investigatedNode5 = null;
            Node investigatedNode6 = null;
            var stoppingNode = latestPath.PeekPrev(startingNode);
            //Console.WriteLine("Finding node5 values");
            while (investigatedNode5 != stoppingNode)
            {
                investigatedNode5 = latestPath.Next();
                investigatedNode6 = latestPath.PeekPrev();
                if (!brokenEdges.Contains(new Edge(investigatedNode5, investigatedNode6)))
                    nodes5Values.Add((investigatedNode5, new Edge(investigatedNode5, investigatedNode6).Length() - new Edge(enclosingNode, investigatedNode5).Length()));
            }
            //Console.WriteLine("Ordering node5 values");
            var orderedNodes5 = nodes5Values.OrderByDescending(v => v.Value).Select(v => v.Node).ToList();

            foreach (var node5 in orderedNodes5)
            {
                //Console.WriteLine($"Node3: {node5}");
                i = 2;
                RestoreState(i);
                improvement = 0;
                var currentPath = latestPath.ToPath();
                enclosingNode = node4;

                var addedEdge2 = new Edge(node4, node5);
                var node6 = currentPath.PeekPrev(node5);
                //Console.WriteLine($"Node4: {node6}");
                var brokenEdge3 = new Edge(node5, node6);

                //Console.WriteLine("Updating partial sum");
                if (!UpdatePartialSum(brokenEdge2.Length(), addedEdge2.Length()))
                    break;
                //Console.WriteLine("Breaking and adding first pair");
                brokenEdges.Insert(i - 1, brokenEdge2);
                addedEdges.Insert(i - 1, addedEdge2);
                originalEdges.Remove(brokenEdge2);

                //Console.WriteLine("Reconnecting edges");
                currentPath.ReconnectEdges(startingNode, enclosingNode, node5, node6);
                //Console.WriteLine("Updating local optimum");
                UpdateLocalOptimum(i, brokenEdge3, node6, currentPath);
                enclosingNode = node6;
                improvedPath = currentPath;
            }
        }

        private static void ImprovePathFromAlternativeBrokenEdge2(Edge brokenEdge2)
        {
            int i = 2;
            RestoreState(i);
            improvement = 0;
        }

        private static List<Node> GetFruitlessNodesForPath()
        {
            foreach (var record in fruitlessNodesForPaths)
            {
                if (path.Equals(record.Path))
                    return record.Nodes;
            }

            return new List<Node>();
        }

        private static Edge[] FindAttachedOriginalEdges(Node node)
        {
            var attachedEdges = new Edge[2];
            int count = 0;
            foreach (var edge in originalEdges)
            {
                if (edge.Contains(node))
                {
                    attachedEdges[count] = edge;
                    count++;
                }
                if (count == 2)
                    break;
            }

            if (attachedEdges[1] != null && attachedEdges[0].Length() > attachedEdges[1].Length())
                (attachedEdges[0], attachedEdges[1]) = (attachedEdges[1], attachedEdges[0]);

            return attachedEdges;
        }

        private static Node[] FindAttachedOriginalNodes(Node node)
        {
            var attachedNodes = new Node[2];
            var attachedEdges = FindAttachedOriginalEdges(node);

            if (attachedEdges[1] != null)
            {
                attachedNodes[1] = attachedEdges[1].GetOtherNode(node);

                if (attachedEdges[0] != null)
                    attachedNodes[0] = attachedEdges[0].GetOtherNode(node);
            }

            return attachedNodes;
        }

        private static void RestoreState(int i)
        {
            for (int j = brokenEdges.Count - 1; j >= i - 1; j--)
            {
                originalEdges.Add(brokenEdges[j]);
                brokenEdges.RemoveAt(j);
            }
            addedEdges = addedEdges.Take(i - 1).ToList();
            k = 0;
        }

        private static bool UpdatePartialSum(double brokenEdgeLeght, double addedEdgeLength)
        {
            double currentPartialSum = partialSum + brokenEdgeLeght - addedEdgeLength;
            if (currentPartialSum > 0)
            {
                partialSum = currentPartialSum;
                return true;
            }
            return false;
        }

        private static double GetPartialSum() => brokenEdges.Select(e => e.Length()).Sum() - addedEdges.Select(e => e.Length()).Sum();
        private static double GetPartialSum(int i)
        {
            var brokenSum = brokenEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            var addedSum = addedEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            return brokenSum - addedSum;
        }

        private static double GetPartialSum(int i, double xDistance, double yDistance) => GetPartialSum(i) + xDistance - yDistance;


        public static bool CheckGainCriterion() => GetPartialSum() > 0;

        public static bool CheckGainCriterion(int i) => GetPartialSum(i) > 0;

        public static bool CheckGainCriterion(int i, double xDistance, double yDistance) => GetPartialSum(i - 1, xDistance, yDistance) > 0;

        private static void UpdateLocalOptimum(int i, Edge lastBrokenEdge, Node enclosingNode, Path currentPath)
        {
            Edge enclosingEdge = new Edge(enclosingNode, startingNode);

            double currentImprovement = partialSum + (lastBrokenEdge.Length() - enclosingEdge.Length());
            if (currentImprovement > improvement)
            {
                improvement = currentImprovement;
                k = i;
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
                    //Console.WriteLine($"Locally optimum solution count {count}");
                    locallyOptimalPaths[count] = currentPath;
                    return;
                }
                if (index == -1)
                    return;
                //Console.WriteLine($"Adding locally optimal solution on index {index}");

                locallyOptimalPaths[index] = currentPath;
                //Console.WriteLine("Getting edges");
                var pathEdges = currentPath.GetEdges();
                var newGoodEdges = pathEdges;
                //Console.WriteLine($"Finding new good links");
                for (int j = 0; j < count; j++)
                {
                    if (j == index)
                        continue;
                    newGoodEdges = newGoodEdges.Intersect(locallyOptimalPaths[j].GetEdges()).ToList();
                }
                goodEdges = newGoodEdges;
            }
        }

        private static void UpdateCurrentBestPathImprovement(Node lastNode, Edge lastBrokenEdge, int i)
        {
            Edge enclosingEdge = new Edge(lastNode, startingNode);

            double currentImprovement = GetPartialSum(i - 1) + (lastBrokenEdge.Length() - enclosingEdge.Length());
            if (currentImprovement > improvement)
            {
                improvement = currentImprovement;
                k = i;
            }
        }

        private static List<Node> GetPath(int i)
        {
            var edges = new List<Edge>();
            foreach (var edge in originalEdges)
            {
                if (brokenEdges.Take(i).Contains(edge))
                    continue;
                edges.Add(edge);
            }
            foreach (var edge in addedEdges.Take(i - 1))
            {
                edges.Add(edge);
            }
            edges = edges.Concat(brokenEdges.Skip(i).ToList()).ToList();
            edges.Add(new Edge(startingNode, enclosingNode));

            return graph.DepthFirstSearch(edges, startingNode);
        }
    }
}
