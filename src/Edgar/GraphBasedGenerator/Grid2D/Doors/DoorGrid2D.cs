using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a single door of a room template. The door is placed between the From and To points.
    /// </summary>
    public class DoorGrid2D
    {
        /// <summary>
        /// First point of the door.
        /// </summary>
        public Vector2Int From { get; }

        /// <summary>
        /// Last point of the door.
        /// </summary>
        public Vector2Int To { get; }

        /// <summary>
        /// Door socket. Only doors with the same socket (test with .Equals()) can be connected.
        /// </summary>
        public IDoorSocket Socket { get; }

        /// <param name="from">See the <see cref="From"/> property.</param>
        /// <param name="to">See the <see cref="To"/> property.</param>
        /// <param name="socket">See the <see cref="Socket"/> property.</param>
        public DoorGrid2D(Vector2Int from, Vector2Int to, IDoorSocket socket = null)
        {
            From = from;
            To = to;
            Socket = socket;
        }

        /// <summary>
        /// Gets a line from the From point to the To point.
        /// </summary>
        /// <returns></returns>
        public OrthogonalLineGrid2D GetLine()
        {
            return new OrthogonalLineGrid2D(From, To);
        }
    }
}