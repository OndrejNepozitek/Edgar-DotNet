namespace GUI.MapDrawing
{
	using System;
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
				var polygon = new GridPolygon(polygons[i].GetPoints().Select(point => TransformPoint(point, scale, offset)));
				DrawRoom(polygon, 3);

				if (rooms[i].Doors == null)
					continue;

				foreach (var door in rooms[i].Doors)
				{
					DrawDoorLine(new OrthogonalLine(TransformPoint(door.Item2.From, scale, offset), TransformPoint(door.Item2.To, scale, offset)), 3);
				}

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

		protected abstract void DrawRoom(GridPolygon polygon, float penWidth);

		protected abstract void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth);

		protected abstract void DrawDoorLine(OrthogonalLine line, float penWidth);
	}
}