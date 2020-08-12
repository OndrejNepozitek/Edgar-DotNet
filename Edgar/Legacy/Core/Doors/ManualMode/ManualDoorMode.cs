using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Doors.ManualMode
{
    /// <summary>
	/// Mode that holds all the door positions.
	/// </summary>
	public class ManualDoorMode : IDoorMode
	{
		public List<OrthogonalLine> DoorPositions { get; }

		public ManualDoorMode(List<OrthogonalLine> doorPositions)
		{
			if (doorPositions.Distinct().Count() != doorPositions.Count)
				throw new ArgumentException("All door positions must be unique");

			DoorPositions = doorPositions;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private ManualDoorMode()
		{
			/* empty */
		}
	}
}