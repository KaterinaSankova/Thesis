using System.Runtime.InteropServices;
using TravellingSalesmanProblem.GraphStructures;

namespace TravellingSalesmanProblem.Algorithms.TSP
{
    static public class PerfectMatching
    {

        [DllImport("./../../../libs/PMDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]

        public static extern unsafe int* find_minimal_perfect_matching(int nodeCount, int edgeCount, int* edges, double* weights, int* output);

        public static List<Edge> FindMinimalPerfectMatching(Graph graph) //pak se zbavit souboru + prazdne soubory
        {
            int nodeCount = graph.nodes.Count();
            int edgeCount = nodeCount * (nodeCount - 1) / 2;
            int[] pairs = new int[edgeCount * 2];
            double[] weights = new double[edgeCount];
            List<Edge> edgeMatching = new();

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
                                edgeMatching.Add(new(graph.nodes[node1], graph.nodes[node2]));
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
    }
}
