﻿using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Svg;
using SVGTravellingSalesmanProblem.GraphStructures;
using SVGTravellingSalesmanProblem.PTASStructures;
using System.Drawing;
using System.Xml.Linq;


namespace SVGTravellingSalesmanProblem
{
    public class Squares
    {
        public GcSvgDocument svgDoc;
        public Point origin = new Point(100, 40);
        public int bigSquareSideLen = 400;
        public int m = 2;
        public int r = 2;

        public Squares()
        {
            svgDoc = new GcSvgDocument();
        }

        public void Trying()
        {
            var nodes = new List<Node>
            {
                new Node(0, 40, 40),
                new Node(1, 60, 250),
                new Node(2, 250, 100),
              //  new Node(3, 300, 70),
              //  new Node(4, 10, 350),
            };

            var portals = GetOutsidePortals();
            portals = portals.Concat(GetInsidePortals()).ToList();
            nodes.Add(portals.First());

            FindPTour(nodes, GetInsidePortals(), GetOutsidePortals());

            DrawSquareWithNodes(portals.ToList<Node>(), "../../../Squares/square2.svg");

        }

        private List<Node> FindPTour(List<Node> nodes, List<NoSidePortal> insidePortals, List<NoSidePortal> outsidePortals)
        {
            List<List<Node>> paths = new List<List<Node>>();

            NoSidePortal inPortal = outsidePortals.First();
            NoSidePortal outPortal = outsidePortals.Where((x) => x.id == 4).First();

            (int Crosses, List <NoSidePortal> portals) sideA = (r, insidePortals.Where((x) => x.id < 2).ToList());
            (int Crosses, List<NoSidePortal> portals) sideB = (r, insidePortals.Where((x) => x.id >= 1 && x.id < 3).ToList());
            (int Crosses, List<NoSidePortal> portals) sideC = (r, insidePortals.Where((x) => (x.id >= 3 && x.id < 4) || x.id == m-1).ToList());
            (int Crosses, List<NoSidePortal> portals) sideD = (r, insidePortals.Where((x) => (x.id >= 4 && x.id < 5) || x.id == m - 1).ToList());



            for (int i = 0; i < paths.Count; i++)
                DrawSquareWithPath(paths[i], $"../../../Squares/square{i}.svg");
            
            return null;
        }

        private List<NoSidePortal> GetOutsidePortals()
        {
            var portals = new List<NoSidePortal>();
            portals.Add(new NoSidePortal(0, origin.X, origin.Y));

            int step = bigSquareSideLen / m;
            var previous = new NoSidePortal(0, origin.X, origin.Y);
            int id = 1;

            while (previous.x < origin.X + bigSquareSideLen)
            {
                portals.Add(new NoSidePortal(id, previous.x + step, previous.y));
                id++;
                previous.x += step;
            }

            while (previous.y < origin.Y + bigSquareSideLen)
            {
                portals.Add(new NoSidePortal(id, previous.x, previous.y + step));
                id++;
                previous.y += step;
            }

            while (previous.x > origin.X)
            {
                portals.Add(new NoSidePortal(id, previous.x - step, previous.y));
                id++;
                previous.x -= step;
            }

            while (previous.y > origin.Y + step)
            {
                portals.Add(new NoSidePortal(id, previous.x, previous.y - step));
                id++;
                previous.y -= step;
            }

            return portals;
        }

        private List<NoSidePortal> GetInsidePortals()
        {
            var portals = new List<NoSidePortal>();
            int step = bigSquareSideLen / (2 * m);
            var portalPoint = new Point(origin.X + step, origin.Y + bigSquareSideLen / 2);
            int portalId = 0;

            while (portalPoint.X < bigSquareSideLen + origin.X)
            {
                portals.Add(new NoSidePortal(portalId, portalPoint.X, portalPoint.Y));
                portalPoint.X += step;
                portalId++;
            }

            portalPoint = new Point(origin.X + bigSquareSideLen / 2, origin.Y + step);

            int skippedOne = (int)Math.Floor((double)(portalId / 2)) + portalId;

            while (portalPoint.Y < bigSquareSideLen + origin.Y)
            {
                if (portalId != skippedOne)
                    portals.Add(new NoSidePortal(portalId, portalPoint.X, portalPoint.Y));
                else
                {
                    portalId--;
                    skippedOne--;
                }
                portalPoint.Y += step;
                portalId++;
            }

            return portals;
        }

