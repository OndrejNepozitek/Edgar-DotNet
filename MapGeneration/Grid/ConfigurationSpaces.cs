namespace MapGeneration.Grid
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

	public class ConfigurationSpaces
	{
		private readonly List<GridPolygon> polygons;
		private readonly Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>> configugurationSpaces;

		public ConfigurationSpaces(Dictionary<GridPolygon, Dictionary<GridPolygon, ConfigurationSpace>> configugurationSpaces)
		{
			this.configugurationSpaces = configugurationSpaces;
			polygons = configugurationSpaces.Keys.ToList();
		}

		public List<IntVector2> GetIntersection(List<Configuration> configurations)
		{
			throw new NotImplementedException();
		}

		public List<IntVector2> GetMaximumIntersection(List<Configuration> configurations, Configuration mainConfiguration)
		{
			throw new NotImplementedException();
		}

		public List<GridPolygon> GetPolygons()
		{
			// TODO: Maybe return only a readonly collection
			return polygons;
		}
	}
}
