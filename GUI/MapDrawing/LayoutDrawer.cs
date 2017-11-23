namespace GUI.MapDrawing
{
	using System;
	using System.Linq;
	using System.Windows.Forms;

	public class LayoutDrawer
	{
		public static void DrawLayout<TNode>(MapGeneration.Grid.Layout<TNode> layout, PictureBox canvas, PaintEventArgs e) where TNode : IComparable<TNode>
		{
			var minPoint = layout.GetAllConfigurations().Select(x => x.Position).Max();

			foreach (var configuration in layout.GetAllConfigurations())
			{
				PolygonDrawer.DrawPolygon<TNode>(configuration.Polygon + (configuration.Position - minPoint), canvas, e);
			}
		}
	}
}