namespace MapGeneration.Core.Doors.DoorHandlers
{
	using System;
	using System.Collections.Generic;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces.Core.Doors;

	public class OverlapModeHandler : IDoorHandler
	{
		public List<IDoorLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorModeRaw)
		{
			if (!(doorModeRaw is OverlapMode doorMode)) 
				throw new InvalidOperationException("Invalid door mode supplied");

			var lines = new List<IDoorLine>();

			foreach (var line in polygon.GetLines())
			{
				if (line.Length < 2 * doorMode.MinimumOverlap + doorMode.DoorLength)
					continue;
				
				lines.Add(new DoorLine(line.Shrink(doorMode.MinimumOverlap, doorMode.MinimumOverlap + doorMode.DoorLength), doorMode.DoorLength));
			}

			return lines;
		}
	}
}