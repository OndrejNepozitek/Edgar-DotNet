using Edgar.Geometry;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	/// <summary>
	/// Represents a door between two rooms in the final layout.
	/// </summary>
    public class LayoutDoorGrid2D<TRoom>
	{
		/// <summary>
		/// Room on one side of the door.
		/// </summary>
		public TRoom FromRoom { get; }

		/// <summary>
		/// Room on the other side of the door.
		/// </summary>
		public TRoom ToRoom { get; }

		/// <summary>
		/// Line containing all the door points.
		/// </summary>
		public OrthogonalLineGrid2D DoorLine { get; }

		/// <param name="fromRoom">See the <see cref="FromRoom"/> property.</param>
		/// <param name="toRoom">See the <see cref="ToRoom"/> property.</param>
		/// <param name="doorLine">See the <see cref="DoorLine"/> property.</param>
		public LayoutDoorGrid2D(TRoom fromRoom, TRoom toRoom, OrthogonalLineGrid2D doorLine)
		{
            DoorLine = doorLine;
            FromRoom = fromRoom;
            ToRoom = toRoom;
        }
	}
}