using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;

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
        /// <remarks>
        /// This lines is in the local space of the room. To get the world position, the position of the <see cref="FromRoom"/> must be added to the line.
        /// </remarks>
        public OrthogonalLineGrid2D DoorLine { get; }

        /// <summary>
        /// Which socket was used for this door.
        /// </summary>
        public IDoorSocket Socket { get; }

        /// <summary>
        /// Which door type was used for this door.
        /// </summary>
        public DoorType Type { get; }

        /// <param name="fromRoom">See the <see cref="FromRoom"/> property.</param>
        /// <param name="toRoom">See the <see cref="ToRoom"/> property.</param>
        /// <param name="doorLine">See the <see cref="DoorLine"/> property.</param>
        /// <param name="socket">See the <see cref="Socket"/> property.</param>
        /// <param name="type">See the <see cref="Type"/> property.</param>
        public LayoutDoorGrid2D(TRoom fromRoom, TRoom toRoom, OrthogonalLineGrid2D doorLine, IDoorSocket socket,
            DoorType type)
        {
            DoorLine = doorLine;
            Socket = socket;
            Type = type;
            FromRoom = fromRoom;
            ToRoom = toRoom;
        }
    }
}