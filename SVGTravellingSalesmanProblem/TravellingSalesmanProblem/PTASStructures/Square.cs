using SVGTravellingSalesmanProblem.GraphStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class Square
    {
        public (SquareSide TopLeft, SquareSide TopRight, SquareSide LeftTop, SquareSide LeftBottom,
            SquareSide RightTop, SquareSide RightBottom, SquareSide BottomLeft, SquareSide BottomRight) externalSides;
        public (SquareSide Left, SquareSide Right, SquareSide Top, SquareSide Bottom) internalSides;
        public int sideLenght = 400;
        public Point origin = new Point(100, 40);
        public double middleX;
        public double middleY;
        int m = 2;
        int r = 2;

        public Square()
        {
            this.middleX = origin.X + sideLenght / 2;
            this.middleY = origin.Y + sideLenght / 2;
            this.externalSides = (new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r));
            this.internalSides = (new SquareSide(r), new SquareSide(r), new SquareSide(r), new SquareSide(r));
        }

        public List<List<Node>> FindPTour()
        {
            List<List<Node>> paths = new List<List<Node>>();
            /*
            NoSidePortal inPortal = outsidePortals.First();
            NoSidePortal outPortal = outsidePortals.Where((x) => x.id == 4).First();

            (int Crosses, List<NoSidePortal> portals) sideA = (r, insidePortals.Where((x) => x.id < 2).ToList());
            (int Crosses, List<NoSidePortal> portals) sideB = (r, insidePortals.Where((x) => x.id >= 1 && x.id < 3).ToList());
            (int Crosses, List<NoSidePortal> portals) sideC = (r, insidePortals.Where((x) => (x.id >= 3 && x.id < 4) || x.id == m - 1).ToList());
            (int Crosses, List<NoSidePortal> portals) sideD = (r, insidePortals.Where((x) => (x.id >= 4 && x.id < 5) || x.id == m - 1).ToList());
            */
            var a = GetOutsidePortals();
            var b = GetInsidePortals();

            Console.Write($"\t");
            foreach (var p in externalSides.TopLeft.portals)
                Console.Write($"{p} ");

            Console.Write($"\t");

            foreach (var p in externalSides.TopRight.portals)
                Console.Write($"{p} ");

            Console.Write($"\n");

            foreach (var p in externalSides.LeftTop.portals)
                Console.Write($"{p} ");

            Console.Write($"\t\t\t");
            foreach (var p in externalSides.RightTop.portals)
                Console.Write($"{p} ");
            Console.Write($"\n");

            foreach (var p in externalSides.LeftBottom.portals)
                Console.Write($"{p} ");

            Console.Write($"\t\t\t");
            foreach (var p in externalSides.RightBottom.portals)
                Console.Write($"{p} ");
            Console.Write($"\n");

            Console.Write($"\t");
            foreach (var p in externalSides.BottomLeft.portals)
                Console.Write($"{p} ");

            Console.Write($"\t");

            foreach (var p in externalSides.BottomRight.portals)
                Console.Write($"{p} ");

            Console.Write($"\n");

            return paths;
        }

        private List<Portal> GetOutsidePortals()
        {
            var portals = new List<Portal>();
           // portals.Add(new Portal(0, origin.X, origin.Y, externalSides.TopLeft, externalSides.LeftTop));

            int step = sideLenght / m;
            var previous = new Portal(0, origin.X - step, origin.Y, externalSides.TopLeft, externalSides.LeftTop); //stačí point
            int id = 0;

            while (previous.x < origin.X + sideLenght)
            {
                previous.x += step;

                if (previous.x < middleX)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopLeft);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                }                    
                else if(previous.x == middleX)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopLeft, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }

                id++;
            }

            previous.y -= step;

            while (previous.y < origin.Y + sideLenght)
            {
                previous.y += step;

                if (previous.y < middleY)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.RightTop);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                }
                else if (previous.y == middleY)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.RightTop, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                }

                id++;
            }


            previous.x += step;

            while (previous.x > origin.X)
            {
                previous.x -= step;

                if (previous.x < middleX)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.BottomRight);
                    portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                }
                else if (previous.x == middleX)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.BottomRight, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                }

                id++;
            }

            previous.y += step;
            while (previous.y > origin.Y + step)
            {
                previous.y -= step;

                if (previous.y < middleY)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.LeftBottom);
                    portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                }
                else if (previous.y == middleY)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.LeftBottom, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }

                id++;
            }

            return portals;
        }

        private List<Portal> GetInsidePortals()
        {
            var portals = new List<Portal>();
            int step = sideLenght / (2 * m);
            var portalPoint = new Point(origin.X + step, origin.Y + sideLenght / 2);
            int portalId = 0;

            while (portalPoint.X < sideLenght + origin.X)
            {
                if (portalPoint.X < middleX)
                {
                    var portal = new Portal(portalId, portalPoint.X, portalPoint.Y, internalSides.Left);
                    portals.Add(portal);
                    internalSides.Left.portals.Add(portal);
                }
                else if (portalPoint.X == middleX)
                {
                    var portal = new Portal(portalId, portalPoint.X, portalPoint.Y, true);
                    portals.Add(portal);
                    internalSides.Left.portals.Add(portal);
                    internalSides.Right.portals.Add(portal);
                    internalSides.Top.portals.Add(portal);
                    internalSides.Bottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(portalId, portalPoint.X, portalPoint.Y, internalSides.Right);
                    portals.Add(portal);
                    internalSides.Right.portals.Add(portal);
                }

                portalPoint.X += step;
                portalId++;
            }

            portalPoint = new Point(origin.X + sideLenght / 2, origin.Y + step);

            while (portalPoint.Y < sideLenght + origin.Y)
            {
                if (portalPoint.Y < middleY)
                {
                    var portal = new Portal(portalId, portalPoint.X, portalPoint.Y, internalSides.Top);
                    portals.Add(portal);
                    internalSides.Top.portals.Add(portal);
                }
                else if (portalPoint.Y == middleY)
                {
                    portalPoint.Y += step;
                    continue;
                }
                else
                {
                    var portal = new Portal(portalId, portalPoint.X, portalPoint.Y, internalSides.Bottom);
                    portals.Add(portal);
                    internalSides.Bottom.portals.Add(portal);
                }              

                portalPoint.Y += step;
                portalId++;
            }

            return portals;
        }

        public (SquareSide Side1, SquareSide Side2) GetPerpendicularInternalSides(SquareSide side)
        {
            if (side == internalSides.Left)
                return (internalSides.Top, internalSides.Bottom);
            else if (side == internalSides.Right)
                return (internalSides.Top, internalSides.Bottom);
            else if (side == internalSides.Top)
                return (internalSides.Left, internalSides.Right);
            else if (side == internalSides.Bottom)
                return (internalSides.Left, internalSides.Right);
            else
                throw new Exception("Given side is not an internal side of this square"); //or throw exception
        }

        public List<Portal> ReachablePortals(Portal portal)
        {
            var perpendicularSides = GetPerpendicularInternalSides(portal.side);
            var reachableSides = new List<SquareSide>() { portal.side, perpendicularSides.Side1, perpendicularSides.Side2 };
            var reachablePorals = reachableSides.Where((side) => side.crosses > 0).SelectMany((side) => side.portals).ToList();

            return reachablePorals;
        }

        public SquareSide CrossSide(SquareSide side) => new SquareSide(side.portals, side.crosses - 1);

        public bool FormSquare(SquareSide internalSide, SquareSide externalSide)
        {
            if (internalSide == internalSides.Left &&
                (externalSide == externalSides.TopLeft || externalSide == externalSides.LeftTop ||
                externalSide == externalSides.LeftBottom || externalSide == externalSides.BottomLeft))
                return true;
            else if (internalSide == internalSides.Right &&
                (externalSide == externalSides.TopRight || externalSide == externalSides.RightTop ||
                externalSide == externalSides.RightBottom || externalSide == externalSides.BottomRight))
                return true;
            else if (internalSide == internalSides.Top &&
                (externalSide == externalSides.TopLeft || externalSide == externalSides.LeftTop ||
                externalSide == externalSides.TopRight || externalSide == externalSides.RightTop))
                return true;
            else if (internalSide == internalSides.Bottom &&
                (externalSide == externalSides.RightBottom || externalSide == externalSides.BottomRight ||
                externalSide == externalSides.LeftBottom || externalSide == externalSides.BottomLeft))
                return true;
            else
                return false;
        }

        public bool AreReachable(Portal firstPortal, Portal secondPortal) => FormSquare(firstPortal.side, secondPortal.side);
    }
}
