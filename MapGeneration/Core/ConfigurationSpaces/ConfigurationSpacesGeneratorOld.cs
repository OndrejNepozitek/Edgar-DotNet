namespace MapGeneration.Core.ConfigurationSpaces
{
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Doors;
	using Interfaces.Core.Configuration;
	using Interfaces.Core.ConfigurationSpaces;
	using Interfaces.Core.Doors;
	using MapDescriptions;

	/// <summary>
	/// Class that computes configuration spaces.
	/// </summary>
	public class ConfigurationSpacesGeneratorOld
	{
		private readonly IPolygonOverlap<GridPolygon> polygonOverlap;
		private readonly IDoorHandler doorHandler;
		private readonly ILineIntersection<OrthogonalLine> lineIntersection;
		private readonly IPolygonUtils<GridPolygon> polygonUtils;

		/// <summary>
		/// Information about which int alias corresponds to which map description and possible rotation.
		/// </summary>
		/// <remarks>
		/// Is overwritten after each call to Generate().
		/// </remarks>
		public Dictionary<int, RoomTemplateInstance> LastIntAliasMapping { get; private set; }

		public ConfigurationSpacesGeneratorOld(
			IPolygonOverlap<GridPolygon> polygonOverlap,
			IDoorHandler doorHandler,
			ILineIntersection<OrthogonalLine> lineIntersection,
			IPolygonUtils<GridPolygon> polygonUtils)
		{
			this.polygonOverlap = polygonOverlap;
			this.doorHandler = doorHandler;
			this.lineIntersection = lineIntersection;
			this.polygonUtils = polygonUtils;
		}

		/// <summary>
		/// Computes configuration spaces for a given map description.
		/// </summary>
		/// <typeparam name="TNode"></typeparam>
		/// <typeparam name="TConfiguration"></typeparam>
		/// <param name="mapDescriptionOld"></param>
		/// <param name="offsets"></param>
		/// <returns></returns>
		public IConfigurationSpaces<int, IntAlias<GridPolygon>, TConfiguration, ConfigurationSpace> Generate<TNode, TConfiguration>(MapDescriptionOld<TNode> mapDescriptionOld, List<int> offsets = null)
			where TConfiguration : IConfiguration<IntAlias<GridPolygon>, int>
		{
			if (offsets != null && offsets.Count == 0)
				throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

			if (mapDescriptionOld.IsWithCorridors && mapDescriptionOld.GetCorridorShapes().Count == 0)
				throw new ArgumentException("The map description has corridors enabled but there are no shapes for them.", nameof(mapDescriptionOld));

			var graph = mapDescriptionOld.GetGraph();
			var aliasCounter = 0;
			var allShapes = new Dictionary<int, Tuple<IntAlias<GridPolygon>, List<IDoorLine>>>();
			var shapes = new List<ConfigurationSpacesOld<TConfiguration>.WeightedShape>();
			var shapesForNodes = new Dictionary<int, List<ConfigurationSpacesOld<TConfiguration>.WeightedShape>>();
			var intAliasMapping = new Dictionary<int, RoomTemplateInstance>();
			var defaultTransformations = new List<Transformation>(mapDescriptionOld.GetDefaultTransformations());

			// Handle universal shapes
			foreach (var shape in mapDescriptionOld.GetRoomShapes())
			{
				var rotatedShapes = TransformPolygons(shape.RoomTemplate, shape.Transformations ?? defaultTransformations).Select(CreateAlias).ToList();
				var probability = shape.NormalizeProbabilities ? shape.Probability / rotatedShapes.Count : shape.Probability;

				shapes.AddRange(rotatedShapes.Select(x => new ConfigurationSpacesOld<TConfiguration>.WeightedShape(x.Item1, probability)));
			}

			// Handle shapes for nodes
			foreach (var vertex in graph.Vertices.Where(x => !mapDescriptionOld.IsCorridorRoom(x)))
			{
				var shapesForNode = mapDescriptionOld.GetRoomShapesForNodes()[vertex];

				if (shapesForNode == null)
				{
					shapesForNodes.Add(vertex, null);
					continue;
				}

				var shapesContainer = new List<ConfigurationSpacesOld<TConfiguration>.WeightedShape>();
				foreach (var shape in shapesForNode)
				{
					var rotatedShapes = TransformPolygons(shape.RoomTemplate, shape.Transformations ?? defaultTransformations).Select(CreateAlias).ToList();
					var probability = shape.NormalizeProbabilities ? shape.Probability / rotatedShapes.Count : shape.Probability;

					shapesContainer.AddRange(rotatedShapes.Select(x => new ConfigurationSpacesOld<TConfiguration>.WeightedShape(x.Item1, probability)));
				}

				shapesForNodes.Add(vertex, shapesContainer);
			}

			// Corridor shapes
			var corridorShapesContainer = new List<ConfigurationSpacesOld<TConfiguration>.WeightedShape>();
			foreach (var shape in mapDescriptionOld.GetCorridorShapes())
			{
				var rotatedShapes = TransformPolygons(shape.RoomTemplate, shape.Transformations ?? defaultTransformations).Select(CreateAlias).ToList();
				var probability = shape.NormalizeProbabilities ? shape.Probability / rotatedShapes.Count : shape.Probability;

				corridorShapesContainer.AddRange(rotatedShapes.Select(x => new ConfigurationSpacesOld<TConfiguration>.WeightedShape(x.Item1, probability)));
			}

			// Handle shapes for corridores
			foreach (var vertex in graph.Vertices.Where(mapDescriptionOld.IsCorridorRoom))
			{
				shapesForNodes.Add(vertex, corridorShapesContainer);
			}

			// Prepare data structures
			var shapesForNodesArray = new List<ConfigurationSpacesOld<TConfiguration>.WeightedShape>[shapesForNodes.Count];

			foreach (var pair in shapesForNodes)
			{
				shapesForNodesArray[pair.Key] = pair.Value;
			}

			var configurationSpaces = new ConfigurationSpace[aliasCounter][];

			for (var i = 0; i < aliasCounter; i++)
			{
				var shape1 = allShapes[i];
				configurationSpaces[i] = new ConfigurationSpace[aliasCounter];

				for (var j = 0; j < aliasCounter; j++)
				{
					var shape2 = allShapes[j];

					configurationSpaces[i][j] =
						GetConfigurationSpace(shape1.Item1.Value, shape1.Item2, shape2.Item1.Value, shape2.Item2, offsets);
				}
			}

			if (shapes.Count == 0 && graph.Vertices.Any(x => !shapesForNodes.ContainsKey(x) || shapesForNodes[x] == null || shapesForNodes[x].Count == 0))
			{
				throw new ArgumentException("There must be at least one shape for each node", nameof(mapDescriptionOld));
			}

			LastIntAliasMapping = intAliasMapping;

			return new ConfigurationSpacesOld<TConfiguration>(shapes, shapesForNodesArray, configurationSpaces, lineIntersection);

			// Return an already existing alias or create a new one
			Tuple<IntAlias<GridPolygon>, List<IDoorLine>> CreateAlias(RoomTemplateInstance roomInfo)
			{
				foreach (var pair in allShapes)
				{
					var otherRoomInfo = intAliasMapping[pair.Key];

					if (roomInfo.RoomTemplate == otherRoomInfo.RoomTemplate 
					    && roomInfo.Transformations.SequenceEqualWithoutOrder(otherRoomInfo.Transformations))
					{
						return pair.Value;
					}
				}

				var alias = new IntAlias<GridPolygon>(aliasCounter++, roomInfo.RoomShape);
				var aliasTuple = Tuple.Create(alias, roomInfo.DoorLines);
				allShapes.Add(alias.Alias, aliasTuple);
				intAliasMapping.Add(alias.Alias, roomInfo);

				return aliasTuple;
			}
		}

		/// <summary>
		/// Applies all given transformation to a given room description.
		/// </summary>
		/// <remarks>
		/// Groups room shapes that are equal after transformation.
		/// </remarks>
		/// <param name="roomTemplate"></param>
		/// <param name="transformations"></param>
		/// <returns></returns>
		private List<RoomTemplateInstance> TransformPolygons(RoomTemplate roomTemplate, IEnumerable<Transformation> transformations)
		{
			var result = new List<RoomTemplateInstance>();
			var doorLines = doorHandler.GetDoorPositions(roomTemplate.Shape, roomTemplate.DoorsMode);
			var shape = roomTemplate.Shape;

			foreach (var transformation in transformations)
			{
				var transformedShape = shape.Transform(transformation);
				var smallestPoint = transformedShape.BoundingRectangle.A;

				// Both the shape and doors are moved so the polygon is in the first quadrant and touches axes
				transformedShape = transformedShape + (-1 * smallestPoint);
				transformedShape = polygonUtils.NormalizePolygon(transformedShape);
				var transformedDoorLines = doorLines
					.Select(x => DoorUtils.TransformDoorLine(x, transformation))
					.Select(x => new DoorLine(x.Line + (-1 * smallestPoint), x.Length))
					.Cast<IDoorLine>()
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

		private ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, List<IDoorLine> doorLines, GridPolygon fixedCenter, List<IDoorLine> doorLinesFixed, List<int> offsets = null)
		{
			if (offsets != null && offsets.Count == 0)
				throw new ArgumentException("There must be at least one offset if they are set", nameof(offsets));

			var configurationSpaceLines = new List<OrthogonalLine>();
			var reverseDoor = new List<Tuple<OrthogonalLine, DoorLine>>();

			doorLines = DoorUtils.MergeDoorLines(doorLines);
			doorLinesFixed = DoorUtils.MergeDoorLines(doorLinesFixed);

			// One list for every direction
			var lines = new List<IDoorLine>[4];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<IDoorLine>();
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
					var from = new IntVector2(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - doorLine.Length), y);
					var to = new IntVector2(cline.To.X - rotatedLine.From.X - (rotatedLine.Length + doorLine.Length), y);

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
							var offsetVector = new IntVector2(0, offset);
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
			configurationSpaceLines = RemoveIntersections(configurationSpaceLines);

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

		/// <summary>
		/// Returns a list of lines where every point is unique.
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		private List<OrthogonalLine> RemoveIntersections(List<OrthogonalLine> lines)
		{
			    var linesWithoutIntersections = new List<OrthogonalLine>();

			    foreach (var line in lines)
			    {
				    var intersection = lineIntersection.GetIntersections(new List<OrthogonalLine>() {line}, linesWithoutIntersections);

				    if (intersection.Count == 0)
				    {
					    linesWithoutIntersections.Add(line);
				    }
				    else
				    {
					    linesWithoutIntersections.AddRange(PartitionByIntersection(line, intersection));
				    }
			    }

			    return linesWithoutIntersections;
		}

		/// <summary>
		/// Returns a list of lines obtained by removing all the intersections from the original line.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="intersection"></param>
		/// <returns></returns>
		private List<OrthogonalLine> PartitionByIntersection(OrthogonalLine line, IList<OrthogonalLine> intersection)
		{
			var result = new List<OrthogonalLine>();
			var rotation = line.ComputeRotation();
			var rotatedLine = line.Rotate(rotation, true);
			var directionVector = rotatedLine.GetDirectionVector();
			var rotatedIntersection = intersection.Select(x => x.Rotate(rotation, false).GetNormalized()).ToList();
			rotatedIntersection.Sort((x1, x2) => x1.From.CompareTo(x2.From));

			var lastPoint = rotatedLine.From - directionVector;

			foreach (var intersectionLine in rotatedIntersection)
			{
				if (intersectionLine.From != lastPoint && intersectionLine.From - directionVector != lastPoint)
				{
					result.Add(new OrthogonalLine(lastPoint + directionVector, intersectionLine.From - directionVector));
				}

				lastPoint = intersectionLine.To;
			}

			if (rotatedLine.To != lastPoint && rotatedLine.To - directionVector != lastPoint)
			{
				result.Add(new OrthogonalLine(lastPoint + directionVector, rotatedLine.To));
			}

			return result.Select(x => x.Rotate(-rotation, false)).ToList();
		}
    }
}
