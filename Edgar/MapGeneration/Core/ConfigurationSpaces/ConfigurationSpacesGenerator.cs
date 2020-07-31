using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Configurations.Interfaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapDescriptions.Interfaces;
using MapGeneration.Utils;

namespace MapGeneration.Core.ConfigurationSpaces
{
    /// <summary>
    /// Class responsible for generating configuration spaces.
    /// </summary>
    public class ConfigurationSpacesGenerator
    {
        private readonly IPolygonOverlap<GridPolygon> polygonOverlap;
        private readonly IDoorHandler doorHandler;
        private readonly ILineIntersection<OrthogonalLine> lineIntersection;
        private readonly IPolygonUtils<GridPolygon> polygonUtils;

        public ConfigurationSpacesGenerator(IPolygonOverlap<GridPolygon> polygonOverlap, IDoorHandler doorHandler, ILineIntersection<OrthogonalLine> lineIntersection, IPolygonUtils<GridPolygon> polygonUtils)
        {
            this.polygonOverlap = polygonOverlap;
            this.doorHandler = doorHandler;
            this.lineIntersection = lineIntersection;
            this.polygonUtils = polygonUtils;
        }
        
        public ConfigurationSpaces<TConfiguration> GetConfigurationSpaces<TConfiguration>(IMapDescription<int> mapDescription)
            where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
        {
            var graph = mapDescription.GetGraph();
            
            var roomDescriptions = graph
                .Vertices
                .ToDictionary(x => x, mapDescription.GetRoomDescription);
            
            var roomTemplates = roomDescriptions
                .Values
                .SelectMany(x => x.RoomTemplates)
                .Distinct()
                .ToList();

            var roomTemplateInstances = roomTemplates
                .ToDictionary(x => x, GetRoomTemplateInstances);

            var roomTemplateInstancesMapping = roomTemplateInstances
                .SelectMany(x => x.Value)
                .CreateIntMapping();

            var roomTemplateInstancesCount = roomTemplateInstancesMapping.Count;

            var corridorRoomDescriptionsMapping = roomDescriptions
                .Values
                .Where(x => x.GetType() == typeof(CorridorRoomDescription))
                .Cast<CorridorRoomDescription>()
                .Distinct()
                .CreateIntMapping();

            var corridorRoomTemplateInstances = corridorRoomDescriptionsMapping
                .Keys
                .ToDictionary(
                    x => x,
                    x => x.RoomTemplates.SelectMany(y => roomTemplateInstances[y]).ToList());

            var nodesToCorridorMapping = GetNodesToCorridorMapping(mapDescription)
                .ToDictionary(
                    x => x.Key,
                    x => corridorRoomDescriptionsMapping[x.Value] + 1
                );

            var configurationSpaces = new ConfigurationSpaces<TConfiguration>(lineIntersection,
                roomTemplateInstancesCount, graph.VerticesCount, (configuration1, configuration2) =>
                {
                    if (nodesToCorridorMapping.TryGetValue(new Tuple<int, int>(configuration1.Node, configuration2.Node), out var corridor))
                    {
                        return corridor;
                    }

                    return 0;
                });

            // Generate configuration spaces
            foreach (var shape1 in roomTemplateInstancesMapping.Keys)
            {
                foreach (var shape2 in roomTemplateInstancesMapping.Keys)
                {
                    var configurationSpacesList = new ConfigurationSpace[corridorRoomDescriptionsMapping.Count + 1];

                    configurationSpacesList[0] = GetConfigurationSpace(shape1, shape2);

                    foreach (var pair in corridorRoomDescriptionsMapping)
                    {
                        var roomDescription = pair.Key;
                        var intAlias = pair.Value;

                        configurationSpacesList[intAlias + 1] = GetConfigurationSpaceOverCorridors(shape1, shape2,
                            corridorRoomTemplateInstances[roomDescription]);
                    }

                    configurationSpaces.AddConfigurationSpace(shape1, shape2, configurationSpacesList.ToArray());
                }
            }

            foreach (var vertex in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(vertex);

                foreach (var roomTemplate in roomDescription.RoomTemplates)
                {
                    var instances = roomTemplateInstances[roomTemplate];

                    foreach (var roomTemplateInstance in instances)
                    {
                        configurationSpaces.AddShapeForNode(vertex, roomTemplateInstance, 1d / instances.Count);
                    }
                }
            }

            return configurationSpaces;
        }

