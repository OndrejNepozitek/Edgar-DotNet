namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Utils;

	public class ConfigurationSpaces
	{
		private readonly List<GridPolygon> polygons;
		private readonly Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>> configugurationSpaces;

		public ConfigurationSpaces(Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>> configugurationSpaces)
		{
			this.configugurationSpaces = configugurationSpaces;
			polygons = configugurationSpaces.Keys.ToList();
		}

		public List<IntVector2> GetMaximumIntersection(List<Configuration> configurations, Configuration mainConfiguration)
		{
			var spaces = configugurationSpaces[mainConfiguration.Polygon];

			for (var i = configurations.Count; i > 0; i--)
			{
				foreach (var indices in configurations.GetCombinations(i))
				{
					List<IntVector2> points = null;

					foreach (var index in indices)
					{
						var newPoints = spaces[configurations[index].Polygon].Points.Select(x => x + configurations[index].Position);
						points = points?.Intersect(newPoints).ToList() ?? newPoints.ToList();

						if (points.Count == 0)
						{
							break;
						}
					}

					if (points != null && points.Count != 0)
					{
						return points;
					}
				}
			}

			throw new InvalidOperationException("There should always be at least one point in the intersection");
		}

		public List<GridPolygon> GetPolygons()
		{
			// TODO: Maybe return only a readonly collection
			return polygons;
		}
	}
}
