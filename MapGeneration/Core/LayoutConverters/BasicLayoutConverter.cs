namespace MapGeneration.Core.LayoutConverters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ConfigurationSpaces;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.MapDescription;
	using Utils;

	public class BasicLayoutConverter<TLayout, TNode, TConfiguration> : ILayoutConverter<TLayout, IMapLayout<TNode>>, IRandomInjectable
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IConfiguration<IntAlias<GridPolygon>>
	{
		protected readonly IMapDescription<TNode> MapDescription;
		protected Random Random;
		protected readonly IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces;

		public BasicLayoutConverter(IMapDescription<TNode> mapDescription, IConfigurationSpaces<TNode, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> configurationSpaces)
		{
			MapDescription = mapDescription;
			this.configurationSpaces = configurationSpaces;
		}

		public IMapLayout<TNode> Convert(TLayout layout, bool addDoors)
		{
			var rooms = new List<IRoom<TNode>>();
			var roomsDict = new Dictionary<TNode, Room<TNode>>();

			var corridorMapDescription = MapDescription as ICorridorMapDescription<TNode>;
			var hasCorridors = corridorMapDescription != null;

			foreach (var vertex in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertex, out var configuration))
				{
					var room = new Room<TNode>(vertex, configuration.Shape, configuration.Position, hasCorridors && corridorMapDescription.IsCorridorRoom(vertex));
					rooms.Add(room);

					if (!addDoors)
						continue;

					var doors = new List<Tuple<TNode, OrthogonalLine>>();
					room.Doors = doors;

					roomsDict[vertex] = room;
				}
			}

			if (addDoors)
			{
				var generatedDoors = new HashSet<Tuple<TNode, TNode>>();

				foreach (var vertex in layout.Graph.Vertices)
				{
					if (layout.GetConfiguration(vertex, out var configuration))
					{
						var neighbours = layout.Graph.GetNeighbours(vertex);

						foreach (var neighbour in neighbours)
						{
							if (layout.GetConfiguration(neighbour, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbour, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);
								var randomChoice = doorChoices.GetRandom(Random);

								roomsDict[vertex].Doors.Add(Tuple.Create(neighbour, randomChoice));
								roomsDict[neighbour].Doors.Add(Tuple.Create(vertex, randomChoice));
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
				configurationSpaces.GetConfigurationSpace(configuration2.ShapeContainer, configuration1.ShapeContainer))
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