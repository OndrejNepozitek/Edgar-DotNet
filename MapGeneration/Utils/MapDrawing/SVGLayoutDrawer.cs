using MapGeneration.Core.MapLayouts;

namespace MapGeneration.Utils.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

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
		/// <param name="layout"></param>
		/// <param name="width">Result will have this width and height will be computed to match the layout.</param>
		/// <param name="showRoomNames"></param>
		/// <param name="fixedFontSize"></param>
		/// <returns></returns>
		public string DrawLayout(MapLayout<TNode> layout, int width, bool showRoomNames = true, int? fixedFontSize = null, bool forceSquare = false, bool flipY = false)
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

		private float GetWidthHeightRatio(MapLayout<TNode> layout)
		{
			var polygons = layout.Rooms.Select(x => x.Shape + x.Position).ToList();
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
		protected override void DrawRoom(GridPolygon polygon, List<Tuple<IntVector2, bool>> outline, float penWidth)
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

        private int GetY(IntVector2 point)
        {
            if (flipY)
            {
                return height - point.Y;
            }

            return point.Y;
        }

        private int GetX(IntVector2 point)
        {
            return point.X;
        }

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="text"></param>
		/// <param name="penWidth"></param>
		protected override void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioning.GetPartitions(polygon);
			var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

			data.AppendLine(
				$"    <text x=\"{GetX(biggestRectangle.Center)}\" y=\"{GetY(biggestRectangle.Center)}\" alignment-baseline=\"middle\" text-anchor=\"middle\" style=\"font-family: arial;\" font-size=\"{(int) (1.2 * penWidth)}\">{text}</text>");
		}
	}
}