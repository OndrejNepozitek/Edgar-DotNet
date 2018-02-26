namespace MapGeneration.Core.Doors.DoorModes
{
	using Interfaces.Core;
	using Interfaces.Core.Doors;

	public class OverlapMode : IDoorMode
	{
		public int DoorLength { get; private set; }

		public int MinimumOverlap { get; private set; }

		public OverlapMode(int doorLength, int minimumOverlap)
		{
			DoorLength = doorLength;
			MinimumOverlap = minimumOverlap;
		}

		/// <summary>
		/// For YAML deserialization.
		/// </summary>
		private OverlapMode()
		{

		}
	}
}