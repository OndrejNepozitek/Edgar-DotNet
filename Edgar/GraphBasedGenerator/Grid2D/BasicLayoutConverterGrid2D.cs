using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.General;
using Edgar.GraphBasedGenerator.General.Configurations;
using Edgar.Legacy.Core.LayoutConverters.Interfaces;
using Edgar.Legacy.Core.Layouts.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;
using Edgar.Legacy.Utils.Interfaces;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	/// <summary>
	/// Converts layout from its internal representation to a representation more suitable for users.
	/// </summary>
	/// <typeparam name="TLayout"></typeparam>
	/// <typeparam name="TNode"></typeparam>
	/// <typeparam name="TConfiguration"></typeparam>
    public class BasicLayoutConverterGrid2D<TNode, TConfiguration> : ILayoutConverter<ILayout<RoomNode<TNode>, TConfiguration>, LevelGrid2D<TNode>>, IRandomInjectable
        where TConfiguration : IConfiguration<RoomTemplateInstance, Vector2Int, RoomNode<TNode>>
	{
		protected readonly LevelDescriptionGrid2D<TNode> MapDescription;
		protected Random Random;
		protected readonly ConfigurationSpacesGrid2D<TConfiguration, RoomNode<TNode>> ConfigurationSpaces;
        protected readonly TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> IntAliasMapping;

		public BasicLayoutConverterGrid2D(
            LevelDescriptionGrid2D<TNode> mapDescription, 
            ConfigurationSpacesGrid2D<TConfiguration, RoomNode<TNode>> configurationSpaces, 
			TwoWayDictionary<RoomTemplateInstance, IntAlias<PolygonGrid2D>> intAliasMapping
        )
		{
			MapDescription = mapDescription;
			ConfigurationSpaces = configurationSpaces;
			IntAliasMapping = intAliasMapping;
        }

		/// <inheritdoc />
		public LevelGrid2D<TNode> Convert(ILayout<RoomNode<TNode>, TConfiguration> layout, bool addDoors)
		{
			var rooms = new List<General.Room<TNode>>();
			var roomsDict = new Dictionary<TNode, General.Room<TNode>>();

            foreach (var vertexAlias in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertexAlias, out var configuration))
                {
                    var vertex = vertexAlias.Room;
                    var roomTemplateInstance = configuration.RoomShape;

					// Make sure that the returned shape has the same position as the original room template shape and is not moved to (0,0)
					// TODO: maybe make a unit/integration test?
                    var transformation = roomTemplateInstance.Transformations.GetRandom(Random);
                    var shape = configuration.RoomShape.RoomShape;
                    var originalShape = roomTemplateInstance.RoomTemplate.Shape;
                    var transformedShape = originalShape.Transform(transformation);
                    var offset = transformedShape.BoundingRectangle.A - shape.BoundingRectangle.A;

                    var room = new General.Room<TNode>(vertex, transformedShape, configuration.Position - offset, MapDescription.GetRoomDescription(vertexAlias.Room).IsCorridor, roomTemplateInstance.RoomTemplate, MapDescription.GetRoomDescription(vertexAlias.Room), transformation, roomTemplateInstance.Transformations, roomTemplateInstance);
					rooms.Add(room);

					if (!addDoors)
						continue;

					var doors = new List<DoorInfo<TNode>>();
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
						var neighbours = layout.Graph.GetNeighbours(vertexAlias);

						foreach (var neighbourAlias in neighbours)
						{
							var neighbour = neighbourAlias.Room;

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

			return new LevelGrid2D<TNode>(rooms);
		}

		private List<OrthogonalLine> GetDoors(TConfiguration configuration1, TConfiguration configuration2)
		{
			return GetDoors(configuration2.Position - configuration1.Position,
				ConfigurationSpaces.GetConfigurationSpace(configuration2, configuration1))
				.Select(x => x + configuration1.Position).ToList();
		}

		private List<OrthogonalLine> GetDoors(Vector2Int position, ConfigurationSpaceGrid2D configurationSpace)
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