using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;

namespace Edgar.GraphBasedGenerator.Grid2D
{
    /// <summary>
    /// Represents a door line.
    /// </summary>
    /// <remarks>
    /// The difference between this class and the <see cref="DoorGrid2D"/> class is that a single instance of this class
    /// can represent multiple instances of the <see cref="DoorGrid2D"/> class.
    /// </remarks>
    public struct DoorLineGrid2D : IEquatable<DoorLineGrid2D>
    {
        /// <summary>
        /// Set of points where doors can start. 
        /// </summary>
        public OrthogonalLineGrid2D Line { get; }

        /// <summary>
        /// Length of doors.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Door socket.
        /// </summary>
        public IDoorSocket DoorSocket { get; }

        public DoorType Type { get; }

        /// <param name="line">See the <see cref="Line"/> property.</param>
        /// <param name="length">See the <see cref="Length"/> property.</param>
        /// <param name="doorSocket">See the <see cref="DoorSocket"/> property.</param>
        public DoorLineGrid2D(OrthogonalLineGrid2D line, int length, IDoorSocket doorSocket, DoorType type)
        {
            Line = line;
            Length = length;
            DoorSocket = doorSocket;
            Type = type;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is DoorLineGrid2D))
            {
                return false;
            }

            var line = (DoorLineGrid2D) obj;
            return Equals(line);
        }

        /// <inheritdoc />
        public bool Equals(DoorLineGrid2D other)
        {
            return Line.Equals(other.Line) && Length == other.Length &&
                   Line.GetDirection().Equals(other.Line.GetDirection()) &&
                   EqualityComparer<IDoorSocket>.Default.Equals(DoorSocket, other.DoorSocket) &&
                   Type == other.Type;
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