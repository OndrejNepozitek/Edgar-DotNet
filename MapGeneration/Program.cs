namespace MapGeneration
{
	using Core;
	using Core.ConfigurationSpaces;
	using Core.Doors;
	using Core.Doors.DoorModes;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Polygons;

	internal class Program
	{
		private static void Main(string[] args)
		{
			var configurationSpacesGenerator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler,
				new OrthogonalLineIntersection(), new GridPolygonUtils());

			var squareRoom = new RoomDescription(GridPolygon.GetSquare(4), new OverlapMode(1, 1));
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(3, 5), new OverlapMode(1, 1));

			var mapDescription = new MapDescription<int>();

			mapDescription.AddRoom(0);
			mapDescription.AddRoom(1);
			mapDescription.AddRoom(2);
			mapDescription.AddRoom(3);

			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(0, 2);
			mapDescription.AddPassage(2, 3);

			mapDescription.AddRoomShapes(squareRoom);
			mapDescription.AddRoomShapes(rectangleRoom);

			var configurationSpaces = configurationSpacesGenerator.Generate(mapDescription);
			var layoutGenerator = new SALayoutGenerator<int>(configurationSpaces);

			var layouts = layoutGenerator.GetLayouts(mapDescription, 10);

			/*var configuartionSpacesGenerator = new ConfigSpacesGenerator();
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
			};

			polygons = polygons.Select(x => x.Scale(new IntVector2(4, 4))).ToList();
			var benchmark = new Benchmark();

			{
				var generator = new LayoutGenerator<int>(configuartionSpacesGenerator.Generate(polygons));
				benchmark.Execute<GridPolygon, IntVector2, AbstractLayoutGenerator<int, GridPolygon, IntVector2>>(generator, "Basic generator");
			}*/

		}
	}
}
