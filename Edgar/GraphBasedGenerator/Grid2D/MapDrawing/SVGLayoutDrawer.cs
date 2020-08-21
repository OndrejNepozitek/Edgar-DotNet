using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edgar.Geometry;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D.MapDrawing
{
    /// <inheritdoc />
	/// <summary>
	/// Class to draw a layout as an SVG.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public class SVGLayoutDrawer<TNode> : AbstractLayoutDrawer<TNode>
	{
		private readonly CachedPolygonPartitioning polygonPartitioning = new CachedPolygonPartitioning(new GridPolygonPartitioning());
		private readonly StringBuilder data = new StringBuilder();

        private int width;
        private int height;
        private bool flipY;

        /// <summary>
        /// Draws a given layout and returns a string with SVG data.
        /// </summary>
        /// <param name="layout">Layout that should be drawn</param>
        /// <param name="width">Width of the SVG</param>
        /// <param name="showRoomNames">Whether to show rooms names</param>
        /// <param name="fixedFontSize">What should be the font size of room names</param>
        /// <param name="forceSquare">Whether to force the output to have 1:1 aspect ration</param>
        /// <param name="flipY">Whether to flip ty Y axis</param>
        /// <returns></returns>
        public string DrawLayout(LayoutGrid2D<TNode> layout, int width, bool showRoomNames = true, int? fixedFontSize = null, bool forceSquare = false, bool flipY = false)
		{
			if (width <= 0)
				throw new ArgumentException("Width must be greater than zero.", nameof(width));

			var ratio = GetWidthHeightRatio(layout);
			var height = forceSquare ? width : (int) (width / ratio);

            data.AppendLine($"<svg viewBox=\"0 0 {width} {height}\" xmlns=\"http://www.w3.org/2000/svg\">");

            this.width = width;
            this.height = height;
            this.flipY = flipY;

			DrawLayout(layout, width, height, showRoomNames, fixedFontSize, 0.1f);

			data.AppendLine("</svg>");

			var svgData = data.ToString();
			data.Clear();

			return svgData;
		}

		private float GetWidthHeightRatio(LayoutGrid2D<TNode> layout)
		{
			var polygons = layout.Rooms.Select(x => x.Outline + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			var ratio = (maxx - minx) / (float) (maxy - miny);

			return ratio;
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="outline"></param>
		/// <param name="penWidth"></param>
		protected override void DrawRoom(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth)
		{
			// Draw polygon
			data.Append($"    <polygon points=\"");

			foreach (var point in polygon.GetPoints())
			{
				data.Append($"{GetX(point)},{GetY(point)} ");
			}

			data.Append($"\" style=\"fill:rgb(211,211,211);stroke:rgb(211,211,211)\" />");
			data.AppendLine();

			// Draw path
			data.Append($"    <path d=\"");

			var lastPoint = outline[outline.Count - 1].Item1;
			data.Append($"M {GetX(lastPoint)} {GetY(lastPoint)} ");

			foreach (var pair in outline)
			{
				var point = pair.Item1;

				data.Append(pair.Item2 ? $"L {GetX(point)} {GetY(point)} " : $"M {GetX(point)} {GetY(point)} ");
			}

			data.Append($"\" fill=\"none\" stroke=\"black\" stroke-linecap=\"square\" stroke-width=\"{penWidth}\" />");
			data.AppendLine();
		}

        private int GetY(Vector2Int point)
        {
            if (flipY)
            {
                return height - point.Y;
            }

            return point.Y;
        }

        private int GetX(Vector2Int point)
        {
            return point.X;
        }

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="text"></param>
		/// <param name="penWidth"></param>
		protected override void DrawTextOntoPolygon(PolygonGrid2D polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioning.GetPartitions(polygon);
			var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

			data.AppendLine(
				$"    <text x=\"{GetX(biggestRectangle.Center)}\" y=\"{GetY(biggestRectangle.Center)}\" alignment-baseline=\"middle\" text-anchor=\"middle\" style=\"font-family: arial;\" font-size=\"{(int) (1.2 * penWidth)}\">{text}</text>");
		}
	}
}