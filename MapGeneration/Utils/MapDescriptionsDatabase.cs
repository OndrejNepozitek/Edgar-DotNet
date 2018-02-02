namespace MapGeneration.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Core.Doors.DoorModes;
	using Core.Interfaces;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class MapDescriptionsDatabase
	{ 

		public static MapDescription<int> Reference_9Vertices_WithoutRoomShapes => Make_Reference_9Vertices_WithoutRoomShapes();

		public static MapDescription<int> Reference_17Vertices_WithoutRoomShapes => Make_Reference_17Vertices_WithoutRoomShapes();

		public static MapDescription<int> Reference_41Vertices_WithoutRoomShapes => Make_Reference_41Vertices_WithoutRoomShapes();

		public static List<Tuple<string, IMapDescription<int>>> BasicSet;

		public static List<Tuple<string, IMapDescription<int>>> Reference_17Vertices_ScaledSet;

		static MapDescriptionsDatabase()
		{
			{
				var m1 = Reference_9Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m1);
				var m2 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m2);
				var m3 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m3);

				BasicSet = new List<Tuple<string, IMapDescription<int>>>()
				{
					Tuple.Create("Reference 9 vertices", (IMapDescription<int>) m1),
					Tuple.Create("Reference 17 vertices", (IMapDescription<int>) m2),
					Tuple.Create("Reference 41 vertices", (IMapDescription<int>) m3),
				};
			}

			{
				var m1 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m1, new IntVector2(2, 2));
				var m2 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m2, new IntVector2(3, 3));
				var m3 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m3, new IntVector2(5, 5));

				Reference_17Vertices_ScaledSet = new List<Tuple<string, IMapDescription<int>>>()
				{
					Tuple.Create("17 vertices - scale 2", (IMapDescription<int>) m1),
					Tuple.Create("17 vertices - scale 3", (IMapDescription<int>) m2),
					Tuple.Create("17 vertices - scale 5", (IMapDescription<int>) m3),
				};
			}
		}

		private static MapDescription<int> Make_Reference_9Vertices_WithoutRoomShapes()
		{
			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 9).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(0, 3);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(1, 4);
			mapDescription.AddPassage(1, 5);
			mapDescription.AddPassage(2, 3);
			mapDescription.AddPassage(3, 6);
			mapDescription.AddPassage(4, 5);
			mapDescription.AddPassage(6, 7);
			mapDescription.AddPassage(6, 8);
			mapDescription.AddPassage(7, 8);

			return mapDescription;
		}

		private static MapDescription<int> Make_Reference_17Vertices_WithoutRoomShapes()
		{
			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 17).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 2);
			mapDescription.AddPassage(0, 3);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(1, 9);
			mapDescription.AddPassage(2, 5);
			mapDescription.AddPassage(3, 6);
			mapDescription.AddPassage(4, 7);
			mapDescription.AddPassage(4, 8);
			mapDescription.AddPassage(5, 6);
			mapDescription.AddPassage(5, 10);
			mapDescription.AddPassage(6, 11);
			mapDescription.AddPassage(7, 12);
			mapDescription.AddPassage(8, 13);
			mapDescription.AddPassage(9, 10);
			mapDescription.AddPassage(11, 12);
			mapDescription.AddPassage(11, 14);
			mapDescription.AddPassage(12, 15);
			mapDescription.AddPassage(12, 16);
			mapDescription.AddPassage(13, 16);
			mapDescription.AddPassage(14, 15);

			return mapDescription;
		}

		private static MapDescription<int> Make_Reference_41Vertices_WithoutRoomShapes()
		{
			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 41).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 2);
			mapDescription.AddPassage(0, 3);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(1, 9);
			mapDescription.AddPassage(2, 5);
			mapDescription.AddPassage(3, 6);
			mapDescription.AddPassage(4, 7);
			mapDescription.AddPassage(4, 8);
			mapDescription.AddPassage(5, 6);
			mapDescription.AddPassage(5, 10);
			mapDescription.AddPassage(6, 11);
			mapDescription.AddPassage(7, 12);
			mapDescription.AddPassage(8, 13);
			mapDescription.AddPassage(9, 10);
			mapDescription.AddPassage(11, 12);
			mapDescription.AddPassage(11, 18);
			mapDescription.AddPassage(12, 19);
			mapDescription.AddPassage(12, 20);
			mapDescription.AddPassage(13, 14);
			mapDescription.AddPassage(13, 20);
			mapDescription.AddPassage(14, 15);
			mapDescription.AddPassage(15, 26);
			mapDescription.AddPassage(16, 22);
			mapDescription.AddPassage(17, 22);
			mapDescription.AddPassage(18, 19);
			mapDescription.AddPassage(18, 23);
			mapDescription.AddPassage(21, 24);
			mapDescription.AddPassage(22, 23);
			mapDescription.AddPassage(22, 27);
			mapDescription.AddPassage(22, 28);
			mapDescription.AddPassage(22, 29);
			mapDescription.AddPassage(23, 24);
			mapDescription.AddPassage(24, 30);
			mapDescription.AddPassage(25, 26);
			mapDescription.AddPassage(26, 32);
			mapDescription.AddPassage(31, 32);
			mapDescription.AddPassage(32, 33);
			mapDescription.AddPassage(32, 35);
			mapDescription.AddPassage(34, 35);
			mapDescription.AddPassage(35, 36);
			mapDescription.AddPassage(35, 38);
			mapDescription.AddPassage(37, 38);
			mapDescription.AddPassage(38, 39);
			mapDescription.AddPassage(39, 40);

			return mapDescription;
		}

		public static void AddClassicRoomShapes<TNode>(MapDescription<TNode> mapDescription, IntVector2 scale)
		{
			var squareRoom = new RoomDescription(GridPolygon.GetSquare(3).Scale(scale), new OverlapMode(1, 0));
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(3, 5).Scale(scale), new OverlapMode(1, 0));
			var room1 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));
			var room2 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 4)
					.AddPoint(2, 4)
					.AddPoint(2, 2)
					.AddPoint(4, 2)
					.AddPoint(4, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));
			var room3 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 2)
					.AddPoint(2, 2)
					.AddPoint(2, 4)
					.AddPoint(4, 4)
					.AddPoint(4, 2)
					.AddPoint(6, 2)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, new OverlapMode(1, 0));

			mapDescription.AddRoomShapes(squareRoom, true, 4);
			mapDescription.AddRoomShapes(rectangleRoom, true, 2);
			mapDescription.AddRoomShapes(room1);
			mapDescription.AddRoomShapes(room2);
			mapDescription.AddRoomShapes(room3);
		}

		public static void AddClassicRoomShapes<TNode>(MapDescription<TNode> mapDescription)
		{
			AddClassicRoomShapes(mapDescription, new IntVector2(3, 3));
		}
	}
}