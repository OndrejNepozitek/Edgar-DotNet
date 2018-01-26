namespace GUI
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapDrawing;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Grid.Fast;
	using MapGeneration.Interfaces;

	public partial class Form1 : Form
	{
		private ILayout<int, GridPolygon, IntVector2, OrthogonalLine> layout;
		private readonly LayoutDrawer layoutDrawer = new LayoutDrawer();

		public Form1()
		{
			InitializeComponent();

		}

		private void canvas_Paint(object sender, PaintEventArgs e)
		{
			if (layout == null)
			{
				return;
			}

			layoutDrawer.DrawLayout(layout, canvas, e);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{

			var configuartionSpacesGenerator = new ConfigurationSpacesGenerator();
			var polygons = new List<GridPolygon>()
			{
				GridPolygon.GetSquare(3),
				GridPolygon.GetRectangle(3, 5),
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build(),
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(4, 2)
					.AddPoint(4, 0)
					.Build(),
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 2)
					.AddPoint(2, 2)
					.AddPoint(2, 4)
					.AddPoint(4, 4)
					.AddPoint(4, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build()

				/*GridPolygonUtils.GetSquare(3),
				//GridPolygonUtils.GetSquare(6),
				GridPolygonUtils.GetRectangle(2, 4),
				GridPolygonUtils.GetRectangle(3, 4),
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 6)
					.AddPoint(3, 6)
					.AddPoint(3, 3)
					.AddPoint(6, 3)
					.AddPoint(6, 0)
					.Build(),*/
			};

			polygons = polygons.Select(x => x.Scale(new IntVector2(3, 3))).ToList();

			var generator = new LayoutGenerator<int>(configuartionSpacesGenerator.Generate(polygons, true, true));
			generator.EnableDebug(true);

			/*generator.OnPerturbed += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(50);
			};*/

			/*generator.OnValid += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(100);
			};*/

			/*generator.OnValidAndDifferent += (l) =>
			{
				layout = l;
				canvas.Invoke((Action)(() => canvas.Refresh()));
				Thread.Sleep(1500);
			};*/

			Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{

					var layouts = generator.GetLayouts(DummyGraphDecomposer<int>.DummyGraph3);

					foreach (var layout in layouts)
					{
						this.layout = layout;
						canvas.Invoke((Action)(() => canvas.Refresh()));
						Thread.Sleep(2000);
					}
				}
			});
		}
	}
}
