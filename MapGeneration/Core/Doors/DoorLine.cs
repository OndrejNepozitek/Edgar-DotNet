namespace MapGeneration.Core.Doors
{
	using System;
	using GeneralAlgorithms.DataStructures.Common;
	using Interfaces.Core.Doors;

	/// <inheritdoc cref="IDoorLine" />
	public struct DoorLine : IDoorLine, IEquatable<DoorLine>
	{
		/// <inheritdoc />
		public OrthogonalLine Line { get; }

		/// <inheritdoc />
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