        public Dictionary<Tuple<int, int>, CorridorRoomDescription> GetNodesToCorridorMapping(IMapDescription<int> mapDescription)
        {
            var mapping = new Dictionary<Tuple<int, int>, CorridorRoomDescription>();

            var graph = mapDescription.GetGraph();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                if (roomDescription is CorridorRoomDescription corridorRoomDescription)
                {
                    var neighbors = graph.GetNeighbours(room).ToList();
                    mapping.Add(new Tuple<int, int>(neighbors[0], neighbors[1]), corridorRoomDescription);
                    mapping.Add(new Tuple<int, int>(neighbors[1], neighbors[0]), corridorRoomDescription);
                }
            }

            return mapping;
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridors(RoomTemplateInstance roomTemplateInstance, RoomTemplateInstance fixedRoomTemplateInstance, List<RoomTemplateInstance> corridors)
        {
            var configurationSpaceLines = new List<OrthogonalLine>();

            foreach (var corridor in corridors)
            {
                var corridorConfigurationSpace = GetConfigurationSpaceOverCorridor(
                    roomTemplateInstance.RoomShape, roomTemplateInstance.DoorLines,
                    fixedRoomTemplateInstance.RoomShape, fixedRoomTemplateInstance.DoorLines,
                    corridor.RoomShape, corridor.DoorLines);

                configurationSpaceLines.AddRange(corridorConfigurationSpace.Lines);
            }

            configurationSpaceLines = lineIntersection.RemoveIntersections(configurationSpaceLines);

            return new ConfigurationSpace()
            {
                Lines = configurationSpaceLines,
            };
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(GridPolygon polygon, IDoorMode doorsMode,
            GridPolygon fixedPolygon, IDoorMode fixedDoorsMode, GridPolygon corridor,
            IDoorMode corridorDoorsMode)
        {
            var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);
            var doorLinesFixed = doorHandler.GetDoorPositions(fixedPolygon, fixedDoorsMode);
            var doorLinesCorridor = doorHandler.GetDoorPositions(corridor, corridorDoorsMode);

            return GetConfigurationSpaceOverCorridor(polygon, doorLines, fixedPolygon, doorLinesFixed, corridor,
                doorLinesCorridor);
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(GridPolygon polygon, List<DoorLine> doorLines, GridPolygon fixedPolygon, List<DoorLine> fixedDoorLines, GridPolygon corridor, List<DoorLine> corridorDoorLines)
        {
            var fixedAndCorridorConfigurationSpace = GetConfigurationSpace(corridor, corridorDoorLines, fixedPolygon, fixedDoorLines);
            var newCorridorDoorLines = new List<DoorLine>();
            corridorDoorLines = DoorUtils.MergeDoorLines(corridorDoorLines);
                
            foreach (var corridorPositionLine in fixedAndCorridorConfigurationSpace.Lines)
            {
                foreach (var corridorDoorLine in corridorDoorLines)
                {
                    var rotation = corridorDoorLine.Line.ComputeRotation();
                    var rotatedLine = corridorDoorLine.Line.Rotate(rotation);
                    var rotatedCorridorLine = corridorPositionLine.Rotate(rotation).GetNormalized();

                    if (rotatedCorridorLine.GetDirection() == OrthogonalLine.Direction.Right)
                    {
                        var correctPositionLine = (rotatedCorridorLine + rotatedLine.From);
                        var correctLengthLine = new OrthogonalLine(correctPositionLine.From, correctPositionLine.To + rotatedLine.Length * rotatedLine.GetDirectionVector(), rotatedCorridorLine.GetDirection());
                        var correctRotationLine = correctLengthLine.Rotate(-rotation);

                        // TODO: problem with corridors overlapping
                        newCorridorDoorLines.Add(new DoorLine(correctRotationLine, corridorDoorLine.Length));
                    } else if (rotatedCorridorLine.GetDirection() == OrthogonalLine.Direction.Top)
                    {
                        foreach (var corridorPosition in rotatedCorridorLine.GetPoints())
                        {
                            var transformedDoorLine = rotatedLine + corridorPosition;
                            var newDoorLine = transformedDoorLine.Rotate(-rotation);

                            // TODO: problem with corridors overlapping
                            // TODO: problem with too many small lines instead of bigger lines
                            newCorridorDoorLines.Add(new DoorLine(newDoorLine, corridorDoorLine.Length));
                        }
                    }
                }
            }

            var configurationSpace = GetConfigurationSpace(polygon, doorLines, fixedPolygon, newCorridorDoorLines);
            configurationSpace.ReverseDoors = null;

            return configurationSpace;
        }

        private ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, List<DoorLine> doorLines, GridPolygon fixedCenter, List<DoorLine> doorLinesFixed, List<int> offsets = null)
		{
			if (offsets != null && offsets.Count == 0)
				throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

			var configurationSpaceLines = new List<OrthogonalLine>();
			var reverseDoor = new List<Tuple<OrthogonalLine, DoorLine>>();

			doorLines = DoorUtils.MergeDoorLines(doorLines);
			doorLinesFixed = DoorUtils.MergeDoorLines(doorLinesFixed);

			// One list for every direction
            // TODO: maybe use dictionary instead of array?
			var lines = new List<DoorLine>[5];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<DoorLine>();
			}

