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
	public class OldMapDrawer<TNode> : GraphBasedGenerator.Grid2D.MapDrawing.AbstractLayoutDrawer<TNode>
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

        private List<Vector2> usedPoints;
        private readonly Random random = new Random();
        private readonly int scale = 4;

		/// <summary>
		/// Draws a given layout and returns a bitmap.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="width">Result will have this width and height will be computed to match the layout.</param>
		/// <param name="height"></param>
		/// <param name="withNames"></param>
		/// <param name="fixedFontSize"></param>
		/// <returns></returns>
		public Bitmap DrawLayout(LevelGrid2D<TNode> layout, int width, int height, bool withNames = true, int? fixedFontSize = null)
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

            base.DrawLayout(layout, width, height, withNames, fixedFontSize);

			textureImgInner.Dispose();
			innerBrush.Dispose();

			textureImgOutline.Dispose();
			outlineBrush.Dispose();
			outlinePen.Dispose();

			return bitmap;
		}

		/// <inheritdoc />
		protected override void DrawRoom(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
        {
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			var offset = polygon.BoundingRectangle.A;

			//var random = new Random(0);
   //         var pen = new Pen(Color.Black, 0.5f);
   //         graphics.SmoothingMode = SmoothingMode.HighQuality;
   //         foreach (var line in polygon.GetLines())
   //         {
   //             var ints = line.GetPoints();
   //             for (var i = 0; i < ints.Count; i++)
   //             {
   //                 var point = ints[i];

   //                 if (i % 5 == 0)
   //                 {
   //                     var offsetScale = 50;
   //                     var offsetX = (float) random.NextDouble() * offsetScale - offsetScale / 2f;
   //                     var offsetY = (float) random.NextDouble() * offsetScale - offsetScale / 2f;

   //                     var rotationScale = 10;
   //                     var rotationX1 = (float) random.NextDouble() * rotationScale - rotationScale / 2f;
   //                     var rotationY1 = (float) random.NextDouble() * rotationScale - rotationScale / 2f;

   //                     var length = 5;

   //                     for (int j = -1; j <= 1; j++)
   //                     {
   //                         var positionOffset = j * 5;

   //                         graphics.DrawLine(pen, new PointF(point.X + offsetX + length + rotationX1 + positionOffset, point.Y + offsetY + length + rotationY1+ positionOffset), new PointF(point.X + offsetX - length + positionOffset, point.Y + offsetY - length + positionOffset));
   //                     }
   //                 }
   //             }
   //         }

         

			innerBrush.TranslateTransform(offset.X, offset.Y);
			//graphics.FillPolygon(innerBrush, polyPoints.ToArray());
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(248, 248, 244)), polyPoints.ToArray());
			innerBrush.ResetTransform();

			var lastPoint = outline[outline.Count - 1].Item1;

			foreach (var pair in outline)
			{
				var point = pair.Item1;

				if (pair.Item2)
				{

					graphics.DrawLine(outlinePen, lastPoint.X, lastPoint.Y, point.X, point.Y);
				}

				lastPoint = point;
			}
		}

        protected override void DrawRoomBefore(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
        {
            var pen = new Pen(Color.Black, 0.50f * scale);
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var usedPointsAdd = new List<Vector2>();
            

            foreach (var line in polygon.GetLines())
            {
                var points = line.GetPoints();

                //for (int i = 0; i < 5; i++)
                //{
                //    points.Add(points.Last() + line.GetDirectionVector()); 
                //}
                
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    if (i % (6 * scale) == 0 || i == points.Count - 1)
                    {
                        if (points.Count - 1 - i < 5)
                        {
                            continue;
                        }

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
                                var length = scale * 3f;
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

		/// <inheritdoc />
		protected override void DrawTextOntoPolygon(PolygonGrid2D polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioning.GetPartitions(polygon);
			var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

			using (var font = new Font("Arial", penWidth, FontStyle.Regular, GraphicsUnit.Pixel))
			{
				var rect = new RectangleF(
					biggestRectangle.A.X,
					biggestRectangle.A.Y,
					biggestRectangle.Width,
					biggestRectangle.Height
				);

				var sf = new StringFormat
				{
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Center
				};

				graphics.DrawString(text, font, Brushes.Black, rect, sf);
			}
		}
	}
}