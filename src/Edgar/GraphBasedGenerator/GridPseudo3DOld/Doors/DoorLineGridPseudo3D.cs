using System;
using System.Collections.Generic;
using Edgar.GraphBasedGenerator.Common.Doors;

namespace Edgar.GraphBasedGenerator.GridPseudo3DOld.Doors
{
    /// <summary>
    /// Represents a door line.
    /// </summary>
    /// <remarks>
    /// The difference between this class and the <see cref="DoorGridPseudo3D"/> class is that a single instance of this class
    /// can represent multiple instances of the <see cref="DoorGridPseudo3D"/> class.
    /// </remarks>
	public struct DoorLineGridPseudo3D : IEquatable<DoorLineGridPseudo3D>
	{
        /// <summary>
        /// Set of points where doors can start. 
        /// </summary>
		public OrthogonalLineGrid3D Line { get; }

        /// <summary>
        /// Length of doors.
        /// </summary>
		public int Length { get; }

		/// <summary>
		/// Door socket.
		/// </summary>
        public IDoorSocket DoorSocket { get; }

		/// <param name="line">See the <see cref="Line"/> property.</param>
		/// <param name="length">See the <see cref="Length"/> property.</param>
		/// <param name="doorSocket">See the <see cref="DoorSocket"/> property.</param>
        public DoorLineGridPseudo3D(OrthogonalLineGrid3D line, int length, IDoorSocket doorSocket)
		{
			Line = line;
			Length = length;
            DoorSocket = doorSocket;
        }

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (!(obj is DoorLineGridPseudo3D))
			{
				return false;
			}

			var line = (DoorLineGridPseudo3D)obj;
			return Equals(line);
		}

		/// <inheritdoc />
		public bool Equals(DoorLineGridPseudo3D other)
		{
			return Line.Equals(other.Line) && Length == other.Length && Line.Line.GetDirection().Equals(other.Line.Line.GetDirection()) && EqualityComparer<IDoorSocket>.Default.Equals(DoorSocket, other.DoorSocket);
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