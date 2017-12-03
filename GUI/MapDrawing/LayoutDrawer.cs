namespace GUI.MapDrawing
{
	using System;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Interfaces;

	public class LayoutDrawer
	{
		public static void DrawLayout<TNode>(ILayout<TNode, GridPolygon, IntVector2> layout, PictureBox canvas, PaintEventArgs e) where TNode : IComparable<TNode>
		{
			var polygons = layout.GetConfigurations().Select(x => x.Shape + x.Position).ToList();
			var points = polygons.SelectMany(x => x.GetPoints()).ToList();

			var minx = points.Min(x => x.X);
			var miny = points.Min(x => x.Y);
			var maxx = points.Max(x => x.X);
			var maxy = points.Max(x => x.Y);
			var scale = GetScale(minx, miny, maxx, maxy, canvas);

			var offset = GetOffset(minx, miny, maxx, maxy, canvas, scale);

			foreach (var polygon in polygons)
			{
				PolygonDrawer.DrawPolygon<TNode>(polygon, canvas, e, offset, scale);
			}
		}

		private static IntVector2 GetOffset(int minx, int miny, int maxx, int maxy, PictureBox canvas, float scale = 1)
		{
			var width = canvas.Width;
			var height = canvas.Height;

			var centerx = scale * (maxx + minx) / 2;
			var centery = scale * (maxy + miny) / 2;

			return new IntVector2((int) (width / 2f - centerx), (int) (height / 2f - centery));
		}

		private static float GetScale(int minx, int miny, int maxx, int maxy, PictureBox canvas)
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