namespace MapGeneration.Core.Doors.DoorHandlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Doors;

	/// <summary>
	/// Generates door positions for <see cref="SpecificPositionsMode"/>.
	/// </summary>
	public class SpecificPositionsModeHandler : IDoorHandler
	{
		/// <inheritdoc />
		/// <remarks>
		/// Checks if all positions are contained on one of polygon's sides.
		/// Changes direction if needed.
		/// </remarks>
		public List<IDoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorModeRaw)
		{
			if (!(doorModeRaw is SpecificPositionsMode doorMode))
				throw new InvalidOperationException("Invalid door mode supplied");

			if (doorMode.DoorPositions.Distinct().Count() != doorMode.DoorPositions.Count)
				throw new ArgumentException("All door positions must be unique");

			var doors = new List<IDoorLine>();

			foreach (var doorPosition in doorMode.DoorPositions)
			{
				doors.AddRange(GetDoorLine(polygon, doorPosition));
			}

			return doors;
		}

		private IEnumerable<IDoorLine> GetDoorLine(GridPolygon polygon, OrthogonalLine doorPosition)
		{
			var found = false;

			foreach (var side in polygon.GetLines())
			{
				if (side.Contains(doorPosition.From) == -1 || side.Contains(doorPosition.To) == -1)
					continue;

				var isGoodDirection = doorPosition.From + doorPosition.Length * side.GetDirectionVector() == doorPosition.To;
				var from = isGoodDirection ? doorPosition.From : doorPosition.To;

				found = true;
				yield return new DoorLine(new OrthogonalLine(from, from, side.GetDirection()), doorPosition.Length);
			}

			if (found == false)
				throw new InvalidOperationException("Given door position is not on any side of the polygon");
		}
	}
}