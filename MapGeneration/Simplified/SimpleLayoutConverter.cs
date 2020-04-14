using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.ConfigurationSpaces.Interfaces;
using MapGeneration.Core.LayoutConverters.Interfaces;
using MapGeneration.Core.Layouts.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils;
using MapGeneration.Utils.Interfaces;

namespace MapGeneration.Simplified
{
    public class SimpleLayoutConverter<TLayout, TNode, TConfiguration> : ILayoutConverter<TLayout, MapLayout<TNode>>, IRandomInjectable
		where TLayout : ILayout<TNode, TConfiguration>
		where TConfiguration : IConfiguration<RoomTemplateInstance, TNode>
	{
        protected Random Random;
		protected readonly IConfigurationSpaces<TNode, RoomTemplateInstance, TConfiguration, ConfigurationSpace> ConfigurationSpaces;

        public SimpleLayoutConverter(
            IConfigurationSpaces<TNode, RoomTemplateInstance, TConfiguration, ConfigurationSpace> configurationSpaces
        )
		{
            ConfigurationSpaces = configurationSpaces;
        }

		/// <inheritdoc />
		public MapLayout<TNode> Convert(TLayout layout, bool addDoors)
		{
			var rooms = new List<Room<TNode>>();
			var roomsDict = new Dictionary<TNode, Room<TNode>>();
			
            foreach (var vertex in layout.Graph.Vertices)
			{
				if (layout.GetConfiguration(vertex, out var configuration))
                {
                    var roomTemplateInstance = configuration.ShapeContainer;

					// Make sure that the returned shape has the same position as the original room template shape and is not moved to (0,0)
					// TODO: maybe make a unit/integration test?
                    var transformation = roomTemplateInstance.Transformations.GetRandom(Random);
                    var shape = configuration.Shape;
                    var originalShape = roomTemplateInstance.RoomTemplate.Shape;
                    var transformedShape = originalShape.Transform(transformation);
                    var offset = transformedShape.BoundingRectangle.A - shape.BoundingRectangle.A;

                    var room = new Room<TNode>(vertex, transformedShape, configuration.Position - offset, false, roomTemplateInstance.RoomTemplate, null, transformation, roomTemplateInstance.Transformations, roomTemplateInstance);
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

				foreach (var vertex in layout.Graph.Vertices)
				{
                    if (layout.GetConfiguration(vertex, out var configuration))
					{
						var neighbours = layout.Graph.GetNeighbours(vertex);

						foreach (var neighbor in neighbours)
						{
                            if (layout.GetConfiguration(neighbor, out var neighbourConfiguration) && !generatedDoors.Contains(Tuple.Create(neighbor, vertex)))
							{
								var doorChoices = GetDoors(configuration, neighbourConfiguration);
								var randomChoice = doorChoices.GetRandom(Random);

								roomsDict[vertex].Doors.Add(new DoorInfo<TNode>(neighbor, randomChoice));
								roomsDict[neighbor].Doors.Add(new DoorInfo<TNode>(vertex, randomChoice));
								generatedDoors.Add(Tuple.Create(vertex, neighbor));
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