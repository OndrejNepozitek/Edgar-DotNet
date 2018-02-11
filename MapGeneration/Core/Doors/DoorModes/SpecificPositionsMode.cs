namespace MapGeneration.Core.Doors.DoorModes
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces;

	public class SpecificPositionsMode : IDoorMode
	{
		public List<OrthogonalLine> DoorPositions { get; }

		public SpecificPositionsMode(List<OrthogonalLine> doorPositions)
		{
			DoorPositions = doorPositions;
		}
	}
}