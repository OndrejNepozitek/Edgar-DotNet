using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	/// <summary>
	/// Represents a door between two rooms in the level.
	/// </summary>
    public class LayoutDoorGrid2D<TRoom>
	{
		/// <summary>
		/// Room on one side of the door.
		/// </summary>
		public TRoom From { get; }

		/// <summary>
		/// Room on the other side of the door.
		/// </summary>
		public TRoom To { get; }

		/// <summary>
		/// Line containing all the door points.
		/// </summary>
		public OrthogonalLineGrid2D DoorLine { get; }

		public LayoutDoorGrid2D(TRoom from, TRoom to, OrthogonalLineGrid2D doorLine)
		{
            DoorLine = doorLine;
            From = from;
            To = to;
        }
	}
}