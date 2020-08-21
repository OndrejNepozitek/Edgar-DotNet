using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.Interfaces;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Doors.ManualMode
{
    /// <summary>
	/// Mode that holds all the door positions.
	/// </summary>
	public class ManualDoorMode : IDoorMode
	{
		public List<OrthogonalLineGrid2D> DoorPositions { get; }

		public ManualDoorMode(List<OrthogonalLineGrid2D> doorPositions)
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