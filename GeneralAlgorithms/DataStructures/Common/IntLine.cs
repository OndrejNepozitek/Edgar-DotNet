namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Polygons;

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

		public IntLine Rotate(int degrees)
		{
			// TODO: check if degrees mod 90 == 0
			/*if (!GridPolygon.PossibleRotations.Contains(degrees))
				throw new InvalidOperationException();*/

			// TODO: resharper was unhappy about unpure functions
			var from = new IntVector2(From.X, From.Y);
			var to = new IntVector2(To.X, To.Y);

			return new IntLine(from.RotateAroundCenter(degrees), to.RotateAroundCenter(degrees));
		}

		/// <summary>
		/// Switch From and To.
		/// </summary>
		/// <returns></returns>
		public IntLine SwitchOrientation()
		{
			return new IntLine(To, From);
		}

		/// <summary>
		/// Shrink the line.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public IntLine Shrink(int from, int to)
		{
			if (Length - from - to <= 0)
				throw new InvalidOperationException();

			var rotation = ComputeRotation();
			var rotated = Rotate(rotation);

			var movedFrom = new IntVector2(rotated.From.X + from, rotated.From.Y);
			var movedTo = new IntVector2(rotated.To.X - to, rotated.To.Y);

			return new IntLine(movedFrom, movedTo).Rotate(-rotation);
		}

		/// <summary>
		/// Shrink the line by the same amount on both sides.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public IntLine Shrink(int length)
		{
			return Shrink(length, length);
		}

		/// <summary>
		/// Compute how the line must be rotated around the center to have "Right" direction.
		/// </summary>
		/// <returns></returns>
		public int ComputeRotation()
		{
			switch (GetDirection())
			{
				case Direction.Right:
					return 0;

				case Direction.Bottom:
					return 90;

				case Direction.Left:
					return 180;

				case Direction.Top:
					return 270;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Get opposite direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static Direction GetOppositeDirection(Direction direction)
		{
			switch (direction)
			{
				case Direction.Bottom:
					return Direction.Top;

				case Direction.Top:
					return Direction.Bottom;

				case Direction.Right:
					return Direction.Left;

				case Direction.Left:
					return Direction.Right;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Get all points of the line. Both "From" and "To" are inclusive.
		/// </summary>
		/// <returns></returns>
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
