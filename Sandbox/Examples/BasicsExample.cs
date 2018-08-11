namespace Sandbox.Examples
{
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;

	public class BasicsExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			var mapDescription = new MapDescription<int>();

			// Add rooms ( - you would normally use a for cycle)
			mapDescription.AddRoom(0);
			mapDescription.AddRoom(1);
			mapDescription.AddRoom(2);
			mapDescription.AddRoom(3);

			// Add passages
			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(0, 3);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(2, 3);

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

			return mapDescription;
		}
	}
}