        public void DrawSquareWithNodes(List<Node> nodes, string filePath, bool absoluteCoordinates = false)
        {
            svgDoc.RootSvg.Width = new SvgLength(1000, SvgLengthUnits.Pixels);
            svgDoc.RootSvg.Height = new SvgLength(800, SvgLengthUnits.Pixels);

            DrawOutlineSquares();
            DrawGrid(40);
            DrawPortals();

            if(absoluteCoordinates)
                foreach (var node in nodes)
                    DrawNode(node);
            else
                foreach (var node in nodes)
                    DrawNode(ToRelativePoint(node));


            SvgViewBox view = new SvgViewBox();
            view.MinX = 0;
            view.MinY = 0;
            view.Width = 700;
            view.Height = 500;

            svgDoc.RootSvg.ViewBox = view;


            svgDoc.Save(filePath);
        }

        public void DrawSquareWithPath(List<Node> path, string filePath)
        {
            svgDoc.RootSvg.Width = new SvgLength(1000, SvgLengthUnits.Pixels);
            svgDoc.RootSvg.Height = new SvgLength(800, SvgLengthUnits.Pixels);

            DrawOutlineSquares();
            DrawGrid(40);
            DrawPortals();
            DrawPath(path);

            SvgViewBox view = new SvgViewBox();
            view.MinX = 0;
            view.MinY = 0;
            view.Width = 500;
            view.Height = 500;

            svgDoc.RootSvg.ViewBox = view;


            svgDoc.Save(filePath);
        }

        private Node ToRelativePoint(Node node) => new Node(node.id, node.x + origin.X, bigSquareSideLen + origin.Y - node.y);

        private void DrawNode(Node node)
        {
            var nodeElement = new SvgEllipseElement()
            {
                CenterX = new SvgLength((float)node.x, SvgLengthUnits.Pixels),
                CenterY = new SvgLength((float)node.y, SvgLengthUnits.Pixels),
                RadiusX = new SvgLength(5, SvgLengthUnits.Pixels),
                RadiusY = new SvgLength(5, SvgLengthUnits.Pixels),
                Fill = new SvgPaint(Color.Black),
                Stroke = new SvgPaint(Color.Black)
            };

            var tag = new SvgTextElement()
            {
                X = new List<SvgLength> { new SvgLength((float)node.x + 5, SvgLengthUnits.Pixels) },
                Y = new List<SvgLength> { new SvgLength((float)node.y, SvgLengthUnits.Pixels) },
                FontSize = new SvgLength(25, SvgLengthUnits.Pixels),
            };
            var tagc = new SvgContentElement()
            {
                Content = $"{node.id}",
            };

            tag.Children.Add(tagc);
            svgDoc.RootSvg.Children.Add(tag);

            svgDoc.RootSvg.Children.Add(nodeElement);
        }

        private void DrawPath(List<Node> path)
        {
            foreach (var node in path)
                DrawNode(node);

            var relativePath = path.Select((x) => ToRelativePoint(x)).ToArray();
            for (int i = 0; i < relativePath.Length - 1; i++)
            {
                var line = new SvgLineElement()
                {
                    X1 = new SvgLength((float)relativePath[i].x, SvgLengthUnits.Pixels),
                    Y1 = new SvgLength((float)relativePath[i].y, SvgLengthUnits.Pixels),
                    X2 = new SvgLength((float)relativePath[i+1].x, SvgLengthUnits.Pixels),
                    Y2 = new SvgLength((float)relativePath[i+1].y, SvgLengthUnits.Pixels),
                    Stroke = new SvgPaint(Color.Blue),
                    StrokeWidth = new SvgLength(1, SvgLengthUnits.Pixels),
                };

                svgDoc.RootSvg.Children.Add(line);
            }
        }

        private void DrawOutlineSquares()
        {
            var rect = new SvgRectElement()
            {
                X = new SvgLength(origin.X, SvgLengthUnits.Pixels),
                Y = new SvgLength(origin.Y, SvgLengthUnits.Pixels),
                Height = new SvgLength(bigSquareSideLen, SvgLengthUnits.Pixels),
                Width = new SvgLength(bigSquareSideLen, SvgLengthUnits.Pixels),
                Stroke = new SvgPaint(Color.Black),
                StrokeWidth = new SvgLength(5, SvgLengthUnits.Pixels),
                Fill = new SvgPaint(Color.White)
            };

            var line1 = new SvgLineElement()
            {
                X1 = new SvgLength(origin.X + bigSquareSideLen / 2, SvgLengthUnits.Pixels),
                Y1 = new SvgLength(origin.Y, SvgLengthUnits.Pixels),
                X2 = new SvgLength(origin.X + bigSquareSideLen / 2, SvgLengthUnits.Pixels),
                Y2 = new SvgLength(origin.Y + bigSquareSideLen, SvgLengthUnits.Pixels),
                Stroke = new SvgPaint(Color.Gray),
                StrokeWidth = new SvgLength(2, SvgLengthUnits.Pixels),
            };

            var line2 = new SvgLineElement()
            {
                X1 = new SvgLength(origin.X, SvgLengthUnits.Pixels),
                Y1 = new SvgLength(origin.Y + bigSquareSideLen / 2, SvgLengthUnits.Pixels),
                X2 = new SvgLength(origin.X + bigSquareSideLen, SvgLengthUnits.Pixels),
                Y2 = new SvgLength(origin.Y + bigSquareSideLen / 2, SvgLengthUnits.Pixels),
                Stroke = new SvgPaint(Color.Gray),
                StrokeWidth = new SvgLength(2, SvgLengthUnits.Pixels),
            };

            svgDoc.RootSvg.Children.Add(rect);
            svgDoc.RootSvg.Children.Add(line1);
            svgDoc.RootSvg.Children.Add(line2);
        }

