namespace MapGeneration.Core.Doors.DoorModes
{
	using System;
	using Interfaces.Core.Doors;

	/// <summary>
	/// Mode that is used to generate doors of a specified length on all sides of the polygon.
	/// The only condition is that doors are at least CornerDistance far from corners.
	/// </summary>
	public class OverlapMode : IDoorMode
	{
		/// <summary>
		/// Length of doors.
		/// </summary>
		public int DoorLength { get; private set; }

		/// <summary>
		/// How far from the corners must the door be.
		/// </summary>
		public int CornerDistance { get; private set; }

		public OverlapMode(int doorLength, int cornerDistance)
		{
			if (cornerDistance < 0)
				throw new ArgumentException("Minimum overlap must not be less than 0.", nameof(cornerDistance));

			DoorLength = doorLength;
			CornerDistance = cornerDistance;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private OverlapMode()
		{
			/* empty */
		}
	}
}