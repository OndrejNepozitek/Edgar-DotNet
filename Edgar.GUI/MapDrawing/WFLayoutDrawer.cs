using MapGeneration.Core.MapLayouts;

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
    using MapGeneration.Utils.MapDrawing;

	/// <inheritdoc />
	/// <summary>
	/// Class to draw a layout to a given PictureBox.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class WFLayoutDrawer<TNode> : AbstractLayoutDrawer<TNode>
	{
		private readonly CachedPolygonPartitioning polygonPartitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());
		private Bitmap bitmap;
		private Graphics graphics;

		/// <summary>
		/// Draws the layout to a given PictureBox.
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="height"></param>
		/// <param name="withNames"></param>
		/// <param name="fixedFontSize"></param>
		/// <param name="width"></param>
		public Bitmap DrawLayout(MapLayout<TNode> layout, int width, int height, bool withNames = true, int? fixedFontSize = null, float? scale = null, Vector2Int? offset = null)
		{
			bitmap = new Bitmap(width, height);
			graphics = Graphics.FromImage(bitmap);

			if (scale != null || offset != null)
			{
				if (scale == null || offset == null)
				{
					throw new ArgumentException("Scale and offset must either both be null or not-null");
				}

				base.DrawLayout(layout, scale.Value, offset.Value, withNames, fixedFontSize);
			}
			else
			{
				base.DrawLayout(layout, width, height, withNames, fixedFontSize);
			}

			return bitmap;
		}

		protected override void DrawRoom(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());

			var lastPoint = outline[outline.Count - 1].Item1;
			var pen = new Pen(Color.Black, penWidth)
			{
				EndCap = LineCap.Flat,
				StartCap = LineCap.Flat
			};

			foreach (var pair in outline)
			{
				var point = pair.Item1;

				if (pair.Item2)
				{
					
					graphics.DrawLine(pen, lastPoint.X, lastPoint.Y, point.X, point.Y);
				}

				lastPoint = point;
			}
		}

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