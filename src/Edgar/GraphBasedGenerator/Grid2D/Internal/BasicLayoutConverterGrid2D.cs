using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Configurations;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.LayoutConverters.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
	/// <summary>
	/// Converts layout from its internal representation to a representation more suitable for users.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
    public class BasicLayoutConverterGrid2D<TNode, TConfiguration> : ILayoutConverter<ILayout<RoomNode<TNode>, TConfiguration>, LayoutGrid2D<TNode>>, IRandomInjectable
        where TConfiguration : IConfiguration<RoomTemplateInstanceGrid2D, Vector2Int, RoomNode<TNode>>
	{
		protected readonly LevelDescriptionGrid2D<TNode> MapDescription;
		protected Random Random;
		protected readonly ConfigurationSpacesGrid2D<TConfiguration, RoomNode<TNode>> ConfigurationSpaces;
        protected readonly TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>> IntAliasMapping;

		public BasicLayoutConverterGrid2D(
            LevelDescriptionGrid2D<TNode> mapDescription, 
            ConfigurationSpacesGrid2D<TConfiguration, RoomNode<TNode>> configurationSpaces, 
			TwoWayDictionary<RoomTemplateInstanceGrid2D, IntAlias<PolygonGrid2D>> intAliasMapping
        )
		{
			MapDescription = mapDescription;
			ConfigurationSpaces = configurationSpaces;
			IntAliasMapping = intAliasMapping;
        }

		/// <inheritdoc />
		public LayoutGrid2D<TNode> Convert(ILayout<RoomNode<TNode>, TConfiguration> layout, bool addDoors)
		{
			var rooms = new List<LayoutRoomGrid2D<TNode>>();
			var roomsDict = new Dictionary<TNode, LayoutRoomGrid2D<TNode>>();
            var fixedRoomConstraints = MapDescription
                .Constraints?
                .Where(x => x is FixedConfigurationConstraint<TNode>)
                .Cast<FixedConfigurationConstraint<TNode>>()
                .ToDictionary(x => x.Room, x => x);

            foreach (var vertexAlias in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertexAlias, out var configuration))
                {
                    var vertex = vertexAlias.Room;
                    var roomTemplateInstance = configuration.RoomShape;

                    // Make sure that the returned shape has the same position as the original room template shape and is not moved to (0,0)
					// TODO: maybe make a unit/integration test?
					// Do not choose random transformation if the transformation is fixed
                    var transformation = fixedRoomConstraints != null && fixedRoomConstraints.ContainsKey(vertex)
						? fixedRoomConstraints[vertex].Transformation
						: roomTemplateInstance.Transformations.GetRandom(Random);

					var shape = configuration.RoomShape.RoomShape;
                    var originalShape = roomTemplateInstance.RoomTemplate.Outline;
                    var transformedShape = originalShape.Transform(transformation);
                    var offset = transformedShape.BoundingRectangle.A - shape.BoundingRectangle.A;

                    var room = new LayoutRoomGrid2D<TNode>(vertex, transformedShape, configuration.Position - offset, MapDescription.GetRoomDescription(vertexAlias.Room).IsCorridor, roomTemplateInstance.RoomTemplate, MapDescription.GetRoomDescription(vertexAlias.Room), transformation);
					rooms.Add(room);

					if (!addDoors)
						continue;

					var doors = new List<LayoutDoorGrid2D<TNode>>();
					room.Doors = doors;

					roomsDict[vertex] = room;
				}
			}

			if (addDoors)
			{
				var generatedDoors = new HashSet<Tuple<TNode, TNode>>();

				foreach (var vertexAlias in layout.Graph.Vertices)
				{
					var vertex = vertexAlias.Room;

					if (layout.GetConfiguration(vertexAlias, out var configuration))
					{
						var neighbours = layout.Graph.GetNeighbors(vertexAlias);

						foreach (var neighbourAlias in neighbours)
						{
							var neighbour = neighbourAlias.Room;

							if (layout.GetConfiguration(neighbourAlias, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbour, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);

								var doorPair = doorChoices.GetRandom(Random);
                                var doorLine = doorPair.Door;
                                var door = new OrthogonalLineGrid2D(
                                    doorLine.Line.From,
                                    doorLine.Line.From + doorLine.Length * doorLine.Line.GetDirectionVector(),
                                    doorLine.Line.GetDirection()
                                );

                                var otherDoorLine = doorPair.OtherDoor;
                                var otherDoor = new OrthogonalLineGrid2D(
                                    otherDoorLine.Line.From,
                                    otherDoorLine.Line.From + otherDoorLine.Length * otherDoorLine.Line.GetDirectionVector(),
                                    otherDoorLine.Line.GetDirection()
                                );


								roomsDict[vertex].Doors.Add(new LayoutDoorGrid2D<TNode>(
                                    vertex,
                                    neighbour,
                                    door + -1 * roomsDict[vertex].Position,
                                    doorLine.DoorSocket,
                                    doorLine.Type
                                ));
								roomsDict[neighbour].Doors.Add(new LayoutDoorGrid2D<TNode>(
                                    neighbour,
                                    vertex,
                                    otherDoor + -1 * roomsDict[neighbour].Position,
                                    otherDoorLine.DoorSocket,
                                    otherDoorLine.Type
                                ));
								generatedDoors.Add(Tuple.Create(vertex, neighbour));
							}
						}
					}
				}
			}

			return new LayoutGrid2D<TNode>(rooms);
		}

		private List<DoorLinePair> GetDoors(TConfiguration configuration1, TConfiguration configuration2)
		{
			return GetDoors(
                    configuration2.Position - configuration1.Position,
				    ConfigurationSpaces.GetConfigurationSpace(configuration2, configuration1)
                )
				.Select(x => x + configuration1.Position)
                .ToList();
		}

		private List<DoorLinePair> GetDoors(Vector2Int position, ConfigurationSpaceGrid2D configurationSpace)
		{
			var doors = new List<DoorLinePair>();

			foreach (var reverseDoorsInfo in configurationSpace.ReverseDoors)
			{
				var line = reverseDoorsInfo.ConfigurationSpaceLine;
				var doorLine = reverseDoorsInfo.DoorLine;
                var otherDoorLine = reverseDoorsInfo.OtherDoorLine;

				var index = line.Contains(position);

				if (index == -1)
					continue;

				var offset = line.Length - doorLine.Line.Length;
				var numberOfPositions = Math.Min(Math.Min(offset, Math.Min(index, line.Length - index)), doorLine.Line.Length) + 1;

				if (numberOfPositions == 0)
					throw new InvalidOperationException();

				for (var i = 0; i < numberOfPositions; i++)
				{
					var doorStart = doorLine.Line.GetNthPoint(Math.Max(0, index - offset) + i);
                    var singleDoorLine = new OrthogonalLineGrid2D(doorStart, doorStart, doorLine.Line.GetDirection());
                    var singleDoor = new DoorLineGrid2D(singleDoorLine, doorLine.Length, doorLine.DoorSocket, doorLine.Type);

                    var otherSingleDoorLine = singleDoorLine.SwitchOrientation();
                    var otherSingleDoor = new DoorLineGrid2D(otherSingleDoorLine, otherDoorLine.Length,
                        otherDoorLine.DoorSocket, otherDoorLine.Type);

					doors.Add(new DoorLinePair(singleDoor, otherSingleDoor));
				}
			}

			if (doors.Count == 0)
				throw new InvalidOperationException();

			return doors;
		}

		public void InjectRandomGenerator(Random random)
		{
			Random = random;
		}

        private class DoorLinePair
        {
            public DoorLineGrid2D Door { get; }

			public DoorLineGrid2D OtherDoor { get; }

            public DoorLinePair(DoorLineGrid2D door, DoorLineGrid2D otherDoor)
            {
                Door = door;
                OtherDoor = otherDoor;
            }

            public static DoorLinePair operator +(DoorLinePair pair, Vector2Int offset)
            {
                return new DoorLinePair(MoveDoorLine(pair.Door, offset), MoveDoorLine(pair.OtherDoor, offset));
            }

			private static DoorLineGrid2D MoveDoorLine(DoorLineGrid2D doorLine, Vector2Int offset)
            {
                return new DoorLineGrid2D(
                    doorLine.Line + offset,
                    doorLine.Length,
                    doorLine.DoorSocket,
                    doorLine.Type);
            }
		}
	}
}