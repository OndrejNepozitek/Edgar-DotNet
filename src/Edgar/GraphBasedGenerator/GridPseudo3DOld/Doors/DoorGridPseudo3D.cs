using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;

namespace Edgar.GraphBasedGenerator.GridPseudo3DOld.Doors
{
    /// <summary>
    /// Represents a single door of a room template. The door is placed between the From and To points.
    /// </summary>
    public class DoorGridPseudo3D
    {
        /// <summary>
        /// First point of the door.
        /// </summary>
        public Vector3Int From { get; }

        /// <summary>
        /// Last point of the door.
        /// </summary>
        public Vector3Int To { get; }

        /// <summary>
        /// Door socket. Only doors with the same socket (test with .Equals()) can be connected.
        /// </summary>
        public IDoorSocket Socket { get; }

        /// <param name="from">See the <see cref="From"/> property.</param>
        /// <param name="to">See the <see cref="To"/> property.</param>
        /// <param name="socket">See the <see cref="Socket"/> property.</param>
        public DoorGridPseudo3D(Vector3Int from, Vector3Int to, IDoorSocket socket = null)
        {
            From = from;
            To = to;
            Socket = socket;
        }
    }
}