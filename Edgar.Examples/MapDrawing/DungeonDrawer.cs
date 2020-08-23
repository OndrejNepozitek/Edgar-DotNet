using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.Examples.MapDrawing
{
    /// <summary>
	/// Draws a layout on an old paper texture.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class DungeonDrawer<TNode>
	{
		private readonly CachedPolygonPartitioning polygonPartitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());
		private Bitmap bitmap;
		private Graphics graphics;

		private const string TextureInnerPath = @"Resources\Images\texture_inner.png";
		private const string TextureOutterPath = @"Resources\Images\texture_outter.png";
		private const string TexturePenPath = @"Resources\Images\texture_pen.png";

		private TextureBrush innerBrush;
		private Brush outlineBrush;
		private Pen outlinePen;
		private Pen shadePen;

        private List<Vector2> usedPoints;
        private readonly Random random = new Random();
        private readonly float scale = 0.1f;
		private readonly CachedPolygonPartitioning partitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());

		/// <summary>
		/// Draws a given layout and returns a bitmap.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="width">Result will have this width and height will be computed to match the layout.</param>
		/// <param name="height"></param>
		/// <param name="withNames"></param>
		/// <param name="fixedFontSize"></param>
		/// <returns></returns>
		public Bitmap DrawLayout(LayoutGrid2D<TNode> layout, int width, int height, bool withNames = true, float? fixedFontSize = null)
		{
			bitmap = new Bitmap(width, height);
			graphics = Graphics.FromImage(bitmap);

			//var textureImgOuter = new Bitmap(TextureOutterPath);
			//using (var brush = new TextureBrush(textureImgOuter, WrapMode.Tile))
			//{
			//	graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
			//}

            usedPoints = new List<Vector2>();

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 248, 244)))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }

			var textureImgInner = new Bitmap(TextureInnerPath);
			innerBrush = new TextureBrush(textureImgInner, WrapMode.Tile);

            var textureImgOutline = new Bitmap(TexturePenPath);
			outlineBrush = new SolidBrush(Color.FromArgb(50, 50, 50));

			outlinePen = new Pen(outlineBrush, 2 * scale)
			{
				EndCap = LineCap.Round,
				StartCap = LineCap.Round
			};

            shadePen = new Pen(Color.FromArgb(204, 206, 206), 13 * scale)
            {
                EndCap = LineCap.Round,
                StartCap = LineCap.Round
            };

            DrawLayoutBase(layout, width, height, withNames, fixedFontSize);

			textureImgInner.Dispose();
			innerBrush.Dispose();

			textureImgOutline.Dispose();
			outlineBrush.Dispose();
			outlinePen.Dispose();

			return bitmap;
		}

		private void DrawOutline(PolygonGrid2D polygon, List<OutlineSegment> outlineSegments)
        {
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			var offset = polygon.BoundingRectangle.A;
            
			innerBrush.TranslateTransform(offset.X, offset.Y);
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(248, 248, 244)), polyPoints.ToArray());
			innerBrush.ResetTransform();

            var rectangles = partitioning.GetPartitions(polygon);
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

            var gridPen = new Pen(Color.FromArgb(100, 100, 100), scale * 0.50f);
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

        private void DrawHatching(PolygonGrid2D outline)
        {
            var pen = new Pen(Color.FromArgb(50, 50, 50), scale * 0.55f);
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

                            var offsetLength = scale * NextFloat(2, 4) * 1.75f;
                            var clusterOffset = scale * NextFloat(1.5f, 3f);

                            if (j == 1)
                            {
                                offsetLength = 0;
                            }

                            var c = point + offsetLength * directionPerpendicular;

                            if (usedPoints.Any(x => Vector2.EuclideanDistance(x, c) < scale * 5))
                            {
                                continue;
                            }
                            else
                            {
                                usedPointsAdd.Add(c);
                            }

                            for (int k = -1; k <= 1; k++)
                            {
                                var length = scale * 3;
                                var center = point + offsetLength * directionPerpendicular + k * clusterOffset * directionPerpendicular;
                                center = RotatePoint(center, point + offsetLength * directionPerpendicular, rotation);

                                var from = center + length * direction;
                                var to = center - length * direction;

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

        private void DrawShading(PolygonGrid2D polygon, List<OutlineSegment> outlineSegments)
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

		private void DrawLayoutBase(LayoutGrid2D<TNode> layout, int width, int height, bool withNames, float? fixedFontSize = null, float borderSize = 0.2f)
		{
            var polygons = layout.Rooms.Select(x => x.Outline + x.Position).ToList();
            var (scale, offset) = GetScaleAndOffset(polygons, width, height);

			var rooms = layout.Rooms.ToList();
			var minWidth = layout.Rooms.Where(x => !x.IsCorridor).Select(x => x.Outline + x.Position).Min(x => x.BoundingRectangle.Width);

			graphics.ScaleTransform(scale, scale);
            graphics.TranslateTransform(offset.X, offset.Y);

            foreach (var room in rooms)
            {
                DrawShading(room.Outline + room.Position, GetOutlineNew(room.Outline, room.Doors, room.Position));
            }

			foreach (var room in rooms)
            {
                DrawHatching(room.Outline + room.Position);
            }

            foreach (var room in rooms)
            {
                DrawOutline(room.Outline + room.Position, GetOutlineNew(room.Outline, room.Doors, room.Position));
            }

            foreach (var room in rooms)
            {
                if (withNames && !room.IsCorridor)
                {
                    DrawTextOntoPolygon(room.Outline + room.Position, room.Room.ToString(), fixedFontSize ?? 2.5f * minWidth);
                }
            }
        }

        private (float scale, Vector2 offset) GetScaleAndOffset(List<PolygonGrid2D> polygons, int width, int height, float borderSize = 0.2f)
        {
            var points = polygons.SelectMany(x => x.GetPoints()).ToList();

            var minx = points.Min(x => x.X);
            var miny = points.Min(x => x.Y);
            var maxx = points.Max(x => x.X);
            var maxy = points.Max(x => x.Y);

            var scale = GetScale(minx, miny, maxx, maxy, width, height, borderSize);
            var offset = GetOffset(minx, miny, maxx, maxy, width, height, scale);
            offset = new Vector2Int((int) (offset.X / scale), (int) (offset.Y / scale));

            return (scale, offset);
        }

        private Vector2 GetOffset(int minx, int miny, int maxx, int maxy, int width, int height, float scale = 1)
		{
			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new Vector2((width / 2f - centerx), (height / 2f - centery));
		}

        private float GetScale(int minx, int miny, int maxx, int maxy, int expectedWidth, int expectedHeight, float borderSize = 0.2f)
		{
			var neededWidth = (1 + borderSize) * (maxx - minx);
			var neededHeight = (1 + borderSize) * (maxy - miny);

			var scale = expectedWidth / neededWidth;

			if (scale * neededHeight > expectedHeight)
			{
				scale = expectedHeight / neededHeight;
			}

			return scale;
		}

        private List<OutlineSegment> GetOutlineNew(PolygonGrid2D polygon, List<LayoutDoorGrid2D<TNode>> doorLines, Vector2Int offset = default)
        {
            var old = GetOutline(polygon, doorLines);
            var outline = new List<OutlineSegment>();

            for (int i = 0; i < old.Count; i++)
            {
                var current = old[i];
                var previous = old[Mod(i - 1, old.Count)];
                outline.Add(new OutlineSegment(new OrthogonalLineGrid2D(previous.Item1 + offset, current.Item1 + offset), current.Item2 == false));
            }

            return outline;
        }

        /// <summary>
        /// Computes the outline of a given polygon and its door lines.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="doorLines"></param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> GetOutline(PolygonGrid2D polygon, List<LayoutDoorGrid2D<TNode>> doorLines)
        {
            var outline = new List<Tuple<Vector2Int, bool>>();

            foreach (var line in polygon.GetLines())
            {
                AddToOutline(Tuple.Create(line.From, true));

                if (doorLines == null)
                    continue;

                var doorDistances = doorLines.Select(x =>
                    new Tuple<LayoutDoorGrid2D<TNode>, int>(x, Math.Min(line.Contains(x.DoorLine.From), line.Contains(x.DoorLine.To)))).ToList();
                doorDistances.Sort((x1, x2) => x1.Item2.CompareTo(x2.Item2));

                foreach (var pair in doorDistances)
                {
                    if (pair.Item2 == -1)
                        continue;

                    var doorLine = pair.Item1.DoorLine;

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
        }
	}
}