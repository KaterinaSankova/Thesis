using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using TravellingSalesmanProblem.Algorithms;
using TravellingSalesmanProblem.Formats;

namespace TravellingSalesmanProblem.GraphStructures
{
    public class Graph //osetrit grafy s 0 nebo 1 nodem
    {
        public readonly List<Node> nodes;
        public List<Edge>? edges;
        private Hashtable shapes;

        public int Size => nodes.Count;

        public bool IsEmpty => nodes.Count == 0;
        
        public Graph()
        {
            nodes = new();
        }

        public Graph(List<Node> nodes)
        {
            this.nodes = nodes;
        }
        public Graph(List<Node> nodes, List<Edge> edges)
        {
            this.nodes = nodes;
            this.edges = edges;
        }

        public Edge? ShortestEdge(List<Node>? fromNodes = null, List<Node>? toNodes = null)
        {
            fromNodes ??= nodes;
            toNodes ??= nodes;
            if (fromNodes.Count == 0 || toNodes.Count == 0)
                return null;

            double minDistance = double.MaxValue;
            double currDistance;
            Edge minEdge = new (fromNodes.First(), toNodes.First());

            foreach (Node fromNode in fromNodes)
            {
                foreach (Node toNode in toNodes)
                {
                    currDistance = fromNode.Distance(toNode);
                    if (currDistance < minDistance && fromNode != toNode)
                    {
                        minDistance = currDistance;
                        minEdge = new(fromNode, toNode);
                    }
                }
            }

            return minEdge;
        }

        public List<Node> OddDegreeNodes(List<Edge> edges = null)
        {
            if (edges == null)
                edges = this.edges;

            Dictionary<int, int> nodeDegres = new();
            int degree;

            foreach (Edge edge in edges)
            {
                if (!nodeDegres.TryGetValue(edge.node1.id, out degree))
                    nodeDegres.Add(edge.node1.id, 1);
                else
                    nodeDegres[edge.node1.id]++;

                if (!nodeDegres.TryGetValue(edge.node2.id, out degree))
                    nodeDegres.Add(edge.node2.id, 1);
                else
                    nodeDegres[edge.node2.id]++;
            }

            return nodes.Where(n => nodeDegres[n.id] % 2 == 1) .ToList();
        }

        public List<Edge> OutgoingEdges(Node node)
        {
            List<Edge> outgoingEdges = new();
            foreach (var edge in edges)
            {
                if (edge.node1 == node)
                    outgoingEdges.Add(edge);
                else if (edge.node2 == node)
                    outgoingEdges.Add(edge);
            }
            return outgoingEdges;
        }

        private void FindCircles()
        {
            shapes = new();

            if (Size == 0)
                return;

            List<Edge> unvisitedEdges = this.edges.ToList();

            FindCircles(nodes.First(), new(), ref unvisitedEdges);
        }

        private void FindCircles(Node startingNode, List<Node> visited, ref List<Edge> unvisitedEdges)
        {
            List<Node> circleNodes;
            List<Edge> circle;
            int circleStartIndex = visited.FindLastIndex(n => n == startingNode);

            if (circleStartIndex != -1)
            {
                circleNodes = visited.Skip(circleStartIndex).Select(n => n).ToList();
                circle = new();

                for (int i = 0; i < circleNodes.Count - 1; i++)
                {
                    circle.Add(new(circleNodes[i], circleNodes[i + 1]));
                }
                circle.Add(new(circleNodes[^1], circleNodes[0]));

                foreach (var n in circleNodes)
                {
                    if (!shapes.ContainsKey(n))
                        shapes[n] = new List<(bool IsCircle, List<Edge> Shape)>() { };
                    ((List<(bool IsCircle, List<Edge> Shape)>)shapes[n]).Add((true, circle));
                }

                visited = visited.Take(circleStartIndex).ToList();
            }

            List<Edge> outgoingEdges = unvisitedEdges.Where(e => e.Contains(startingNode)).ToList();
            visited.Add(startingNode);

            while (outgoingEdges.Count == 1)
            {
                startingNode = outgoingEdges.First().GetOtherNode(startingNode);
                circleStartIndex = visited.FindLastIndex(n => n == startingNode);

                if (circleStartIndex != -1)
                {
                    circleNodes = visited.Skip(circleStartIndex).Select(n => n).ToList();
                    circle = new();

                    for (int i = 0; i < circleNodes.Count - 1; i++)
                    {
                        circle.Add(new(circleNodes[i], circleNodes[i + 1]));
                    }
                    circle.Add(new(circleNodes[^1], circleNodes[0]));

                    foreach (var n in circleNodes)
                    {
                        if (!shapes.ContainsKey(n))
                            shapes[n] = new List<(bool IsCircle, List<Edge> Shape)>() { };
                        ((List<(bool IsCircle, List<Edge> Shape)>)shapes[n]).Add((true, circle));
                    }

                    visited = visited.Take(circleStartIndex).ToList();
                }

                unvisitedEdges.Remove(outgoingEdges.First());
                visited.Add(startingNode);
                outgoingEdges = unvisitedEdges.Where(e => e.Contains(startingNode)).ToList();
            }

            if (outgoingEdges.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var e in outgoingEdges)
                {
                    if (!unvisitedEdges.Contains(e))
                        continue;
                    unvisitedEdges.Remove(e);
                    FindCircles(e.GetOtherNode(startingNode), visited.ToList(), ref unvisitedEdges);
                }
            }
        }

