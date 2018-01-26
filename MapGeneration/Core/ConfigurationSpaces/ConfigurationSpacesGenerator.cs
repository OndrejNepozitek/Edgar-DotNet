namespace MapGeneration.Core.ConfigurationSpaces
{
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ConfigurationSpacesGenerator
	{
		private readonly IPolygonOverlap polygonOverlap;
		private readonly IDoorHandler doorHandler;
		private readonly ILineIntersection<OrthogonalLine> lineIntersection;
		private readonly IPolygonUtils<GridPolygon> polygonUtils;

		public ConfigurationSpacesGenerator(
			IPolygonOverlap polygonOverlap,
			IDoorHandler doorHandler,
			ILineIntersection<OrthogonalLine> lineIntersection,
			IPolygonUtils<GridPolygon> polygonUtils)
		{
			this.polygonOverlap = polygonOverlap;
			this.doorHandler = doorHandler;
			this.lineIntersection = lineIntersection;
			this.polygonUtils = polygonUtils;
		}

		public IConfigurationSpaces<int, IntAlias<GridPolygon>, Configuration> Generate<TNode>(MapDescription<TNode> mapDescription)
		{
			var graph = mapDescription.GetGraph();
			var aliasCounter = 0;
			var allShapes = new Dictionary<int, Tuple<IntAlias<GridPolygon>, List<OrthogonalLine>>>();
			var shapes = new List<ConfigurationSpaces.WeightedShape>();
			var shapesForNodes = new Dictionary<int, List<ConfigurationSpaces.WeightedShape>>();

			// Handle universal shapes
			foreach (var shape in mapDescription.RoomShapes)
			{
				var rotatedShapes = PreparePolygons(shape.RoomDescription, shape.ShouldRotate, ref aliasCounter);

				rotatedShapes.ForEach(x => allShapes.Add(x.Item1.Alias, x));
				shapes.AddRange(rotatedShapes.Select(x => new ConfigurationSpaces.WeightedShape(x.Item1, shape.Probability)));
			}

			// Handle shapes for nodes
			foreach (var vertex in graph.Vertices)
			{
				var node = graph.GetVertex(vertex);
				var shapesForNode = mapDescription.RoomShapesForNodes[node];

				if (shapesForNode == null)
				{
					shapesForNodes.Add(vertex, null);
					continue;
				}

				var shapesContainer = new List<ConfigurationSpaces.WeightedShape>();
				foreach (var shape in shapesForNode)
				{
					var rotatedShapes = PreparePolygons(shape.RoomDescription, shape.ShouldRotate, ref aliasCounter);

					rotatedShapes.ForEach(x => allShapes.Add(x.Item1.Alias, x));
					shapesContainer.AddRange(rotatedShapes.Select(x => new ConfigurationSpaces.WeightedShape(x.Item1, shape.Probability)));
				}

				shapesForNodes.Add(vertex, shapesContainer);
			}

			// Prepare data structures
			var shapesForNodesArray = new List<ConfigurationSpaces.WeightedShape>[shapesForNodes.Count];

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
						GetConfigurationSpace(shape2.Item1.Value, shape2.Item2, shape1.Item1.Value, shape1.Item2);
				}
			}

			return new ConfigurationSpaces(shapes, shapesForNodesArray, configurationSpaces, lineIntersection);
		}

		private List<Tuple<IntAlias<GridPolygon>, List<OrthogonalLine>>> PreparePolygons(
			RoomDescription roomDescription, bool shouldRotate, ref int aliasCounter)
		{
			var result = new List<Tuple<IntAlias<GridPolygon>, List<OrthogonalLine>>>();
			var doorLines = doorHandler.GetDoorPositions(roomDescription.Shape, roomDescription.DoorsMode);
			var shape = roomDescription.Shape;
			var rotations = shouldRotate ? GridPolygon.PossibleRotations : new[] {0};

			foreach (var rotation in rotations)
			{
				var rotatedShape = shape.Rotate(rotation);
				var smallesPoint = rotatedShape.BoundingRectangle.A;

				// Both the shape and doors are moved so the polygon is in the first quadrant and touches axes
				rotatedShape = rotatedShape + (-1 * smallesPoint);
				rotatedShape = polygonUtils.NormalizePolygon(rotatedShape);
				var rotatedDoorLines = doorLines.Select(x => x.Rotate(rotation) + (-1 * smallesPoint)).ToList();

				if (result.Any(x => x.Item1.Value.Equals(rotatedShape)))
					continue;

				result.Add(Tuple.Create(new IntAlias<GridPolygon>(aliasCounter++, rotatedShape), rotatedDoorLines));
			}

			return result;
		}

		private ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, List<OrthogonalLine> doorLines, GridPolygon fixedCenter, List<OrthogonalLine> doorLinesFixed)
		{
			var configurationSpaceLines = new List<OrthogonalLine>();

			// One list for every direction
			var lines = new List<OrthogonalLine>[4];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<OrthogonalLine>();
			}

			// Populate lists with lines
			foreach (var line in doorLinesFixed)
			{
				lines[(int) line.GetDirection()].Add(line);
			}

			foreach (var line in doorLines)
			{
				var oppositeDirection = OrthogonalLine.GetOppositeDirection(line.GetDirection());
				var rotation = line.ComputeRotation();
				var rotatedLine = line.Rotate(rotation);
				var correspondingLines = lines[(int)oppositeDirection].Select(x => x.Rotate(rotation));

				foreach (var cline in correspondingLines)
				{
					var y = cline.From.Y - rotatedLine.From.Y;
					var from = new IntVector2(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - 1), y);
					var to = new IntVector2(cline.To.X - rotatedLine.From.X - (rotatedLine.Length + 1), y);

					if (from.X < to.X) continue;

					var resultLine = new OrthogonalLine(from, to).Rotate(-rotation);
					configurationSpaceLines.Add(resultLine);
				}

			}

			// Remove all positions when the two polygons overlap
			configurationSpaceLines = RemoveOverlapping(polygon, fixedCenter, configurationSpaceLines);

			// Remove all non-unique positions
			configurationSpaceLines = RemoveIntersections(configurationSpaceLines);

			return new ConfigurationSpace() { Lines = configurationSpaceLines };
		}

		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, IDoorMode doorsMode, GridPolygon fixedCenter,
			IDoorMode fixedDoorsMode)
		{
			var doorLinesFixed = doorHandler.GetDoorPositions(fixedCenter, fixedDoorsMode);
			var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);

			return GetConfigurationSpace(polygon, doorLines, fixedCenter, doorLinesFixed);
		}

		private List<OrthogonalLine> RemoveOverlapping(GridPolygon polygon, GridPolygon fixedCenter, List<OrthogonalLine> lines)
		{
			var nonOverlapping = new List<OrthogonalLine>();

			foreach (var line in lines)
			{
				nonOverlapping.AddRange(PartitionLine(line, point => polygonOverlap.DoOverlap(polygon, point, fixedCenter, new IntVector2(0, 0))));
			}

			return nonOverlapping;
		}

		private List<OrthogonalLine> RemoveIntersections(List<OrthogonalLine> lines)
		{
			var linesWithoutIntersections = new List<OrthogonalLine>();

			foreach (var line in lines)
			{
				var intersection =
					lineIntersection.GetIntersections(new List<OrthogonalLine>() {line}, linesWithoutIntersections);

				if (intersection.Count == 0)
				{
					linesWithoutIntersections.Add(line);
				}
				else
				{
					var intersectionPoints = intersection.SelectMany(x => x.GetPoints()).ToList();
					// TODO: this is far from optimal as it checks if the point is contained among all points
					//- it could utilize that intersection is given as lines and work with endpoints
					linesWithoutIntersections.AddRange(PartitionLine(line, point => intersectionPoints.Contains(point)));
				}
			}

			return linesWithoutIntersections;
		}

		private List<OrthogonalLine> PartitionLine(OrthogonalLine line, Predicate<IntVector2> predicate)
		{
			var result = new List<OrthogonalLine>();
			var normalized = line.GetNormalized();

			if (normalized.GetDirection() == OrthogonalLine.Direction.Right)
			{
				var lastRemoved = normalized.From.X - 1;
				var y = normalized.From.Y;
				for (var i = normalized.From.X; i <= normalized.To.X + 1; i++)
				{
					var point = new IntVector2(i, y);
					if (predicate(point) || i == normalized.To.X + 1)
					{
						if (lastRemoved != i - 1)
						{
							result.Add(new OrthogonalLine(new IntVector2(lastRemoved + 1, y), new IntVector2(i - 1, y)));
						}

						lastRemoved = i;
					}
				}
			}
			else
			{
				var lastRemoved = normalized.From.Y - 1;
				var x = normalized.From.X;
				for (var i = normalized.From.Y; i <= normalized.To.Y + 1; i++)
				{
					var point = new IntVector2(x, i);
					if (predicate(point) || i == normalized.To.Y + 1)
					{
						if (lastRemoved != i - 1)
						{
							result.Add(new OrthogonalLine(new IntVector2(x, lastRemoved + 1), new IntVector2(x, i - 1)));
						}

						lastRemoved = i;
					}
				}
			}

			return result;
		}
	}
}