        private void DrawPortals()
        {
            DrawHorizontalPortals(new Point(origin.X, origin.Y + bigSquareSideLen / 2), bigSquareSideLen / (2 * m), 7, Color.Firebrick);
            DrawVerticalPortals(new Point(origin.X + bigSquareSideLen / 2, origin.Y), bigSquareSideLen / (2 * m), 7, Color.Firebrick);

            DrawHorizontalPortals(origin, bigSquareSideLen / m, 10, Color.ForestGreen);
            DrawHorizontalPortals(new Point(origin.X, origin.Y + bigSquareSideLen), bigSquareSideLen / m, 10, Color.ForestGreen);
            DrawVerticalPortals(origin, bigSquareSideLen / m, 10, Color.ForestGreen);
            DrawVerticalPortals(new Point(origin.X + bigSquareSideLen, origin.Y), bigSquareSideLen / m, 10, Color.ForestGreen);
        }

        private void DrawHorizontalPortals(Point startPoint, int step, int pixelSize, Color color)
        {
            var portalPoint = startPoint;
            while (portalPoint.X <= bigSquareSideLen + origin.X)
            {
                var portal = new SvgEllipseElement()
                {
                    CenterX = new SvgLength(portalPoint.X, SvgLengthUnits.Pixels),
                    CenterY = new SvgLength(portalPoint.Y, SvgLengthUnits.Pixels),
                    RadiusX = new SvgLength(pixelSize, SvgLengthUnits.Pixels),
                    RadiusY = new SvgLength(pixelSize, SvgLengthUnits.Pixels),
                    Fill = new SvgPaint(color),
                    Stroke = new SvgPaint(color)
                };

                svgDoc.RootSvg.Children.Add(portal);
                portalPoint.X += step;
            }
        }

        private void DrawVerticalPortals(Point startPoint, int step, int pixelSize, Color color)
        {
            var portalPoint = startPoint;
            while (portalPoint.Y <= bigSquareSideLen + origin.X)
            {
                var portal = new SvgEllipseElement()
                {
                    CenterX = new SvgLength(portalPoint.X, SvgLengthUnits.Pixels),
                    CenterY = new SvgLength(portalPoint.Y, SvgLengthUnits.Pixels),
                    RadiusX = new SvgLength(pixelSize, SvgLengthUnits.Pixels),
                    RadiusY = new SvgLength(pixelSize, SvgLengthUnits.Pixels),
                    Fill = new SvgPaint(color),
                    Stroke = new SvgPaint(color)
                };

                svgDoc.RootSvg.Children.Add(portal);
                portalPoint.Y += step;
            }
        }

        private void DrawGrid(int step)
        {
            var gridPoint = new Point(origin.X, origin.Y + bigSquareSideLen + 20);
            int x = 0;
            while (gridPoint.X <= bigSquareSideLen + origin.X)
            {
                var tag = new SvgTextElement()
                {
                    X = new List<SvgLength> { new SvgLength(gridPoint.X - 5, SvgLengthUnits.Pixels) },
                    Y = new List<SvgLength> { new SvgLength(gridPoint.Y, SvgLengthUnits.Pixels) },
                    FontSize = new SvgLength(10, SvgLengthUnits.Pixels),
                };
                var tagc = new SvgContentElement()
                {
                    Content = $"{x}",
                };

                tag.Children.Add(tagc);
                svgDoc.RootSvg.Children.Add(tag);

                gridPoint.X += step;
                x += step;
            }

            gridPoint = new Point(20, origin.Y + bigSquareSideLen);
            int y = 0;
            while (gridPoint.Y >= origin.Y)
            {
                var tag = new SvgTextElement()
                {
                    X = new List<SvgLength> { new SvgLength(gridPoint.X - 10, SvgLengthUnits.Pixels) },
                    Y = new List<SvgLength> { new SvgLength(gridPoint.Y + 5, SvgLengthUnits.Pixels) },
                    FontSize = new SvgLength(10, SvgLengthUnits.Pixels),
                };
                var tagc = new SvgContentElement()
                {
                    Content = $"{y}",
                };

                tag.Children.Add(tagc);
                svgDoc.RootSvg.Children.Add(tag);

                gridPoint.Y -= step;
                y += step;
            }
        }
    }
}