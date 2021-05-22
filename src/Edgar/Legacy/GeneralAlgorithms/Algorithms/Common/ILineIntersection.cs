using System.Collections.Generic;
using System.Collections.Immutable;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Common
{
    public interface ILineIntersection<TLine>
	{
		/// <summary>
		/// Get all intersections where one line is from the first set and the other one from the second one.
		/// </summary>
		/// <remarks>
		/// There may be duplicities in the output if the two sets have duplicities themselves.
		/// </remarks>
		/// <param name="lines1"></param>
		/// <param name="lines2"></param>
		/// <returns></returns>
		List<TLine> GetIntersections(List<TLine> lines1, List<TLine> lines2);

		/// <summary>
		/// Like GetIntersections() but only reports if there is an intersection.
		/// </summary>
		/// <param name="lines1"></param>
		/// <param name="lines2"></param>
		/// <returns></returns>
		bool DoIntersect(IEnumerable<TLine> lines1, List<TLine> lines2);

		/// <summary>
		/// Checks if a set of lines intersect with a given line.
		/// </summary>
		/// <remarks>
		/// The ImmutableArray parameter is much faster than IEnumerable/IList if this function is called very often.
		/// </remarks>
		/// <param name="lines"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		bool DoIntersect(ImmutableArray<TLine> lines, TLine line);

		/// <summary>
		/// Gets intersection between two given lines.
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <param name="intersection"></param>
		/// <returns>True if there is a non-empty intersection.</returns>
		bool TryGetIntersection(TLine line1, TLine line2, out TLine intersection);

        List<TLine> RemoveIntersections(List<TLine> lines);

		/// <summary>
		/// Returns a list of lines obtained by removing all the intersections from the original line.
		/// </summary>
		/// <remarks>
		/// Intersection lines must lie on a given line
		/// Intersection lines must not overlap
		/// Intersection lines may be single points - lines with length 0
		/// </remarks>
		/// <param name="line">Line to be partitioned</param>
		/// <param name="intersection"></param>
		/// <returns></returns>
		List<TLine> PartitionByIntersection(TLine line, IList<TLine> intersection);

    }
}