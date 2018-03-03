namespace GUI.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Interfaces.Core;
	using MapGeneration.Utils.MapDrawing;

	public class OldMapDrawer<TNode> : AbstractLayoutDrawer<TNode>
	{
		private readonly CachedPolygonPartitioning polygonPartitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());
		private Bitmap bitmap;
		private Graphics graphics;

		private const string TextureInnerPath = @"Resources\Images\texture_inner.png";
		private const string TextureOutterPath = @"Resources\Images\texture_outter.png";
		private const string TexturePenPath = @"Resources\Images\texture_pen.png";

		private TextureBrush innerBrush;
		private TextureBrush outlineBrush;
		private Pen outlinePen;

		public new Bitmap DrawLayout(IMapLayout<TNode> layout, int width, int height, bool withNames = true)
		{
			bitmap = new Bitmap(width, height);
			graphics = Graphics.FromImage(bitmap);

			var textureImgOuter = new Bitmap(TextureOutterPath);
			using (var brush = new TextureBrush(textureImgOuter, WrapMode.Tile))
			{
				graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
			}

			var textureImgInner = new Bitmap(TextureInnerPath);
			innerBrush = new TextureBrush(textureImgInner, WrapMode.Tile);

			var textureImgOutline = new Bitmap(TexturePenPath);
			outlineBrush = new TextureBrush(textureImgOutline, WrapMode.Tile);

			outlinePen = new Pen(outlineBrush, 2)
			{
				EndCap = LineCap.Square,
				StartCap = LineCap.Square
			};

			base.DrawLayout(layout, width, height, withNames);

			textureImgInner.Dispose();
			innerBrush.Dispose();

			textureImgOutline.Dispose();
			outlineBrush.Dispose();
			outlinePen.Dispose();

			return bitmap; // TODO: memory leak?
		}

		protected override void DrawRoom(GridPolygon polygon, List<Tuple<IntVector2, bool>> outline, float penWidth)
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			var offset = polygon.BoundingRectangle.A;

			innerBrush.TranslateTransform(offset.X, offset.Y);
			graphics.FillPolygon(innerBrush, polyPoints.ToArray());
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

		protected override void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth)
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