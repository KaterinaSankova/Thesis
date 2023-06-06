using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KeringhanLin
    {
        private Graph graph;
        private List<Edge> broken = new List<Edge>();
        private List<Edge> added = new List<Edge>();
        private List<Edge> available = new List<Edge>();
        private Node t1;
        Random rand = new Random();

        private double _partialSum { get; } = 0;

        private double GetPartialSum() => broken.Select(e => e.Distance()).Sum() - added.Select(e => e.Distance()).Sum();
        private double GetPartialSum(int i)
        {
            var brokenSum = broken.Take(i).Select(e => e.Distance()).ToList().Sum();
            var addedSum = added.Take(i).Select(e => e.Distance()).ToList().Sum();
            return brokenSum - addedSum;
        }

        private double GetPartialSum(double xDistance, double yDistance) => GetPartialSum() + xDistance - yDistance;

        public List<Node> FindShortestPath(Graph graph)
        {
            //@TODO pro 0-1 prvků
            this.graph = graph;
            var path = graph.nodes.OrderBy(_ => rand.Next()).ToList(); //random path

            SetupFromPath(path);

            FindShortestPathA(graph);

            return new List<Node>();
        }

        public void SetupFromPath(List<Node> path)
        {
            for (int j = 0; j < path.Count - 1; j++)
                available.Add(new Edge(path[j], path[j + 1]));
            available.Add(new Edge(path[path.Count - 1], path[0]));
            broken = new List<Edge>();
            added = new List<Edge>();
            Console.Write($"available: ");
            foreach (var node in graph.nodes)
                Console.Write($"({node.x}, {node.y})");
            Console.WriteLine();
        }

        public void FindShortestPathA(Graph graph)
        {
            //STEP 2
            int i = 1, k = 0;
            Edge x1 = available[rand.Next(graph.nodes.Count)];
            this.t1 = x1.node1;
            Break(x1);

            //STEP 3
            Edge? y1 = new Edge(x1.node2, null);
            var unuseableEdges = available.Concat(broken).Concat(added).Where(e => e.Contains(x1.node2)).ToList();
            foreach (var node in graph.nodes.OrderBy(node => node.Distance(y1.node1)).ToList())
            {
                if (node == y1.node1)
                    continue;
                y1.node2 = node;
                if (unuseableEdges.Contains(y1))
                    continue;
                if (x1.Distance() - y1.Distance() <= 0) //y1 distance only decreases, because node2 are are picked from a list ordered by distance to node1
                {
                    //go to backtracking 6(d)
                }
                break;
            }            
            Add(y1);

            Step45(x1, y1);         
        }

        public void Step45 (Edge x1, Edge y1)
        {
            double bestImprovement = 0;
            //STEP 4
            Edge? xi = x1, yi = y1;
            while (GetPartialSum(i) > bestImprovement)
            {
                Console.WriteLine($"i: {i}, xi: {xi}, yi:{yi}");
                PrintState();

                i++;
                xi = FindX(yi.node2);

                if (xi == null)
                {
                    Console.WriteLine("POSSIBLE ERROR: xi was not found");
                    break; //asi by  nikdy nemelo nastat?
                }
                yi = FindY(xi);

                if (yi == null)
                    break;

                Break(xi);

                double improvement = GetPartialSum(i - 1) + xi.node2.Distance(t1) - xi.Distance();

                if (improvement > bestImprovement)
                {
                    bestImprovement = improvement;
                    k = i;
                }

                Add(yi);
            }

            //STEP 5
            if (bestImprovement > 0)
            {
                for (int j = 0; j < added.Count - 1; j++) //last yi is not used
                    available.Add(added[j]);
                available.Add(new Edge(xi.node2, t1)); //add closing edge to make a tour

                FindShortestPathA(graph);
            }
        }

        public void PrintState()
        {
            Console.Write($"available: ");
            foreach (var edge in available)
                Console.Write($"{edge}, ");
            Console.WriteLine();

            Console.Write($"broken: ");
            foreach (var edge in broken)
                Console.Write($"{edge}, ");
            Console.WriteLine();

            Console.Write($"added: ");
            foreach (var edge in added)
                Console.Write($"{edge}, ");
            Console.WriteLine();
        }

        public bool CheckGainCriterion() => GetPartialSum() > 0;

        public bool CheckGainCriterion(int i) => GetPartialSum(i) > 0;

        public bool CheckGainCriterion(double xDistance, double yDistance) => GetPartialSum(xDistance, yDistance) > 0;

        public Edge? FindX(Node xNode)
        {
            Edge? x = null;
            var edgesFromxNode = available.Where(e => e.Contains(xNode)).ToList();
            foreach (var edge in edgesFromxNode)
            {
                if(edge.node2 == xNode)
                {
                    edge.node2 = edge.node1;
                    edge.node1 = xNode;
                }
                if (CheckIfTourIsClosable(edge))
                {
                    x = new Edge(xNode, edge.node1 == xNode ? edge.node2 : edge.node1);
                    break;
                }
            }
            if (x == null)
                Console.WriteLine("X IS NULL");
            return x;
        }

        private bool CheckIfTourIsClosable(Edge edge)
        {
            var tourEdges = available.Where(e => e != edge).Concat(added).Append(new Edge(edge.node2, t1)).ToList();
            return graph.DepthFirstSearch(tourEdges, t1).Count() == graph.Size;
        }

        public Edge? FindY(Edge xEdge)
        {
            var unuseableEdges = available.Concat(broken).Concat(added).Where(e => e.Contains(xEdge.node2)).ToList();
            Edge y = new Edge(xEdge.node2, null);
            foreach (var node in graph.nodes.OrderBy(node => node.Distance(xEdge.node2)).ToList()) //we want y to be as small as possible
            {
                if (node == y.node1)
                    continue;
                y.node2 = node;
                if (unuseableEdges.Contains(y)) //there are no possible
                    continue;
                else if (available.Where(edge => edge.Contains(node)).Where(e => CheckIfTourIsClosable(e)).ToList().Count() == 0) ///does not permit breaking xi+1
                    continue;
                else if (!CheckGainCriterion(xEdge.Distance(), y.Distance())) //partial sum isnt positive
                    continue;
                else return y;     
            }
            return y;
        }

        private void Break(Edge edge)
        {
            available.Remove(edge);
            broken.Add(edge);
        }

        private void Add(Edge edge)
        {
            available.Remove(edge);
            added.Add(edge);
        }
    }
}
