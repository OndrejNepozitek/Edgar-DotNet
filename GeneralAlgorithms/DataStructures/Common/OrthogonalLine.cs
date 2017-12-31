namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections.Generic;
	using Algorithms.Common;

	public struct OrthogonalLine : IEquatable<OrthogonalLine>
	{
		public readonly IntVector2 From;
		public readonly IntVector2 To;
		private readonly Direction degeneratedDirection;

		private static readonly List<Direction> orderedDirections = new List<Direction>() { Direction.Right, Direction.Bottom, Direction.Left, Direction.Top };

		// TODO: must be orthogonal for this to work
		public int Length => IntVector2.ManhattanDistance(new IntVector2(0, 0), From - To);

		public OrthogonalLine(IntVector2 from, IntVector2 to)
		{
			// TODO: is it ok to throw in constructor?
			if (from.X != to.X && from.Y != to.Y)
			{
				throw new InvalidOperationException("The line is not orthogonal");
			}

			From = from;
			To = to;
			degeneratedDirection = Direction.Undefined;
		}

		public OrthogonalLine(IntVector2 from, IntVector2 to, Direction direction)
		{
			// TODO: should be handled differently
			if (from.X != to.X && from.Y != to.Y)
			{
				throw new InvalidOperationException("The line is not orthogonal");
			}

			if (from != to && direction != GetDirection(from, to))
			{
				throw new InvalidOperationException("Given direction is wrong");
			}

			From = from;
			To = to;
			degeneratedDirection = direction;
		}

		public Direction GetDirection()
		{
			if (From == To)
			{
				return degeneratedDirection;
			}

			return GetDirection(From, To);
		}

		public static Direction GetDirection(IntVector2 from, IntVector2 to)
		{
			if (from == to)
			{
				return Direction.Undefined;
			}

			if (from.X == to.X)
			{
				return from.Y > to.Y ? Direction.Bottom : Direction.Top;
			}

			return from.X > to.X ? Direction.Left : Direction.Right;
		}

		/// <summary>
		/// Rotate the line.
		/// </summary>
		/// <remarks>
		/// Positive degrees mean clockwise rotation.
		/// </remarks>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public OrthogonalLine Rotate(int degrees)
		{
			if (degrees % 90 != 0)
				throw new InvalidOperationException();

			// TODO: resharper was unhappy about unpure functions
			var from = new IntVector2(From.X, From.Y);
			var to = new IntVector2(To.X, To.Y);

			return new OrthogonalLine(from.RotateAroundCenter(degrees), to.RotateAroundCenter(degrees), RotateDirection(GetDirection(), degrees));
		}

		/// <summary>
		/// Switch From and To.
		/// </summary>
		/// <returns></returns>
		public OrthogonalLine SwitchOrientation()
		{
			return new OrthogonalLine(To, From, GetOppositeDirection(GetDirection()));
		}

		/// <summary>
		/// Shrink the line.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public OrthogonalLine Shrink(int from, int to)
		{
			if (Length - from - to < 0)
				throw new InvalidOperationException();

			var rotation = ComputeRotation();
			var rotated = Rotate(rotation);

			var movedFrom = new IntVector2(rotated.From.X + from, rotated.From.Y);
			var movedTo = new IntVector2(rotated.To.X - to, rotated.To.Y);

			return new OrthogonalLine(movedFrom, movedTo, RotateDirection(GetDirection(), rotation)).Rotate(-rotation);
		}

		/// <summary>
		/// Shrink the line by the same amount on both sides.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public OrthogonalLine Shrink(int length)
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
					return 270;

				case Direction.Left:
					return 180;

				case Direction.Top:
					return 90;

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
				case var d when(From == To):
					points.Add(From);
					break;

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
					throw new ArgumentOutOfRangeException();
			}

			return points;
		}

		public enum Direction
		{
			Undefined, Top, Right, Bottom, Left
		}

		public bool Equals(OrthogonalLine other)
		{
			return From.Equals(other.From) && To.Equals(other.To);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			return obj is OrthogonalLine line && Equals(line);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (From.GetHashCode() * 397) ^ To.GetHashCode();
			}
		}

		public override string ToString()
		{
			return $"IntLine: {From.ToStringShort()} -> {To.ToStringShort()} ({GetDirection()})";
		}

		/// <summary>
		/// Compute rotated direction.
		/// </summary>
		/// <remarks>
		/// Rotation is clockwise.
		/// </remarks>
		/// <param name="direction"></param>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static Direction RotateDirection(Direction direction, int degrees)
		{
			if (direction == Direction.Undefined)
				throw new InvalidOperationException("Direction must be specified if it should be rotated.");

			degrees = degrees.Mod(360);

			if (degrees % 90 != 0)
				throw new InvalidOperationException();

			var shift = degrees / 90;
			var index = orderedDirections.FindIndex(x => x == direction);
			var newIndex = (index + shift).Mod(4);

			return orderedDirections[newIndex];
		}

		/// <summary>
		/// Returns a line that has the same endpoints and From is smaller than TO.
		/// </summary>
		/// <returns></returns>
		public OrthogonalLine GetNormalized()
		{
			return From < To ? new OrthogonalLine(From, To) : new OrthogonalLine(To, From);
		}
	}
}
