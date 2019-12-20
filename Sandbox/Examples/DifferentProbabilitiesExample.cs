namespace Sandbox.Examples
{
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;
	using Utils;

	public class DifferentProbabilitiesExample : IExample
	{
		public MapDescriptionOld<int> GetMapDescription()
		{
			var mapDescription = new MapDescriptionOld<int>();
			mapDescription.SetupWithGraph(GraphsDatabase.GetExample5());

			// Add room shapes
			var doorMode = new OverlapMode(1, 1);

			var squareRoomBig = new RoomTemplate(
				GridPolygon.GetSquare(8),
				doorMode
			);
			var squareRoomSmall = new RoomTemplate(
				GridPolygon.GetSquare(6),
				doorMode
			);
			var rectangleRoomBig = new RoomTemplate(
				GridPolygon.GetRectangle(8, 12),
				doorMode
			);
			var rectangleRoomSmall = new RoomTemplate(
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