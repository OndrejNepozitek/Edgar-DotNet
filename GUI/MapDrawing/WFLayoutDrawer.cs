namespace GUI.MapDrawing
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
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

		protected override void DrawRoom(GridPolygon polygon, List<Tuple<IntVector2, bool>> outline, float penWidth)
		{
			var polyPoints = polygon.GetPoints().Select(point => new Point(point.X, point.Y)).ToList();
			eventArgs.Graphics.FillPolygon(Brushes.LightGray, polyPoints.ToArray());

			var lastPoint = outline[outline.Count - 1].Item1;
			var pen = new Pen(Color.Black, penWidth)
			{
				EndCap = LineCap.Flat,
				StartCap = LineCap.Flat
			};


			foreach (var pair in outline)
			{
				var point = pair.Item1;

				if (pair.Item2)
				{
					
					eventArgs.Graphics.DrawLine(pen, lastPoint.X, lastPoint.Y, point.X, point.Y);
				}

				lastPoint = point;
			}
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

		private void DrawLine(OrthogonalLine line, float penWidth)
		{
			eventArgs.Graphics.DrawLine(new Pen(Color.LightGray, penWidth), new Point(line.From.X, line.From.Y), new Point(line.To.X, line.To.Y));
		}

		private void DrawPoint(IntVector2 point, Color color)
		{
			eventArgs.Graphics.FillRectangle(new SolidBrush(color), point.X, point.Y, 1, 1);
		}
	}
}