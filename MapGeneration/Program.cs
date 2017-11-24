namespace MapGeneration
{
	using System.Collections.Generic;
	using GeneralAlgorithms.Algorithms.Graphs.GraphDecomposition;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Grid;
	using Interfaces;

	internal class Program
	{
		private static void Main(string[] args)
		{
			var configuartionSpacesGenerator = new ConfigurationSpacesGenerator();
			var polygons = new List<GridPolygon>()
			{
				GridPolygonUtils.GetSquare(3),
				//GridPolygonUtils.GetSquare(6),
				GridPolygonUtils.GetRectangle(2, 4),
				//GridPolygonUtils.GetRectangle(3, 5),
			};

			ILayoutGenerator<Layout<int>, GridPolygon, int> generator = new LayoutGenerator<int>(configuartionSpacesGenerator.Generate(polygons));
			var layouts = generator.GetLayouts(DummyGraphDecomposer<int>.DummyGraph, (layout) =>
			{
				/*this.layout = layout;
				canvas.Refresh();
				Thread.Sleep(5);*/
			}, 30);
		}
	}
}
