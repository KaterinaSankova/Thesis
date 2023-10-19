using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;
using Path = TravellingSalesmanProblem.GraphStructures.Path;

namespace TravellingSalesmanProblem.Algorithms.TSP
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
        private static List<(Path Path, List<Node> Nodes)> fruitlessNodesForPaths = new();

        //Reduction
        private static Path[] locallyOptimalPaths = new Path[5]; //constants
        private static List<Edge> goodEdges = new();


        private static Path improvedPath;
        private static Path shortestPath;

        private static Node startingNode;
        private static Node enclosingNode;

        public static Path FindShortestPath(Graph inputGraph)
        {
            if (inputGraph.nodes.Count < 4)
                return new Path (inputGraph.nodes.ToList().Append(inputGraph.nodes.First()).ToList());

            graph = inputGraph;
            GeneratePath();
            SetupInitialState();
            PrintState();
            ImprovePath();
            return shortestPath;
        }

        private static void FindShortestPath(Path inputPath)
        {
            path = inputPath;
            SetupEdges();
            improvement = 0;
            partialSum = 0;
            PrintState();
            ImprovePath();
        }

        private static void GeneratePath() => path = new Path(graph.nodes.OrderBy(_ => rand.Next()).ToList());

        private static void SetupInitialState()
        {
            k = 0;
            improvement = 0;
            partialSum = 0;

            fruitlessNodesForPaths = new List<(Path Path, List<Node> Nodes)>();

            locallyOptimalPaths = new Path[5]; //constants
            goodEdges = new List<Edge>();

            improvedPath = path.ToPath();
            shortestPath = path.ToPath();

            SetupEdges();
        }

        private static void SetupEdges()
        {
            originalEdges = new List<Edge>();
            brokenEdges = new List<Edge>();
            addedEdges = new List<Edge>();

            for (int j = 0; j < path.Count - 1; j++)
                originalEdges.Add(new Edge(path[j], path[j + 1]));
            originalEdges.Add(new Edge(path[path.Count - 1], path[0]));
        }

        private static void PrintState()
        {
            Console.WriteLine("######################### CURRENT STATE #########################");
            Console.WriteLine($"PATH: {path}");
            Console.WriteLine($"PATH LENGTH: {path.Length}");

            double len = 0;
            foreach (var e in originalEdges)
            {
                len += e.Length();
            }
            Console.WriteLine($"PATH EDGES LENGTH: {len}");

            Console.WriteLine($"IMPROVEMENT: {improvement}");
            PrintEdges("ORIGINAL EDGES", originalEdges);
            PrintEdges("BROKEN EDGES", brokenEdges);
            PrintEdges("ADDED EDGES", addedEdges);

            Console.WriteLine("FRUITLESS NODES");
            foreach (var pair in fruitlessNodesForPaths)
            {
                Console.WriteLine($"Path: {pair.Path}");

                Console.WriteLine("\nFruitless nodes: ");
                foreach (var node in pair.Nodes)
                    Console.Write($"{node} ");
            }

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
            Console.Write($"SHORTEST PATH: {shortestPath}");
            Console.Write($"PATH LENGTH: {shortestPath.Length}\n");
        }

        private static void PrintEdgeState()
        {
            PrintEdges("\tORIGINAL EDGES", originalEdges);
            PrintEdges("\tBROKEN EDGES", brokenEdges);
            PrintEdges("\tADDED EDGES", addedEdges);
        }

        private static void PrintEdges(string name, List<Edge> edges)
        {
            Console.Write($"{name}: ");
            foreach (var edge in edges)
                Console.Write($"{edge} ");
            Console.WriteLine();
        }

        private static void PrintImprovementState(Node node1, Node node2, int i)
        {
            Console.WriteLine("[*] CURRENT STATE");
            Console.WriteLine($"\tNODE{i}: {node1}");
            Console.WriteLine($"\tNODE{i+1}: {node2}");
            PrintEdgeState();
            Console.WriteLine($"\tSTARTING NODE: {startingNode}");
            Console.WriteLine($"\tENCLOSING NODE: {enclosingNode}");
        }

        private static void ImprovePath()
        {
            int i = 0;
            k = 0;
            Path currentPath;

            var fruitlessNodes = GetFruitlessNodesForPath();
            foreach (var node1 in graph.nodes)
            {
                if (fruitlessNodes.Contains(node1))
                    continue;
                startingNode = node1;
                currentPath = path.ToPath();
                RestoreState(currentPath, i);

                foreach (var brokenEdge1 in FindAttachedOriginalEdges(node1))
                {
                    if (brokenEdge1 == null)
                    {
                        Console.WriteLine("*Could not find brokenEdge1*");
                        break;
                    }
                    i = 1;
                    currentPath = path.ToPath();
                    RestoreState(currentPath, i);

                    var node2 = brokenEdge1.GetOtherNode(node1);
                    enclosingNode = node2;
                    PrintImprovementState(node1, node2, 1);
                    Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                    Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                    currentPath.SetDirection(startingNode, enclosingNode);
                    currentPath.CurrentIndex = currentPath.IndexOf(node2);
                    currentPath.Next();

                    List<(Node Node, double Value)> nodes3Values = new List<(Node Node, double Value)>();
                    Node investigatedNode3 = null;
                    Node investigatedNode4 = null;
                    var stoppingNode = currentPath.PeekPrev(node1);
                    while (investigatedNode3 != stoppingNode)
                    {
                        investigatedNode3 = currentPath.Next();
                        investigatedNode4 = currentPath.PeekPrev();
                        nodes3Values.Add((investigatedNode3, new Edge(investigatedNode3, investigatedNode4).Length() - new Edge(node2, investigatedNode3).Length()));
                    }
                    var orderedNodes3 = nodes3Values.OrderByDescending(v => v.Value).Select(v => v.Node).ToList();

                    foreach (var node3 in orderedNodes3)
                    {
                        i = 1;
                        currentPath = path.ToPath();
                        RestoreState(currentPath, i);
                        enclosingNode = node2;

                        currentPath.SetDirection(startingNode, enclosingNode);

                        var addedEdge1 = new Edge(node2, node3);
                        var node4 = currentPath.PeekPrev(node3);

                        PrintImprovementState(node3, node4, 3);
                        Console.WriteLine($"\tLATEST PATH: {currentPath}");
                        Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                        if (!UpdatePartialSum(brokenEdge1.Length(), addedEdge1.Length()))
                            break;
                        brokenEdges.Insert(i - 1, brokenEdge1);
                        addedEdges.Insert(i - 1, addedEdge1);
                        originalEdges.Remove(brokenEdge1);

                        Console.WriteLine("[+] RECONNECTING EDGES");
                        currentPath.ReconnectEdges(startingNode, enclosingNode, node3, node4);
                        Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                        Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                        UpdateLocalOptimum(i, currentPath);
                        enclosingNode = node4;
                        improvedPath = currentPath;

                        bool end = ImprovePathFromBrokenEdge2(node3, node4, currentPath);
                        if (currentPath.PeekNext(node3) != node1)
                            ImprovePathFromAlternativeBrokenEdge2(node3, currentPath.PeekNext(node3), currentPath);

                        if (end)
                            return;
                    }

                }
            }

        }

        private static bool ImprovePathFromBrokenEdge2(Node node3, Node node4, Path latestPath)
        {
            var brokenEdge2 = new Edge(node3, node4);
            int i = 2;

            RestoreState(latestPath, i);

            latestPath.SetDirection(startingNode, enclosingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);
            latestPath.Next();

            List<(Node Node, double Value)> nodes5Values = new List<(Node Node, double Value)>();
            Node investigatedNode5 = null;
            Node investigatedNode6 = null;
            var stoppingNode = latestPath.PeekPrev(startingNode);
            while (investigatedNode5 != stoppingNode)
            {
                investigatedNode5 = latestPath.Next();
                if (brokenEdges.Contains(new Edge(node4, investigatedNode5)))
                    continue;
                investigatedNode6 = latestPath.PeekPrev();
                if (!addedEdges.Contains(new Edge(investigatedNode5, investigatedNode6)))
                    nodes5Values.Add((investigatedNode5, new Edge(investigatedNode5, investigatedNode6).Length() - new Edge(enclosingNode, investigatedNode5).Length()));
            }
            var orderedNodes5 = nodes5Values.OrderByDescending(v => v.Value).Select(v => v.Node).ToList();

            foreach (var node5 in orderedNodes5)
            {
                i = 2;
                RestoreState(latestPath, i);

                var currentPath = latestPath.ToPath();
                enclosingNode = node4;

                var addedEdge2 = new Edge(node4, node5);
                var node6 = currentPath.PeekPrev(node5);
                //Console.WriteLine($"Node4: {nextNode}");
                var brokenEdge3 = new Edge(node5, node6);

                PrintImprovementState(node5, node6, 5);
                Console.WriteLine($"\tLATEST PATH: {currentPath}");
                Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                if (!UpdatePartialSum(brokenEdge2.Length(), addedEdge2.Length()))
                    break;

                brokenEdges.Insert(i - 1, brokenEdge2);
                addedEdges.Insert(i - 1, addedEdge2);
                originalEdges.Remove(brokenEdge2);

                currentPath.ReconnectEdges(startingNode, enclosingNode, node5, node6);
                Console.WriteLine($"\tCURRENT PATH: {currentPath}");
                Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

                UpdateLocalOptimum(i, currentPath);
                enclosingNode = node6;
                improvedPath = currentPath;

                ImprovePathFromNode(node6, brokenEdge3, currentPath, i + 1);

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
                    FindShortestPath(shortestPath);
                    return true;
                }
            }

            return false;
        }

        private static void ImprovePathFromAlternativeBrokenEdge2(Node node3, Node node4, Path latestPath)
        {
            int i = 2;

            RestoreState(latestPath, i);

            latestPath.SetDirection(enclosingNode, startingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(startingNode);
            latestPath.Next();

            List<(Node Node, double Value)> nodes5Values = new List<(Node Node, double Value)>();
            Node investigatedNode5 = null;
            while (investigatedNode5 != node3)
            {
                investigatedNode5 = latestPath.Next();
                if (brokenEdges.Contains(new Edge(node4, investigatedNode5)))
                    continue;
                nodes5Values.Add((investigatedNode5, new Edge(node4, investigatedNode5).Length()));
            }
            var orderedNodes5 = nodes5Values.OrderByDescending(v => v.Value).Take(5).Select(v => v.Node).ToList();

        }

        private static void ImprovePathFromNode(Node fromNode, Edge lastBrokenEdge, Path latestPath, int i)
        {
            latestPath.SetDirection(startingNode, enclosingNode);
            latestPath.CurrentIndex = latestPath.IndexOf(enclosingNode);
            latestPath.Next();

            List<(Node Node, double Value)> nodesValues = new List<(Node Node, double Value)>();
            Node investigatedNode5 = null;
            Node investigatedNode6 = null;
            var stoppingNode = latestPath.PeekPrev(startingNode);
            while (investigatedNode5 != stoppingNode)
            {
                investigatedNode5 = latestPath.Next();
                if (brokenEdges.Contains(new Edge(fromNode, investigatedNode5)))
                    continue;
                if (goodEdges.Contains(new Edge(fromNode, investigatedNode5)))
                    continue;
                investigatedNode6 = latestPath.PeekPrev();
                if (!addedEdges.Contains(new Edge(investigatedNode5, investigatedNode6)))
                    nodesValues.Add((investigatedNode5, new Edge(investigatedNode5, investigatedNode6).Length() - new Edge(enclosingNode, investigatedNode5).Length()));
            }

            if (nodesValues.Count == 0)
                return;

            var node = nodesValues.OrderByDescending(v => v.Value).First().Node;

            var currentPath = latestPath.ToPath();
            enclosingNode = fromNode;

            var addedEdge = new Edge(fromNode, node);
            var nextNode = currentPath.PeekPrev(node);
            var nextBrokenEdge = new Edge(node, nextNode);

            PrintImprovementState(node, nextNode, 2*i+1);
            Console.WriteLine($"\tLATEST PATH: {currentPath}");
            Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            if (!UpdatePartialSum(lastBrokenEdge.Length(), addedEdge.Length()))
                return;

            if ((i - 1) < brokenEdges.Count)
                brokenEdges.Insert(i - 1, lastBrokenEdge);
            else
                brokenEdges.Add(lastBrokenEdge);
            if ((i - 1) < addedEdges.Count)
                addedEdges.Insert(i - 1, lastBrokenEdge);
            else
                addedEdges.Add(lastBrokenEdge);

            originalEdges.Remove(lastBrokenEdge);

            Console.WriteLine("[+] RECONNECTING EDGES");
            currentPath.ReconnectEdges(startingNode, enclosingNode, node, nextNode);
            Console.WriteLine($"\tCURRENT PATH: {currentPath}");
            Console.WriteLine($"\tLENGTH: {currentPath.Length}\n");

            UpdateLocalOptimum(i, currentPath);
            enclosingNode = nextNode;
            improvedPath = currentPath;

            ImprovePathFromNode(nextNode, nextBrokenEdge, currentPath, i+1);
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

        private static void RestoreState(Path currentPath, int i)
        {
            RestoreState(currentPath);
            if (i == 2)
            {
                originalEdges.Remove(brokenEdges[0]);
                brokenEdges = brokenEdges.Take(1).ToList();
                addedEdges = brokenEdges.Take(1).ToList();
            }
            else
            {
                brokenEdges.Clear();
                addedEdges.Clear();
            }
        }

        private static void RestoreState(Path currentPath)
        {
            originalEdges = currentPath.GetEdges();
            k = 0;
            partialSum = 0;
        }

        private static bool UpdatePartialSum(double brokenEdgeLegth, double addedEdgeLength)
        {
            Console.WriteLine("[+] PARTIAL SUM UPDATE");
            Console.WriteLine($"\tPARTIAL SUM: {partialSum}");
            Console.WriteLine($"\tBROKEN EDGE LEN: {brokenEdgeLegth}");
            Console.WriteLine($"\tADDED EDGE LEN: {addedEdgeLength}");
            double currentPartialSum = partialSum + brokenEdgeLegth - addedEdgeLength;
            Console.WriteLine($"\tCURRENT PARTIAL SUM: {currentPartialSum}");
            if (currentPartialSum > 0)
            {
                Console.WriteLine("\tUpdating partial sum\n");
                partialSum = currentPartialSum;
                return true;
            }
            Console.WriteLine("\tNot updating partial sum\n");
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

        private static void UpdateLocalOptimum(int i, Path currentPath)
        {
            Console.WriteLine("[+] LOCAL OPTIMUM UPDATE");

            double currentImprovement = path.Length - currentPath.Length;
            Console.WriteLine($"\tIMPROVEMENT: {improvement}");
            Console.WriteLine($"\tCURRENT IMPROVEMENT: {currentImprovement}");
            if (currentImprovement > improvement)
            {
                Console.WriteLine($"\tCurrent path is local optimum\n");
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
                    Console.WriteLine($"\tAdding current path to list of locally optimal paths\n");
                    locallyOptimalPaths[count] = currentPath;
                    UpdateShortestPath(currentPath);
                    return;
                }
                if (index == -1)
                    return;

                Console.WriteLine("\tLOCALLY OPTIMAL PATHS");
                foreach (var path in locallyOptimalPaths)
                {
                    if (path != null)
                    {
                        Console.WriteLine($"\t\tPath: {path}");
                        Console.WriteLine($"\t\tLength: {path.Length}");
                    }
                }

                if (currentPath.Length < highestOptimalPathLength)
                {
                    Console.WriteLine($"\tAdding current path to list of locally optimal paths");
                    locallyOptimalPaths[index] = currentPath;
                    var pathEdges = currentPath.GetEdges();
                    var newGoodEdges = pathEdges;
                    Console.WriteLine($"\tUpdating good edges");
                    for (int j = 0; j < count; j++)
                    {
                        if (j == index)
                            continue;
                        newGoodEdges = newGoodEdges.Intersect(locallyOptimalPaths[j].GetEdges()).ToList();
                    }
                    goodEdges = newGoodEdges;
                    PrintEdges("\tGOOD EDGES", goodEdges);
                }

                UpdateShortestPath(currentPath);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"\tCurrent path is NOT local optimum\n");
            }
        }

        private static void UpdateShortestPath(Path currentPath)
        {
            Console.WriteLine("[+] UPDATE SHORTEST PATH");
            Console.WriteLine($"\tSHORTEST PATH: {shortestPath}");
            Console.WriteLine($"\tSHORTEST PATH LENGTH: {shortestPath.Length}");
            if (currentPath.Length < shortestPath.Length)
            {
                Console.WriteLine("\t*Updating shortest path*");
                shortestPath = currentPath;
                Console.WriteLine($"\tNEW SHORTEST PATH: {shortestPath}");
                Console.WriteLine($"\tNEW SHORTEST PATH LENGTH: {shortestPath.Length}");
            }
            Console.WriteLine();
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
