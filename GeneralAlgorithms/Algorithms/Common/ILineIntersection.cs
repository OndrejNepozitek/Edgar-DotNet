namespace GeneralAlgorithms.Algorithms.Common
{
	using System.Collections.Generic;

	public interface ILineIntersection<T>
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
		List<T> GetIntersections(List<T> lines1, List<T> lines2);

		/// <summary>
		/// Like GetIntersections() but only reports if there is an intersection.
		/// </summary>
		/// <param name="lines1"></param>
		/// <param name="lines2"></param>
		/// <returns></returns>
		bool DoIntersect(List<T> lines1, List<T> lines2);

		/// <summary>
		/// Gets intersection between two given lines.
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <param name="intersection"></param>
		/// <returns>True if there is a non-empty intersection.</returns>
		bool TryGetIntersection(T line1, T line2, out T intersection);
	}
}