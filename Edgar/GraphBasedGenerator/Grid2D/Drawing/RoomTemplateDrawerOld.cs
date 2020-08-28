using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D.Drawing
{
    /// <summary>
	/// Draws a layout on an old paper texture.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class RoomTemplateDrawerOld<TNode>
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
		public Bitmap DrawRoomTemplates(List<RoomTemplateGrid2D> roomTemplates, int width, int height, bool withNames = true, float? fixedFontSize = null, float borderSize = 0.2f)
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



            DrawRoomTemplatesBase(GetRoomTemplateConfigurations(roomTemplates, width / (double) height), width, height, withNames, fixedFontSize, borderSize);

			textureImgInner.Dispose();
			innerBrush.Dispose();

			textureImgOutline.Dispose();
			outlineBrush.Dispose();
			outlinePen.Dispose();

			return bitmap;
		}

        private List<RoomTemplateConfiguration> GetRoomTemplateConfigurations(List<RoomTemplateGrid2D> roomTemplates, double expectedRatio)
        {
            var orderedRoomTemplates = roomTemplates.OrderByDescending(x => x.Outline.BoundingRectangle.Width).ToList();
            var minDistance = 3;

            var bestRatio = double.MaxValue;
            List<RoomTemplateConfiguration> bestConfigurations = null;
            List<List<RoomTemplateGrid2D>> lastRows = null;

            while (true)
            {
                int rowWidth;

                if (lastRows == null)
                {
                    rowWidth = orderedRoomTemplates[0].Outline.BoundingRectangle.Width;
                }
                else
                {
                    rowWidth = GetNextBreakPoint(lastRows);
                }

                var rows = GetRows(roomTemplates, rowWidth);
                var configurations = GetConfigurations(rows, rowWidth, minDistance);
                var ratio = GetWidthHeightRatio(configurations);
                var emptySpaceRatio = GetEmptySpaceRatio(rows, rowWidth);

                if (Math.Abs(expectedRatio - ratio) < Math.Abs(expectedRatio - bestRatio))
                {
                    bestConfigurations = configurations;
                    bestRatio = ratio;
                }

                lastRows = rows;

                if (rows.Count == 1)
                {
                    if (bestConfigurations == null)
                    {
                        bestConfigurations = configurations;
                    }

                    break;
                }
            }

            return bestConfigurations;
        }

        private double GetWidthHeightRatio(List<RoomTemplateConfiguration> configurations)
        {
            var minX = configurations.Min(x => x.RoomTemplate.Outline.BoundingRectangle.A.X + x.Position.X);
            var minY = configurations.Min(x => x.RoomTemplate.Outline.BoundingRectangle.A.Y + x.Position.Y);
            var maxX = configurations.Max(x => x.RoomTemplate.Outline.BoundingRectangle.B.X + x.Position.X);
            var maxY = configurations.Max(x => x.RoomTemplate.Outline.BoundingRectangle.B.Y + x.Position.Y);

            var width = maxX - minX;
            var height = maxY - minY;

            return width / (double) height;
        }

        private double GetEmptySpaceRatio(List<List<RoomTemplateGrid2D>> rows, int rowWidth)
        {
            var worstRatio = double.MinValue;

            foreach (var row in rows)
            {
                var width = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var emptySpace = rowWidth - width;

                worstRatio = Math.Max(worstRatio, emptySpace / (double) rowWidth);
            }

            return worstRatio;
        }

        private int GetNextBreakPoint(List<List<RoomTemplateGrid2D>> rows)
        {
            var minWidth = int.MaxValue;

            for (var i = 0; i < rows.Count - 1; i++)
            {
                var row = rows[i];
                var nextRow = rows[i + 1];

                var currentWidth = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var nextItemWidth = nextRow[0].Outline.BoundingRectangle.Width;
                var nextWidth = currentWidth + nextItemWidth;

                minWidth = Math.Min(minWidth, nextWidth);
            }

            return minWidth;
        }

        private List<RoomTemplateConfiguration> GetConfigurations(List<List<RoomTemplateGrid2D>> rows, int rowWidth, int minDistance)
        {
            var configurations = new List<RoomTemplateConfiguration>();
            var maxHeight = int.MinValue;

            rowWidth = rows.Max(x => x.Sum(y => y.Outline.BoundingRectangle.Width) + (x.Count - 1) * minDistance);

            var offset = new Vector2Int();
            foreach (var row in rows)
            {
                var totalWidth = row.Sum(x => x.Outline.BoundingRectangle.Width);
                var space = rowWidth - totalWidth;
                var distance = row.Count != 1 ? space / (row.Count - 1) : 0;
                var remainder = space - distance * (row.Count - 1);

                if (row == rows.Last())
                {
                    distance = minDistance;
                    remainder = 0;
                }

                if (row.Count == 1)
                {
                    offset += new Vector2Int(space / 2, 0);
                }

                for (var i = 0; i < row.Count; i++)
                {
                    var roomTemplate = row[i];
                    var outline = roomTemplate.Outline;
                    configurations.Add(new RoomTemplateConfiguration(roomTemplate,
                        -1 * roomTemplate.Outline.BoundingRectangle.A + offset));
                    offset += new Vector2Int(roomTemplate.Outline.BoundingRectangle.Width, 0);
                    maxHeight = Math.Max(maxHeight, outline.BoundingRectangle.Height);

                    offset += new Vector2Int(distance, 0);

                    if (i < remainder)
                    {
                        offset += new Vector2Int(1, 0);
                    }
                }

                offset = new Vector2Int(0, offset.Y + maxHeight + minDistance);
                maxHeight = int.MinValue;
            }

            return configurations;
        }

        private List<List<RoomTemplateGrid2D>> GetRows(List<RoomTemplateGrid2D> roomTemplates, int rowWidth)
        {
            roomTemplates = roomTemplates.OrderByDescending(x => x.Outline.BoundingRectangle.Height).ToList();
            

            var maxHeight = int.MinValue;
            var rows = new List<List<RoomTemplateGrid2D>>();
            rows.Add(new List<RoomTemplateGrid2D>());

            foreach (var roomTemplate in roomTemplates)
            {
                var outline = roomTemplate.Outline;

                if (rows.Last().Sum(x => x.Outline.BoundingRectangle.Width) + outline.BoundingRectangle.Width > rowWidth)
                {
                    maxHeight = int.MinValue;
                    rows.Add(new List<RoomTemplateGrid2D>());
                }

                // configurations.Add(new RoomTemplateConfiguration(roomTemplate, -1 * roomTemplate.Outline.BoundingRectangle.A + offset));
                rows.Last().Add(roomTemplate);
                
                maxHeight = Math.Max(maxHeight, outline.BoundingRectangle.Height);
            }

            return rows;
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
            gridPen.DashPattern = new float[] {1.2f / 10, 3.1f};
            gridPen.DashOffset = 0.5f;

            //foreach (var point in points)
            //{
            //    var right = point + new Vector2Int(1, 0);
            //    var bottom = point + new Vector2Int(0, -1);

            //    if (points.Contains(right))
            //    {
            //        graphics.DrawLine(gridPen, point.X, point.Y, right.X, right.Y);
            //    }

            //    if (points.Contains(bottom))
            //    {
            //        graphics.DrawLine(gridPen, point.X, point.Y, bottom.X, bottom.Y);
            //    }
            //}

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

		private void DrawRoomTemplatesBase(List<RoomTemplateConfiguration> roomTemplates, int width, int height, bool withNames, float? fixedFontSize = null, float borderSize = 0.2f)
		{
            var polygons = roomTemplates.Select(x => x.RoomTemplate.Outline + x.Position).ToList();
            var (scale, offset) = GetScaleAndOffset(polygons, width, height, borderSize);

			var minWidth = polygons.Min(x => x.BoundingRectangle.Width);

			graphics.ScaleTransform(scale, scale);
            graphics.TranslateTransform(offset.X, offset.Y);

   //         foreach (var room in rooms)
   //         {
   //             DrawShading(room.Outline + room.Position, GetOutlineNew(room.Outline, room.Doors, room.Position));
   //         }

			//foreach (var room in rooms)
   //         {
   //             DrawHatching(room.Outline + room.Position);
   //         }


            foreach (var roomTemplate in roomTemplates)
            {
                DrawShading(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, GetOutlineNew(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position));
            }

            foreach (var roomTemplate in roomTemplates)
            {
                DrawHatching(roomTemplate.RoomTemplate.Outline + roomTemplate.Position);
            }

            foreach (var roomTemplate in roomTemplates)
            {
                DrawOutline(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, GetOutlineNew(roomTemplate.RoomTemplate.Outline, null, roomTemplate.Position));
            }

            foreach (var roomTemplate in roomTemplates)
            {
                if (withNames)
                {
                    DrawTextOntoPolygon(roomTemplate.RoomTemplate.Outline + roomTemplate.Position, roomTemplate.RoomTemplate.Name.ToString(), fixedFontSize ?? 2.5f * minWidth);
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
            offset = new Vector2(offset.X / scale, offset.Y / scale);

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

        private List<OutlineSegment> GetOutlineNew(PolygonGrid2D polygon, List<LayoutDoorGrid2D<TNode>> doorLines = null, Vector2Int offset = default)
        {
            var old = GetOutline(polygon, doorLines);
            var outline = new List<OutlineSegment>();

            if (doorLines == null)
            {
                foreach (var line in polygon.GetLines())
                {
                    outline.Add(new OutlineSegment(line + offset, false));
                }
            }
            else
            {
                for (int i = 0; i < old.Count; i++)
                {
                    var current = old[i];
                    var previous = old[Mod(i - 1, old.Count)];
                    outline.Add(new OutlineSegment(new OrthogonalLineGrid2D(previous.Item1 + offset, current.Item1 + offset), current.Item2 == false));
                }
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

        protected class RoomTemplateConfiguration
        {
            public RoomTemplateGrid2D RoomTemplate { get; }

            public Vector2Int Position { get; }

            public RoomTemplateConfiguration(RoomTemplateGrid2D roomTemplate, Vector2Int position)
            {
                RoomTemplate = roomTemplate;
                Position = position;
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