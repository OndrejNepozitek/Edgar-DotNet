namespace GeneralAlgorithms.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using DataStructures.Common;

	/// <summary>
	/// Interface for computing when polygons overlap.
	/// </summary>
	/// <typeparam name="TShape">This generic type lets us improve performance by using IntAlias.</typeparam>
	public interface IPolygonOverlap<in TShape>
	{
		/// <summary>
		/// Checks if two polygons overlap.
		/// </summary>
		/// <param name="polygon1"></param>
		/// <param name="position1"></param>
		/// <param name="polygon2"></param>
		/// <param name="position2"></param>
		/// <returns></returns>
		bool DoOverlap(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2);

		/// <summary>
		/// Computes the area of overlap of two given polygons.
		/// </summary>
		/// <param name="polygon1"></param>
		/// <param name="position1"></param>
		/// <param name="polygon2"></param>
		/// <param name="position2"></param>
		/// <returns></returns>
		int OverlapArea(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2);

		/// <summary>
		/// Checks if two polygons touch 
		/// </summary>
		/// <param name="polygon1"></param>
		/// <param name="position1"></param>
		/// <param name="polygon2"></param>
		/// <param name="position2"></param>
		/// <param name="minimumLength"></param>
		/// <returns></returns>
		bool DoTouch(TShape polygon1, IntVector2 position1, TShape polygon2, IntVector2 position2, int minimumLength = 1);

		/// <summary>
		/// Returns a list of point where a given moving polygon starts or ends overlapping a given fixed polygon.
		/// </summary>
		/// <remarks>
		/// True - starts overlapping (inclusive)
		/// False - ends overlapping (inclusive)
		/// 
		/// The list must be normalized - containing the minimum number of points required.
		/// Default value of the list is false - empty list means no overlap.
		/// </remarks>
		/// <param name="movingPolygon">A polygon that will move along a given line.</param>
		/// <param name="fixedPolygon">A polygon that stays fixed.</param>
		/// <param name="line"></param>
		/// <returns></returns>
		IList<Tuple<IntVector2, bool>> OverlapAlongLine(TShape movingPolygon, TShape fixedPolygon, OrthogonalLine line);
	}
}