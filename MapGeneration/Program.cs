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
				GridPolygonUtils.GetRectangle(3, 5),
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

			ILayoutGenerator<Layout<int>, GridPolygon, int> generator = new LayoutGenerator<int>(configuartionSpacesGenerator.Generate(polygons));
			var layouts = generator.GetLayouts(DummyGraphDecomposer<int>.DummyGraph2, (layout) =>
			{
				/*this.layout = layout;
				canvas.Refresh();
				Thread.Sleep(5);*/
			}, 10);
		}
	}
}
