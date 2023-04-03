using SVGTravellingSalesmanProblem.GraphStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class NewSquare
    {
        public SquareOutline outline;

        public SquareSide leftSide;
        public SquareSide rightSide;
        public SquareSide topSide;
        public SquareSide bottomSide;

        public InternalSquare topLeftSquare;
        public InternalSquare topRightSquare;
        public InternalSquare bottomLeftSquare;
        public InternalSquare bottomRightSquare;

        public (SquareSide TopLeft, SquareSide TopRight, SquareSide LeftTop, SquareSide LeftBottom,
            SquareSide RightTop, SquareSide RightBottom, SquareSide BottomLeft, SquareSide BottomRight) externalSides;
        public (SquareSide Left, SquareSide Right, SquareSide Top, SquareSide Bottom) internalSides;
        public int sideLenght = 400;
        public Point origin = new Point(100, 40);
        public double middleX;
        public double middleY;
        int m = 4;
        int r = 4;

        public NewSquare()
        {
            this.middleX = origin.X + sideLenght / 2;
            this.middleY = origin.Y + sideLenght / 2;
            this.externalSides = (new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false), new SquareSide(r, false));
            this.internalSides = (new SquareSide(r, true), new SquareSide(r, true), new SquareSide(r, true), new SquareSide(r, true));
            SetUpInternalSides();
            SetUpOutsidePortals();
            leftSide = new SquareSide(externalSides.LeftTop.portals.Concat(externalSides.LeftBottom.portals).Distinct().ToList(), r, false);
            rightSide = new SquareSide(externalSides.RightTop.portals.Concat(externalSides.RightBottom.portals).Distinct().ToList(), r, false);
            topSide = new SquareSide(externalSides.TopLeft.portals.Concat(externalSides.TopRight.portals).Distinct().ToList(), r, false);
            bottomSide = new SquareSide(externalSides.BottomLeft.portals.Concat(externalSides.BottomRight.portals).Distinct().ToList(), r, false);
            this.topLeftSquare = new InternalSquare(externalSides.LeftTop, internalSides.Top, externalSides.TopLeft, internalSides.Left);
            this.topRightSquare = new InternalSquare(internalSides.Top, externalSides.RightTop, externalSides.TopRight, internalSides.Right);
            this.bottomLeftSquare = new InternalSquare(externalSides.LeftBottom, internalSides.Bottom, internalSides.Left, externalSides.BottomLeft);
            this.bottomRightSquare = new InternalSquare(internalSides.Bottom, externalSides.RightBottom, internalSides.Right, externalSides.BottomRight);

            outline = new SquareOutline(leftSide, rightSide, topSide, bottomSide);
        }

        public NewSquare((SquareSide Left, SquareSide Right, SquareSide Top, SquareSide Bottom) internalSides,
            (SquareSide TopLeft, SquareSide TopRight, SquareSide LeftTop, SquareSide LeftBottom,
            SquareSide RightTop, SquareSide RightBottom, SquareSide BottomLeft, SquareSide BottomRight) externalSides)
        {
            this.middleX = origin.X + sideLenght / 2;
            this.middleY = origin.Y + sideLenght / 2;
            this.externalSides = externalSides;
            this.internalSides = internalSides;
            this.topLeftSquare = new InternalSquare(externalSides.LeftTop, internalSides.Top, externalSides.TopLeft, internalSides.Left);
            this.topRightSquare = new InternalSquare(internalSides.Top, externalSides.RightTop, externalSides.TopRight, internalSides.Right);
            this.bottomLeftSquare = new InternalSquare(externalSides.LeftBottom, internalSides.Bottom, internalSides.Left, externalSides.BottomLeft);
            this.bottomRightSquare = new InternalSquare(internalSides.Bottom, externalSides.RightBottom, internalSides.Right, externalSides.BottomRight);
            leftSide = new SquareSide(externalSides.LeftTop.portals.Concat(externalSides.LeftBottom.portals).Distinct().ToList(), r, false);
            rightSide = new SquareSide(externalSides.RightTop.portals.Concat(externalSides.RightBottom.portals).Distinct().ToList(), r, false);
            topSide = new SquareSide(externalSides.TopLeft.portals.Concat(externalSides.TopRight.portals).Distinct().ToList(), r, false);
            bottomSide = new SquareSide(externalSides.BottomLeft.portals.Concat(externalSides.BottomRight.portals).Distinct().ToList(), r, false);
        }

        public List<InternalSquare> GetInternalSquares() => new List<InternalSquare>() { topLeftSquare, topRightSquare, bottomLeftSquare, bottomLeftSquare };

        public List<SquareSide> GetInternalSides()
            => new List<SquareSide>() { topLeftSquare.bottomSide, topLeftSquare.rightSide, bottomRightSquare.leftSide, bottomRightSquare.topSide };

        /*
        public List<SquareSide> GetInternalSides()
            => new List<SquareSide>()
            {
                topLeftSquare.leftSide,
                topLeftSquare.topSide,
                topRightSquare.topSide,
                topRightSquare.rightSide,
                bottomRightSquare.rightSide,
                bottomRightSquare.bottomSide,
                bottomLeftSquare.leftSide,
                bottomLeftSquare.bottomSide
            };*/


        public List<Portal> GetPortals() => GetInternalSquares().SelectMany((square) => square.GetPortals()).Distinct().ToList();

        public bool AreReachable(Portal firstPortal, Portal secondPortal, InternalSquare excludedSquare) //pokud jsou v 1 ctverci jsou reachable
        {
            if (excludedSquare == topLeftSquare)
                return topRightSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomLeftSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomRightSquare.ContainsPortals(firstPortal, secondPortal);
            else if (excludedSquare == topRightSquare)
                return topLeftSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomLeftSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomRightSquare.ContainsPortals(firstPortal, secondPortal);
            else if (excludedSquare == bottomLeftSquare)
                return topLeftSquare.ContainsPortals(firstPortal, secondPortal)
                    || topRightSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomRightSquare.ContainsPortals(firstPortal, secondPortal);
            else if (excludedSquare == bottomRightSquare)
                return topLeftSquare.ContainsPortals(firstPortal, secondPortal)
                    || topRightSquare.ContainsPortals(firstPortal, secondPortal)
                    || bottomLeftSquare.ContainsPortals(firstPortal, secondPortal);
            return false;
        } 

        public List<Portal> ReachableInternalPortals(Portal portal, InternalSquare excludedSquare)
        {
            if (excludedSquare == topLeftSquare)
                return topRightSquare.ReachableInternalPortals(portal)
                .Concat(bottomLeftSquare.ReachableInternalPortals(portal))
                .Concat(bottomRightSquare.ReachableInternalPortals(portal)).ToList();
            else if (excludedSquare == topRightSquare)
                return topLeftSquare.ReachableInternalPortals(portal)
                .Concat(bottomLeftSquare.ReachableInternalPortals(portal))
                .Concat(bottomRightSquare.ReachableInternalPortals(portal)).ToList();
            else if (excludedSquare == bottomLeftSquare)
                return topLeftSquare.ReachableInternalPortals(portal)
                .Concat(topRightSquare.ReachableInternalPortals(portal))
                .Concat(bottomRightSquare.ReachableInternalPortals(portal)).ToList();
            else if (excludedSquare == bottomRightSquare)
                return topLeftSquare.ReachableInternalPortals(portal)
                .Concat(topRightSquare.ReachableInternalPortals(portal))
                .Concat(bottomLeftSquare.ReachableInternalPortals(portal)).ToList();
            return new List<Portal>();
        }

        public List<List<PortalPairing>> FindOutlinePortalPairings()
        {
            var pairings = new List<List<PortalPairing>>();
            outline.FindOutlinePortalPairings(new List<PortalPairing>(), ref pairings, 0);

            

            return pairings;
        }

        /*
        public List<(List<Node> Path, NewSquare Square)> FindPTour()
        {
            
            
            
            
            internalSides.Left.crosses--;
            FindPath(ref paths, outsidePortals.Find((x) => x.id == 4), internalPortals.Find((x) => x.id == 0), new List<Node>() { outsidePortals.Find((x) => x.id == 0), internalPortals.Find((x) => x.id == 0) });




            DirectoryInfo di = new DirectoryInfo("../../../Squares/TestResults");
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);


            int testIndex = 0;
            foreach (var item in paths)
            {
                var secondPath = new List<(List<Node> Path, NewSquare Square)>();
                item.Square.internalSides.Left.crosses--;
                if (item.Square.internalSides.Left.crosses == 0)
                {
                    Console.WriteLine();
                }
                item.Square.FindPath(ref secondPath, outsidePortals.Find((x) => x.id == 1), item.Square.AllPortals().Find((x) => x.id == 0), new List<Node>() { outsidePortals.Find((x) => x.id == 6), internalPortals.Find((x) => x.id == 0) });
                if (secondPath.Count > 0)
                {
                    DirectoryInfo directory = Directory.CreateDirectory($"../../../Squares/TestResults/Test{testIndex}");
                    for (int i = 0; i < secondPath.Count; i++)
                    {
                        var drawer = new Squares(m, r);
                        drawer.DrawSquareWithPaths(new List<List<Node>> { item.Path, secondPath[i].Path }, $"../../../Squares/TestResults/Test{testIndex}/square{i}.svg");
                    }
                    testIndex++;
                }
            }

            return paths;
        }
        
        public void FindPath(Portal currentPortal, List<PortalPairing> currentPath, Portal outPortal, ref List<(List<PortalPairing> Path, NewSquare Square)> paths)
        {
            if (internalSides.Left.crosses == -1 || internalSides.Right.crosses == -1 || internalSides.Top.crosses == -1 || internalSides.Bottom.crosses == -1)
                return;
            Console.WriteLine("Current path: ");
            foreach (var node in currentPath)
            {
                Console.Write($"{node} -> ");
            }
            Console.WriteLine();

            if (currentPortal.isCenter)
                return;
            if (AreReachable(currentPortal, outPortal))
                paths.Add((currentPath.Append(outPortal).ToList(), this));
            foreach (var reachablePortal in ReachablePortals(currentPortal))
            {
                var rp = ReachablePortals(currentPortal);
                if (reachablePortal.isCenter)
                    continue;
                var updatedSquare = new NewSquare(CrossPortalsSide(reachablePortal), externalSides);
                var newPortal = updatedSquare.AllPortals().Find((x) => x.id == reachablePortal.id);
                updatedSquare.FindPath(ref paths, outPortal, newPortal, currentPath.Append(reachablePortal).ToList());
            }
        }*/


        /*####################################################################################*/


        private List<Portal> SetUpOutsidePortals()
        {
            var portals = new List<Portal>();

            int step = sideLenght / m;
            var previous = new Point(origin.X - step, origin.Y);
            int id = 0;

            while (previous.X < origin.X + sideLenght)
            {
                previous.X += step;

                if (previous.X == origin.X)
                {
                    var portal = new Portal(id, externalSides.TopLeft, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }
                else if (previous.X < middleX)
                {
                    var portal = new Portal(id, externalSides.TopLeft);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                }
                else if (previous.X == middleX)
                {
                    var portal = new Portal(id, externalSides.TopLeft, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.TopLeft.portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }
                else if (previous.X == origin.X + sideLenght)
                {
                    var portal = new Portal(id, externalSides.TopRight, externalSides.RightTop);
                    portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }

                id++;
            }

            while (previous.Y < origin.Y + sideLenght)
            {
                previous.Y += step;

                if (previous.Y == origin.Y)
                {
                    var portal = new Portal(id, externalSides.RightTop, externalSides.TopRight);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                    externalSides.TopRight.portals.Add(portal);
                }
                if (previous.Y < middleY)
                {
                    var portal = new Portal(id, externalSides.RightTop);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                }
                else if (previous.Y == middleY)
                {
                    var portal = new Portal(id, externalSides.RightTop, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightTop.portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                }
                else if (previous.Y == origin.Y + sideLenght)
                {
                    var portal = new Portal(id, externalSides.BottomRight, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, externalSides.RightBottom);
                    portals.Add(portal);
                    externalSides.RightBottom.portals.Add(portal);
                }

                id++;
            }

            while (previous.X > origin.X)
            {
                previous.X -= step;

                if (previous.X > middleX)
                {
                    var portal = new Portal(id, externalSides.BottomRight);
                    portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                }
                else if (previous.X == middleX)
                {
                    var portal = new Portal(id, externalSides.BottomRight, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomRight.portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                }
                else if (previous.X == origin.X)
                {
                    var portal = new Portal(id, externalSides.LeftBottom, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, externalSides.BottomLeft);
                    portals.Add(portal);
                    externalSides.BottomLeft.portals.Add(portal);
                }

                id++;
            }

            while (previous.Y > origin.Y + step)
            {
                previous.Y -= step;

                if (previous.Y > middleY)
                {
                    var portal = new Portal(id, externalSides.LeftBottom);
                    portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                }
                else if (previous.Y == middleY)
                {
                    var portal = new Portal(id, externalSides.LeftBottom, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }
                else if (previous.Y == middleY)
                {
                    var portal = new Portal(id, externalSides.LeftBottom, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                    externalSides.LeftBottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(id, externalSides.LeftTop);
                    portals.Add(portal);
                    externalSides.LeftTop.portals.Add(portal);
                }

                id++;
            }

            return portals;
        }

        private void SetUpInternalSides()
        {
            var portals = new List<Portal>();
            int step = sideLenght / (2 * m);
            var portalPoint = new Point(origin.X + step, origin.Y + sideLenght / 2);
            int portalId = 0;

            while (portalPoint.X < sideLenght + origin.X)
            {
                if (portalPoint.X < middleX)
                {
                    var portal = new Portal(portalId, internalSides.Left);
                    portals.Add(portal);
                    internalSides.Left.portals.Add(portal);
                }
                else if (portalPoint.X == middleX)
                {
                    var portal = new Portal(portalId, internalSides.Left, internalSides.Right, internalSides.Top, internalSides.Bottom);
                    portals.Add(portal);
                    internalSides.Left.portals.Add(portal);
                    internalSides.Right.portals.Add(portal);
                    internalSides.Top.portals.Add(portal);
                    internalSides.Bottom.portals.Add(portal);
                }
                else
                {
                    var portal = new Portal(portalId, internalSides.Right);
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
                    var portal = new Portal(portalId, internalSides.Top);
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
                    var portal = new Portal(portalId, internalSides.Bottom);
                    portals.Add(portal);
                    internalSides.Bottom.portals.Add(portal);
                }

                portalPoint.Y += step;
                portalId++;
            }
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
    }
}
