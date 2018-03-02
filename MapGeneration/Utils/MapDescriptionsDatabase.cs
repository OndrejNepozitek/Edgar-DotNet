namespace MapGeneration.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Core.Doors.DoorModes;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core;
	using Interfaces.Core.MapDescription;

	public class MapDescriptionsDatabase
	{ 

		public static MapDescription<int> Reference_9Vertices_WithoutRoomShapes => Make_Reference_9Vertices_WithoutRoomShapes();

		public static MapDescription<int> Reference_17Vertices_WithoutRoomShapes => Make_Reference_17Vertices_WithoutRoomShapes();

		public static MapDescription<int> Reference_41Vertices_WithoutRoomShapes => Make_Reference_41Vertices_WithoutRoomShapes();

		public static MapDescription<int> Reference_Fig7Bottom_WithoutRoomShapes => Make_Reference_Fig7Bottom_WithoutRoomShapes();

		public static MapDescription<int> Reference_Fig9_WithoutRoomShapes => Make_Reference_Fig9_WithoutRoomShapes();

		public static List<Tuple<string, MapDescription<int>>> BasicSet;

		public static List<Tuple<string, MapDescription<int>>> Reference_17Vertices_ScaledSet;

		public static List<Tuple<string, MapDescription<int>>> ReferenceSet;

		static MapDescriptionsDatabase()
		{
			{
				var m1 = Reference_9Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m1);
				var m2 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m2);
				var m3 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m3);

				BasicSet = new List<Tuple<string, MapDescription<int>>>()
				{
					Tuple.Create("Reference 9 vertices", m1),
					Tuple.Create("Reference 17 vertices", m2),
					Tuple.Create("Reference 41 vertices", m3),
				};
			}

			{
				var scale = new IntVector2(1, 1);

				var m1 = Reference_9Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m1, scale);
				var m2 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m2, scale);
				var m3 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m3, scale);
				var m4 = Reference_Fig7Bottom_WithoutRoomShapes;
				AddClassicRoomShapes(m4, scale);
				var m5 = Reference_Fig9_WithoutRoomShapes;
				AddClassicRoomShapes(m5, scale);

				ReferenceSet = new List<Tuple<string, MapDescription<int>>>()
				{
					Tuple.Create("Fig 1 (17 vertices)", m2),
					Tuple.Create("Fig 7 top (9 vertices)", m1),
					Tuple.Create("Fig 7 bottom (17 vertices)", m4),
					Tuple.Create("Fig 8 (41 vertices)", m3),
					Tuple.Create("Fig 9 (15 vertices)", m5),
				};
			}

			{
				var m0 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m0, new IntVector2(1, 1));
				var m1 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m1, new IntVector2(5, 5));
				var m2 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m2, new IntVector2(10, 10));
				var m3 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m3, new IntVector2(25, 25));

				var m7 = Reference_17Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m7, new IntVector2(50, 50));

				var m4 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m4, new IntVector2(5, 5));
				var m5 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m5, new IntVector2(10, 10));
				var m6 = Reference_41Vertices_WithoutRoomShapes;
				AddClassicRoomShapes(m6, new IntVector2(25, 25));

				Reference_17Vertices_ScaledSet = new List<Tuple<string, MapDescription<int>>>()
				{
					Tuple.Create("17 vertices - scale 1", m0),
					Tuple.Create("17 vertices - scale 5", m1),
					Tuple.Create("17 vertices - scale 10", m2),
					Tuple.Create("17 vertices - scale 25", m3),
					Tuple.Create("17 vertices - scale 50", m7),
					/*Tuple.Create("41 vertices - scale 5", (IMapDescription<int>) m4),
					Tuple.Create("41 vertices - scale 10", (IMapDescription<int>) m5),
					Tuple.Create("41 vertices - scale 25", (IMapDescription<int>) m6),*/
				};
			}
		}

		public static List<Tuple<string, MapDescription<int>>> GetReferenceSetWithCorridors(int offset = 2)
		{
			var scale = new IntVector2(1, 1);

			var m1 = Reference_9Vertices_WithoutRoomShapes;
			m1.SetWithCorridors(true);
			AddClassicRoomShapes(m1, scale);
			AddCorridorRoomShapes(m1, offset);

			var m2 = Reference_17Vertices_WithoutRoomShapes;
			m2.SetWithCorridors(true);
			AddClassicRoomShapes(m2, scale);
			AddCorridorRoomShapes(m2, offset);

			var m3 = Reference_41Vertices_WithoutRoomShapes;
			m3.SetWithCorridors(true);
			AddClassicRoomShapes(m3, scale);
			AddCorridorRoomShapes(m3, offset);

			var m4 = Reference_Fig7Bottom_WithoutRoomShapes;
			m4.SetWithCorridors(true);
			AddClassicRoomShapes(m4, scale);
			AddCorridorRoomShapes(m4, offset);

			var m5 = Reference_Fig9_WithoutRoomShapes;
			m5.SetWithCorridors(true);
			AddClassicRoomShapes(m5, scale);
			AddCorridorRoomShapes(m5, offset);

			return new List<Tuple<string, MapDescription<int>>>()
			{
				Tuple.Create("Fig 1 (17 vertices)", m2),
				Tuple.Create("Fig 7 top (9 vertices)", m1),
				Tuple.Create("Fig 7 bottom (17 vertices)", m4),
				Tuple.Create("Fig 8 (41 vertices)", m3),
				Tuple.Create("Fig 9 (15 vertices)", m5),
			};
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

		private static MapDescription<int> Make_Reference_Fig7Bottom_WithoutRoomShapes()
		{
			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 17).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(1, 2);
			mapDescription.AddPassage(1, 7);
			mapDescription.AddPassage(1, 10);
			mapDescription.AddPassage(2, 3);
			mapDescription.AddPassage(2, 5);
			mapDescription.AddPassage(3, 4);
			mapDescription.AddPassage(4, 5);
			mapDescription.AddPassage(4, 6);
			mapDescription.AddPassage(6, 11);
			mapDescription.AddPassage(7, 8);
			mapDescription.AddPassage(8, 9);
			mapDescription.AddPassage(9, 16);
			mapDescription.AddPassage(10, 11);
			mapDescription.AddPassage(10, 13);
			mapDescription.AddPassage(11, 12);
			mapDescription.AddPassage(12, 13);
			mapDescription.AddPassage(12, 14);
			mapDescription.AddPassage(14, 15);
			mapDescription.AddPassage(15, 16);

			return mapDescription;
		}

		private static MapDescription<int> Make_Reference_Fig9_WithoutRoomShapes()
		{
			var mapDescription = new MapDescription<int>();
			Enumerable.Range(0, 15).ToList().ForEach(x => mapDescription.AddRoom(x));

			mapDescription.AddPassage(0, 1);
			mapDescription.AddPassage(0, 2);
			mapDescription.AddPassage(0, 8);
			mapDescription.AddPassage(0, 9);
			mapDescription.AddPassage(1, 3);
			mapDescription.AddPassage(1, 4);
			mapDescription.AddPassage(1, 5);
			mapDescription.AddPassage(2, 6);
			mapDescription.AddPassage(2, 7);
			mapDescription.AddPassage(8, 10);
			mapDescription.AddPassage(8, 11);
			mapDescription.AddPassage(8, 12);
			mapDescription.AddPassage(9, 13);
			mapDescription.AddPassage(9, 14);

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
			var overlapScale = Math.Min(scale.X, scale.Y);
			var doorMode = new OverlapMode(1 * overlapScale, 0);

			var squareRoom = new RoomDescription(GridPolygon.GetSquare(6).Scale(scale), doorMode);
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(6, 9).Scale(scale), doorMode);
			var room1 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 6)
					.AddPoint(3, 6)
					.AddPoint(3, 3)
					.AddPoint(6, 3)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, doorMode);
			var room2 = new RoomDescription(
				new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 9)
					.AddPoint(3, 9)
					.AddPoint(3, 3)
					.AddPoint(6, 3)
					.AddPoint(6, 0)
					.Build().Scale(scale)
				, doorMode);
			var room3 = new RoomDescription(
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

			mapDescription.AddRoomShapes(squareRoom, true, 4);
			mapDescription.AddRoomShapes(rectangleRoom, true, 2);
			mapDescription.AddRoomShapes(room1);
			mapDescription.AddRoomShapes(room2);
			mapDescription.AddRoomShapes(room3);
		}

		public static void AddCorridorRoomShapes<TNode>(MapDescription<TNode> mapDescription, int offset)
		{
			{
				var width = offset;
				var room = new RoomDescription(
					GridPolygon.GetRectangle(width, 1),
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 1)),
						new OrthogonalLine(new IntVector2(width, 0), new IntVector2(width, 1)),
					})
				);

				mapDescription.AddCorridorShapes(room);
			}

			{
				var width = offset;
				var room = new RoomDescription(
					new GridPolygonBuilder()
						.AddPoint(0, 0)
						.AddPoint(0, 2)
						.AddPoint(2, 2)
						.AddPoint(2, 1)
						.AddPoint(1, 1)
						.AddPoint(1, 0)
						.Build()
					,
					new SpecificPositionsMode(new List<OrthogonalLine>()
					{
						new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)),
						new OrthogonalLine(new IntVector2(2, 1), new IntVector2(2, 2)),
					})
				);

				mapDescription.AddCorridorShapes(room);
			}
		}

		public static void AddClassicRoomShapes<TNode>(MapDescription<TNode> mapDescription)
		{
			AddClassicRoomShapes(mapDescription, new IntVector2(1, 1));
		}
	}
}