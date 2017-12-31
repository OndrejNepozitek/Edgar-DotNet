namespace MapGeneration.Core.ConfigurationSpaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class CSGenerator
	{
		private readonly GridPolygonUtils polygonUtils = new GridPolygonUtils();
		private readonly GridPolygonOverlap polygonOverlap = new GridPolygonOverlap();

		public IConfigurationSpaces<TNode, IntAlias<GridPolygon>, Configuration> Generate<TNode>(MapDescription<TNode> mapDescription)
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
		}

		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, GridPolygon fixedCenter, int doorsMargin = 1)
		{
			var points = new List<IntVector2>();

			// One list for every direction
			// TODO: polygons must be clockwise oriented
			var lines = new List<IntLine>[4];

			// Init array
			for (var i = 0; i < lines.Length; i++)
			{
				lines[i] = new List<IntLine>();
			}

			var doorLinesFixed = fixedCenter.GetLines().Where(x => x.Length > 2 * doorsMargin).Select(x => x.Shrink(doorsMargin));
			var doorLines = polygon.GetLines().Where(x => x.Length > 2 * doorsMargin).Select(x => x.Shrink(doorsMargin));

			// Populate lists with lines
			foreach (var line in doorLinesFixed)
			{
				lines[(int) line.GetDirection()].Add(line);
			}

			foreach (var line in doorLines)
			{
				var oppositeDirection = IntLine.GetOppositeDirection(line.GetDirection());
				var rotation = line.ComputeRotation();
				var rotatedLine = line.Rotate(rotation);
				var correspondingLines = lines[(int)oppositeDirection].Select(x => x.Rotate(rotation));

				foreach (var cline in correspondingLines)
				{
					var y = cline.From.Y - rotatedLine.From.Y;
					var from = new IntVector2(cline.From.X - rotatedLine.To.X + (rotatedLine.Length - 1), y);
					var to = new IntVector2(cline.To.X - rotatedLine.From.X - (rotatedLine.Length - 1), y);

					if (from.X < to.X) continue;

					var resultLine = new IntLine(from, to).Rotate(-rotation);
					points.AddRange(resultLine.GetPoints());
				}

			}

			points = points.Distinct().Where(x => !polygonOverlap.DoOverlap(polygon, x, fixedCenter, new IntVector2(0, 0))).ToList();

			return new ConfigurationSpace() { Points = points };
		}

		private List<GridPolygon> ProcessPolygons(IEnumerable<GridPolygon> polygons, bool rotate, bool normalizeChances = false)
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
		}


	}
}
