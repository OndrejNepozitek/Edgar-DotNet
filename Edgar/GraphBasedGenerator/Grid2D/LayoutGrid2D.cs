using System.Collections.Generic;

namespace Edgar.GraphBasedGenerator.Grid2D
{
	/// <summary>
	/// Represents a final layout. Each room has a shape and a position assigned.
	/// </summary>
    public class LayoutGrid2D<TRoom>
	{
		/// <summary>
		/// List of rooms in the level.
		/// </summary>
		public List<LayoutRoomGrid2D<TRoom>> Rooms { get; }

		public LayoutGrid2D(List<LayoutRoomGrid2D<TRoom>> rooms)
		{
			Rooms = rooms;
		}
	}
}