			// Populate lists with lines
			foreach (var line in doorLinesFixed)
			{
				lines[(int) line.Line.GetDirection()].Add(line);
			}

			foreach (var doorLine in doorLines)
			{
				var line = doorLine.Line;
				var oppositeDirection = OrthogonalLine.GetOppositeDirection(line.GetDirection());
				var rotation = line.ComputeRotation();
				var rotatedLine = line.Rotate(rotation);
				var correspondingLines = lines[(int)oppositeDirection].Where(x => x.Length == doorLine.Length).Select(x => new DoorLine(x.Line.Rotate(rotation), x.Length));

				foreach (var cDoorLine in correspondingLines)
				{
					var cline = cDoorLine.Line;
					var y = cline.From.Y - rotatedLine.From.Y;
					var from = new Vector2Int(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - doorLine.Length), y);
					var to = new Vector2Int(cline.To.X - rotatedLine.From.X - (rotatedLine.Length + doorLine.Length), y);

					if (from.X < to.X) continue;

					if (offsets == null)
					{
						var resultLine = new OrthogonalLine(from, to, OrthogonalLine.Direction.Left).Rotate(-rotation);
						reverseDoor.Add(Tuple.Create(resultLine, new DoorLine(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length)));
						configurationSpaceLines.Add(resultLine);
					}
					else
					{
						foreach (var offset in offsets)
						{
							var offsetVector = new Vector2Int(0, offset);
							var resultLine = new OrthogonalLine(from - offsetVector, to - offsetVector, OrthogonalLine.Direction.Left).Rotate(-rotation);
							reverseDoor.Add(Tuple.Create(resultLine, new DoorLine(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length)));
							configurationSpaceLines.Add(resultLine);
						}
					}
				}
			}

			// Remove all positions when the two polygons overlap
			configurationSpaceLines = RemoveOverlapping(polygon, fixedCenter, configurationSpaceLines);

			// Remove all non-unique positions
			configurationSpaceLines = lineIntersection.RemoveIntersections(configurationSpaceLines);

