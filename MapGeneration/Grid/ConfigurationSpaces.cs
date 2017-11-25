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

			foreach (var list in configugurationSpaces.Values.SelectMany(x => x.Values).Select(x => x.Points))
			{
				list.Sort();
			}
		}

		public List<IntVector2> GetMaximumIntersection(List<Configuration> configurations, Configuration mainConfiguration)
		{
			var spaces = configugurationSpaces[mainConfiguration.Polygon];

			for (var i = configurations.Count; i > 0; i--)
			{
				foreach (var indices in configurations.GetCombinations(i))
				{
					IEnumerable<IntVector2> points = null;
					IEnumerable<IntVector2> points2 = null;

					foreach (var index in indices)
					{
						var newPoints = spaces[configurations[index].Polygon].Points.Select(x => x + configurations[index].Position);
						points = points != null ? IntersectSorted(points, newPoints) : newPoints;

						if (!points.Any())
						{
							break;
						}
					}

					if (points != null && points.Any())
					{
						return points.ToList();
					}
				}
			}

			throw new InvalidOperationException("There should always be at least one point in the intersection");
		}

		private static IEnumerable<T> IntersectSorted<T>(IEnumerable<T> sequence1,
			IEnumerable<T> sequence2) where T : IComparable<T>
		{
			using (var cursor1 = sequence1.GetEnumerator())
			using (var cursor2 = sequence2.GetEnumerator())
			{
				if (!cursor1.MoveNext() || !cursor2.MoveNext())
				{
					yield break;
				}
				var value1 = cursor1.Current;
				var value2 = cursor2.Current;

				while (true)
				{
					int comparison = value1.CompareTo(value2);
					if (comparison < 0)
					{
						if (!cursor1.MoveNext())
						{
							yield break;
						}
						value1 = cursor1.Current;
					}
					else if (comparison > 0)
					{
						if (!cursor2.MoveNext())
						{
							yield break;
						}
						value2 = cursor2.Current;
					}
					else
					{
						yield return value1;
						if (!cursor1.MoveNext() || !cursor2.MoveNext())
						{
							yield break;
						}
						value1 = cursor1.Current;
						value2 = cursor2.Current;
					}
				}
			}
		}

		public List<GridPolygon> GetPolygons()
		{
			// TODO: Maybe return only a readonly collection
			return polygons;
		}
	}
}