        private void FindSCC()
        {
            shapes = new();

            if (Size == 0)
                return;

            Stack<int> stack = new();
            int[] ids = new int[nodes.Count];
            int[] lows = new int[nodes.Count];
            bool[] onStack = new bool[nodes.Count];
            int count = 0;
            Dictionary<Node, int> idedNodes = new Dictionary<Node, int>();
            for (int i = 0; i < nodes.Count; i++)
                idedNodes.Add(nodes[i], i);

            for (int i = 0; i < nodes.Count; i++)
                DFS(i, ref stack, ref ids, ref lows, ref onStack, ref count, idedNodes);
        }
        private void DFS(int index, ref Stack<int> stack, ref int[] ids, ref int[] lows, ref bool[] onStack, ref int count, Dictionary<Node, int> idedNodes)
        {
            if (ids[index] != 0)
                return;
            count++;
            ids[index] = count;
            lows[index] = count;
            stack.Push(index);
            onStack[index] = true;

            var connectedNodes = nodes[index].ConnectedNodes(edges);

            foreach (var connNode in connectedNodes)
                DFS(idedNodes[connNode], ref stack, ref ids, ref lows, ref onStack, ref count, idedNodes);

            foreach (var connNode in connectedNodes)
            {
                if (connectedNodes.Count != 2 && edges.Where(e => e == new Edge(connNode, idedNodes.FirstOrDefault(x => x.Value == index).Key)).ToList().Count == 2)
                    continue;
                if (onStack[idedNodes[connNode]])
                    lows[index] = Math.Min(lows[index], lows[idedNodes[connNode]]);
                if (ids[index] == lows[index])
                {
                    List<Node> comp = new();
                    int x;
                    while (true)
                    {
                        if (!stack.TryPop(out x))
                            break;
                        onStack[x] = false;
                        comp.Add(idedNodes.FirstOrDefault(n => n.Value == x).Key);

                        if (x == index)
                            break;
                    }

                    if (comp.Count >= 2)
                    {
                        if (edges.Where(e => e == new Edge(comp[^1], comp[^2])).ToList().Count == 2)
                        {
                            stack.Push(idedNodes[comp[^1]]);
                            stack.Push(idedNodes[comp[^2]]);
                            onStack[idedNodes[comp[^1]]] = true;
                            onStack[idedNodes[comp[^2]]] = true;
                        }

                        List<Edge> compEdges = new();

                        for (int i = 0; i < comp.Count - 1; i++)
                        {
                            compEdges.Add(new(comp[i], comp[i + 1]));
                        }
                        compEdges.Add(new(comp[^1], comp[0]));

                        foreach (var node in comp)
                        {
                            if (!shapes.ContainsKey(node))
                                shapes[node] = new List<(bool IsCircle, List<Edge> Shape)>() { };
                            ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node]).Add((true, compEdges));
                        }
                    }
                }
            }

        }

        public List<Node> FindEulerCircuit()
        {
            List<Node> result = new();
            var graph = new Graph(nodes, edges);

            if (this.OddDegreeNodes(edges).Count != 0)
                throw new Exception("Euler circuit does not exist");

            if (Size == 0)
                return new();

            FindCircles();

            Node startingNode = nodes[new Random().Next(nodes.Count)];
            Node? currentNode = startingNode;
            Node? otherNode;
            List<(bool IsCircle, List<Edge> Shape)> currShapes;
            int circleIndex;

            while (currentNode != null)
            {
                currShapes = (List<(bool IsCircle, List<Edge> Shape)>)shapes[currentNode];
                if (currShapes == null || currShapes.Count == 0)
                    break;

                int index = currShapes.FindIndex(x => x.IsCircle);

                if (index != -1)
                {
                    var circle = currShapes[index];

                    (bool IsCircle, List<Edge> Shape) line = (false, circle.Shape.ToList());
                    var edge = circle.Shape.Find(e => e.Contains(currentNode));
                    line.Shape.Remove(edge);

                    otherNode = edge.GetOtherNode(currentNode);

                    foreach (var node in line.Shape.SelectMany(e => new List<Node> {e.node1, e.node2}).Distinct().ToList())
                    {
                        circleIndex = ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node]).FindIndex(s => s == circle);
                        ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node])[circleIndex] = line;
                    }
                }
                else
                {
                    var line = currShapes.First();

                    (bool IsCircle, List<Edge> Shape) shortenedLine = (false, line.Shape.ToList());
                    var edge = line.Shape.Find(e => e.Contains(currentNode));
                    shortenedLine.Shape.Remove(edge);

                    otherNode = edge.GetOtherNode(currentNode);

                    foreach (var node in line.Shape.SelectMany(e => new List<Node> { e.node1, e.node2 }).Distinct().ToList())
                    {
                        circleIndex = ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node]).FindIndex(s => s == line);
                        if (shortenedLine.Shape.Count != 0)
                            ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node])[circleIndex] = shortenedLine;
                        else
                            ((List<(bool IsCircle, List<Edge> Shape)>)shapes[node]).RemoveAt(circleIndex);
                    }

                    circleIndex = ((List<(bool IsCircle, List<Edge> Shape)>)shapes[currentNode]).FindIndex(s => s == shortenedLine);
                    if (circleIndex != -1)
                        ((List<(bool IsCircle, List<Edge> Shape)>)shapes[currentNode]).RemoveAt(circleIndex);
                }

                result.Add(currentNode);
                currentNode = otherNode;
            }

            result.Add(startingNode);

            return result;
        }

        public bool IsBridge(Edge edge)
        {
            var edgesWithoutSelf = edges.ToList();
            edgesWithoutSelf.Remove(edge);

            return Graph.DepthFirstSearch(edges, edge.node1).Count != Graph.DepthFirstSearch(edgesWithoutSelf, edge.node1).Count;
        }

        public static List<Node> DepthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> visited = new();
            Stack<Node> stack = new();

            stack.Push(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();

                if(!visited.Contains(node))
                {
                    visited.Add(node);

                    var connectedNodes = node.ConnectedNodes(edges);
                    foreach (Node adjacentNode in connectedNodes)
                        if (!visited.Contains(adjacentNode))
                            stack.Push(adjacentNode);
                }

            }

            return visited;
        }

        public static List<Node> BreadthFirstSearch(List<Edge> edges, Node node)
        {
            List<Node> visited = new();
            Queue<Node> queue = new();

            queue.Enqueue(node);
            visited.Add(node);

            while (queue.Count > 0)
            {
                node = queue.Dequeue();

                var connectedNodes = node.ConnectedNodes(edges);

                foreach (Node adjacentNode in connectedNodes)
                    if (!visited.Contains(adjacentNode))
                    {
                        visited.Add(adjacentNode);
                        queue.Enqueue(adjacentNode);
                    }
            }

            return visited;
        }

        public double GetLength()
        {
            double length = 0;

            for (int i = 0; i < nodes.Count - 1; i++)
                length += nodes[i].Distance(nodes[i+1]);
            length += nodes[0].Distance(nodes[^1]);

            return length;
        }

        public (double MaxX, double MaxY, double MinX, double MinY) GetExtremeCoordinatesValues()
        {
            double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;

            foreach (Node node in nodes)
            {
                if (node.x > maxX) maxX = node.x;
                if (node.x < minX) minX = node.x;

                if (node.y > maxY) maxY = node.y;
                if (node.y < minY) minY = node.y;
            }

            return (maxX, maxY, minX, minY);

        }
    }
}
