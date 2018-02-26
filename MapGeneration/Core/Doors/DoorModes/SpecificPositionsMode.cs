namespace MapGeneration.Core.Doors.DoorModes
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core;
	using Interfaces.Core.Doors;

	public class SpecificPositionsMode : IDoorMode
	{
		public List<OrthogonalLine> DoorPositions { get; private set; }

		public SpecificPositionsMode(List<OrthogonalLine> doorPositions)
		{
			DoorPositions = doorPositions;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private SpecificPositionsMode()
		{

		}
	}
}