namespace MapGeneration.Core.Doors
{
	using System;
	using GeneralAlgorithms.DataStructures.Common;

	public struct DoorLine : IEquatable<DoorLine>
	{
		public readonly OrthogonalLine Line;
		public readonly int Length;

		public DoorLine(OrthogonalLine line, int length)
		{
			Line = line;
			Length = length;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DoorLine))
			{
				return false;
			}

			var line = (DoorLine)obj;
			return Equals(line);
		}

		public bool Equals(DoorLine other)
		{
			return Line.Equals(other.Line) && Length == other.Length;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Line.GetHashCode() * 397) ^ Length;
			}
		}
	}
}