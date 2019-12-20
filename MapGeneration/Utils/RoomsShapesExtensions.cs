namespace Sandbox.Utils
{
	using System;
	using System.Collections.Generic;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorModes;
	using MapGeneration.Core.MapDescriptions;

	public static class RoomsShapesExtensions
	{
		/// <summary>
		/// Adds basic room shapes to a given map description.
		/// </summary>
		/// <typeparam name="TNode"></typeparam>
		/// <param name="mapDescriptionOld"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static MapDescriptionOld<TNode> AddClassicRoomShapes<TNode>(this MapDescriptionOld<TNode> mapDescriptionOld,
			IntVector2 scale)
		{
			var overlapScale = Math.Min(scale.X, scale.Y);
			var doorMode = new OverlapMode(1 * overlapScale, 0);

			var squareRoom = new RoomTemplate(GridPolygon.GetSquare(6).Scale(scale), doorMode);
			var rectangleRoom = new RoomTemplate(GridPolygon.GetRectangle(6, 9).Scale(scale), doorMode);
			var room1 = new RoomTemplate(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 6)
					.AddPoint(3, 6)
					.AddPoint(3, 3)
					.AddPoint(6, 3)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, doorMode);
			var room2 = new RoomTemplate(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 9)
					.AddPoint(3, 9)
					.AddPoint(3, 3)
					.AddPoint(6, 3)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, doorMode);
			var room3 = new RoomTemplate(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 3)
					.AddPoint(3, 3)
					.AddPoint(3, 6)
					.AddPoint(6, 6)
					.AddPoint(6, 3)
					.AddPoint(9, 3)
					.AddPoint(9, 0)
					.Build().Scale(scale)
				, doorMode);

			mapDescriptionOld.AddRoomShapes(squareRoom, probability: 4);
			mapDescriptionOld.AddRoomShapes(rectangleRoom, probability: 2);
			mapDescriptionOld.AddRoomShapes(room1);
			mapDescriptionOld.AddRoomShapes(room2);
			mapDescriptionOld.AddRoomShapes(room3);

			return mapDescriptionOld;
		}

		/// <summary>
		/// Adds basic corridor room shape to a given map description.
		/// </summary>
		/// <typeparam name="TNode"></typeparam>
		/// <param name="mapDescriptionOld"></param>
		/// <param name="offsets"></param>
		/// <param name="enableCorridors"></param>
		/// <returns></returns>
		public static MapDescriptionOld<TNode> AddCorridorRoomShapes<TNode>(this MapDescriptionOld<TNode> mapDescriptionOld, List<int> offsets, bool enableCorridors = true)
		{
			foreach (var offset in offsets)
			{
				var width = offset;
				var room = new RoomTemplate(
					GridPolygon.GetRectangle(width, 1),
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
						new OrthogonalLine(new IntVector2(width, 0), new IntVector2(width, 1)),
					})
				);

				mapDescriptionOld.AddCorridorShapes(room);
			}

			if (enableCorridors)
			{
				mapDescriptionOld.SetWithCorridors(true, offsets);
			}
			
			return mapDescriptionOld;
		}
	}
}