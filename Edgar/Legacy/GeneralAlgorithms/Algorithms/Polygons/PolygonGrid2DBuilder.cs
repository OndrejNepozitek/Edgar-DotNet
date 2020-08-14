using System.Collections.Generic;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons
{
    /// <summary>
	/// Helper class for creating polygons.
	/// </summary>
	public class PolygonGrid2DBuilder
	{
		private readonly List<Vector2Int> points = new List<Vector2Int>();

		/// <summary>
		/// Adds point to the polygon.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public PolygonGrid2DBuilder AddPoint(int x, int y)
		{
			points.Add(new Vector2Int(x, y));
			return this;
		}

		/// <summary>
		/// Builds the polygon from added points.
		/// </summary>
		/// <returns></returns>
		public PolygonGrid2D Build()
		{
			return new PolygonGrid2D(points);
		}
	}
}
