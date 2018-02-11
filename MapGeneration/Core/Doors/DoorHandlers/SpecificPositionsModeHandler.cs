namespace MapGeneration.Core.Doors.DoorHandlers
{
	using System;
	using System.Collections.Generic;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class SpecificPositionsModeHandler : IDoorHandler
	{
		public List<DoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorModeRaw)
		{
			if (!(doorModeRaw is SpecificPositionsMode doorMode))
				throw new InvalidOperationException("Invalid door mode supplied");

			var doors = new List<DoorLine>();

			foreach (var doorPosition in doorMode.DoorPositions)
			{
				doors.Add(GetDoorLine(polygon, doorPosition));
			}

			return doors;
		}

		private DoorLine GetDoorLine(GridPolygon polygon, OrthogonalLine doorPosition)
		{
			if (doorPosition.Length == 0)
				throw new InvalidOperationException();

			foreach (var side in polygon.GetLines())
			{
				if (side.Contains(doorPosition.From) == -1 || side.Contains(doorPosition.To) == -1)
					continue;

				var isGoodDirection = doorPosition.From + doorPosition.Length * side.GetDirectionVector() == doorPosition.To;
				var from = isGoodDirection ? doorPosition.From : doorPosition.To;

				return new DoorLine(new OrthogonalLine(from, from, side.GetDirection()), doorPosition.Length);
			}

			throw new InvalidOperationException("Given door position is not on any side of the polygon");
		}
	}
}