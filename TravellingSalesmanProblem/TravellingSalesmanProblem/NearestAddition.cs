﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TravellingSalesmanProblem
{
    public class NearestAddition : AlgorithmBase
    {
        public List<Node> FindShortestPath(List<Node> nodes)
        {
            (Node, Node) firstPair = FindShortestEdge(nodes, nodes);
            (Node, Node) shortestEdge;
            List<Node> path = new List<Node>();
            List<Node> remainingCities = nodes;

            path.Add(firstPair.Item1);
            path.Add(firstPair.Item2);

            remainingCities.Remove(firstPair.Item1);
            remainingCities.Remove(firstPair.Item2);

            while (remainingCities.Count > 0)
            {
                shortestEdge = FindShortestEdge(path, remainingCities);
                path.Insert(path.IndexOf(shortestEdge.Item1) + 1, shortestEdge.Item2);
                remainingCities.Remove(shortestEdge.Item2);
            }
            return path;
        }
    }
}