namespace SVGTravellingSalesmanProblem.PTASStructures
{
    public class SquareOutline
    {
        public SquareSide leftSide;
        public SquareSide rightSide;
        public SquareSide topSide;
        public SquareSide bottomSide;

        public Portal topLeftCorner;
        public Portal bottomLeftCorner;
        public Portal topRightCorner;
        public Portal bottomRightCorner;

        public SquareOutline (SquareSide leftSide, SquareSide rightSide, SquareSide topSide, SquareSide bottomSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            this.topSide = topSide;
            this.bottomSide = bottomSide;
            var leftCorners = leftSide.portals.FindAll((portal) => portal.sides.Count == 2);
            if (leftCorners[0].sides[0] == topSide || leftCorners[0].sides[1] == topSide)
            {
                topLeftCorner = leftCorners[0];
                bottomLeftCorner = leftCorners[1];
            }
            else
            {
                bottomLeftCorner = leftCorners[0];
                topLeftCorner = leftCorners[1];
            }

            var rightCorners = rightSide.portals.FindAll((portal) => portal.sides.Count == 2);
            if (rightCorners[0].sides[0] == topSide || rightCorners[0].sides[1] == topSide)
            {
                topRightCorner = rightCorners[0];
                bottomRightCorner = rightCorners[1];
            }
            else
            {
                bottomRightCorner = rightCorners[0];
                topRightCorner = rightCorners[1];
            }

        }

        public SquareOutline(SquareSide leftSide, SquareSide rightSide, SquareSide topSide, SquareSide bottomSide,
            Portal topLeftCorner, Portal bottomLeftCorner, Portal topRightCorner, Portal bottomRightCorner)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            this.topSide = topSide;
            this.bottomSide = bottomSide;
            this.topLeftCorner = topLeftCorner;
            this.topRightCorner = topRightCorner;
            this.bottomLeftCorner = bottomLeftCorner;
            this.bottomRightCorner = bottomRightCorner;
        }

        public List<Portal> AvailablePortals()
        {
            List<Portal> availablePortals = new List<Portal>();
            if (leftSide.crosses > 0)
                availablePortals.AddRange(leftSide.portals);
            if (rightSide.crosses > 0)
                availablePortals.AddRange(rightSide.portals);
            if (topSide.crosses > 0)
                availablePortals.AddRange(topSide.portals);
            if (bottomSide.crosses > 0)
                availablePortals.AddRange(bottomSide.portals);

            if (topLeftCorner.sides[0].crosses <= 0 || topLeftCorner.sides[1].crosses <= 0)
                availablePortals.Remove(topLeftCorner);
            if (topRightCorner.sides[0].crosses <= 0 || topRightCorner.sides[1].crosses <= 0)
                availablePortals.Remove(topRightCorner);
            if (bottomLeftCorner.sides[0].crosses <= 0 || bottomLeftCorner.sides[1].crosses <= 0)
                availablePortals.Remove(bottomLeftCorner);
            if (bottomRightCorner.sides[0].crosses <= 0 || bottomRightCorner.sides[1].crosses <= 0)
                availablePortals.Remove(bottomRightCorner);

            return availablePortals.Distinct().ToList();
        }

        public void FindOutlinePortalPairings(List<PortalPairing> currentPairings, ref List<List<PortalPairing>> pairings, int i)
        {
            Console.WriteLine(pairings.Count);
            DirectoryInfo directory = Directory.CreateDirectory($"../../../Squares/OutlineTest/");
            var drawer = new Squares(4, 4);
            drawer.DrawSquareWithPairings(currentPairings, $"../../../Squares/OutlineTest/pairing{i}.svg");
            var ap = AvailablePortals();
            foreach (var enterPortal in AvailablePortals())
            {
                var newOutline = new SquareOutline( leftSide.ShallowCopy(), rightSide.ShallowCopy(), topSide.ShallowCopy(), bottomSide.ShallowCopy());
                if (newOutline.leftSide.portals.Contains(enterPortal))
                    newOutline.leftSide.crosses--;
                if (newOutline.rightSide.portals.Contains(enterPortal))
                    newOutline.rightSide.crosses--;
                if (newOutline.topSide.portals.Contains(enterPortal))
                    newOutline.topSide.crosses--;
                if (newOutline.bottomSide.portals.Contains(enterPortal))
                    newOutline.bottomSide.crosses--;
                foreach (var exitPortal in newOutline.AvailablePortals())
                {
                    var nap = newOutline.AvailablePortals();
                    if (newOutline.leftSide.portals.Contains(exitPortal))
                        newOutline.leftSide.crosses--;
                    if (newOutline.rightSide.portals.Contains(exitPortal))
                        newOutline.rightSide.crosses--;
                    if (newOutline.topSide.portals.Contains(exitPortal))
                        newOutline.topSide.crosses--;
                    if (newOutline.bottomSide.portals.Contains(exitPortal))
                        newOutline.bottomSide.crosses--;
                    var newPairing = new PortalPairing(enterPortal, exitPortal);
                    pairings.Add(currentPairings.Append(newPairing).ToList());
                    newOutline.FindOutlinePortalPairings(currentPairings.Append(newPairing).ToList(), ref pairings, i + 1);
                }
            }
        }
    }
}
