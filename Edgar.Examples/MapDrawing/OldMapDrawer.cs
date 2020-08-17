using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils.MapDrawing;

namespace Edgar.Examples.MapDrawing
{
    /// <summary>
	/// Draws a layout on an old paper texture.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class OldMapDrawer<TNode>
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
		public Bitmap DrawLayout(LevelGrid2D<TNode> layout, int width, int height, bool withNames = true, float? fixedFontSize = null)
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

		/// <inheritdoc />
		protected void DrawRoom(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
        {
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			var offset = polygon.BoundingRectangle.A;
            
			innerBrush.TranslateTransform(offset.X, offset.Y);
			//graphics.FillPolygon(innerBrush, polyPoints.ToArray());
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

            for (var i = 0; i < outline.Count; i++)
            {
                var pair = outline[i];
                var point = pair.Item1;

                var previous = outline[Mod(i - 1, outline.Count)];
                var previousPoint = previous.Item1;

                var next = outline[Mod(i + 1, outline.Count)];

                if (previous.Item2 == false)
                {
                    outlinePen.StartCap = LineCap.Square;
                }
                else
                {
                    outlinePen.StartCap = LineCap.Round;
                }

                if (next.Item2 == false)
                {
                    outlinePen.EndCap = LineCap.Square;
                }
                else
                {
                    outlinePen.EndCap = LineCap.Round;
                }

                if (pair.Item2)
                {
                    graphics.DrawLine(outlinePen, previousPoint.X, previousPoint.Y, point.X, point.Y);
                }
            }
        }

        private int Mod(int x, int m) {
            return (x%m + m)%m;
        }

        protected void DrawRoomBefore(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
        {
            var pen = new Pen(Color.FromArgb(50, 50, 50), scale * 0.55f);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var usedPointsAdd = new List<Vector2>();

            foreach (var line in polygon.GetLines())
            {
                var points = line.GetPoints().Select(x => (Vector2) x).ToList();
				points.AddRange(points.Select(x => x + 0.5f * (Vector2) line.GetDirectionVector()).ToList());

                //for (int i = 0; i < 5; i++)
                //{
                //    points.Add(points.Last() + line.GetDirectionVector()); 
                //}
                
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    if (true)
                    {
                        //if (points.Count - 1 - i < 5)
                        //{
                        //    continue;
                        //}

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

        protected void DrawRoomBefore2(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
        {
            for (var i = 0; i < outline.Count; i++)
            {
                var pair = outline[i];
                var point = pair.Item1;

                var previous = outline[Mod(i - 1, outline.Count)];
                var previousPoint = previous.Item1;

                if (pair.Item2)
                {
                    graphics.DrawLine(shadePen, previousPoint.X, previousPoint.Y, point.X, point.Y);
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

        		/// <summary>
		/// Entry point of the class. Draws a given layout to an output with given dimensions.
		/// </summary>
		/// <param name="layout">Layout do be drawn</param>
		/// <param name="width">Width of the output</param>
		/// <param name="height">Height of the output</param>
		/// <param name="withNames">Whether names should be displayed</param>
		/// <param name="fixedFontSize"></param>
		/// <param name="borderSize"></param>
		protected void DrawLayoutBase(LevelGrid2D<TNode> layout, int width, int height, bool withNames, float? fixedFontSize = null, float borderSize = 0.2f)
		{
			var polygons = layout.Rooms.Select(x => x.Shape + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			var scale = GetScale(minx, miny, maxx, maxy, width, height, borderSize);
			var offset = GetOffset(minx, miny, maxx, maxy, width, height, scale);
			offset = new Vector2Int((int) (offset.X / scale), (int) (offset.Y / scale));

			DrawLayout(layout, scale, offset, withNames, fixedFontSize);
		}

		/// <summary>
		/// Draws a given layout to an output using a given scale and offset. 
		/// </summary>
		/// <remarks>
		/// All points are tranfosmer using the TransformPoint method.
		/// </remarks>
		/// <param name="layout">Layout do be drawn</param>
		/// <param name="scale">Scale factor</param>
		/// <param name="offset"></param>
		/// <param name="withNames">Whether names should be displayed</param>
		/// <param name="fixedFontSize"></param>
		protected void DrawLayout(LevelGrid2D<TNode> layout, float scale, Vector2 offset, bool withNames, float? fixedFontSize = null)
		{
			var polygons = layout.Rooms.Select(x => x.Shape + x.Position).ToList();
			var rooms = layout.Rooms.ToList();
			var minWidth = layout.Rooms.Where(x => !x.IsCorridor).Select(x => x.Shape + x.Position).Min(x => x.BoundingRectangle.Width);

			graphics.ScaleTransform(scale, scale);
            var scaleOriginal = scale;
            scale = 1;

			graphics.TranslateTransform(offset.X, offset.Y);
            var offsetOriginal = offset;
			offset = new Vector2Int();

            //// TODO: remove later
            for (var i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                var outline = GetOutline(polygons[i], room.Doors?.ToList())
                    .Select(x => Tuple.Create(TransformPoint(x.Item1, scale, offset), x.Item2)).ToList();

                var transformedPoints = polygons[i].GetPoints().Select(point => TransformPoint(point, scale, offset)).ToList();

                if (transformedPoints.All(x => x == new Vector2Int(0, 0)))
                {
                    throw new InvalidOperationException("One of the polygons could not be drawn because the canvas size is too small.");
                }

                var polygon = new PolygonGrid2D(transformedPoints);
                DrawRoomBefore2(polygon, outline, 2);
            }

			//// TODO: remove later
			for (var i = 0; i < rooms.Count; i++)
			{
				var room = rooms[i];
				var outline = GetOutline(polygons[i], room.Doors?.ToList())
					.Select(x => Tuple.Create(TransformPoint(x.Item1, scale, offset), x.Item2)).ToList();

				var transformedPoints = polygons[i].GetPoints().Select(point => TransformPoint(point, scale, offset)).ToList();

				if (transformedPoints.All(x => x == new Vector2Int(0, 0)))
				{
					throw new InvalidOperationException("One of the polygons could not be drawn because the canvas size is too small.");
				}

				var polygon = new PolygonGrid2D(transformedPoints);
				DrawRoomBefore(polygon, outline, 2);
			}

			for (var i = 0; i < rooms.Count; i++)
			{
				var room = rooms[i];
				var outline = GetOutline(polygons[i], room.Doors?.ToList())
					.Select(x => Tuple.Create(TransformPoint(x.Item1, scale, offset), x.Item2)).ToList();

				var transformedPoints = polygons[i].GetPoints().Select(point => TransformPoint(point, scale, offset)).ToList();

				if (transformedPoints.All(x => x == new Vector2Int(0, 0)))
				{
					throw new InvalidOperationException("One of the polygons could not be drawn because the canvas size is too small.");
				}

				var polygon = new PolygonGrid2D(transformedPoints);
				DrawRoom(polygon, outline, 2);

				if (withNames && !room.IsCorridor)
				{
					DrawTextOntoPolygon(polygon, room.Node.ToString(), fixedFontSize ?? 2.5f * minWidth);
				}
			}
		}

		/// <summary>
		/// Both coordinates are first multiplied by the scale factor and then the offset is added.
		/// </summary>
		/// <remarks>
		/// Resulting coordinates must be cast back to int.
		/// </remarks>
		/// <param name="point"></param>
		/// <param name="scale"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		protected Vector2Int TransformPoint(Vector2Int point, float scale, Vector2 offset)
		{
			return new Vector2Int((int)(scale * point.X + offset.X), (int)(scale * point.Y + offset.Y));
		}

		/// <summary>
		/// Computes an offset that will move points to the first quadrant as close to axis as possible.
		/// </summary>
		/// <param name="minx"></param>
		/// <param name="miny"></param>
		/// <param name="maxx"></param>
		/// <param name="maxy"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public Vector2 GetOffset(int minx, int miny, int maxx, int maxy, int width, int height, float scale = 1)
		{
			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new Vector2((width / 2f - centerx), (height / 2f - centery));
		}

		/// <summary>
		/// Computes a scale factor that will transform points to match given width and height. 
		/// Some space can be also left for borders.
		/// </summary>
		/// <param name="minx"></param>
		/// <param name="miny"></param>
		/// <param name="maxx"></param>
		/// <param name="maxy"></param>
		/// <param name="expectedWidth"></param>
		/// <param name="expectedHeight"></param>
		/// <param name="borderSize">How much of the original image should be used for each border. </param>
		/// <returns></returns>
		public float GetScale(int minx, int miny, int maxx, int maxy, int expectedWidth, int expectedHeight, float borderSize = 0.2f)
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

        /// <summary>
        /// Computes the outline of a given polygon and its door lines.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="doorLines"></param>
        /// <returns></returns>
        protected List<Tuple<Vector2Int, bool>> GetOutline(PolygonGrid2D polygon, List<DoorInfo<TNode>> doorLines)
        {
            var outline = new List<Tuple<Vector2Int, bool>>();

            foreach (var line in polygon.GetLines())
            {
                AddToOutline(Tuple.Create(line.From, true));

                if (doorLines == null)
                    continue;

                var doorDistances = doorLines.Select(x =>
                    new Tuple<DoorInfo<TNode>, int>(x, Math.Min(line.Contains(x.DoorLine.From), line.Contains(x.DoorLine.To)))).ToList();
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
	}
}