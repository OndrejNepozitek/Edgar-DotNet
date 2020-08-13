using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D.Doors;
using Edgar.Legacy.Core.Configurations.Interfaces;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapDescriptions.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils;
using IRoomDescription = Edgar.GraphBasedGenerator.Common.RoomTemplates.IRoomDescription;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Class responsible for generating configuration spaces.
    /// </summary>
    public class ConfigurationSpacesGenerator
    {
        private readonly IPolygonOverlap<PolygonGrid2D> polygonOverlap;
        private readonly IDoorHandler doorHandler;
        private readonly ILineIntersection<OrthogonalLine> lineIntersection;
        private readonly IPolygonUtils<PolygonGrid2D> polygonUtils;

        public ConfigurationSpacesGenerator(IPolygonOverlap<PolygonGrid2D> polygonOverlap, IDoorHandler doorHandler, ILineIntersection<OrthogonalLine> lineIntersection, IPolygonUtils<PolygonGrid2D> polygonUtils)
        {
            this.polygonOverlap = polygonOverlap;
            this.doorHandler = doorHandler;
            this.lineIntersection = lineIntersection;
            this.polygonUtils = polygonUtils;
        }
        
        // TODO: remove when possible
        public Dictionary<Tuple<TNode, TNode>, IRoomDescription> GetNodesToCorridorMapping<TNode>(ILevelDescription<TNode> mapDescription)
        {
            var mapping = new Dictionary<Tuple<TNode, TNode>, IRoomDescription>();

            var graph = mapDescription.GetGraph();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                if (roomDescription.IsCorridor)
                {
                    var neighbors = graph.GetNeighbours(room).ToList();
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[0], neighbors[1]), roomDescription);
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[1], neighbors[0]), roomDescription);
                }
            }

            return mapping;
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridors(RoomTemplateInstanceGrid2D roomTemplateInstance, RoomTemplateInstanceGrid2D fixedRoomTemplateInstance, List<RoomTemplateInstanceGrid2D> corridors)
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

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(PolygonGrid2D polygon, IDoorModeGrid2D doorsMode,
            PolygonGrid2D fixedPolygon, IDoorModeGrid2D fixedDoorsMode, PolygonGrid2D corridor,
            IDoorModeGrid2D corridorDoorsMode)
        {
            var doorLines = doorsMode.GetDoors(polygon);
            var doorLinesFixed = fixedDoorsMode.GetDoors(fixedPolygon);
            var doorLinesCorridor = corridorDoorsMode.GetDoors(corridor);

            return GetConfigurationSpaceOverCorridor(polygon, doorLines, fixedPolygon, doorLinesFixed, corridor,
                doorLinesCorridor);
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(PolygonGrid2D polygon, List<DoorLineGrid2D> doorLines, PolygonGrid2D fixedPolygon, List<DoorLineGrid2D> fixedDoorLines, PolygonGrid2D corridor, List<DoorLineGrid2D> corridorDoorLines)
        {
            var fixedAndCorridorConfigurationSpace = GetConfigurationSpace(corridor, corridorDoorLines, fixedPolygon, fixedDoorLines);
            var newCorridorDoorLines = new List<DoorLineGrid2D>();
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
                        newCorridorDoorLines.Add(new DoorLineGrid2D(correctRotationLine, corridorDoorLine.Length, corridorDoorLine.DoorSocket));
                    } else if (rotatedCorridorLine.GetDirection() == OrthogonalLine.Direction.Top)
                    {
                        foreach (var corridorPosition in rotatedCorridorLine.GetPoints())
                        {
                            var transformedDoorLine = rotatedLine + corridorPosition;
                            var newDoorLine = transformedDoorLine.Rotate(-rotation);

                            // TODO: problem with corridors overlapping
                            // TODO: problem with too many small lines instead of bigger lines
                            newCorridorDoorLines.Add(new DoorLineGrid2D(newDoorLine, corridorDoorLine.Length, corridorDoorLine.DoorSocket));
                        }
                    }
                }
            }

            var configurationSpace = GetConfigurationSpace(polygon, doorLines, fixedPolygon, newCorridorDoorLines);
            // configurationSpace.ReverseDoors = null;

            return new ConfigurationSpace()
            {
                Lines = configurationSpace.Lines.ToList()
            };
        }

        private ConfigurationSpaceGrid2D GetConfigurationSpace(PolygonGrid2D polygon, List<DoorLineGrid2D> doorLines, PolygonGrid2D fixedCenter, List<DoorLineGrid2D> doorLinesFixed, List<int> offsets = null)
		{
			if (offsets != null && offsets.Count == 0)
				throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

			var configurationSpaceLines = new List<OrthogonalLine>();
			var reverseDoor = new List<Tuple<OrthogonalLine, DoorLineGrid2D>>();

			doorLines = DoorUtils.MergeDoorLines(doorLines);
			doorLinesFixed = DoorUtils.MergeDoorLines(doorLinesFixed);

			// One list for every direction
            // TODO: maybe use dictionary instead of array?
			var lines = new List<DoorLineGrid2D>[5];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<DoorLineGrid2D>();
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
				var correspondingLines = lines[(int)oppositeDirection].Where(x => x.Length == doorLine.Length && x.DoorSocket == doorLine.DoorSocket).Select(x => new DoorLineGrid2D(x.Line.Rotate(rotation), x.Length, x.DoorSocket));

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
						reverseDoor.Add(Tuple.Create(resultLine, new DoorLineGrid2D(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length, cDoorLine.DoorSocket)));
						configurationSpaceLines.Add(resultLine);
					}
					else
					{
						foreach (var offset in offsets)
						{
							var offsetVector = new Vector2Int(0, offset);
							var resultLine = new OrthogonalLine(from - offsetVector, to - offsetVector, OrthogonalLine.Direction.Left).Rotate(-rotation);
							reverseDoor.Add(Tuple.Create(resultLine, new DoorLineGrid2D(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length, cDoorLine.DoorSocket)));
							configurationSpaceLines.Add(resultLine);
						}
					}
				}
			}

			// Remove all positions when the two polygons overlap
			configurationSpaceLines = RemoveOverlapping(polygon, fixedCenter, configurationSpaceLines);

			// Remove all non-unique positions
			configurationSpaceLines = lineIntersection.RemoveIntersections(configurationSpaceLines);

			return new ConfigurationSpaceGrid2D(configurationSpaceLines, reverseDoor);
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
		public ConfigurationSpaceGrid2D GetConfigurationSpace(PolygonGrid2D polygon, IDoorModeGrid2D doorsMode, PolygonGrid2D fixedCenter,
			IDoorModeGrid2D fixedDoorsMode, List<int> offsets = null)
		{
			var doorLinesFixed = fixedDoorsMode.GetDoors(fixedCenter);
			var doorLines = doorsMode.GetDoors(polygon);

			return GetConfigurationSpace(polygon, doorLines, fixedCenter, doorLinesFixed, offsets);
		}

        public ConfigurationSpaceGrid2D GetConfigurationSpace(RoomTemplateInstanceGrid2D roomTemplateInstance, RoomTemplateInstanceGrid2D fixedRoomTemplateInstance, List<int> offsets = null)
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
        private List<OrthogonalLine> RemoveOverlapping(PolygonGrid2D polygon, PolygonGrid2D fixedCenter, List<OrthogonalLine> lines)
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

        public List<RoomTemplateInstanceGrid2D> GetRoomTemplateInstances(RoomTemplateGrid2D roomTemplate)
        {
            var result = new List<RoomTemplateInstanceGrid2D>();
            var doorLines = roomTemplate.Doors.GetDoors(roomTemplate.Outline);
            var shape = roomTemplate.Outline;

            foreach (var transformation in roomTemplate.AllowedTransformations)
            {
                var transformedShape = shape.Transform(transformation);
                var smallestPoint = transformedShape.BoundingRectangle.A;

                // Both the shape and doors are moved so the polygon is in the first quadrant and touches axes
                transformedShape = transformedShape + (-1 * smallestPoint);
                transformedShape = polygonUtils.NormalizePolygon(transformedShape);
                var transformedDoorLines = doorLines
                    .Select(x => DoorUtils.TransformDoorLine(x, transformation))
                    .Select(x => new DoorLineGrid2D(x.Line + (-1 * smallestPoint), x.Length, x.DoorSocket))
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

                result.Add(new RoomTemplateInstanceGrid2D(roomTemplate, transformedShape, transformedDoorLines, new List<Transformation>() { transformation }));
            }

            return result;
        }
    }
}