namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Polygons;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;

	public class ConfigurationSpacesGenerator
	{
		private readonly GridPolygonUtils polygonUtils = new GridPolygonUtils();
		private readonly GridPolygonOverlap polygonOverlap = new GridPolygonOverlap();

		public ConfigurationSpaces Generate(List<GridPolygon> polygons, bool rotate = true)
		{
			if (polygons.Any(x => !polygonUtils.CheckIntegrity(x)))
			{
				throw new InvalidOperationException("One or more polygons are not valid.");
			}

			var allPolygons = ProcessPolygons(polygons, rotate);
			var configurationSpaces = new Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>>();

			foreach (var p1 in allPolygons)
			{
				var spaces = new Dictionary<GridPolygon, ConfigurationSpace>();

				foreach (var p2 in allPolygons)
				{
					var configurationSpace = GetConfigurationSpace(p1, p2);
					spaces.Add(p2, configurationSpace);
				}

				configurationSpaces.Add(p1, spaces);
			}

			return new ConfigurationSpaces(configurationSpaces);
		}

		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, GridPolygon fixedCenter, int minimumCommonLength = 4)
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

			// Populate lists with lines
			foreach (var line in fixedCenter.GetLines())
			{
				lines[(int) line.GetDirection()].Add(line);
			}

			foreach (var line in polygon.GetLines())
			{
				switch (line.GetDirection())
				{
					case IntLine.Direction.Left:
					{
						var correspondingLines = lines[(int)IntLine.Direction.Right];

						foreach (var cline in correspondingLines)
						{
							var y = cline.From.Y - line.From.Y;
							var from = new IntVector2(cline.From.X - line.To.X - (line.Length - minimumCommonLength), y);
							var to = new IntVector2(cline.To.X - line.From.X + (line.Length - minimumCommonLength), y);

							if (from.X > to.X) continue;

							var resultLine = new IntLine(from, to);
							points.AddRange(resultLine.GetPoints());
						}

						break;
					}

					case IntLine.Direction.Right:
					{
						var correspondingLines = lines[(int)IntLine.Direction.Left];

						foreach (var cline in correspondingLines)
						{
							var y = cline.From.Y - line.From.Y;
							var from = new IntVector2(cline.From.X - line.To.X + (line.Length - minimumCommonLength), y);
							var to = new IntVector2(cline.To.X - line.From.X - (line.Length - minimumCommonLength), y);

							if (from.X < to.X) continue;

							var resultLine = new IntLine(from, to);
							points.AddRange(resultLine.GetPoints());
						}

						break;
					}

					case IntLine.Direction.Top:
					{
						var correspondingLines = lines[(int)IntLine.Direction.Bottom];

						foreach (var cline in correspondingLines)
						{
							var x = cline.From.X - line.From.X;
							var from = new IntVector2(x, cline.From.Y - line.To.Y + (line.Length - minimumCommonLength));
							var to = new IntVector2(x, cline.To.Y - line.From.Y - (line.Length - minimumCommonLength));

							if (from.Y < to.Y) continue;

							var resultLine = new IntLine(from, to);
							points.AddRange(resultLine.GetPoints());
						}

						break;
					}

					case IntLine.Direction.Bottom:
					{
						var correspondingLines = lines[(int)IntLine.Direction.Top];

						foreach (var cline in correspondingLines)
						{
							var x = cline.From.X - line.From.X;
							var from = new IntVector2(x, cline.From.Y - line.To.Y - (line.Length - minimumCommonLength));
							var to = new IntVector2(x, cline.To.Y - line.From.Y + (line.Length - minimumCommonLength));

							if (from.Y > to.Y) continue;

							var resultLine = new IntLine(from, to);
							points.AddRange(resultLine.GetPoints());
						}

						break;
					}

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			points = points.Distinct().Where(x => !polygonOverlap.DoOverlap(polygon, x, fixedCenter, new IntVector2(0, 0))).ToList();

			return new ConfigurationSpace() { Points = points };
		}

		private List<GridPolygon> ProcessPolygons(IEnumerable<GridPolygon> polygons, bool rotate)
		{
			var newPolygons = new List<GridPolygon>();

			foreach (var polygon in polygons)
			{
				if (rotate)
				{
					foreach (var rotation in polygon.GetAllRotations().Select(x => polygonUtils.NormalizePolygon(x)))
					{
						// TODO: do we want duplicates?
						if (!newPolygons.Contains(rotation))
						{
							newPolygons.Add(rotation);
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
