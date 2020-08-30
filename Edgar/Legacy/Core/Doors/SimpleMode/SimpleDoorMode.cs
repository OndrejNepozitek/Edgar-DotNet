using System;
using Edgar.Legacy.Core.Doors.Interfaces;

namespace Edgar.Legacy.Core.Doors.SimpleMode
{
    /// <summary>
	/// Mode that is used to generate doors of a specified length on all sides of the polygon.
	/// The only condition is that doors are at least CornerDistance units far from corners.
	/// </summary>
	public class SimpleDoorMode : IDoorMode
	{
		/// <summary>
		/// Length of doors.
		/// </summary>
        public int DoorLength { get; }

		/// <summary>
		/// How far from the corners must the door be.
		/// </summary>
        public int CornerDistance { get; }

		public SimpleDoorMode(int doorLength, int cornerDistance)
		{
			if (cornerDistance < 0)
				throw new ArgumentException("Minimum overlap must not be less than 0.", nameof(cornerDistance));

			DoorLength = doorLength;
			CornerDistance = cornerDistance;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private SimpleDoorMode()
		{
			/* empty */
		}
	}
}