namespace MapGeneration.Core.Doors.DoorModes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core.Doors;

	/// <summary>
	/// Mode that holds all the door positions.
	/// </summary>
	public class SpecificPositionsMode : IDoorMode
	{
		public List<OrthogonalLine> DoorPositions { get; private set; }

		public SpecificPositionsMode(List<OrthogonalLine> doorPositions)
		{
			if (doorPositions.Distinct().Count() != doorPositions.Count)
				throw new ArgumentException("All door positions must be unique");

			DoorPositions = doorPositions;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private SpecificPositionsMode()
		{
			/* empty */
		}
	}
}