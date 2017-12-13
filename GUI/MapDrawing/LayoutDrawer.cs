namespace GUI.MapDrawing
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Interfaces;

	public class LayoutDrawer
	{
		private readonly PolygonDrawer polygonDrawer = new PolygonDrawer();

		public void DrawLayout<TNode>(ILayout<TNode, GridPolygon, IntVector2, IntLine> layout, PictureBox canvas, PaintEventArgs e) where TNode : IComparable<TNode>
		{
			var polygons = layout.GetConfigurations().Select(x => x.Shape + x.Position).ToList();
			var rooms = layout.GetRooms().ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);
			var scale = GetScale(minx, miny, maxx, maxy, canvas);

			var offset = GetOffset(minx, miny, maxx, maxy, canvas, scale);

			for (var i = 0; i < rooms.Count; i++)
			{
				polygonDrawer.DrawPolygon<TNode>(polygons[i], canvas, e, offset, scale/*, rooms[i].Node.ToString()*/);
			}

			if (layout.GetDoors() == null)
				return;

			foreach (var door in layout.GetDoors())
			{
				e.Graphics.DrawLine(
					new Pen(Color.LightGray, 3), 
					new Point((int)(scale * door.From.X + offset.X), (int)(scale * door.From.Y + offset.Y)),
					new Point((int)(scale * door.To.X + offset.X), (int)(scale * door.To.Y + offset.Y)));
			}
		}

		private IntVector2 GetOffset(int minx, int miny, int maxx, int maxy, PictureBox canvas, float scale = 1)
		{
			var width = canvas.Width;
			var height = canvas.Height;

			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new IntVector2((int) (width / 2f - centerx), (int) (height / 2f - centery));
		}

		private float GetScale(int minx, int miny, int maxx, int maxy, PictureBox canvas)
		{
			var actualX = canvas.Width;
			var actualY = canvas.Height;

			var neededX = 1.2f * (maxx - minx);
			var neededY = 1.2f * (maxy - miny);

			var scale = actualX / neededX;
			if (scale * neededY > actualY)
			{
				scale = actualY / neededY;
			}

			return scale;
		}
	}
}