using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;

namespace Edgar.GraphBasedGenerator.GridPseudo3D.Doors
{
    /// <summary>
	/// Mode that is used to generate doors of a specified length on all sides of the polygon.
	/// The only condition is that doors are at least CornerDistance tiles far from corners.
	/// </summary>
	public class SimpleDoorModeGrid3D : IDoorModeGridPseudo3D
	{
		/// <summary>
		/// Length of doors.
		/// </summary>
        public int DoorLength { get; }

		/// <summary>
		/// How far from the corners must the door be.
		/// </summary>
        public int CornerDistance { get; }

        public int Z { get; }

        /// <summary>
        /// Door socket. Only doors with the same socket (test with .Equals()) can be connected.
        /// </summary>
		public IDoorSocket DoorSocket { get; }

        /// <param name="doorLength">See the <seealso cref="DoorLength"/> property.</param>
        /// <param name="cornerDistance">See the <seealso cref="CornerDistance"/> property.</param>
        /// <param name="doorSocket">See the <seealso cref="DoorSocket"/> property.</param>
        public SimpleDoorModeGrid3D(int doorLength, int cornerDistance, int z, IDoorSocket doorSocket = null)
		{
			if (cornerDistance < 0)
				throw new ArgumentException("Minimum overlap must not be less than 0.", nameof(cornerDistance));

			DoorLength = doorLength;
			CornerDistance = cornerDistance;
            Z = z;
            DoorSocket = doorSocket;
        }

        /// <inheritdoc />
        public List<DoorLineGridPseudo3D> GetDoors(PolygonGrid2D roomShape)
        {
            var lines = new List<DoorLineGridPseudo3D>();

            foreach (var line in roomShape.GetLines())
            {
                if (line.Length < 2 * CornerDistance + DoorLength)
                    continue;
				
                lines.Add(new DoorLineGridPseudo3D(new OrthogonalLineGrid3D(line.Shrink(CornerDistance, CornerDistance + DoorLength), Z), DoorLength, DoorSocket));
            }

            return lines;
        }
    }
}