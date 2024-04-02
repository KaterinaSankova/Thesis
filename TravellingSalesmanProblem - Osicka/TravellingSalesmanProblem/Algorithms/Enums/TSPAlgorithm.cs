namespace TravellingSalesmanProblem.Algorithms.Enums
{
    public enum TSPAlgorithm
    {
        NearestAddition = 0,
        DoubleTree = 1,
        Christofides = 2,
        KernighanLin = 3,
        KernighanLinRb = 4
    }

    public static class TSPAlgorithmExtentions
    {
        public static string ToFullString(TSPAlgorithm algo)
        {
            switch (algo)
            {
                case TSPAlgorithm.NearestAddition:
                    return "nearest-addition algorithm";
                case TSPAlgorithm.DoubleTree:
                    return "double-tree algorithm";
                case TSPAlgorithm.Christofides:
                    return "Christofides' algorithm";
                case TSPAlgorithm.KernighanLin:
                    return "Kernighan-Lin algorithm";
                case TSPAlgorithm.KernighanLinRb:
                    return "Kernighan-Lin algorithm with reduced backtracking";
                default:
                    throw new Exception("Unknown algorithm type");
            }
        }

        public static string ToAbbreviationString(TSPAlgorithm algo)
        {
            switch (algo)
            {
                case TSPAlgorithm.NearestAddition:
                    return "NA";
                case TSPAlgorithm.DoubleTree:
                    return "DT";
                case TSPAlgorithm.Christofides:
                    return "Ch";
                case TSPAlgorithm.KernighanLin:
                    return "KL";
                case TSPAlgorithm.KernighanLinRb:
                    return "KLrB";
                default:
                    throw new Exception("Unknown algorithm type");
            }
        }
    }
}
