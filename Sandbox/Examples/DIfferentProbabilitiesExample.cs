namespace Sandbox.Examples
{
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;
	using Utils;

	public class DIfferentProbabilitiesExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			var mapDescription = new MapDescription<int>();
			mapDescription.SetupWithGraph(GraphsDatabase.GetExample5());

			// Add room shapes
			var doorMode = new OverlapMode(1, 1);

			var squareRoomBig = new RoomDescription(
				GridPolygon.GetSquare(8),
				doorMode
			);
			var squareRoomSmall = new RoomDescription(
				GridPolygon.GetSquare(6),
				doorMode
			);
			var rectangleRoomBig = new RoomDescription(
				GridPolygon.GetRectangle(8, 12),
				doorMode
			);
			var rectangleRoomSmall = new RoomDescription(
				GridPolygon.GetRectangle(6, 10),
				doorMode
			);

			mapDescription.AddRoomShapes(squareRoomBig, probability: 10);
			mapDescription.AddRoomShapes(squareRoomSmall);
			mapDescription.AddRoomShapes(rectangleRoomBig);
			mapDescription.AddRoomShapes(rectangleRoomSmall);

			return mapDescription;
		}
	}
}