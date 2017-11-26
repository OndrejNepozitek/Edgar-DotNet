namespace GUI.MapDrawing
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class PolygonDrawer
	{
		public static void DrawPolygon<TNode>(GridPolygon polygon, PictureBox canvas, PaintEventArgs e, IntVector2 offset = new IntVector2(), float scale = 1) where TNode : IComparable<TNode>
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point((int) (scale * point.X + offset.X), (int) (scale * point.Y + offset.Y))).ToList();
			e.Graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());
			e.Graphics.DrawPolygon(Pens.DarkBlue, polyPoints.ToArray());
		}
	}
}