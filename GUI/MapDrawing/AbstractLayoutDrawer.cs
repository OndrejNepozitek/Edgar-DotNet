namespace GUI.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Interfaces;

	public abstract class AbstractLayoutDrawer<TNode>
	{
		protected void DrawLayout(IMapLayout<TNode> layout, int width, int height, bool withNames)
		{
			var polygons = layout.GetRooms().Select(x => x.Shape + x.Position).ToList();
			var rooms = layout.GetRooms().ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);

			var scale = GetScale(minx, miny, maxx, maxy, width, height);
			var offset = GetOffset(minx, miny, maxx, maxy, width, height, scale);

			var minWidth = polygons.Min(x => x.BoundingRectangle.Width);

			for (var i = 0; i < rooms.Count; i++)
			{
				var outline = GetOutline(polygons[i], rooms[i].Doors?.Select(x => x.Item2).ToList())
					.Select(x => Tuple.Create(TransformPoint(x.Item1, scale, offset), x.Item2)).ToList();

				var polygon = new GridPolygon(polygons[i].GetPoints().Select(point => TransformPoint(point, scale, offset)));
				DrawRoom(polygon, outline, 2);

				if (withNames)
				{
					DrawTextOntoPolygon(polygon, rooms[i].Node.ToString(), 2.5f * minWidth);
				}
			}
		}

		protected IntVector2 TransformPoint(IntVector2 point, float scale, IntVector2 offset)
		{
			return new IntVector2((int)(scale * point.X + offset.X), (int)(scale * point.Y + offset.Y));
		}

		private IntVector2 GetOffset(int minx, int miny, int maxx, int maxy, int width, int height, float scale = 1)
		{
			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new IntVector2((int)(width / 2f - centerx), (int)(height / 2f - centery));
		}

		private float GetScale(int minx, int miny, int maxx, int maxy, int actualWidth, int actualHeight)
		{
			var neededWidth = 1.2f * (maxx - minx);
			var neededHeight = 1.2f * (maxy - miny);

			var scale = actualWidth / neededWidth;

			if (scale * neededHeight > actualHeight)
			{
				scale = actualHeight / neededHeight;
			}

			return scale;
		}

		protected abstract void DrawRoom(GridPolygon polygon, List<Tuple<IntVector2, bool>> outline, float penWidth);

		protected abstract void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth);

		protected List<Tuple<IntVector2, bool>> GetOutline(GridPolygon polygon, List<OrthogonalLine> doorLines)
		{
			var outline = new List<Tuple<IntVector2, bool>>();

			foreach (var line in polygon.GetLines())
			{
				outline.Add(Tuple.Create(line.From, true));

				if (doorLines == null)
					continue;

				var doorDistances = doorLines.Select(x =>
					new Tuple<OrthogonalLine, int>(x, Math.Min(line.Contains(x.From), line.Contains(x.To)))).ToList();
				doorDistances.Sort((x1, x2) => x1.Item2.CompareTo(x2.Item2));

				foreach (var pair in doorDistances)
				{
					if (pair.Item2 == -1)
						continue;

					var doorLine = pair.Item1;

					if (line.Contains(doorLine.From) != pair.Item2)
					{
						doorLine = doorLine.SwitchOrientation();
					}

					doorLines.Remove(doorLine);
					outline.Add(Tuple.Create(doorLine.From, true));
					outline.Add(Tuple.Create(doorLine.To, false));
				}
			}

			return outline;
		}
	}
}