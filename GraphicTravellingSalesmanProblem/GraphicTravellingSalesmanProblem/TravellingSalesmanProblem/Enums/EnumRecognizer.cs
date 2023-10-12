using System;
using TravellingSalesmanProblem.Enums.TSPLib;
using Type = TravellingSalesmanProblem.Enums.TSPLib.Type;

namespace TravellingSalesmanProblem.Enums
{
    public class EnumRecognizer
    {
        public Type RecognizeType(string type)
        {
            switch (type)
            {
                case "TSP":
                    return Type.TSP;
                case "HCP":
                    return Type.HCP;
                case "ATSP":
                    return Type.ATSP;
                case "SOP":
                    return Type.SOP;
                case "CVRP":
                    return Type.CVRP;
                case "TOUR":
                    return Type.TOUR;
                default:
                    throw new Exception();//idk
            }
        }

        public EdgeWeightType RecognizeEdgeWeightType(string type)
        {
            switch (type)
            {
                case "EXPLICIT":
                    return EdgeWeightType.EXPLICIT;
                case "EUC_2D":
                    return EdgeWeightType.EUC2D;
                case "EUC_3D":
                    return EdgeWeightType.EUC3D;
                case "MAX_2D":
                    return EdgeWeightType.MAX2D;
                case "MAX_3D":
                    return EdgeWeightType.MAX3D;
                case "MAN_2D":
                    return EdgeWeightType.MAN2D;
                case "MAN_3D":
                    return EdgeWeightType.MAN3D;
                case "CEIL_2D":
                    return EdgeWeightType.CEIL2D;
                case "GEO":
                    return EdgeWeightType.GEO;
                case "ATT":
                    return EdgeWeightType.ATT;
                case "XRAY1":
                    return EdgeWeightType.XRAY1;
                case "XRAY2":
                    return EdgeWeightType.XRAY2;
                case "SPECIAL":
                    return EdgeWeightType.SPECIAL;
                default:
                    throw new Exception();//idk
            }    
        }

        public EdgeWeightFormat RecognizeEdgeWeightFormat(string format)
        {
            switch (format)
            {
                case "FUNCTION":
                    return EdgeWeightFormat.Function;
                case "FULL_MATRIX":
                    return EdgeWeightFormat.FullMatrix;
                case "UPPER_ROW":
                    return EdgeWeightFormat.UpperRow;
                case "LOWER_ROW":
                    return EdgeWeightFormat.LowerRow;
                case "UPPER_DIAG_ROW":
                    return EdgeWeightFormat.UpperDiagRow;
                case "LOWER_DIAG_ROW":
                    return EdgeWeightFormat.LowerDiagRow;
                case "UPPER_COL":
                    return EdgeWeightFormat.UpperCol;
                case "LOWER_COL":
                    return EdgeWeightFormat.LowerCol;
                case "UPPER_DIAG_COL":
                    return EdgeWeightFormat.UpperDiagCol;
                case "LOWER_DIAG_COL":
                    return EdgeWeightFormat.LowerDiagCol;
                default:
                    throw new Exception();//idk
            }
        }

        public EdgeDataFormat RecognizeEdgeDataFormat(string fromat)
        {
            switch (fromat)
            {
                case "EDGE_LIST":
                    return EdgeDataFormat.EdgeList;
                case "ADJ_LIST":
                    return EdgeDataFormat.AdjList;
                default:
                    throw new Exception();//idk
            }
        }

        public NodeCoordType RecognizeNodeCoordType(string type)
        {
            switch (type)
            {
                case "TWOD_COORDS":
                    return NodeCoordType.TwoDCoords;
                case "THREED_COORDS":
                    return NodeCoordType.ThreeDCoords;
                case "NO_COORDS":
                    return NodeCoordType.NoCoords;
                default:
                    throw new Exception();//idk
            }
        }

        public DisplayDataType RecognizeDisplayDataType(string type)
        {
            switch (type)
            {
                case "COORD_DISPLAY":
                    return DisplayDataType.CoordDisplay;
                case "TWOD_DISPLAY":
                    return DisplayDataType.TwoDDisplay;
                case "NO_DISPLAY":
                    return DisplayDataType.NoDisplay;
                default:
                    throw new Exception();//idk
            }
        }
    }
}