using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using TravellingSalesmanProblem.GraphStructures;

namespace GraphicTravellingSalesmanProblem.TravellingSalesmanProblem.Algorithms
{
    static public class PerfectMatching
    {

        [DllImport("../../../libs/PMDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]

        public static extern unsafe int* find_minimal_perfect_matching(int nodeCount, int edgeCount, int* edges, double* weights, int* output);


        static string inputPath = "C:/tmp/InputFile.txt";
        static string outputPath = "C:/tmp/InputFile.txt";

        public static List<Edge> FindMinimalPerfectMatching(Graph graph) //pak se zbavit souboru + prazdne soubory
        {
            int nodeCount = graph.nodes.Count();
            int edgeCount = nodeCount * (nodeCount - 1) / 2;
            int[] pairs = new int[edgeCount * 2];
            double[] weights = new double[edgeCount];
            List<Edge> edgeMatching = new List<Edge>();

            PrepareInputForBlossomV(graph, nodeCount, ref pairs, ref weights);
            unsafe
            {
                int[] result = new int[nodeCount];
                int* matching;
                fixed (int* p = pairs)
                {
                    fixed (double* w = weights)
                    {
                        fixed (int* r = result)
                        {
                            find_minimal_perfect_matching(nodeCount, edgeCount, p, w, r);
                            for (int e = 0; e < nodeCount / 2; e++)
                            {
                                int node1 = result[2 * e];
                                int node2 = result[2 * e + 1];
                                edgeMatching.Add(new Edge(graph.nodes[node1], graph.nodes[node2]));
                            }
                        }
                    }
                }

            }
            return edgeMatching;
        }



        private static void PrepareInputForBlossomV(Graph graph, int nodeCount, ref int[] pairs, ref double[] weights)
        {
            int weightCount = 0, pairCount = 0;
            Node node1, node2;

            for (int i = 0; i < nodeCount - 1; i++)
            {
                node1 = graph.nodes[i];
                for (int j = i + 1; j < nodeCount; j++)
                {
                    node2 = graph.nodes[j];
                    pairs[pairCount] = i;
                    pairs[pairCount + 1] = j;
                    weights[weightCount] = node1.Distance(node2);
                    pairCount += 2;
                    weightCount++;
                }
            }
        }

        private static List<Edge> CreateEdgesFromBlossomVOutput(Graph graph)
        {
            if (!File.Exists(outputPath))
            {
                throw new Exception($"BlossomV output file {outputPath} does not exist");
            }

            var edges = new List<Edge>();

            using (StreamReader sr = new StreamReader(outputPath))
            {
                string line = sr.ReadLine();
                if (line == null)
                    throw new Exception($"BlossomV output file {outputPath} is empty");
                while ((line = sr.ReadLine()) != null)
                {
                    var ids = line.Split(' ');
                    if (ids.Count() != 2)
                        throw new Exception($"Invalid edge format of line {line} in blossomV output file {outputPath}");

                    int nodeId1, nodeId2;
                    if (!int.TryParse(ids[0], out nodeId1))
                        throw new Exception($"Invalid edge format of line {line} in blossomV output file {outputPath}");
                    if (!int.TryParse(ids[1], out nodeId2))
                        throw new Exception($"Invalid edge format of line {line} in blossomV output file {outputPath}");

                    Node? node1 = graph.nodes.Find(n => n.id == nodeId1);
                    if (node1 == null)
                        throw new Exception($"No node with id {nodeId1} was found");

                    Node? node2 = graph.nodes.Find(n => n.id == nodeId2);
                    if (node2 == null)
                        throw new Exception($"No node with id {nodeId2} was found");

                    edges.Add(new Edge(node1, node2));
                }
            }
            return edges;
        }
    }
}