			return new ConfigurationSpace() { Lines = configurationSpaceLines, ReverseDoors = reverseDoor };
		}

		/// <summary>
		/// Computes configuration space of given two polygons.
		/// </summary>
		/// <param name="polygon"></param>
		/// <param name="doorsMode"></param>
		/// <param name="fixedCenter"></param>
		/// <param name="fixedDoorsMode"></param>
		/// <param name="offsets"></param>
		/// <returns></returns>
		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, IDoorMode doorsMode, GridPolygon fixedCenter,
			IDoorMode fixedDoorsMode, List<int> offsets = null)
		{
			var doorLinesFixed = doorHandler.GetDoorPositions(fixedCenter, fixedDoorsMode);
			var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);

			return GetConfigurationSpace(polygon, doorLines, fixedCenter, doorLinesFixed, offsets);
		}

        public ConfigurationSpace GetConfigurationSpace(RoomTemplateInstance roomTemplateInstance, RoomTemplateInstance fixedRoomTemplateInstance, List<int> offsets = null)
        {
            return GetConfigurationSpace(roomTemplateInstance.RoomShape, roomTemplateInstance.DoorLines,
                fixedRoomTemplateInstance.RoomShape, fixedRoomTemplateInstance.DoorLines, offsets);
        }

        /// <summary>
        /// Returns a list of positions such that a given polygon does not overlap a given fixed one.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="fixedCenter"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private List<OrthogonalLine> RemoveOverlapping(GridPolygon polygon, GridPolygon fixedCenter, List<OrthogonalLine> lines)
        {
            var nonOverlapping = new List<OrthogonalLine>();

            foreach (var line in lines)
            {
                var overlapAlongLine = polygonOverlap.OverlapAlongLine(polygon, fixedCenter, line);

                var lastOverlap = false;
                var lastPoint = line.From;

                foreach (var @event in overlapAlongLine)
                {
                    var point = @event.Item1;
                    var overlap = @event.Item2;

                    if (overlap && !lastOverlap)
                    {
                        var endPoint = point + -1 * line.GetDirectionVector();

                        if (line.Contains(endPoint) != -1)
                        {
                            nonOverlapping.Add(new OrthogonalLine(lastPoint, endPoint));
                        }
                    }

                    lastOverlap = overlap;
                    lastPoint = point;
                }

                if (overlapAlongLine.Count == 0)
                {
                    nonOverlapping.Add(line);
                }
                else if (!lastOverlap && lastPoint != line.To)
                {
                    nonOverlapping.Add(new OrthogonalLine(lastPoint, line.To));
                }
            }

            return nonOverlapping;
        }

        public List<RoomTemplateInstance> GetRoomTemplateInstances(RoomTemplate roomTemplate)
        {
            var result = new List<RoomTemplateInstance>();
            var doorLines = doorHandler.GetDoorPositions(roomTemplate.Shape, roomTemplate.DoorsMode);
            var shape = roomTemplate.Shape;

            foreach (var transformation in roomTemplate.AllowedTransformations)
            {
                var transformedShape = shape.Transform(transformation);
                var smallestPoint = transformedShape.BoundingRectangle.A;

                // Both the shape and doors are moved so the polygon is in the first quadrant and touches axes
                transformedShape = transformedShape + (-1 * smallestPoint);
                transformedShape = polygonUtils.NormalizePolygon(transformedShape);
                var transformedDoorLines = doorLines
                    .Select(x => DoorUtils.TransformDoorLine(x, transformation))
                    .Select(x => new DoorLine(x.Line + (-1 * smallestPoint), x.Length))
                    .ToList();

                // Check if we already have the same room shape (together with door lines)
                var sameRoomShapeFound = false;
                foreach (var roomInfo in result)
                {
                    if (roomInfo.RoomShape.Equals(transformedShape) &&
                        roomInfo.DoorLines.SequenceEqualWithoutOrder(transformedDoorLines))
                    {
                        roomInfo.Transformations.Add(transformation);

                        sameRoomShapeFound = true;
                        break;
                    }
                }

                if (sameRoomShapeFound)
                    continue;

                result.Add(new RoomTemplateInstance(roomTemplate, transformedShape, transformation, transformedDoorLines));
            }

            return result;
		}
    }
}