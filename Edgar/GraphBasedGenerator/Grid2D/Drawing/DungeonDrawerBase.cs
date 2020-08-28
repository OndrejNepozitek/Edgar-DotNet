using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    public abstract class DungeonDrawerBase
    {
        protected readonly CachedPolygonPartitioning polygonPartitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());
        protected readonly Random random = new Random();

        protected Bitmap bitmap;
        protected Graphics graphics;

        protected void DrawOutline(PolygonGrid2D polygon, List<OutlineSegment> outlineSegments, Pen outlinePen)
        {
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			var offset = polygon.BoundingRectangle.A;
            
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(248, 248, 244)), polyPoints.ToArray());

            var rectangles = polygonPartitioning.GetPartitions(polygon);
            var points = new HashSet<Vector2Int>();

            foreach (var rectangle in rectangles)
            {
                for (int i = rectangle.A.X; i <= rectangle.B.X; i++)
                {
                    for (int j = rectangle.A.Y; j <= rectangle.B.Y; j++)
                    {
                        points.Add(new Vector2Int(i, j));
                    }
                }
            }

            var gridPen = new Pen(Color.FromArgb(100, 100, 100), 0.05f);
            gridPen.DashStyle = DashStyle.Dash;
            gridPen.DashPattern = new float[] {1.2f, 3.1f};
            gridPen.DashOffset = 0.5f;

            foreach (var point in points)
            {
                var right = point + new Vector2Int(1, 0);
				var bottom = point + new Vector2Int(0, -1);

                if (points.Contains(right))
                {
					graphics.DrawLine(gridPen, point.X, point.Y, right.X, right.Y);
                }

                if (points.Contains(bottom))
                {
                    graphics.DrawLine(gridPen, point.X, point.Y, bottom.X, bottom.Y);
                }
            }

            for (var i = 0; i < outlineSegments.Count; i++)
            {
                var current = outlineSegments[i];
                var previous = outlineSegments[Mod(i - 1, outlineSegments.Count)];
                var next = outlineSegments[Mod(i + 1, outlineSegments.Count)];

                outlinePen.StartCap = previous.IsDoor ? LineCap.Square : LineCap.Round;
                outlinePen.EndCap = next.IsDoor ? LineCap.Square : LineCap.Round;

                if (!current.IsDoor)
                {
                    var from = current.Line.From;
                    var to = current.Line.To;

                    graphics.DrawLine(outlinePen, from.X, from.Y, to.X, to.Y);
                }
            }
        }

        private int Mod(int x, int m) {
            return (x%m + m)%m;
        }

        protected void DrawHatching(PolygonGrid2D outline, List<Vector2> usedPoints, Range<float> hatchingClusterOffset, Range<float> hatchingLength)
        {
            var pen = new Pen(Color.FromArgb(50, 50, 50), 0.05f);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var usedPointsAdd = new List<Vector2>();

            foreach (var line in outline.GetLines())
            {
                var points = line.GetPoints().Select(x => (Vector2) x).ToList();
				points.AddRange(points.Select(x => x + 0.5f * (Vector2) line.GetDirectionVector()).ToList());

               
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    if (true)
                    {
                        var direction = (Vector2) line.GetDirectionVector();
                        var directionPerpendicular = new Vector2(Math.Max(-1, Math.Min(1, direction.Y)), Math.Max(-1, Math.Min(1, direction.X)));

                        if (direction.Y != 0)
                        {
                            directionPerpendicular = -1 * directionPerpendicular;
                        }

                        for (int j = 0; j < 2; j++)
                        {
                            var rotation = random.Next(0, 366);

                            var offsetLength = NextFloat(2, 4) * 1.75f / 10;
                            var clusterOffset = GetRandomFromRange(hatchingClusterOffset);

                            if (j == 1)
                            {
                                offsetLength = 0;
                            }

                            var c = point + offsetLength * directionPerpendicular;

                            if (usedPoints.Any(x => Vector2.EuclideanDistance(x, c) < 0.5f))
                            {
                                continue;
                            }
                            else
                            {
                                usedPointsAdd.Add(c);
                            }
                            
                            for (int k = -1; k <= 1; k++)
                            {
                                var length = GetRandomFromRange(hatchingLength);
                                var center = point + offsetLength * directionPerpendicular + k * clusterOffset * directionPerpendicular;
                                center = RotatePoint(center, point + offsetLength * directionPerpendicular, rotation);

                                var from = center + length / 2 * direction;
                                var to = center - length / 2 * direction;

                                from = RotatePoint(from, center, rotation);
                                to = RotatePoint(to, center, rotation);

                                graphics.DrawLine(pen, from.X, from.Y, to.X, to.Y);
                            }
                        }
                    }
                }
            }

            usedPoints.AddRange(usedPointsAdd);
        }

        private float GetRandomFromRange(Range<float> range)
        {
            return NextFloat(range.Minimum, range.Maximum);
        }

        protected void DrawShading(List<OutlineSegment> outlineSegments, Pen shadePen)
        {
            foreach (var outlineSegment in outlineSegments)
            {
                if (!outlineSegment.IsDoor)
                {
                    var from = outlineSegment.Line.From;
                    var to = outlineSegment.Line.To;

                    graphics.DrawLine(shadePen, from.X, from.Y, to.X, to.Y);
                }
            }
        }

        private float NextFloat(float from, float to)
        {
            return (float) random.NextDouble() * (to - from) + from;
        }

        private static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            (
                (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                        sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                     cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            );
        }

		protected void DrawTextOntoPolygon(PolygonGrid2D polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioning.GetPartitions(polygon);
			var orderedRectangles = partitions.OrderBy(x => Vector2Int.ManhattanDistance(x.Center, polygon.BoundingRectangle.Center)).ToList();
            var targetRectangle = orderedRectangles.First();

            if (orderedRectangles.Any(x => x.Width > 6 && x.Height > 3))
            {
                targetRectangle = orderedRectangles.First(x => x.Width > 6 && x.Height > 3);
            }

			using (var font = new Font("Baskerville Old Face", penWidth, FontStyle.Bold, GraphicsUnit.Pixel))
			{
				var rect = new RectangleF(
                    targetRectangle.A.X,
                    targetRectangle.A.Y,
                    targetRectangle.Width,
                    targetRectangle.Height
				);

				var sf = new StringFormat
				{
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Center
				};

				graphics.DrawString(text, font, Brushes.Black, rect, sf);
			}
		}
        
        protected List<OutlineSegment> GetOutline(PolygonGrid2D polygon, List<OrthogonalLineGrid2D> doorLines, Vector2Int offset = default)
        {
            var old = GetOutlineOld(polygon + offset, doorLines);
            var outline = new List<OutlineSegment>();

            for (int i = 0; i < old.Count; i++)
            {
                var current = old[i];
                var previous = old[Mod(i - 1, old.Count)];
                outline.Add(new OutlineSegment(new OrthogonalLineGrid2D(previous.Item1, current.Item1), current.Item2 == false));
            }

            return outline;
        }

        private List<Tuple<Vector2Int, bool>> GetOutlineOld(PolygonGrid2D polygon, List<OrthogonalLineGrid2D> doorLines)
        {
            var outline = new List<Tuple<Vector2Int, bool>>();
            doorLines = doorLines?.ToList();

            foreach (var line in polygon.GetLines())
            {
                AddToOutline(Tuple.Create(line.From, true));

                if (doorLines == null)
                    continue;

                var doorDistances = doorLines.Select(x =>
                    new Tuple<OrthogonalLineGrid2D, int>(x, Math.Min(line.Contains(x.From), line.Contains(x.To)))).ToList();
                doorDistances.Sort((x1, x2) => x1.Item2.CompareTo(x2.Item2));

                foreach (var pair in doorDistances)
                {
                    if (pair.Item2 == -1)
                        continue;

                    var doorLine = pair.Item1;

                    if (line.Contains(doorLine.From) != pair.Item2)
                    {
                        doorLine = doorLine.SwitchOrientation();
                    }

                    doorLines.Remove(pair.Item1);

                    AddToOutline(Tuple.Create(doorLine.From, true));
                    AddToOutline(Tuple.Create(doorLine.To, false));
                }
            }

            return outline;

            void AddToOutline(Tuple<Vector2Int, bool> point)
            {
                if (outline.Count == 0)
                {
                    outline.Add(point);
                    return;
                }
					
                var lastPoint = outline[outline.Count - 1];

                if (!lastPoint.Item2 && point.Item2 && lastPoint.Item1 == point.Item1)
                    return;

                outline.Add(point);
            }
        }

        protected class OutlineSegment
        {
            public OutlineSegment(OrthogonalLineGrid2D line, bool isDoor)
            {
                Line = line;
                IsDoor = isDoor;
            }

            public OrthogonalLineGrid2D Line { get; }

            public bool IsDoor { get; }

            public override string ToString()
            {
                return $"{Line} {IsDoor}";
            }
        }
    }
}