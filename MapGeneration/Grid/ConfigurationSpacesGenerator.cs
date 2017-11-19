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
		private GridPolygonUtils polygonUtils = new GridPolygonUtils();
		private GridPolygonOverlap polygonOverlap = new GridPolygonOverlap();

		public ConfigurationSpaces Generate(List<GridPolygon> polygons, bool rotate = true)
		{
			if (polygons.Any(x => !polygonUtils.CheckIntegrity(x)))
			{
				throw new InvalidOperationException("One or more polygons are not valid.");
			}

			var allPolygons = ProcessPolygons(polygons, rotate);

			throw new NotImplementedException();
		}

		public ConfigurationSpace GetConfigurationSpace(GridPolygon polygon, GridPolygon fixedCenter)
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
							var from = new IntVector2(cline.From.X - line.To.X - (line.Length - 1), y);
							var to = new IntVector2(cline.To.X - line.From.X + (line.Length - 1), y);

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
							var from = new IntVector2(cline.From.X - line.To.X + (line.Length - 1), y);
							var to = new IntVector2(cline.To.X - line.From.X - (line.Length - 1), y);

							var resultLine = new IntLine(from, to);
							points.AddRange(resultLine.GetPoints());
						}

						break;
					}
				}
			}

			return new ConfigurationSpace() { Points = points };
		}

		private List<GridPolygon> ProcessPolygons(IEnumerable<GridPolygon> polygons, bool rotate)
		{
			var newPolygons = new List<GridPolygon>();

			foreach (var polygon in polygons)
			{
				if (rotate)
				{
					foreach (var rotation in polygonUtils.GetAllRotations(polygon).Select(x => polygonUtils.NormalizePolygon(x)))
					{
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
