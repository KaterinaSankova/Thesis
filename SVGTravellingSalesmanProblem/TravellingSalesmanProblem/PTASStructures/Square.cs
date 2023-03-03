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

        public Square((SquareSide Left, SquareSide Right, SquareSide Top, SquareSide Bottom) internalSides,
            (SquareSide TopLeft, SquareSide TopRight, SquareSide LeftTop, SquareSide LeftBottom,
            SquareSide RightTop, SquareSide RightBottom, SquareSide BottomLeft, SquareSide BottomRight) externalSides)
        {
            this.middleX = origin.X + sideLenght / 2;
            this.middleY = origin.Y + sideLenght / 2;
            this.externalSides = externalSides;
            this.internalSides = internalSides;
        }

        public bool Check(SquareSide newSide)
        {
            bool works = false;
            foreach (var p in newSide.portals)
            {
                if (p.isCenter) { }
                else if (p.hasTwoSides)
                {
                    works = works || (p.side == newSide || p.secondSide == newSide);
                }
                else
                {
                    works = works || p.side == newSide;
                }
            }
            Console.WriteLine($"{works}");
            return works;
        }

        public List<List<Node>> FindPTour(List<Node> inputNodes)
        {
            List<List<Node>> paths = new List<List<Node>>();
            var draw = new Squares(m, r);
            var nodes = inputNodes.Select((x) => draw.ToRelativePoint(x)).ToList();

            var internalPortals = GetInsidePortals();
            var outsidePortals = GetOutsidePortals();

            FindPath(ref paths, outsidePortals.Find((x) => x.id == 4), internalPortals.Find((x) => x.id == 0), new List<Node>() { outsidePortals.Find((x)=> x.id == 0)});

            DirectoryInfo di = new DirectoryInfo("../../../Squares/TestResults");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            for (int i = 0; i < paths.Count; i++)
                draw.DrawSquareWithPath(paths[i], $"../../../Squares/TestResults/square{i}.svg");

            return paths;
        }

        public List<Portal> AllPortals()
        {
            return internalSides.Left.portals.Concat(internalSides.Right.portals.Concat(internalSides.Top.portals.Concat(internalSides.Bottom.portals))).ToList();
        }

        public void FindPath(ref List<List<Node>> paths, Portal outPortal, Portal currentPortal, List<Node> currentPath)
        {
            Console.WriteLine("Current path: ");
            foreach (var node in currentPath)
            {
                Console.Write($"{node} -> ");
            }
            Console.WriteLine();

            if (currentPortal.isCenter)
                return;
            if (AreReachable(currentPortal, outPortal))
                paths.Add(currentPath.Append(outPortal).ToList());
            foreach (var reachablePortal in ReachablePortals(currentPortal))
            {
                if (reachablePortal.isCenter)
                    continue;
                var updatedSquare = new Square(CrossPortalsSide(reachablePortal), externalSides);
                var newPortal = updatedSquare.AllPortals().Find((x) => x.id == reachablePortal.id);
                updatedSquare.FindPath(ref paths, outPortal, newPortal, currentPath.Append(reachablePortal).ToList());
            }
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

                if (previous.x == origin.X)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopLeft, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }
                else if (previous.x < middleX)
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
                else if (previous.x == origin.X + sideLenght)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopRight, externalSides.RightTop);
                    portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }

                id++;
            }

            while (previous.y < origin.Y + sideLenght)
            {
                previous.y += step;

                if (previous.y == origin.Y)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.RightTop, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }
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
                else if (previous.y == origin.Y + sideLenght)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.BottomRight, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                }

                id++;
            }

            while (previous.x > origin.X)
            {
                previous.x -= step;

                if (previous.x > middleX)
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
                else if (previous.x == origin.X)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.LeftBottom, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                }

                id++;
            }

            while (previous.y > origin.Y + step)
            {
                previous.y -= step;

                if (previous.y > middleY)
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
                else if (previous.y == middleY)
                {
                    var portal = new Portal(id, previous.x, previous.y, externalSides.LeftBottom, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
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
            List <SquareSide> reachableSides = new List <SquareSide>();
            if (portal.isCenter)
            {
                reachableSides.Add(internalSides.Top);
                reachableSides.Add(internalSides.Left);
                reachableSides.Add(internalSides.Right);
                reachableSides.Add(internalSides.Bottom);
            }
            else
            {
                var perpendicularSides = GetPerpendicularInternalSides(portal.side);
                reachableSides = new List<SquareSide>() { portal.side, perpendicularSides.Side1, perpendicularSides.Side2 };
            }

            var reachablePorals = reachableSides.Where((side) => side.crosses > 0).SelectMany((side) => side.portals).Distinct().ToList();

            return reachablePorals;
        }

        public SquareSide CrossSide(SquareSide side)
        {
            var newSide = side.Copy();
            newSide.crosses--;
            return newSide;
        }

        public (SquareSide Left, SquareSide Right, SquareSide Top, SquareSide Bottom) CrossPortalsSide(Portal portal)
        {
            if (portal.side == internalSides.Left)
                return (CrossSide(internalSides.Left),
                    internalSides.Right.Copy(),
                    internalSides.Top.Copy(),
                    internalSides.Bottom.Copy());
            else if (portal.side == internalSides.Right)
                return (internalSides.Left.Copy(),
                    CrossSide(internalSides.Right),
                    internalSides.Top.Copy(),
                    internalSides.Bottom.Copy());
            else if (portal.side == internalSides.Top)
                return (internalSides.Left.Copy(),
                    internalSides.Right.Copy(),
                    CrossSide(internalSides.Top),
                    internalSides.Bottom.Copy());
            else if (portal.side == internalSides.Bottom)
                return (internalSides.Left.Copy(),
                    internalSides.Right.Copy(),
                    internalSides.Top.Copy(),
                    CrossSide(internalSides.Bottom));
            else
                throw new Exception("");
        }

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

        public bool AreReachable(Portal internalPortal, Portal outsidePortal)
        { 
            if(internalPortal.isCenter)
            {
                if (outsidePortal.hasTwoSides)
                    return FormSquare(internalSides.Top, outsidePortal.side)
                        || FormSquare(internalSides.Left, outsidePortal.side)
                        || FormSquare(internalSides.Right, outsidePortal.side)
                        || FormSquare(internalSides.Bottom, outsidePortal.side)
                        || FormSquare(internalSides.Top, outsidePortal.secondSide)
                        || FormSquare(internalSides.Left, outsidePortal.secondSide)
                        || FormSquare(internalSides.Right, outsidePortal.secondSide)
                        || FormSquare(internalSides.Bottom, outsidePortal.secondSide);
                else
                    return FormSquare(internalSides.Top, outsidePortal.side)
                        || FormSquare(internalSides.Left, outsidePortal.side)
                        || FormSquare(internalSides.Right, outsidePortal.side)
                        || FormSquare(internalSides.Bottom, outsidePortal.side);
            }
            if (outsidePortal.hasTwoSides)
                return FormSquare(internalPortal.side, outsidePortal.side) || FormSquare(internalPortal.side, outsidePortal.secondSide);
            else
                return FormSquare(internalPortal.side, outsidePortal.side);
        }
    }
}
