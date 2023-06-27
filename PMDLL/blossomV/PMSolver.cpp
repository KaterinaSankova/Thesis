#include <stdio.h>
#include "PerfectMatching.h"

extern "C" __declspec(dllexport) void find_minimal_perfect_matching(int node_num, int edge_num, int * edges, double* weights, int* output){
	struct PerfectMatching::Options options;
	options.verbose = false;
	int e;

	PerfectMatching* pm = new PerfectMatching(node_num, edge_num);
	for (e = 0; e < edge_num; e++) pm->AddEdge(edges[2 * e], edges[2 * e + 1], weights[e]);
	pm->Solve();

	double cost = ComputePerfectMatchingCost(node_num, edge_num, edges, weights, pm);

	int i, j;
	e = 0;
	for (i = 0; i < node_num; i++)
	{
		j = pm->GetMatch(i);
		if (i < j) {
			output[2 * e] = i;
			output[2 * e + 1] = j;
			e++;
		}
	}
	return;
}
