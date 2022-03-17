using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Configurations.Interfaces;
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

namespace Edgar.Legacy.Core.ConfigurationSpaces
{
    /// <summary>
    /// Class responsible for generating configuration spaces.
    /// </summary>
    public class ConfigurationSpacesGenerator
    {
        private readonly IPolygonOverlap<PolygonGrid2D> polygonOverlap;
        private readonly IDoorHandler doorHandler;
        private readonly ILineIntersection<OrthogonalLineGrid2D> lineIntersection;
        private readonly IPolygonUtils<PolygonGrid2D> polygonUtils;

        public ConfigurationSpacesGenerator(IPolygonOverlap<PolygonGrid2D> polygonOverlap, IDoorHandler doorHandler,
            ILineIntersection<OrthogonalLineGrid2D> lineIntersection, IPolygonUtils<PolygonGrid2D> polygonUtils)
        {
            this.polygonOverlap = polygonOverlap;
            this.doorHandler = doorHandler;
            this.lineIntersection = lineIntersection;
            this.polygonUtils = polygonUtils;
        }

        public ConfigurationSpaces<TConfiguration> GetConfigurationSpaces<TConfiguration>(
            IMapDescription<int> mapDescription)
            where TConfiguration : IConfiguration<IntAlias<PolygonGrid2D>, int>
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
                    if (nodesToCorridorMapping.TryGetValue(
                            new Tuple<int, int>(configuration1.Node, configuration2.Node), out var corridor))
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

