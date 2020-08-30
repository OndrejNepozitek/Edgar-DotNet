using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.Legacy.Core.Doors.ManualMode
{
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
		public List<DoorLine> GetDoorPositions(PolygonGrid2D polygon, IDoorMode doorMode)
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

		private IEnumerable<DoorLine> GetDoorLine(PolygonGrid2D polygon, OrthogonalLineGrid2D doorPosition)
		{
			var found = false;

			foreach (var side in polygon.GetLines())
			{
				if (side.Contains(doorPosition.From) == -1 || side.Contains(doorPosition.To) == -1)
					continue;

				var isGoodDirection = doorPosition.From + doorPosition.Length * side.GetDirectionVector() == doorPosition.To;
				var from = isGoodDirection ? doorPosition.From : doorPosition.To;

				found = true;
				yield return new DoorLine(new OrthogonalLineGrid2D(from, from, side.GetDirection()), doorPosition.Length);
			}

            if (found == false)
            {
				throw new ArgumentException($"The door line {doorPosition.ToStringShort()} is not on the outline of the polygon {polygon}. Make sure that all the door lines of a manual door mode are on the outline of the polygon.");
            }
        }
	}
}