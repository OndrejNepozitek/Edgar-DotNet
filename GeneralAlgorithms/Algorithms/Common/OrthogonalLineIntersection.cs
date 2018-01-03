namespace GeneralAlgorithms.Algorithms.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DataStructures.Common;

	public class OrthogonalLineIntersection : ILineIntersection<OrthogonalLine>
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
		public List<OrthogonalLine> GetIntersections(List<OrthogonalLine> lines1, List<OrthogonalLine> lines2)
		{
			return GetIntersectionsLazy(lines1, lines2).ToList();
		}

		private IEnumerable<OrthogonalLine> GetIntersectionsLazy(IEnumerable<OrthogonalLine> lines1, IReadOnlyCollection<OrthogonalLine> lines2)
		{
			foreach (var line1 in lines1)
			{
				foreach (var line2 in lines2)
				{
					if (TryGetIntersection(line1, line2, out var intersection))
					{
						yield return intersection;
					}
				}
			}
		}

		/// <summary>
		/// Like GetIntersections() but only reports if there is an intersection.
		/// </summary>
		/// <param name="lines1"></param>
		/// <param name="lines2"></param>
		/// <returns></returns>
		public bool DoIntersect(List<OrthogonalLine> lines1, List<OrthogonalLine> lines2)
		{
			return GetIntersectionsLazy(lines1, lines2).Any();
		}

		/// <summary>
		/// Gets intersection between two given lines.
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <param name="intersection"></param>
		/// <returns>True if there is a non-empty intersection.</returns>
		public bool TryGetIntersection(OrthogonalLine line1, OrthogonalLine line2, out OrthogonalLine intersection)
		{
			var horizontal1 = line1.From.Y == line1.To.Y;
			var horizontal2 = line2.From.Y == line2.To.Y;

			line1 = line1.GetNormalized();
			line2 = line2.GetNormalized();

			// Both horizontal
			if (horizontal1 && horizontal2)
			{
				if (line1.From.Y != line2.From.Y)
				{
					intersection = new OrthogonalLine();
					return false;
				}

				var x1 = Math.Max(line1.From.X, line2.From.X);
				var x2 = Math.Min(line1.To.X, line2.To.X);

				if (x1 <= x2)
				{
					intersection = new OrthogonalLine(new IntVector2(x1, line1.From.Y), new IntVector2(x2, line1.From.Y));
					return true;
				}

				intersection = new OrthogonalLine();
				return false;
			}
			// Both vertical
			else if (!horizontal1 && !horizontal2)
			{
				if (line1.From.X != line2.From.X)
				{
					intersection = new OrthogonalLine();
					return false;
				}

				var y1 = Math.Max(line1.From.Y, line2.From.Y);
				var y2 = Math.Min(line1.To.Y, line2.To.Y);

				if (y1 <= y2)
				{
					intersection = new OrthogonalLine(new IntVector2(line1.From.X, y1), new IntVector2(line1.From.X, y2));
					return true;
				}

				intersection = new OrthogonalLine();
				return false;
			}
			// One horizontal and one vertical
			else
			{
				var hline = line1;
				var vline = line2;

				if (horizontal2)
				{
					hline = line2;
					vline = line1;
				}

				if (hline.From.X <= vline.From.X &&
				    hline.To.X >= vline.From.X &&
				    vline.From.Y <= hline.From.Y &&
				    vline.To.Y >= hline.From.Y)
				{
					intersection = new OrthogonalLine(new IntVector2(vline.From.X, hline.From.Y), new IntVector2(vline.From.X, hline.From.Y));
					return true;
				}

				intersection = new OrthogonalLine();
				return false;
			}
		}
	}
}