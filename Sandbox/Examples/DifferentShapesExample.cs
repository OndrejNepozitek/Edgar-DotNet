namespace Sandbox.Examples
{
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;

	public class DifferentShapesExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			var mapDescription = new MapDescription<int>();
			mapDescription.SetupWithGraph(GraphsDatabase.GetExample2());

			// Add room shapes
			var doorMode = new OverlapMode(1, 1);

			var squareRoom = new RoomDescription(
				GridPolygon.GetSquare(8),
				doorMode
			);
			var rectangleRoom = new RoomDescription(
				GridPolygon.GetRectangle(6, 10),
				doorMode
			);

			mapDescription.AddRoomShapes(squareRoom);
			mapDescription.AddRoomShapes(rectangleRoom);

			// Add boss room shape
			var bossRoom = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(2, 0).AddPoint(2, 1).AddPoint(1, 1).AddPoint(1, 2)
					.AddPoint(0, 2).AddPoint(0, 7).AddPoint(1, 7).AddPoint(1, 8)
					.AddPoint(2, 8).AddPoint(2, 9).AddPoint(7, 9).AddPoint(7, 8)
					.AddPoint(8, 8).AddPoint(8, 7).AddPoint(9, 7).AddPoint(9, 2)
					.AddPoint(8, 2).AddPoint(8, 1).AddPoint(7, 1).AddPoint(7, 0)
				.Build().Scale(new IntVector2(2, 2)),
				new OverlapMode(1, 1)
			);

			mapDescription.AddRoomShapes(8, bossRoom);

			return mapDescription;
		}
	}
}