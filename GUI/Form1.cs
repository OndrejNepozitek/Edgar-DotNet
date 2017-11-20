namespace GUI
{
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using MapDrawing;
	using MapGeneration.Grid;

	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void canvas_Paint(object sender, PaintEventArgs e)
		{
			var p1 = GridPolygonUtils.GetSquare(3);
			var p2 = GridPolygonUtils.GetSquare(5);
			var p3 = GridPolygonUtils.GetRectangle(5, 2);

			var layout = new Layout<int>();
			layout.SetConfiguration(1, new Configuration(p1, new IntVector2(1, 1)));
			layout.SetConfiguration(2, new Configuration(p2, new IntVector2(6, 1)));
			layout.SetConfiguration(3, new Configuration(p3, new IntVector2(10, 20)));

			LayoutDrawer.DrawLayout(layout, canvas, e);

			/*e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.Clear(Color.White);

			// draw the shading background:
			List<Point> shadePoints = new List<Point>();
			shadePoints.Add(new Point(0, canvas.ClientSize.Height));
			shadePoints.Add(new Point(canvas.ClientSize.Width, 0));
			shadePoints.Add(new Point(canvas.ClientSize.Width,
				canvas.ClientSize.Height));
			e.Graphics.FillPolygon(Brushes.LightGray, shadePoints.ToArray());

			// scale the drawing larger:
			using (Matrix m = new Matrix())
			{
				m.Scale(4, 4);
				e.Graphics.Transform = m;

				List<Point> polyPoints = new List<Point>();
				polyPoints.Add(new Point(10, 10));
				polyPoints.Add(new Point(12, 35));
				polyPoints.Add(new Point(22, 35));
				polyPoints.Add(new Point(24, 22));

				// use a semi-transparent background brush:
				using (SolidBrush br = new SolidBrush(Color.FromArgb(100, Color.Yellow)))
				{
					e.Graphics.FillPolygon(br, polyPoints.ToArray());
				}
				e.Graphics.DrawPolygon(Pens.DarkBlue, polyPoints.ToArray());

				foreach (Point p in polyPoints)
				{
					e.Graphics.FillEllipse(Brushes.Red,
						new Rectangle(p.X - 2, p.Y - 2, 4, 4));
				}
			}*/
		}
	}
}
