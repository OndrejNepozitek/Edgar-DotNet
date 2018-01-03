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

	public class CSGenerator
	{
		private readonly IPolygonUtils<GridPolygon> polygonUtils;
		private readonly IPolygonOverlap polygonOverlap;
		private readonly IDoorHandler doorHandler;
		private readonly ILineIntersection<OrthogonalLine> lineIntersection;

		public CSGenerator(
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

		/*public IConfigurationSpaces<TNode, IntAlias<GridPolygon>, Configuration> Generate<TNode>(MapDescription<TNode> mapDescription)
		{
			if (polygons.Any(x => !polygonUtils.CheckIntegrity(x)))
			{
				throw new InvalidOperationException("One or more polygons are not valid.");
			}

			var allPolygons = ProcessPolygons(polygons, rotate, normalizeChances);
			var uniquePolygons = allPolygons.Distinct().ToList();
			var configurationSpaces = new Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>>();

			foreach (var p1 in uniquePolygons)
			{
				var spaces = new Dictionary<GridPolygon, ConfigurationSpace>();

				foreach (var p2 in uniquePolygons)
				{
					var configurationSpace = GetConfigurationSpace(p1, p2);
					spaces.Add(p2, configurationSpace);
				}

				configurationSpaces.Add(p1, spaces);
			}

			return new ConfigurationSpaces<>(configurationSpaces, allPolygons);
		}*/

		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, IDoorMode doorsMode, GridPolygon fixedCenter, IDoorMode fixedDoorsMode)
		{
			var configurationSpaceLines = new List<OrthogonalLine>();

			// One list for every direction
			// TODO: polygons must be clockwise oriented
			var lines = new List<OrthogonalLine>[4];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<OrthogonalLine>();
			}

			var doorLinesFixed = doorHandler.GetDoorPositions(fixedCenter, fixedDoorsMode);
			var doorLines = doorHandler.GetDoorPositions(polygon, doorsMode);

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

			configurationSpaceLines = RemoveOverlapping(polygon, fixedCenter, configurationSpaceLines);
			configurationSpaceLines = RemoveIntersections(configurationSpaceLines);

			return new ConfigurationSpace() { Lines = configurationSpaceLines };
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

		/*private List<GridPolygon> ProcessPolygons(IEnumerable<GridPolygon> polygons, bool rotate, bool normalizeChances = false)
		{
			var newPolygons = new List<GridPolygon>();

			foreach (var polygon in polygons)
			{
				if (rotate)
				{
					var rotations = polygon.GetAllRotations().Select(x => polygonUtils.NormalizePolygon(x)).ToList();
					var toAdd = 4;

					foreach (var rotation in rotations)
					{
						// TODO: do we want duplicates?
						if (!newPolygons.Contains(rotation))
						{
							newPolygons.Add(rotation);
							toAdd--;
						}
					}

					if (normalizeChances)
					{
						for (var i = 0; i < toAdd; i++)
						{
							newPolygons.Add(rotations[0]);
						}
					}
				}
				else
				{
					var normalized = polygonUtils.NormalizePolygon(polygon);

					if (!newPolygons.Contains(normalized))
					{
						newPolygons.Add(normalized);
					}
				}
			}

			return newPolygons;
		}*/


	}
}
