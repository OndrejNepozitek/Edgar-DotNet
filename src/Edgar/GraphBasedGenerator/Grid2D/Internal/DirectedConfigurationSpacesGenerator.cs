using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D.Internal
{
    public class DirectedConfigurationSpacesGenerator
    {
        private readonly IPolygonOverlap<PolygonGrid2D> polygonOverlap;
        private readonly ILineIntersection<OrthogonalLineGrid2D> lineIntersection;
        private readonly IDoorSocketResolver doorSocketResolver;

        public DirectedConfigurationSpacesGenerator(IPolygonOverlap<PolygonGrid2D> polygonOverlap, ILineIntersection<OrthogonalLineGrid2D> lineIntersection, IDoorSocketResolver doorSocketResolver = null)
        {
            this.polygonOverlap = polygonOverlap;
            this.lineIntersection = lineIntersection;
            this.doorSocketResolver = doorSocketResolver ?? new DefaultDoorSocketResolver(); ;
        }

        private bool AreSocketsCompatible(IDoorSocket socket1, IDoorSocket socket2)
        {
            return doorSocketResolver.AreCompatible(socket1, socket2);
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridors(RoomTemplateInstanceGrid2D roomTemplateInstance, RoomTemplateInstanceGrid2D fixedRoomTemplateInstance, List<RoomTemplateInstanceGrid2D> corridors, ConfigurationSpaceDirection direction)
        {
            var configurationSpaceLines = new List<OrthogonalLineGrid2D>();

            foreach (var corridor in corridors)
            {
                var corridorConfigurationSpace = GetConfigurationSpaceOverCorridor(
                    roomTemplateInstance.RoomShape, roomTemplateInstance.DoorLines,
                    fixedRoomTemplateInstance.RoomShape, fixedRoomTemplateInstance.DoorLines,
                    corridor.RoomShape, corridor.DoorLines, direction);

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
            IDoorModeGrid2D corridorDoorsMode, ConfigurationSpaceDirection direction)
        {
            var doorLines = doorsMode.GetDoors(polygon);
            var doorLinesFixed = fixedDoorsMode.GetDoors(fixedPolygon);
            var doorLinesCorridor = corridorDoorsMode.GetDoors(corridor);

            return GetConfigurationSpaceOverCorridor(polygon, doorLines, fixedPolygon, doorLinesFixed, corridor,
                doorLinesCorridor, direction);
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(PolygonGrid2D polygon, List<DoorLineGrid2D> doorLines, PolygonGrid2D fixedPolygon, List<DoorLineGrid2D> fixedDoorLines, PolygonGrid2D corridor, List<DoorLineGrid2D> corridorDoorLines, ConfigurationSpaceDirection direction)
        {
            var fixedAndCorridorConfigurationSpace = GetConfigurationSpace(corridor, corridorDoorLines, fixedPolygon, fixedDoorLines, direction);
            var newCorridorDoorLines = new List<DoorLineGrid2D>();
            corridorDoorLines = DoorUtils.MergeDoorLines(corridorDoorLines);

            foreach (var corridorPositionLine in fixedAndCorridorConfigurationSpace.Lines)
            {
                foreach (var corridorDoorLine in corridorDoorLines)
                {
                    var rotation = corridorDoorLine.Line.ComputeRotation();
                    var rotatedLine = corridorDoorLine.Line.Rotate(rotation);
                    var rotatedCorridorLine = corridorPositionLine.Rotate(rotation).GetNormalized();

                    if (rotatedCorridorLine.GetDirection() == OrthogonalLineGrid2D.Direction.Right)
                    {
                        var correctPositionLine = (rotatedCorridorLine + rotatedLine.From);
                        var correctLengthLine = new OrthogonalLineGrid2D(correctPositionLine.From, correctPositionLine.To + rotatedLine.Length * rotatedLine.GetDirectionVector(), rotatedCorridorLine.GetDirection());
                        var correctRotationLine = correctLengthLine.Rotate(-rotation);

                        // TODO: problem with corridors overlapping
                        newCorridorDoorLines.Add(new DoorLineGrid2D(correctRotationLine, corridorDoorLine.Length, corridorDoorLine.DoorSocket, corridorDoorLine.Type));
                    }
                    else if (rotatedCorridorLine.GetDirection() == OrthogonalLineGrid2D.Direction.Top)
                    {
                        foreach (var corridorPosition in rotatedCorridorLine.GetPoints())
                        {
                            var transformedDoorLine = rotatedLine + corridorPosition;
                            var newDoorLine = transformedDoorLine.Rotate(-rotation);

                            // TODO: problem with corridors overlapping
                            // TODO: problem with too many small lines instead of bigger lines
                            newCorridorDoorLines.Add(new DoorLineGrid2D(newDoorLine, corridorDoorLine.Length, corridorDoorLine.DoorSocket, corridorDoorLine.Type));
                        }
                    }
                }
            }

            var configurationSpace = GetConfigurationSpace(polygon, doorLines, fixedPolygon, newCorridorDoorLines, direction);
            // configurationSpace.ReverseDoors = null;

            return new ConfigurationSpace()
            {
                Lines = configurationSpace.Lines.ToList()
            };
        }

        private ConfigurationSpaceGrid2D GetConfigurationSpace(PolygonGrid2D polygon, List<DoorLineGrid2D> doorLines, PolygonGrid2D fixedCenter, List<DoorLineGrid2D> doorLinesFixed, ConfigurationSpaceDirection direction, List<int> offsets = null)
		{
			if (offsets != null && offsets.Count == 0)
				throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

			var configurationSpaceLines = new List<OrthogonalLineGrid2D>();
			var reverseDoor = new List<ConfigurationSpaceSourceGrid2D>();

			doorLines = DoorUtils.MergeDoorLines(doorLines);
			doorLinesFixed = DoorUtils.MergeDoorLines(doorLinesFixed);

			doorLines = doorLines.Where(x =>
                x.Type == DoorType.Undirected ||
                (direction == ConfigurationSpaceDirection.FromFixedToMoving && x.Type == DoorType.Entrance) ||
                (direction == ConfigurationSpaceDirection.FromMovingToFixed && x.Type == DoorType.Exit)
            ).ToList();
            doorLinesFixed = doorLinesFixed.Where(x =>
                x.Type == DoorType.Undirected || 
                (direction == ConfigurationSpaceDirection.FromFixedToMoving && x.Type == DoorType.Exit) ||
                (direction == ConfigurationSpaceDirection.FromMovingToFixed && x.Type == DoorType.Entrance)
                ).ToList();

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
				lines[(int)line.Line.GetDirection()].Add(line);
			}

			foreach (var doorLine in doorLines)
			{
				var line = doorLine.Line;
				var oppositeDirection = OrthogonalLineGrid2D.GetOppositeDirection(line.GetDirection());
				var rotation = line.ComputeRotation();
				var rotatedLine = line.Rotate(rotation);
				var correspondingLines = lines[(int)oppositeDirection].Where(x => x.Length == doorLine.Length && AreSocketsCompatible(x.DoorSocket, doorLine.DoorSocket)).Select(x => new DoorLineGrid2D(x.Line.Rotate(rotation), x.Length, x.DoorSocket, x.Type));

				foreach (var cDoorLine in correspondingLines)
				{
					var cline = cDoorLine.Line;
					var y = cline.From.Y - rotatedLine.From.Y;
					var from = new Vector2Int(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - doorLine.Length), y);
					var to = new Vector2Int(cline.To.X - rotatedLine.From.X - (rotatedLine.Length + doorLine.Length), y);

					if (from.X < to.X) continue;

                    if (offsets == null)
                    {
                        var resultLine = new OrthogonalLineGrid2D(from, to, OrthogonalLineGrid2D.Direction.Left).Rotate(-rotation);
                        var configurationSpaceSource = new ConfigurationSpaceSourceGrid2D(
                            resultLine,
                            new DoorLineGrid2D(
                                cDoorLine.Line.Rotate(-rotation),
                                cDoorLine.Length,
                                cDoorLine.DoorSocket,
                                cDoorLine.Type),
                            doorLine
                        );

                        reverseDoor.Add(configurationSpaceSource);
                        configurationSpaceLines.Add(resultLine);
                    }
                    else
                    {
                        foreach (var offset in offsets)
                        {
                            var offsetVector = new Vector2Int(0, offset);
                            var resultLine = new OrthogonalLineGrid2D(from - offsetVector, to - offsetVector, OrthogonalLineGrid2D.Direction.Left).Rotate(-rotation);
                            var configurationSpaceSource = new ConfigurationSpaceSourceGrid2D(
                                resultLine,
                                new DoorLineGrid2D(
                                    cDoorLine.Line.Rotate(-rotation),
                                    cDoorLine.Length,
                                    cDoorLine.DoorSocket,
                                    cDoorLine.Type),
                                doorLine
                            );

                            reverseDoor.Add(configurationSpaceSource);
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

        public ConfigurationSpaceGrid2D GetConfigurationSpace(PolygonGrid2D polygon, IDoorModeGrid2D doorsMode, PolygonGrid2D fixedCenter,
            IDoorModeGrid2D fixedDoorsMode, ConfigurationSpaceDirection direction, List<int> offsets = null)
        {
            var doorLinesFixed = fixedDoorsMode.GetDoors(fixedCenter);
            var doorLines = doorsMode.GetDoors(polygon);

            return GetConfigurationSpace(polygon, doorLines, fixedCenter, doorLinesFixed, direction, offsets);
        }

		public ConfigurationSpaceGrid2D GetConfigurationSpace(RoomTemplateInstanceGrid2D roomTemplateInstance, RoomTemplateInstanceGrid2D fixedRoomTemplateInstance, ConfigurationSpaceDirection direction, List<int> offsets = null)
		{
			return GetConfigurationSpace(roomTemplateInstance.RoomShape, roomTemplateInstance.DoorLines,
				fixedRoomTemplateInstance.RoomShape, fixedRoomTemplateInstance.DoorLines, direction, offsets);
		}

        /// <summary>
        /// Returns a list of positions such that a given polygon does not overlap a given fixed one.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="fixedCenter"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private List<OrthogonalLineGrid2D> RemoveOverlapping(PolygonGrid2D polygon, PolygonGrid2D fixedCenter, List<OrthogonalLineGrid2D> lines)
        {
            var nonOverlapping = new List<OrthogonalLineGrid2D>();

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
                            nonOverlapping.Add(new OrthogonalLineGrid2D(lastPoint, endPoint));
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
                    nonOverlapping.Add(new OrthogonalLineGrid2D(lastPoint, line.To));
                }
            }

            return nonOverlapping;
        }
    }
}