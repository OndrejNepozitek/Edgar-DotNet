using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.ConfigurationSpaces.Interfaces;
using Edgar.Legacy.Core.LayoutConverters.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.Legacy.Core.LayoutConverters
{
	/// <summary>
	/// Converts layout from its internal representation to a representation more suitable for users.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
    public class BasicLayoutConverter<TLayout, TNode, TConfiguration> : ILayoutConverter<TLayout, MapLayout<TNode>>, IRandomInjectable
		where TLayout : ILayout<int, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<PolygonGrid2D>, int>
	{
		protected readonly MapDescriptionMapping<TNode> MapDescription;
		protected Random Random;
		protected readonly IConfigurationSpaces<int, IntAlias<PolygonGrid2D>, TConfiguration, ConfigurationSpace> ConfigurationSpaces;
        protected readonly TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> IntAliasMapping;

		public BasicLayoutConverter(
            MapDescriptionMapping<TNode> mapDescription, 
			IConfigurationSpaces<int, IntAlias<PolygonGrid2D>, TConfiguration, ConfigurationSpace> configurationSpaces, 
			TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> intAliasMapping
        )
		{
			MapDescription = mapDescription;
			ConfigurationSpaces = configurationSpaces;
			IntAliasMapping = intAliasMapping;
        }

		/// <inheritdoc />
		public MapLayout<TNode> Convert(TLayout layout, bool addDoors)
		{
			var rooms = new List<Room<TNode>>();
			var roomsDict = new Dictionary<TNode, Room<TNode>>();

			var mapping = MapDescription.GetMapping();

            foreach (var vertexAlias in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertexAlias, out var configuration))
				{
					var vertex = mapping.GetByValue(vertexAlias);
					var roomTemplateInstance = IntAliasMapping.GetByValue(configuration.ShapeContainer);

					// Make sure that the returned shape has the same position as the original room template shape and is not moved to (0,0)
					// TODO: maybe make a unit/integration test?
                    var transformation = roomTemplateInstance.Transformations.GetRandom(Random);
                    var shape = configuration.Shape;
                    var originalShape = roomTemplateInstance.RoomTemplate.Shape;
                    var transformedShape = originalShape.Transform(transformation);
                    var offset = transformedShape.BoundingRectangle.A - shape.BoundingRectangle.A;

                    var room = new Room<TNode>(vertex, transformedShape, configuration.Position - offset, MapDescription.GetRoomDescription(vertexAlias) is CorridorRoomDescription, roomTemplateInstance.RoomTemplate, MapDescription.GetRoomDescription(vertexAlias), transformation, roomTemplateInstance.Transformations, roomTemplateInstance);
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
					var vertex = mapping.GetByValue(vertexAlias);

					if (layout.GetConfiguration(vertexAlias, out var configuration))
					{
						var neighbours = layout.Graph.GetNeighbors(vertexAlias);

						foreach (var neighbourAlias in neighbours)
						{
							var neighbour = mapping.GetByValue(neighbourAlias);

							if (layout.GetConfiguration(neighbourAlias, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbour, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);
								var randomChoice = doorChoices.GetRandom(Random);

								roomsDict[vertex].Doors.Add(new LayoutDoorGrid2D<TNode>(vertex, neighbour, randomChoice, null, DoorType.Undirected));
								roomsDict[neighbour].Doors.Add(new LayoutDoorGrid2D<TNode>(neighbour, vertex, randomChoice, null, DoorType.Undirected));
								generatedDoors.Add(Tuple.Create(vertex, neighbour));
							}
						}
					}
				}
			}

			return new MapLayout<TNode>(rooms);
		}

		private List<OrthogonalLineGrid2D> GetDoors(TConfiguration configuration1, TConfiguration configuration2)
		{
			return GetDoors(configuration2.Position - configuration1.Position,
				ConfigurationSpaces.GetConfigurationSpace(configuration2, configuration1))
				.Select(x => x + configuration1.Position).ToList();
		}

		private List<OrthogonalLineGrid2D> GetDoors(Vector2Int position, ConfigurationSpace configurationSpace)
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