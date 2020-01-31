using System;

namespace Sandbox.Examples
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;
	using MapGeneration.Utils;
	using Utils;

	public class CorridorsExample : IExample
	{
		public MapDescription<int> GetMapDescription()
		{
			//var mapDescription = new MapDescriptionOld<int>();
			//mapDescription.SetupWithGraph(GraphsDatabase.GetExample1());

			//// Add room shapes
			//var doorMode = new OverlapMode(1, 1);

			//var squareRoom = new RoomTemplate(
			//	GridPolygon.GetSquare(8),
			//	doorMode
			//);
			//var rectangleRoom = new RoomTemplate(
			//	GridPolygon.GetRectangle(6, 10),
			//	doorMode
			//);

			//mapDescription.AddRoomShapes(squareRoom);
			//mapDescription.AddRoomShapes(rectangleRoom);

			//// Setup corridor shapes
			//var corridorRoom = new RoomTemplate(
			//	GridPolygon.GetSquare(1),
			//	new SpecificPositionsMode(new List<OrthogonalLine>()
			//	{
			//		new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
			//		new OrthogonalLine(new IntVector2(0, 1), new IntVector2(1, 1))
			//	})
			//);

			//mapDescription.AddCorridorShapes(corridorRoom);

			//// Enable corridors
			//mapDescription.SetWithCorridors(true, new List<int>() { 1 });

			//return mapDescription;
			// TODO:
            throw new NotImplementedException();
		}
	}
}