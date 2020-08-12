using MapGeneration.Core.MapLayouts;

namespace MapGeneration.Utils.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

    /// <summary>
	/// Class that should help with drawing layouts to different outputs.
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public abstract class AbstractLayoutDrawer<TNode>
	{
		/// <summary>
		/// Entry point of the class. Draws a given layout to an output with given dimensions.
		/// </summary>
		/// <param name="layout">Layout do be drawn</param>
		/// <param name="width">Width of the output</param>
		/// <param name="height">Height of the output</param>
		/// <param name="withNames">Whether names should be displayed</param>
		/// <param name="fixedFontSize"></param>
		/// <param name="borderSize"></param>
		protected void DrawLayout(MapLayout<TNode> layout, int width, int height, bool withNames, int? fixedFontSize = null, float borderSize = 0.2f)
		{
			var polygons = layout.Rooms.Select(x => x.Shape + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			var scale = GetScale(minx, miny, maxx, maxy, width, height, borderSize);
			var offset = GetOffset(minx, miny, maxx, maxy, width, height, scale);

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
		protected void DrawLayout(MapLayout<TNode> layout, float scale, Vector2Int offset, bool withNames, int? fixedFontSize = null)
		{
			var polygons = layout.Rooms.Select(x => x.Shape + x.Position).ToList();
			var rooms = layout.Rooms.ToList();
			var minWidth = layout.Rooms.Where(x => !x.IsCorridor).Select(x => x.Shape + x.Position).Min(x => x.BoundingRectangle.Width);

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
		protected Vector2Int TransformPoint(Vector2Int point, float scale, Vector2Int offset)
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
		public Vector2Int GetOffset(int minx, int miny, int maxx, int maxy, int width, int height, float scale = 1)
		{
			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new Vector2Int((int)(width / 2f - centerx), (int)(height / 2f - centery));
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
		/// Draws a given room.
		/// </summary>
		/// <param name="polygon">Polygon to be drawn.</param>
		/// <param name="outline">Outline of the room. The bool signals whether a line should be drawn (polygon side) or not (doors).</param>
		/// <param name="penWidth"></param>
		protected abstract void DrawRoom(PolygonGrid2D polygon, List<Tuple<Vector2Int, bool>> outline, float penWidth);

		/// <summary>
		/// Draws text onto given polygon.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="text"></param>
		/// <param name="penWidth"></param>
		protected abstract void DrawTextOntoPolygon(PolygonGrid2D polygon, string text, float penWidth);

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