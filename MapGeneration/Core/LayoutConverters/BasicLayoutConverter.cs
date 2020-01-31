using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Interfaces.Core.Configuration;
using MapGeneration.Interfaces.Core.ConfigurationSpaces;
using MapGeneration.Interfaces.Core.LayoutConverters;
using MapGeneration.Interfaces.Core.Layouts;
using MapGeneration.Interfaces.Core.MapLayouts;
using MapGeneration.Interfaces.Utils;
using MapGeneration.Utils;

namespace MapGeneration.Core.LayoutConverters
{
    public class BasicLayoutConverter<TLayout, TNode, TConfiguration> : ILayoutConverter<TLayout, IMapLayout<TNode>>, IRandomInjectable
		where TLayout : ILayout<int, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
	{
		protected readonly MapDescriptionMapping<TNode> MapDescription;
		protected Random Random;
		protected readonly IConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> ConfigurationSpaces;
        protected readonly TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> IntAliasMapping;

		public BasicLayoutConverter(
            MapDescriptionMapping<TNode> mapDescription, 
			IConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces, 
			TwoWayDictionary<RoomTemplateInstance, IntAlias<GridPolygon>> intAliasMapping
        )
		{
			MapDescription = mapDescription;
			ConfigurationSpaces = configurationSpaces;
			IntAliasMapping = intAliasMapping;

			//if (MapDescription.IsWithCorridors)
			//{
			//	CorridorNodesCreator = corridorNodesCreator ?? CorridorNodesCreatorFactory.Default.GetCreator<TNode>();
			//}
		}

		/// <inheritdoc />
		public IMapLayout<TNode> Convert(TLayout layout, bool addDoors)
		{
			var rooms = new List<IRoom<TNode>>();
			var roomsDict = new Dictionary<TNode, Room<TNode>>();

			var mapping = MapDescription.GetMapping();

			//if (MapDescription.IsWithCorridors)
			//{
			//	CorridorNodesCreator.AddCorridorsToMapping(MapDescription, mapping);
			//}
			
			foreach (var vertexAlias in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertexAlias, out var configuration))
				{
					var vertex = mapping.GetByValue(vertexAlias);
					var roomTemplate = IntAliasMapping.GetByValue(configuration.ShapeContainer);

					var room = new Room<TNode>(vertex, configuration.Shape, configuration.Position, MapDescription.GetRoomDescription(vertexAlias) is CorridorRoomDescription, roomTemplate.RoomTemplate, MapDescription.GetRoomDescription(vertexAlias), roomTemplate.Transformations.GetRandom(Random), roomTemplate.Transformations);
					rooms.Add(room);

					if (!addDoors)
						continue;

					var doors = new List<IDoorInfo<TNode>>();
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
						var neighbours = layout.Graph.GetNeighbours(vertexAlias);

						foreach (var neighbourAlias in neighbours)
						{
							var neighbour = mapping.GetByValue(neighbourAlias);

							if (layout.GetConfiguration(neighbourAlias, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbour, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);
								var randomChoice = doorChoices.GetRandom(Random);

								roomsDict[vertex].Doors.Add(new DoorInfo<TNode>(neighbour, randomChoice));
								roomsDict[neighbour].Doors.Add(new DoorInfo<TNode>(vertex, randomChoice));
								generatedDoors.Add(Tuple.Create(vertex, neighbour));
							}
						}
					}
				}
			}

			return new MapLayout<TNode>(rooms);
		}

		private List<OrthogonalLine> GetDoors(TConfiguration configuration1, TConfiguration configuration2)
		{
			return GetDoors(configuration2.Position - configuration1.Position,
				ConfigurationSpaces.GetConfigurationSpace(configuration2, configuration1))
				.Select(x => x + configuration1.Position).ToList();
		}

		private List<OrthogonalLine> GetDoors(IntVector2 position, ConfigurationSpace configurationSpace)
		{
			var doors = new List<OrthogonalLine>();

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

					doors.Add(new OrthogonalLine(doorStart, doorEnd, doorLine.Line.GetDirection()));
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