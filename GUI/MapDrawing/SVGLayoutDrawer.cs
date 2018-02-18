namespace GUI.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Interfaces;

	public class SVGLayoutDrawer<TNode> : AbstractLayoutDrawer<TNode>
	{
		private readonly CachedPolygonPartitioner polygonPartitioner = new CachedPolygonPartitioner();
		private readonly StringBuilder data = new StringBuilder();

		public string DrawLayout(IMapLayout<TNode> layout)
		{
			const int width = 800;
			var ratio = GetWidthHeightRatio(layout);
			var height = (int) (width / ratio);

			data.AppendLine($"<svg width=\"{width}\" height=\"{height}\" xmlns=\"http://www.w3.org/2000/svg\">");

			DrawLayout(layout, width, height, true);

			data.AppendLine("</svg>");

			var svgData = data.ToString();
			data.Clear();

			return svgData;
		}

		private float GetWidthHeightRatio(IMapLayout<TNode> layout)
		{
			var polygons = layout.GetRooms().Select(x => x.Shape + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			var ratio = (maxx - minx) / (float) (maxy - miny);

			return ratio;
		}

		protected override void DrawRoom(GridPolygon polygon, List<Tuple<IntVector2, bool>> outline, float penWidth)
		{
			// Draw polygon
			data.Append($"    <polygon points=\"");

			foreach (var point in polygon.GetPoints())
			{
				data.Append($"{point.X},{point.Y} ");
			}

			data.Append($"\" style=\"fill:rgb(211,211,211);stroke:rgb(211,211,211)\" />");
			data.AppendLine();

			// Draw path
			data.Append($"    <path d=\"");

			var lastPoint = outline[outline.Count - 1].Item1;
			data.Append($"M {lastPoint.X} {lastPoint.Y} ");

			foreach (var pair in outline)
			{
				var point = pair.Item1;

				data.Append(pair.Item2 ? $"L {point.X} {point.Y} " : $"M {point.X} {point.Y} ");
			}

			data.Append($"\" fill=\"none\" stroke=\"black\" stroke-linecap=\"square\" stroke-width=\"{penWidth}\" />");
			data.AppendLine();
		}

		protected override void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioner.GetPartitions(polygon);
			var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

			data.AppendLine(
				$"    <text x=\"{biggestRectangle.Center.X}\" y=\"{biggestRectangle.Center.Y}\" alignment-baseline=\"middle\" text-anchor=\"middle\" font-size=\"{(int) (1.2 * penWidth)}\">{text}</text>");
		}
	}
}