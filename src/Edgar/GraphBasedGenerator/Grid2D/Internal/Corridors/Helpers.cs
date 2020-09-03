using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal.Corridors
{
    public static class Helpers
    {
        public static PolygonGrid2D GetPolygonFromTiles(HashSet<Vector2Int> allPoints)
        {
            if (allPoints.Count == 0)
            {
                throw new ArgumentException("There must be at least one point");
            }

            var orderedDirections = new Dictionary<Vector2Int, List<Vector2Int>>
            {
                {Vector2IntHelper.Top, new List<Vector2Int> {Vector2IntHelper.Left, Vector2IntHelper.Top, Vector2IntHelper.Right}},
                {Vector2IntHelper.Right, new List<Vector2Int> {Vector2IntHelper.Top, Vector2IntHelper.Right, Vector2IntHelper.Bottom}},
                {Vector2IntHelper.Bottom, new List<Vector2Int> {Vector2IntHelper.Right, Vector2IntHelper.Bottom, Vector2IntHelper.Left}},
                {Vector2IntHelper.Left, new List<Vector2Int> {Vector2IntHelper.Bottom, Vector2IntHelper.Left, Vector2IntHelper.Top}}
            };

            var allPointsInternal = allPoints;
            var smallestX = allPointsInternal.Min(x => x.X);
            var smallestXPoints = allPointsInternal.Where(x => x.X == smallestX).ToList();
            var smallestXYPoint = smallestXPoints[smallestXPoints.MinBy(x => x.Y)];

            var startingPoint = smallestXYPoint;
            var startingDirection = Vector2IntHelper.Top;

            var polygonPoints = new List<Vector2Int>();
            var currentPoint = startingPoint + startingDirection;
            var firstPoint = currentPoint;
            var previousDirection = startingDirection;
            var first = true;

            if (!allPointsInternal.Contains(currentPoint))
            {
                throw new ArgumentException("Invalid room shape.");
            }

            while (true)
            {
                var foundNeighbor = false;
                var currentDirection = new Vector2Int();

                foreach (var directionVector in orderedDirections[previousDirection])
                {
                    var newPoint = currentPoint + directionVector;

                    if (allPointsInternal.Contains(newPoint))
                    {
                        currentDirection = directionVector;
                        foundNeighbor = true;
                        break;
                    }
                }

                if (!foundNeighbor)
                    throw new ArgumentException("Invalid room shape.");

                if (currentDirection != previousDirection)
                {
                    polygonPoints.Add(currentPoint);
                }

                currentPoint += currentDirection;
                previousDirection = currentDirection;

                if (first)
                {
                    first = false;
                }
                else if (currentPoint == firstPoint)
                {
                    break;
                }
            }

            if (!IsClockwiseOriented(polygonPoints))
            {
                polygonPoints.Reverse();
            }

            return new PolygonGrid2D(polygonPoints);
        }

        public static bool IsClockwiseOriented(IList<Vector2Int> points)
        {
            var previous = points[points.Count - 1];
            var sum = 0L;

            foreach (var point in points)
            {
                sum += (point.X - previous.X) * (long) (point.Y + previous.Y);
                previous = point;
            }

            return sum > 0;
        }
    }
}