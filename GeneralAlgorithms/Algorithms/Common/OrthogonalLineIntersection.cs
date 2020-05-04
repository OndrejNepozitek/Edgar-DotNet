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
		public bool DoIntersect(IEnumerable<OrthogonalLine> lines1, List<OrthogonalLine> lines2)
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

        public List<OrthogonalLine> RemoveIntersections(List<OrthogonalLine> lines)
        {
            var linesWithoutIntersections = new List<OrthogonalLine>();

            foreach (var line in lines)
            {
                var intersection = GetIntersections(new List<OrthogonalLine>() { line }, linesWithoutIntersections);
				
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

        public List<OrthogonalLine> PartitionByIntersection(OrthogonalLine line, IList<OrthogonalLine> intersection)
        {
            var result = new List<OrthogonalLine>();
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
                    result.Add(new OrthogonalLine(lastPoint + directionVector,
                        intersectionLine.From - directionVector, rotatedLine.GetDirection()));
                }

                lastPoint = intersectionLine.To;
            }

            if (rotatedLine.To != lastPoint && rotatedLine.To - directionVector != lastPoint)
            {
                result.Add(new OrthogonalLine(lastPoint + directionVector, rotatedLine.To, rotatedLine.GetDirection()));
            }

            return result.Select(x => x.Rotate(-rotation, true)).ToList();
		}
    }
}