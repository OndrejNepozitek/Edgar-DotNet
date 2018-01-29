namespace MapGeneration
{
	using System;
	using Benchmarks;
	using Core;
	using Core.ConfigurationSpaces;
	using Core.Doors;
	using Core.Interfaces;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using Utils;

	internal class Program
	{
		private static void Main(string[] args)
		{
			var benchmark = new Benchmark();

			{
				var mapDescription = MapDescriptionsDatabase.Reference_17Vertices_WithoutRoomShapes;
				MapDescriptionsDatabase.AddClassicRoomShapes(mapDescription);

				var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler,
					new OrthogonalLineIntersection(), new GridPolygonUtils());

				var configurationSpaces = configurationSpacesGenerator.Generate(mapDescription);
				var layoutGenerator = new SALayoutGenerator<int>();
				layoutGenerator.InjectRandomGenerator(new Random(0));

				benchmark.Execute(layoutGenerator, "Basic generator");
			}
		}
	}
}
