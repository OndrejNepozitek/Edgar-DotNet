using System.ComponentModel;
using MapGeneration.Core.Doors.Interfaces;

namespace MapGeneration.Core.Doors.DoorHandlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;

    /// <summary>
	/// Generates door positions for <see cref="ManualDoorMode"/>.
	/// </summary>
	public class ManualDoorModeHandler : IDoorHandler
	{
		/// <inheritdoc />
		/// <remarks>
		/// Checks if all positions are contained on one of polygon's sides.
		/// Changes direction if needed.
		/// </remarks>
		public List<DoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorMode)
		{
			if (!(doorMode is ManualDoorMode manualDoorMode))
				throw new InvalidOperationException("Invalid door mode supplied");

			if (manualDoorMode.DoorPositions.Distinct().Count() != manualDoorMode.DoorPositions.Count)
				throw new ArgumentException("All door positions must be unique");

			var doors = new List<DoorLine>();

			foreach (var doorPosition in manualDoorMode.DoorPositions)
			{
				doors.AddRange(GetDoorLine(polygon, doorPosition));
			}

			return doors;
		}

		private IEnumerable<DoorLine> GetDoorLine(GridPolygon polygon, OrthogonalLine doorPosition)
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
            {
				throw new ArgumentException($"The door line {doorPosition.ToStringShort()} is not on the outline of the polygon {polygon}. Make sure that all the door lines of a manual door mode are on the outline of the polygon.");
            }
        }
	}
}