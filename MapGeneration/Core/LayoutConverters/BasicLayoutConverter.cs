namespace MapGeneration.Core.LayoutConverters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using CorridorNodesCreators;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.LayoutConverters;
	using Interfaces.Core.Layouts;
	using Interfaces.Core.MapLayouts;
	using Interfaces.Utils;
	using MapDescriptions;
	using MapLayouts;
	using Utils;

	/// <summary>
	/// Basic implementation of a layout converter.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
	public class BasicLayoutConverter<TLayout, TNode, TConfiguration> : ILayoutConverter<TLayout, IMapLayout<TNode>>, IRandomInjectable
		where TLayout : ILayout<int, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>>
	{
		protected readonly MapDescription<TNode> MapDescription;
		protected Random Random;
		protected readonly IConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> ConfigurationSpaces;
		protected readonly ICorridorNodesCreator<TNode> CorridorNodesCreator;
		protected readonly Dictionary<int, ConfigurationSpacesGenerator.RoomInfo> IntAliasMapping;

		public BasicLayoutConverter(
			MapDescription<TNode> mapDescription, 
			IConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces, 
			Dictionary<int, ConfigurationSpacesGenerator.RoomInfo> intAliasMapping,
			ICorridorNodesCreator<TNode> corridorNodesCreator = null
		)
		{
			MapDescription = mapDescription;
			ConfigurationSpaces = configurationSpaces;
			IntAliasMapping = intAliasMapping;

			if (MapDescription.IsWithCorridors)
			{
				CorridorNodesCreator = corridorNodesCreator ?? CorridorNodesCreatorFactory.Default.GetCreator<TNode>();
			}
		}

		/// <inheritdoc />
		public IMapLayout<TNode> Convert(TLayout layout, bool addDoors)
		{
			var rooms = new List<IRoom<TNode>>();
			var roomsDict = new Dictionary<TNode, Room<TNode>>();

			var mapping = MapDescription.GetRoomsMapping();

			if (MapDescription.IsWithCorridors)
			{
				CorridorNodesCreator.AddCorridorsToMapping(MapDescription, mapping);
			}
			
			foreach (var vertexAlias in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertexAlias, out var configuration))
				{
					var vertex = mapping.GetByValue(vertexAlias);
					var roomInfo = IntAliasMapping[configuration.ShapeContainer.Alias];

					var room = new Room<TNode>(vertex, configuration.Shape, configuration.Position, MapDescription.IsCorridorRoom(vertexAlias), roomInfo.RoomDescription, roomInfo.Transformations.GetRandom(Random), roomInfo.Transformations);
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
				ConfigurationSpaces.GetConfigurationSpace(configuration2.ShapeContainer, configuration1.ShapeContainer))
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

					doors.Add(new OrthogonalLine(doorStart, doorEnd));
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