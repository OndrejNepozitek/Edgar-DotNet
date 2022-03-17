using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
    /// <summary>
    /// Base class for implementing the <see cref="IPolygonOverlap{TShape}"/> interface.
    /// </summary>
    /// <typeparam name="TShape"></typeparam>
    public abstract class PolygonOverlapBase<TShape> : IPolygonOverlap<TShape>
    {
        /// <inheritdoc />
        public bool DoOverlap(TShape polygon1, Vector2Int position1, TShape polygon2, Vector2Int position2)
        {
            // Polygons cannot overlap if their bounding rectangles do not overlap
            if (!DoOverlap(GetBoundingRectangle(polygon1) + position1, GetBoundingRectangle(polygon2) + position2))
                return false;

            var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1).ToList();
            var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2).ToList();

            foreach (var r1 in decomposition1)
            {
                foreach (var r2 in decomposition2)
                {
                    if (DoOverlap(r1, r2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two rectangles overlap.
        /// </summary>
        /// <param name="rectangle1"></param>
        /// <param name="rectangle2"></param>
        /// <returns></returns>
        public bool DoOverlap(RectangleGrid2D rectangle1, RectangleGrid2D rectangle2)
        {
            return rectangle1.A.X < rectangle2.B.X && rectangle1.B.X > rectangle2.A.X &&
                   rectangle1.A.Y < rectangle2.B.Y && rectangle1.B.Y > rectangle2.A.Y;
        }

        public bool DoOverlap(RectangleGrid2D rectangle1, Vector2Int position1, RectangleGrid2D rectangle2,
            Vector2Int position2)
        {
            return (rectangle1.A.X + position1.X) < (rectangle2.B.X + position2.X) &&
                   (rectangle1.B.X + position1.X) > (rectangle2.A.X + position2.X) &&
                   (rectangle1.A.Y + position1.Y) < (rectangle2.B.Y + position2.Y) &&
                   (rectangle1.B.Y + position1.Y) > (rectangle2.A.Y + position2.Y);
        }

        /// <inheritdoc />
        public int OverlapArea(TShape polygon1, Vector2Int position1, TShape polygon2, Vector2Int position2)
        {
            // Polygons cannot overlap if their bounding rectangles do not overlap
            if (!DoOverlap(GetBoundingRectangle(polygon1) + position1, GetBoundingRectangle(polygon2) + position2))
                return 0;

            // Profiling: there should be no lambda/linq expression here as this function is called very often
            var decomposition1 = GetDecomposition(polygon1);
            var decomposition2 = GetDecomposition(polygon2);
            var area = 0;

            foreach (var r1 in decomposition1)
            {
                var rectangle1 = r1 + position1;

                foreach (var r2 in decomposition2)
                {
                    var rectangle2 = r2 + position2;

                    var overlapX = System.Math.Max(0,
                        System.Math.Min(rectangle1.B.X, rectangle2.B.X) -
                        System.Math.Max(rectangle1.A.X, rectangle2.A.X));
                    var overlapY = System.Math.Max(0,
                        System.Math.Min(rectangle1.B.Y, rectangle2.B.Y) -
                        System.Math.Max(rectangle1.A.Y, rectangle2.A.Y));
                    area += overlapX * overlapY;
                }
            }

            return area;
        }

        /// <inheritdoc />
        public bool DoTouch(TShape polygon1, Vector2Int position1, TShape polygon2, Vector2Int position2,
            int minimumLength = 0)
        {
            if (minimumLength < 0)
                throw new ArgumentException("The minimum length must by at least 0.", nameof(minimumLength));

            var bounding1 = GetBoundingRectangle(polygon1) + position1;
            var bounding2 = GetBoundingRectangle(polygon2) + position2;

            if (!DoOverlap(bounding1, bounding2) && !DoTouch(bounding1, bounding2, minimumLength))
            {
                return false;
            }

            var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1);
            var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2);

            foreach (var r1 in decomposition1)
            {
                foreach (var r2 in decomposition2)
                {
                    if (DoTouch(r1, r2, minimumLength))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected bool DoTouch(RectangleGrid2D rectangle1, RectangleGrid2D rectangle2, int minimumLength)
        {
            var overlapX = System.Math.Max(-1,
                System.Math.Min(rectangle1.B.X, rectangle2.B.X) - System.Math.Max(rectangle1.A.X, rectangle2.A.X));
            var overlapY = System.Math.Max(-1,
                System.Math.Min(rectangle1.B.Y, rectangle2.B.Y) - System.Math.Max(rectangle1.A.Y, rectangle2.A.Y));

            if ((overlapX == 0 && overlapY >= minimumLength) || (overlapY == 0 && overlapX >= minimumLength))
            {
                return true;
            }

            return false;
        }

        public bool DoHaveMinimumDistance(TShape polygon1, Vector2Int position1, TShape polygon2, Vector2Int position2,
            int minimumDistance)
        {
            if (minimumDistance < 0)
                throw new ArgumentException("The minimum distance must by at least 0.", nameof(minimumDistance));

            var bounding1 = GetBoundingRectangle(polygon1) + position1;
            var bounding2 = GetBoundingRectangle(polygon2) + position2;

            if (DoHaveMinimumDistance(bounding1, bounding2, minimumDistance))
            {
                return true;
            }

            var decomposition1 = GetDecomposition(polygon1).Select(x => x + position1);
            var decomposition2 = GetDecomposition(polygon2).Select(x => x + position2);

            foreach (var r1 in decomposition1)
            {
                foreach (var r2 in decomposition2)
                {
                    if (!DoHaveMinimumDistance(r1, r2, minimumDistance))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected bool DoHaveMinimumDistance(RectangleGrid2D rectangle1, RectangleGrid2D rectangle2,
            int minimumDistance)
        {
            var distanceX = System.Math.Max(rectangle2.A.X - rectangle1.B.X, rectangle1.A.X - rectangle2.B.X);
            var distanceY = System.Math.Max(rectangle2.A.Y - rectangle1.B.Y, rectangle1.A.Y - rectangle2.B.Y);

            return distanceX >= minimumDistance || distanceY >= minimumDistance;
        }

        /// <inheritdoc />
        public int GetDistance(TShape polygon1, Vector2Int position1, TShape polygon2, Vector2Int position2)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IList<Tuple<Vector2Int, bool>> OverlapAlongLine(TShape movingPolygon, TShape fixedPolygon,
            OrthogonalLineGrid2D line)
        {
            var reverse = line.GetDirection() == OrthogonalLineGrid2D.Direction.Bottom ||
                          line.GetDirection() == OrthogonalLineGrid2D.Direction.Left;

            if (reverse)
            {
                line = line.SwitchOrientation();
            }

            var rotation = line.ComputeRotation();
            var rotatedLine = line.Rotate(rotation);

            var movingDecomposition = GetDecomposition(movingPolygon).Select(x => x.Rotate(rotation)).ToList();
            var fixedDecomposition = GetDecomposition(fixedPolygon).Select(x => x.Rotate(rotation)).ToList();

            var smallestX = movingDecomposition.Min(x => x.A.X);
            var events = new List<Tuple<Vector2Int, bool>>();

            // Compute the overlap for every rectangle in the decomposition of the moving polygon
            foreach (var movingRectangle in movingDecomposition)
            {
                var newEvents = OverlapAlongLine(movingRectangle, fixedDecomposition, rotatedLine,
                    movingRectangle.A.X - smallestX);
                events = MergeEvents(events, newEvents, rotatedLine);
            }

            if (reverse)
            {
                events = ReverseEvents(events, rotatedLine);
            }

            return events.Select(x => Tuple.Create(x.Item1.RotateAroundCenter(-rotation), x.Item2)).ToList();
        }

        /// <summary>
        /// Computes the overlap along a line of a given moving rectangle and a set o fixed rectangles.
        /// </summary>
        /// <param name="movingRectangle"></param>
        /// <param name="fixedRectangles"></param>
        /// <param name="line"></param>
        /// <param name="movingRectangleOffset">Specifies the X-axis offset of a given moving rectangle.</param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> OverlapAlongLine(RectangleGrid2D movingRectangle,
            IList<RectangleGrid2D> fixedRectangles, OrthogonalLineGrid2D line, int movingRectangleOffset = 0)
        {
            if (line.GetDirection() != OrthogonalLineGrid2D.Direction.Right)
                throw new ArgumentException();

            var events = new List<Tuple<Vector2Int, bool>>();

            foreach (var fixedRectangle in fixedRectangles)
            {
                var newEvents = OverlapAlongLine(movingRectangle, fixedRectangle, line, movingRectangleOffset);
                events = MergeEvents(events, newEvents, line);
            }

            return events;
        }

        /// <summary>
        /// Computes the overlap along a line of a given moving rectangle and a fixed rectangle.
        /// </summary>
        /// <param name="movingRectangle"></param>
        /// <param name="fixedRectangle"></param>
        /// <param name="line"></param>
        /// <param name="movingRectangleOffset"></param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> OverlapAlongLine(RectangleGrid2D movingRectangle,
            RectangleGrid2D fixedRectangle, OrthogonalLineGrid2D line, int movingRectangleOffset = 0)
        {
            if (line.GetDirection() != OrthogonalLineGrid2D.Direction.Right)
                throw new ArgumentException();

            // The smallest rectangle that covers both the first and the last position on the line of the moving rectangle
            var boundingRectangle = new RectangleGrid2D(movingRectangle.A + line.From, movingRectangle.B + line.To);

            // They cannot overlap if the bounding rectangle does not overlap with the fixed one
            if (!DoOverlap(boundingRectangle, fixedRectangle))
            {
                return new List<Tuple<Vector2Int, bool>>();
            }

            var events = new List<Tuple<Vector2Int, bool>>();

            if (fixedRectangle.A.X - movingRectangle.Width - movingRectangleOffset <= line.From.X)
            {
                events.Add(Tuple.Create(line.From, true));
            }

            if (fixedRectangle.A.X > line.From.X + movingRectangle.Width + movingRectangleOffset)
            {
                events.Add(Tuple.Create(
                    new Vector2Int(fixedRectangle.A.X - movingRectangle.Width + 1 - movingRectangleOffset, line.From.Y),
                    true));
            }

            if (fixedRectangle.B.X - movingRectangleOffset < line.To.X)
            {
                events.Add(Tuple.Create(new Vector2Int(fixedRectangle.B.X - movingRectangleOffset, line.From.Y),
                    false));
            }

            return events;
        }

        /// <summary>
        /// Reverses a given events list in a way that the line has the opposite direction.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> ReverseEvents(List<Tuple<Vector2Int, bool>> events,
            OrthogonalLineGrid2D line)
        {
            var eventsCopy = new List<Tuple<Vector2Int, bool>>(events);

            if (events.Count == 0)
                return events;

            eventsCopy.Reverse();
            var newEvents = new List<Tuple<Vector2Int, bool>>();

            if (events.Last().Item2)
            {
                newEvents.Add(Tuple.Create(line.To, true));
            }

            foreach (var @event in eventsCopy)
            {
                if (!(@event.Item1 == line.From && @event.Item2 == true))
                {
                    newEvents.Add(Tuple.Create(@event.Item1 - line.GetDirectionVector(), !@event.Item2));
                }
            }

            return newEvents;
        }

        /// <summary>
        /// Merges two lists of events.
        /// </summary>
        /// <param name="events1"></param>
        /// <param name="events2"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> MergeEvents(List<Tuple<Vector2Int, bool>> events1,
            List<Tuple<Vector2Int, bool>> events2, OrthogonalLineGrid2D line)
        {
            if (events1.Count == 0)
                return events2;

            if (events2.Count == 0)
                return events1;

            var merged = new List<Tuple<Vector2Int, bool>>();

            var counter1 = 0;
            var counter2 = 0;

            var lastOverlap = false;
            var overlap1 = false;
            var overlap2 = false;

            // Run the main loop while both lists still have elements
            while (counter1 < events1.Count && counter2 < events2.Count)
            {
                var pair1 = events1[counter1];
                var pos1 = line.Contains(pair1.Item1);

                var pair2 = events2[counter2];
                var pos2 = line.Contains(pair2.Item1);

                if (pos1 <= pos2)
                {
                    overlap1 = pair1.Item2;
                    counter1++;
                }

                if (pos1 >= pos2)
                {
                    overlap2 = pair2.Item2;
                    counter2++;
                }

                var overlap = overlap1 || overlap2;

                if (overlap != lastOverlap)
                {
                    if (pos1 < pos2)
                    {
                        merged.Add(Tuple.Create(pair1.Item1, overlap));
                    }
                    else
                    {
                        merged.Add(Tuple.Create(pair2.Item1, overlap));
                    }
                }

                lastOverlap = overlap;
            }

            // Add remaining elements from the first list
            if (events2.Last().Item2 != true)
            {
                while (counter1 < events1.Count)
                {
                    var pair = events1[counter1];

                    if (merged.Last().Item2 != pair.Item2)
                    {
                        merged.Add(pair);
                    }

                    counter1++;
                }
            }

            // Add remaining elements from the second list
            if (events1.Last().Item2 != true)
            {
                while (counter2 < events2.Count)
                {
                    var pair = events2[counter2];

                    if (merged.Last().Item2 != pair.Item2)
                    {
                        merged.Add(pair);
                    }

                    counter2++;
                }
            }

            return merged;
        }

        /// <summary>
        /// Gets a decomposition of a given polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        protected abstract List<RectangleGrid2D> GetDecomposition(TShape polygon);

        /// <summary>
        /// Gets the bounding rectangle for a given polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        protected abstract RectangleGrid2D GetBoundingRectangle(TShape polygon);
    }
}