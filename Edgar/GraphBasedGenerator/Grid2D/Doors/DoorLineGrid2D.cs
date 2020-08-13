using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.GraphBasedGenerator.Grid2D.Doors
{
    /// <summary>
    /// Represents a door line.
    /// </summary>
	public struct DoorLineGrid2D : IEquatable<DoorLineGrid2D>
	{
        /// <summary>
        /// Set of points where doors can start. 
        /// </summary>
		public OrthogonalLine Line { get; }

        /// <summary>
        /// Length of doors.
        /// </summary>
		public int Length { get; }

        public IDoorSocket DoorSocket { get; }

        public DoorLineGrid2D(OrthogonalLine line, int length, IDoorSocket doorSocket)
		{
			Line = line;
			Length = length;
            DoorSocket = doorSocket;
        }

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (!(obj is DoorLineGrid2D))
			{
				return false;
			}

			var line = (DoorLineGrid2D)obj;
			return Equals(line);
		}

		/// <inheritdoc />
		public bool Equals(DoorLineGrid2D other)
		{
			return Line.Equals(other.Line) && Length == other.Length && Line.GetDirection().Equals(other.Line.GetDirection()) && EqualityComparer<IDoorSocket>.Default.Equals(DoorSocket, other.DoorSocket);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				return (Line.GetHashCode() * 397) ^ Length;
			}
		}
	}
}