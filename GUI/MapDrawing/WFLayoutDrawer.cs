namespace GUI.MapDrawing
{
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Interfaces;

	public class WFLayoutDrawer<TNode> : AbstractLayoutDrawer<TNode>
	{
		private readonly CachedPolygonPartitioner polygonPartitioner = new CachedPolygonPartitioner();
		private PictureBox canvas;
		private PaintEventArgs eventArgs;

		public void DrawLayout(IMapLayout<TNode> layout, PictureBox canvas, PaintEventArgs eventArgs, bool withNames = true)
		{
			var width = canvas.Width;
			var height = canvas.Height;

			this.canvas = canvas;
			this.eventArgs = eventArgs;

			DrawLayout(layout, width, height, withNames);
		}

		protected void DrawPolygon(GridPolygon polygon, float penWidth)
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			eventArgs.Graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());
			eventArgs.Graphics.DrawPolygon(new Pen(Color.Black, 3), polyPoints.ToArray());
		}

		protected override void DrawRoom(GridPolygon polygon, float penWidth)
		{
			DrawPolygon(polygon, penWidth);
		}

		protected override void DrawTextOntoPolygon(GridPolygon polygon, string text, float penWidth)
		{
			var partitions = polygonPartitioner.GetPartitions(polygon);
			var biggestRectangle = partitions.OrderByDescending(x => x.Width).First();

			using (var font = new Font("Arial", penWidth, FontStyle.Regular, GraphicsUnit.Pixel))
			{
				var rect = new RectangleF(
					biggestRectangle.A.X,
					biggestRectangle.A.Y,
					biggestRectangle.Width,
					biggestRectangle.Height
				);

				var sf = new StringFormat
				{
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Center
				};

				eventArgs.Graphics.DrawString(text, font, Brushes.Black, rect, sf);
			}
		}

		protected override void DrawDoorLine(OrthogonalLine line, float penWidth)
		{
			throw new System.NotImplementedException();
		}
	}
}