namespace MapGeneration.Core.Doors.DoorModes
{
	using Interfaces;

	public class OverlapMode : IDoorMode
	{
		public int DoorLength { get; }

		public int MinimumOverlap { get; }

		public OverlapMode(int doorLength, int minimumOverlap)
		{
			DoorLength = doorLength;
			MinimumOverlap = minimumOverlap;
		}
	}
}