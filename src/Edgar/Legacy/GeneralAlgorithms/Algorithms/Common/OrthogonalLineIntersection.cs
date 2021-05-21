using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.GeneralAlgorithms.Algorithms.Common
{
    public class OrthogonalLineIntersection : ILineIntersection<OrthogonalLineGrid2D>
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
		public List<OrthogonalLineGrid2D> GetIntersections(List<OrthogonalLineGrid2D> lines1, List<OrthogonalLineGrid2D> lines2)
		{
			return GetIntersectionsLazy(lines1, lines2).ToList();
		}

		private IEnumerable<OrthogonalLineGrid2D> GetIntersectionsLazy(IEnumerable<OrthogonalLineGrid2D> lines1, IReadOnlyCollection<OrthogonalLineGrid2D> lines2)
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
		
		private IEnumerable<OrthogonalLineGrid2D> GetIntersectionsLazy(IEnumerable<OrthogonalLineGrid2D> lines1, OrthogonalLineGrid2D line)
		{
			foreach (var line1 in lines1)
			{
				if (TryGetIntersection(line1, line, out var intersection))
				{
					yield return intersection;
				}
			}
		}

		/// <summary>
		/// Like GetIntersections() but only reports if there is an intersection.
		/// </summary>
		/// <param name="lines1"></param>
		/// <param name="lines2"></param>
		/// <returns></returns>
		public bool DoIntersect(IEnumerable<OrthogonalLineGrid2D> lines1, List<OrthogonalLineGrid2D> lines2)
		{
			return GetIntersectionsLazy(lines1, lines2).Any();
		}
		
		public bool DoIntersect(ImmutableArray<OrthogonalLineGrid2D> lines, OrthogonalLineGrid2D line)
		{
			foreach (var otherLine in lines)
			{
				if (TryGetIntersection(otherLine, line, out var intersection))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets intersection between two given lines.
		/// </summary>
		/// <param name="line1"></param>
		/// <param name="line2"></param>
		/// <param name="intersection"></param>
		/// <returns>True if there is a non-empty intersection.</returns>
		public bool TryGetIntersection(OrthogonalLineGrid2D line1, OrthogonalLineGrid2D line2, out OrthogonalLineGrid2D intersection)
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
					intersection = new OrthogonalLineGrid2D();
					return false;
				}

				var x1 = System.Math.Max(line1.From.X, line2.From.X);
				var x2 = System.Math.Min(line1.To.X, line2.To.X);

				if (x1 <= x2)
				{
					intersection = new OrthogonalLineGrid2D(new Vector2Int(x1, line1.From.Y), new Vector2Int(x2, line1.From.Y));
					return true;
				}

				intersection = new OrthogonalLineGrid2D();
				return false;
			}
			// Both vertical
			else if (!horizontal1 && !horizontal2)
			{
				if (line1.From.X != line2.From.X)
				{
					intersection = new OrthogonalLineGrid2D();
					return false;
				}

				var y1 = System.Math.Max(line1.From.Y, line2.From.Y);
				var y2 = System.Math.Min(line1.To.Y, line2.To.Y);

				if (y1 <= y2)
				{
					intersection = new OrthogonalLineGrid2D(new Vector2Int(line1.From.X, y1), new Vector2Int(line1.From.X, y2));
					return true;
				}

				intersection = new OrthogonalLineGrid2D();
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
					intersection = new OrthogonalLineGrid2D(new Vector2Int(vline.From.X, hline.From.Y), new Vector2Int(vline.From.X, hline.From.Y));
					return true;
				}

				intersection = new OrthogonalLineGrid2D();
				return false;
			}
		}

        public List<OrthogonalLineGrid2D> RemoveIntersections(List<OrthogonalLineGrid2D> lines)
        {
            var linesWithoutIntersections = new List<OrthogonalLineGrid2D>();

            foreach (var line in lines)
            {
                var intersection = GetIntersections(new List<OrthogonalLineGrid2D>() { line }, linesWithoutIntersections);

                if (intersection.Count == 0)
                {
                    linesWithoutIntersections.Add(line);
                }
                else
                {
                    linesWithoutIntersections.AddRange(PartitionByIntersection(line, intersection));
                }
            }

            return linesWithoutIntersections;
		}

        public List<OrthogonalLineGrid2D> PartitionByIntersection(OrthogonalLineGrid2D line, IList<OrthogonalLineGrid2D> intersection)
        {
            var result = new List<OrthogonalLineGrid2D>();
            var rotation = line.ComputeRotation();
            var rotatedLine = line.Rotate(rotation, true);
            var directionVector = rotatedLine.GetDirectionVector();
            var rotatedIntersection = intersection.Select(x => x.Rotate(rotation, false).GetNormalized()).ToList();
            rotatedIntersection.Sort((x1, x2) => x1.From.CompareTo(x2.From));

            var lastPoint = rotatedLine.From - directionVector;

            for (var i = 0; i < rotatedIntersection.Count; i++)
            {
                var intersectionLine = rotatedIntersection[i];

                if (intersectionLine.From.X < rotatedLine.From.X || intersectionLine.From.X > rotatedLine.To.X)
                {
                    throw new ArgumentException("All intersection lines must lie on the line");
                }

                if (intersectionLine.From.Y != rotatedLine.From.Y || intersectionLine.To.Y != rotatedLine.From.Y)
                {
                    throw new ArgumentException("All intersection lines must lie on the line");
                }

                if (i + 1 < rotatedIntersection.Count)
                {
                    var nextLine = rotatedIntersection[i + 1];

                    if (nextLine.From.X <= intersectionLine.To.X)
                    {
                        throw new ArgumentException("Intersections must not overlap");
					}
                }

                if (intersectionLine.From != lastPoint && intersectionLine.From - directionVector != lastPoint)
                {
                    result.Add(new OrthogonalLineGrid2D(lastPoint + directionVector,
                        intersectionLine.From - directionVector));
                }

                lastPoint = intersectionLine.To;
            }

            if (rotatedLine.To != lastPoint && rotatedLine.To - directionVector != lastPoint)
            {
                result.Add(new OrthogonalLineGrid2D(lastPoint + directionVector, rotatedLine.To));
            }

            return result.Select(x => x.Rotate(-rotation, false)).ToList();
		}
    }
}