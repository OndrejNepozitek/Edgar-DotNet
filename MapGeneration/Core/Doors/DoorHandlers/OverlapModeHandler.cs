namespace MapGeneration.Core.Doors.DoorHandlers
{
	using System;
	using System.Collections.Generic;
	using DoorModes;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Interfaces;

	public class OverlapModeHandler : IDoorHandler
	{
		public List<OrthogonalLine> GetDoorPositions(GridPolygon polygon, IDoorMode doorModeRaw)
		{
			if (!(doorModeRaw is OverlapMode doorMode)) 
				throw new InvalidOperationException("Invalid door mode supplied");

			var lines = new List<OrthogonalLine>();

			foreach (var line in polygon.GetLines())
			{
				if (line.Length + 1 < 2 * doorMode.MinimumOverlap + doorMode.DoorLength)
					continue;
				
				lines.Add(line.Shrink(doorMode.MinimumOverlap, doorMode.MinimumOverlap + doorMode.DoorLength));
			}

			return lines;
		}
	}
}