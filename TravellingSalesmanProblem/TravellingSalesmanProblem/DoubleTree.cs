namespace TravellingSalesmanProblem
{
    public class DoubleTree
    {
        private IPrims prims;
        private Fleurys fleurys; //interface

        public List<Node> FindShortestPath(List<Node> nodes)
        {
            List<Node> path = new List<Node>();

            List<(Node, Node)> minimalSpanningTree = prims.FindSpanningTree(nodes);

            minimalSpanningTree = minimalSpanningTree.Concat(minimalSpanningTree).ToList();

            List<(Node, Node)> eulerCircuit = fleurys.FindEulerCircuit(nodes, minimalSpanningTree);

            List<Node> visitedCities = new List<Node>();  //refactor

            foreach (Node node in path)
                if (!visitedCities.Contains(node))
                    visitedCities.Add(node);

            visitedCities.Add(visitedCities[0]);

            return visitedCities;
        }
    }
}
