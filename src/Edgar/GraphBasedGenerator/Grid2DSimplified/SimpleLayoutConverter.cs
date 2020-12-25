using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.LayoutConverters.Interfaces;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2DSimplified
{
	public class SimpleLayoutConverter<TNode> : ILayoutConverter<SimpleLayout<TNode, SimpleConfiguration<TNode>>, LayoutGrid2D<TNode>>, IRandomInjectable
	{
		protected Random Random;
		protected readonly LazyConfigurationSpaces ConfigurationSpaces;

		public SimpleLayoutConverter(
			LazyConfigurationSpaces configurationSpaces
		)
		{
			ConfigurationSpaces = configurationSpaces;
		}

		/// <inheritdoc />
		public LayoutGrid2D<TNode> Convert(SimpleLayout<TNode, SimpleConfiguration<TNode>> layout, bool addDoors)
		{
			var rooms = new List<LayoutRoomGrid2D<TNode>>();
			var roomsDict = new Dictionary<TNode, LayoutRoomGrid2D<TNode>>();

			foreach (var vertex in layout.GraphWithCorridors.Vertices)
			{
				if (layout.GetConfiguration(vertex, out var configuration))
				{
                    var roomTemplateInstance = configuration.RoomShape;

					// Make sure that the returned shape has the same position as the original room template shape and is not moved to (0,0)
					// TODO: maybe make a unit/integration test?
					var transformation = roomTemplateInstance.Transformations.GetRandom(Random);
					var shape = configuration.RoomShape.RoomShape;
					var originalShape = roomTemplateInstance.RoomTemplate.Outline;
					var transformedShape = originalShape.Transform(transformation);
					var offset = transformedShape.BoundingRectangle.A - shape.BoundingRectangle.A;

					// TODO: improve isCorridor
					var room = new LayoutRoomGrid2D<TNode>(vertex, transformedShape, configuration.Position - offset, !layout.Graph.Vertices.Contains(vertex), roomTemplateInstance.RoomTemplate, null, transformation);
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

				foreach (var vertex in layout.GraphWithCorridors.Vertices)
				{
                    if (layout.GetConfiguration(vertex, out var configuration))
					{
						var neighbours = layout.GraphWithCorridors.GetNeighbours(vertex);

						foreach (var neighbour in neighbours)
						{
			                if (layout.GetConfiguration(neighbour, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbour, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);

								var door = doorChoices.GetRandom(Random);
								var doorFlipped = door.SwitchOrientation();

								roomsDict[vertex].Doors.Add(new LayoutDoorGrid2D<TNode>(vertex, neighbour, door + -1 * roomsDict[vertex].Position));
								roomsDict[neighbour].Doors.Add(new LayoutDoorGrid2D<TNode>(neighbour, vertex, doorFlipped + -1 * roomsDict[neighbour].Position));
								generatedDoors.Add(Tuple.Create(vertex, neighbour));
							}
						}
					}
				}
			}

			return new LayoutGrid2D<TNode>(rooms);
		}

		private List<OrthogonalLineGrid2D> GetDoors(SimpleConfiguration<TNode> configuration1, SimpleConfiguration<TNode> configuration2)
		{
            // TODO: ugly
            var movingRoomConfiguration = new LazyConfigurationSpaces.Configuration(configuration2.Position, configuration2.RoomShape, null);
            var fixedRoomConfiguration = new LazyConfigurationSpaces.Configuration(configuration1.Position, configuration1.RoomShape, null);

			return GetDoors(configuration2.Position - configuration1.Position,
				ConfigurationSpaces.GetConfigurationSpace(movingRoomConfiguration, fixedRoomConfiguration))
				.Select(x => x + configuration1.Position).ToList();
		}

		private List<OrthogonalLineGrid2D> GetDoors(Vector2Int position, ConfigurationSpaceGrid2D configurationSpace)
		{
			var doors = new List<OrthogonalLineGrid2D>();

			foreach (var doorInfo in configurationSpace.ReverseDoors)
			{
				var line = doorInfo.Item1;
				var doorLine = doorInfo.Item2;

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
					var doorEnd = doorStart + doorLine.Length * doorLine.Line.GetDirectionVector();

					doors.Add(new OrthogonalLineGrid2D(doorStart, doorEnd, doorLine.Line.GetDirection()));
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
	}
}