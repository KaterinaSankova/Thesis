using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    public class KLNew
    {
        private Graph graph;

        private List<Node> path = new List<Node>();

        private List<Edge> availableEdges = new List<Edge>();
        private List<Edge> brokenEdges = new List<Edge>();
        private List<Edge> addedEdges = new List<Edge>();

        private Node startingNode;

        private List<Edge> nextEdgeToBreak = new List<Edge>();

        private double currentBestPathImprovement = 0;

        private double globalBestPathInmprovement = 0;

        List<Node> bestPathFound = new List<Node>();

        Random rand = new Random();

        public List<Node> FindShortestPath(Graph graph)
        {
            //@TODO pro 0-1 prvků
            this.graph = graph;
            path = GenerateRandomStartingTour();

            SetAvailableEdgesFromPath();

            FindEdgesToExchange();

            return new List<Node>();
        }

        private List<Node> GenerateRandomStartingTour() => graph.nodes.OrderBy(_ => rand.Next()).ToList();

        private void SetAvailableEdgesFromPath()
        {
            for (int j = 0; j < path.Count - 1; j++)
                availableEdges.Add(new Edge(path[j], path[j + 1]));
            availableEdges.Add(new Edge(path[path.Count - 1], path[0]));
        }

        private void FindEdgesToExchange()
        {
            //Node
            foreach (var firstNode in path.OrderBy(_ => rand.Next()).ToList())
            {
                //x1
                foreach (var firstEdgeToBeBroken in availableEdges.Where(e => e.Contains(firstNode)).ToList())
                {
                    firstEdgeToBeBroken.MakeNode1(firstNode);
                    var firstEdgeToBeAdded = new Edge(firstEdgeToBeBroken.node2, null);
                    var edgesThatCantBeAdded = availableEdges.Concat(brokenEdges).Concat(addedEdges).Where(e => e.Contains(firstEdgeToBeBroken.node2)).ToList();

                    //y1
                    foreach (var secondNode in graph.nodes.OrderBy(node => node.Distance(firstEdgeToBeAdded.node1)).ToList()) //we want y to be as small as possible
                    {
                        if (secondNode == firstEdgeToBeAdded.node1)
                            continue;
                        firstEdgeToBeAdded.node2 = secondNode;
                        if (edgesThatCantBeAdded.Contains(firstEdgeToBeAdded)) //they are no possible
                            continue;
                        else if (!CheckGainCriterion(firstEdgeToBeBroken.Length(), firstEdgeToBeAdded.Length())) //partial sum isnt positive
                            continue;

                        //x2
                        var possibleEdgestToBreakNext = availableEdges.Where(edge => edge.Contains(secondNode)).ToList();
                        if (possibleEdgestToBreakNext.Count() == 0) continue; ///does not permit breaking xi+1

                        Edge secondEdgeToBeBroken, alternativeSecondEdgeToBeBroken;
                        if (CheckIfTourIsClosable(possibleEdgestToBreakNext.First()))
                        {
                            secondEdgeToBeBroken = possibleEdgestToBreakNext.First();
                            alternativeSecondEdgeToBeBroken = possibleEdgestToBreakNext.Last();
                        }
                        else
                        {
                            secondEdgeToBeBroken = possibleEdgestToBreakNext.Last();
                            alternativeSecondEdgeToBeBroken = possibleEdgestToBreakNext.First();
                        }

                        secondEdgeToBeBroken.MakeNode1(firstNode);
                        alternativeSecondEdgeToBeBroken.MakeNode1(firstNode);

                        var secondEdgeToBeAdded = new Edge(secondEdgeToBeBroken.node2, null);
                        var edgesThatCantBeAdded2 = availableEdges.Concat(brokenEdges).Concat(addedEdges).Where(e => e.Contains(secondEdgeToBeBroken.node2)).ToList();

                        //y2
                        foreach (var thirdNode in graph.nodes.OrderBy(node => node.Distance(secondEdgeToBeAdded.node1)).ToList()) //we want y to be as small as possible
                        {
                            if (thirdNode == secondEdgeToBeAdded.node1)
                                continue;
                            firstEdgeToBeAdded.node2 = thirdNode;
                            if (edgesThatCantBeAdded2.Contains(secondEdgeToBeAdded)) //they are no possible
                                continue;
                            else if (!CheckGainCriterion(secondEdgeToBeBroken.Length(), secondEdgeToBeAdded.Length())) //partial sum isnt positive
                                continue;

                            var nextEdgeeToBeBroken = availableEdges.Where(edge => edge.Contains(thirdNode));

                        }
                    }
                }
            }
        }

        private double GetPartialSum() => brokenEdges.Select(e => e.Length()).Sum() - addedEdges.Select(e => e.Length()).Sum();
        private double GetPartialSum(int i)
        {
            var brokenSum = brokenEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            var addedSum = addedEdges.Take(i).Select(e => e.Length()).ToList().Sum();
            return brokenSum - addedSum;
        }

        private double GetPartialSum(double xDistance, double yDistance) => GetPartialSum() + xDistance - yDistance;


        public bool CheckGainCriterion() => GetPartialSum() > 0;

        public bool CheckGainCriterion(int i) => GetPartialSum(i) > 0;

        public bool CheckGainCriterion(double xDistance, double yDistance) => GetPartialSum(xDistance, yDistance) > 0;

        private bool CheckIfTourIsClosable(Edge edge)
        {
            var tourEdges = availableEdges.Where(e => e != edge).Concat(addedEdges).Append(new Edge(edge.node2, startingNode)).ToList();
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

        //private (Edge Y, Edge NextX) FindYAndNextX(Edge firstEdgeToBeBroken, Node ySecondNode)
        //{

        //}

        private void FindEdgesToExchange(int i)
        {

        }

    }
}
