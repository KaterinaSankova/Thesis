using TravellingSalesmanProblem.Algorithms.Enums.TSPLib;
using Type = TravellingSalesmanProblem.Algorithms.Enums.TSPLib.Type;

namespace TravellingSalesmanProblem.Algorithms.Enums
{
    public static class EnumRecognizer
    {
        public static Type RecognizeType(string type)
        {
            return type switch
            {
                "TSP" => Type.TSP,
                "HCP" => Type.HCP,
                "ATSP" => Type.ATSP,
                "SOP" => Type.SOP,
                "CVRP" => Type.CVRP,
                "TOUR" => Type.TOUR,
                _ => throw new Exception(),
            };
        }

        public static EdgeWeightType RecognizeEdgeWeightType(string type)
        {
            return type switch
            {
                "EXPLICIT" => EdgeWeightType.EXPLICIT,
                "EUC_2D" => EdgeWeightType.EUC2D,
                "EUC_3D" => EdgeWeightType.EUC3D,
                "MAX_2D" => EdgeWeightType.MAX2D,
                "MAX_3D" => EdgeWeightType.MAX3D,
                "MAN_2D" => EdgeWeightType.MAN2D,
                "MAN_3D" => EdgeWeightType.MAN3D,
                "CEIL_2D" => EdgeWeightType.CEIL2D,
                "GEO" => EdgeWeightType.GEO,
                "ATT" => EdgeWeightType.ATT,
                "XRAY1" => EdgeWeightType.XRAY1,
                "XRAY2" => EdgeWeightType.XRAY2,
                "SPECIAL" => EdgeWeightType.SPECIAL,
                _ => throw new Exception(),
            };
        }

        public static EdgeWeightFormat RecognizeEdgeWeightFormat(string format)
        {
            return format switch
            {
                "FUNCTION" => EdgeWeightFormat.Function,
                "FULL_MATRIX" => EdgeWeightFormat.FullMatrix,
                "UPPER_ROW" => EdgeWeightFormat.UpperRow,
                "LOWER_ROW" => EdgeWeightFormat.LowerRow,
                "UPPER_DIAG_ROW" => EdgeWeightFormat.UpperDiagRow,
                "LOWER_DIAG_ROW" => EdgeWeightFormat.LowerDiagRow,
                "UPPER_COL" => EdgeWeightFormat.UpperCol,
                "LOWER_COL" => EdgeWeightFormat.LowerCol,
                "UPPER_DIAG_COL" => EdgeWeightFormat.UpperDiagCol,
                "LOWER_DIAG_COL" => EdgeWeightFormat.LowerDiagCol,
                _ => throw new Exception(),
            };
        }

        public static EdgeDataFormat RecognizeEdgeDataFormat(string fromat)
        {
            return fromat switch
            {
                "EDGE_LIST" => EdgeDataFormat.EdgeList,
                "ADJ_LIST" => EdgeDataFormat.AdjList,
                _ => throw new Exception(),
            };
        }

        public static NodeCoordType RecognizeNodeCoordType(string type)
        {
            return type switch
            {
                "TWOD_COORDS" => NodeCoordType.TwoDCoords,
                "THREED_COORDS" => NodeCoordType.ThreeDCoords,
                "NO_COORDS" => NodeCoordType.NoCoords,
                _ => throw new Exception(),
            };
        }

        public static DisplayDataType RecognizeDisplayDataType(string type)
        {
            return type switch
            {
                "COORD_DISPLAY" => DisplayDataType.CoordDisplay,
                "TWOD_DISPLAY" => DisplayDataType.TwoDDisplay,
                "NO_DISPLAY" => DisplayDataType.NoDisplay,
                _ => throw new Exception(),
            };
        }
    }
}