        public Dictionary<Tuple<TNode, TNode>, CorridorRoomDescription> GetNodesToCorridorMapping<TNode>(
            IMapDescription<TNode> mapDescription)
        {
            var mapping = new Dictionary<Tuple<TNode, TNode>, CorridorRoomDescription>();

            var graph = mapDescription.GetGraph();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                if (roomDescription is CorridorRoomDescription corridorRoomDescription)
                {
                    var neighbors = graph.GetNeighbors(room).ToList();
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[0], neighbors[1]), corridorRoomDescription);
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[1], neighbors[0]), corridorRoomDescription);
                }
            }

            return mapping;
        }

        // TODO: remove when possible
        public Dictionary<Tuple<TNode, TNode>, IRoomDescription> GetNodesToCorridorMapping<TNode>(
            ILevelDescription<TNode> mapDescription)
        {
            var mapping = new Dictionary<Tuple<TNode, TNode>, IRoomDescription>();

            var graph = mapDescription.GetGraph();

            foreach (var room in graph.Vertices)
            {
                var roomDescription = mapDescription.GetRoomDescription(room);

                if (roomDescription.IsCorridor)
                {
                    var neighbors = graph.GetNeighbors(room).ToList();
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[0], neighbors[1]), roomDescription);
                    mapping.Add(new Tuple<TNode, TNode>(neighbors[1], neighbors[0]), roomDescription);
                }
            }

            return mapping;
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridors(RoomTemplateInstance roomTemplateInstance,
            RoomTemplateInstance fixedRoomTemplateInstance, List<RoomTemplateInstance> corridors)
        {
            var configurationSpaceLines = new List<OrthogonalLineGrid2D>();

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

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(PolygonGrid2D polygon, IDoorMode doorsMode,
            PolygonGrid2D fixedPolygon, IDoorMode fixedDoorsMode, PolygonGrid2D corridor,
            IDoorMode corridorDoorsMode)
        {
            var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);
            var doorLinesFixed = doorHandler.GetDoorPositions(fixedPolygon, fixedDoorsMode);
            var doorLinesCorridor = doorHandler.GetDoorPositions(corridor, corridorDoorsMode);

            return GetConfigurationSpaceOverCorridor(polygon, doorLines, fixedPolygon, doorLinesFixed, corridor,
                doorLinesCorridor);
        }

        public ConfigurationSpace GetConfigurationSpaceOverCorridor(PolygonGrid2D polygon, List<DoorLine> doorLines,
            PolygonGrid2D fixedPolygon, List<DoorLine> fixedDoorLines, PolygonGrid2D corridor,
            List<DoorLine> corridorDoorLines)
        {
            var fixedAndCorridorConfigurationSpace =
                GetConfigurationSpace(corridor, corridorDoorLines, fixedPolygon, fixedDoorLines);
            var newCorridorDoorLines = new List<DoorLine>();
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
                        var correctLengthLine = new OrthogonalLineGrid2D(correctPositionLine.From,
                            correctPositionLine.To + rotatedLine.Length * rotatedLine.GetDirectionVector(),
                            rotatedCorridorLine.GetDirection());
                        var correctRotationLine = correctLengthLine.Rotate(-rotation);

                        // TODO: problem with corridors overlapping
                        newCorridorDoorLines.Add(new DoorLine(correctRotationLine, corridorDoorLine.Length));
                    }
                    else if (rotatedCorridorLine.GetDirection() == OrthogonalLineGrid2D.Direction.Top)
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

        private ConfigurationSpace GetConfigurationSpace(PolygonGrid2D polygon, List<DoorLine> doorLines,
            PolygonGrid2D fixedCenter, List<DoorLine> doorLinesFixed, List<int> offsets = null)
        {
            if (offsets != null && offsets.Count == 0)
                throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

            var configurationSpaceLines = new List<OrthogonalLineGrid2D>();
            var reverseDoor = new List<Tuple<OrthogonalLineGrid2D, DoorLine>>();

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
                var oppositeDirection = OrthogonalLineGrid2D.GetOppositeDirection(line.GetDirection());
                var rotation = line.ComputeRotation();
                var rotatedLine = line.Rotate(rotation);
                var correspondingLines = lines[(int) oppositeDirection].Where(x => x.Length == doorLine.Length)
                    .Select(x => new DoorLine(x.Line.Rotate(rotation), x.Length));

                foreach (var cDoorLine in correspondingLines)
                {
                    var cline = cDoorLine.Line;
                    var y = cline.From.Y - rotatedLine.From.Y;
                    var from = new Vector2Int(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - doorLine.Length),
                        y);
                    var to = new Vector2Int(cline.To.X - rotatedLine.From.X - (rotatedLine.Length + doorLine.Length),
                        y);

                    if (from.X < to.X) continue;

                    if (offsets == null)
                    {
                        var resultLine =
                            new OrthogonalLineGrid2D(from, to, OrthogonalLineGrid2D.Direction.Left).Rotate(-rotation);
                        reverseDoor.Add(Tuple.Create(resultLine,
                            new DoorLine(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length)));
                        configurationSpaceLines.Add(resultLine);
                    }
                    else
                    {
                        foreach (var offset in offsets)
                        {
                            var offsetVector = new Vector2Int(0, offset);
                            var resultLine = new OrthogonalLineGrid2D(from - offsetVector, to - offsetVector,
                                OrthogonalLineGrid2D.Direction.Left).Rotate(-rotation);
                            reverseDoor.Add(Tuple.Create(resultLine,
                                new DoorLine(cDoorLine.Line.Rotate(-rotation), cDoorLine.Length)));
                            configurationSpaceLines.Add(resultLine);
                        }
                    }
                }
            }

            // Remove all positions when the two polygons overlap
            configurationSpaceLines = RemoveOverlapping(polygon, fixedCenter, configurationSpaceLines);

            // Remove all non-unique positions
            configurationSpaceLines = lineIntersection.RemoveIntersections(configurationSpaceLines);

            return new ConfigurationSpace() {Lines = configurationSpaceLines, ReverseDoors = reverseDoor};
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
        public ConfigurationSpace GetConfigurationSpace(PolygonGrid2D polygon, IDoorMode doorsMode,
            PolygonGrid2D fixedCenter,
            IDoorMode fixedDoorsMode, List<int> offsets = null)
        {
            var doorLinesFixed = doorHandler.GetDoorPositions(fixedCenter, fixedDoorsMode);
            var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);

            return GetConfigurationSpace(polygon, doorLines, fixedCenter, doorLinesFixed, offsets);
        }

        public ConfigurationSpace GetConfigurationSpace(RoomTemplateInstance roomTemplateInstance,
            RoomTemplateInstance fixedRoomTemplateInstance, List<int> offsets = null)
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
        private List<OrthogonalLineGrid2D> RemoveOverlapping(PolygonGrid2D polygon, PolygonGrid2D fixedCenter,
            List<OrthogonalLineGrid2D> lines)
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

                result.Add(new RoomTemplateInstance(roomTemplate, transformedShape, transformation,
                    transformedDoorLines));
            }

            return result;
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
                    .Select(x => new DoorLineGrid2D(x.Line + (-1 * smallestPoint), x.Length, x.DoorSocket, x.Type))
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

                result.Add(new RoomTemplateInstanceGrid2D(roomTemplate, transformedShape, transformedDoorLines,
                    new List<TransformationGrid2D>() {transformation}));
            }

            return result;
        }
    }
}