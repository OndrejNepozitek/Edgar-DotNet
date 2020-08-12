using System;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace Edgar.Legacy.Core.Doors
{
    /// <summary>
    /// Represents a door line.
    /// </summary>
	public struct DoorLine : IEquatable<DoorLine>
	{
        /// <summary>
        /// Set of points where doors can start. 
        /// </summary>
		public OrthogonalLine Line { get; }

        /// <summary>
        /// Length of doors.
        /// </summary>
		public int Length { get; }

		public DoorLine(OrthogonalLine line, int length)
		{
			Line = line;
			Length = length;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (!(obj is DoorLine))
			{
				return false;
			}

			var line = (DoorLine)obj;
			return Equals(line);
		}

		/// <inheritdoc />
		public bool Equals(DoorLine other)
		{
			return Line.Equals(other.Line) && Length == other.Length && Line.GetDirection().Equals(other.Line.GetDirection());
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