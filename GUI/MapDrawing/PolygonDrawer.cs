namespace GUI.MapDrawing
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class PolygonDrawer
	{
		public static void DrawPolygon<TNode>(GridPolygon polygon, PictureBox canvas, PaintEventArgs e) where TNode : IComparable<TNode>
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point(10 * point.X, 10 * point.Y)).ToList();
			e.Graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());
		}
	}
}