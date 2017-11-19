namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections.Generic;

	// TODO: maybe should be named "Orthogonal line"
	public struct IntLine
	{
		public readonly IntVector2 From;
		public readonly IntVector2 To;

		// TODO: must be orthogonal for this to work
		public int Length => IntVector2.ManhattanDistance(new IntVector2(0, 0), From - To);

		public IntLine(IntVector2 from, IntVector2 to)
		{
			// TODO: should be handled differently
			if (from.X != to.X && from.Y != to.Y)
			{
				throw new InvalidOperationException("The line is not orthogonal");
			}

			From = from;
			To = to;
		}

		public Direction GetDirection()
		{
			if (From.X == To.X)
			{
				return From.Y > To.Y ? Direction.Bottom : Direction.Top;
			}
			else
			{
				return From.X > To.X ? Direction.Left : Direction.Right;
			}
		}

		public List<IntVector2> GetPoints()
		{
			var points = new List<IntVector2>();

			switch (GetDirection())
			{
				case Direction.Top:
					for (var i = From.Y; i <= To.Y; i++)
						points.Add(new IntVector2(From.X, i));
					break;
				case Direction.Bottom:
					for (var i = From.Y; i >= To.Y; i--)
						points.Add(new IntVector2(From.X, i));
					break;
				case Direction.Right:
					for (var i = From.X; i <= To.X; i++)
						points.Add(new IntVector2(i, From.Y));
					break;
				case Direction.Left:
					for (var i = From.X; i >= To.X; i--)
						points.Add(new IntVector2(i, From.Y));
					break;
				default:
					throw new InvalidOperationException();
			}

			return points;
		}

		public enum Direction
		{
			Top, Right, Bottom, Left
		}
	}
}
