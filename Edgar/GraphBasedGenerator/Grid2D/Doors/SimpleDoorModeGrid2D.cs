using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace Edgar.GraphBasedGenerator.Grid2D.Doors
{
    /// <summary>
	/// Mode that is used to generate doors of a specified length on all sides of the polygon.
	/// The only condition is that doors are at least CornerDistance units far from corners.
	/// </summary>
	public class SimpleDoorModeGrid2D : IDoorModeGrid2D
	{
		/// <summary>
		/// Length of doors.
		/// </summary>
        public int DoorLength { get; }

		/// <summary>
		/// How far from the corners must the door be.
		/// </summary>
        public int CornerDistance { get; }

		public IDoorSocket DoorSocket { get; }

		public SimpleDoorModeGrid2D(int doorLength, int cornerDistance, IDoorSocket doorSocket = null)
		{
			if (cornerDistance < 0)
				throw new ArgumentException("Minimum overlap must not be less than 0.", nameof(cornerDistance));

			DoorLength = doorLength;
			CornerDistance = cornerDistance;
            DoorSocket = doorSocket;
        }

        public List<DoorLineGrid2D> GetDoors(PolygonGrid2D roomShape)
        {
            var lines = new List<DoorLineGrid2D>();

            foreach (var line in roomShape.GetLines())
            {
                if (line.Length < 2 * CornerDistance + DoorLength)
                    continue;
				
                lines.Add(new DoorLineGrid2D(line.Shrink(CornerDistance, CornerDistance + DoorLength), DoorLength, DoorSocket));
            }

            return lines;
        }
    }
}