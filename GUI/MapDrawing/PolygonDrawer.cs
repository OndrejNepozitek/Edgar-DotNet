namespace GUI.MapDrawing
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class PolygonDrawer
	{
		private readonly CachedPolygonPartitioner polygonPartitioner = new CachedPolygonPartitioner();

		public void DrawPolygon<TNode>(GridPolygon polygon, PictureBox canvas, PaintEventArgs e, IntVector2 offset = new IntVector2(), float scale = 1, string label = null) where TNode : IComparable<TNode>
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point((int) (scale * point.X + offset.X), (int) (scale * point.Y + offset.Y))).ToList();
			e.Graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());
			e.Graphics.DrawPolygon(new Pen(Color.Black, 3), polyPoints.ToArray());

			if (label != null)
			{
				var partitions = polygonPartitioner.GetPartitions(polygon);
				var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

				using (var font1 = new Font("Arial", (int)(scale * 4), FontStyle.Regular, GraphicsUnit.Point))
				{
					var rectF1 = new RectangleF(
						(int)(scale * biggestRectangle.A.X + offset.X),
						(int)(scale * biggestRectangle.A.Y + offset.Y),
						scale * biggestRectangle.Width,
						scale * biggestRectangle.Height
					);

					var sf = new StringFormat
					{
						LineAlignment = StringAlignment.Center,
						Alignment = StringAlignment.Center
					};
					e.Graphics.DrawString(label, font1, Brushes.Black, rectF1, sf);
				}
			}
		}
	}
}