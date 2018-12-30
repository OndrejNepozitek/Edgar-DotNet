namespace GeneralAlgorithms.DataStructures.Common
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using Algorithms.Common;

	/// <summary>
	/// Structure representing an orthogonal line in a integer grid.
	/// </summary>
	public struct OrthogonalLine : IEquatable<OrthogonalLine>
	{
		public readonly IntVector2 From;
		public readonly IntVector2 To;
		private readonly Direction degeneratedDirection;

		private static readonly List<Direction> OrderedDirections = new List<Direction>() { Direction.Right, Direction.Bottom, Direction.Left, Direction.Top };

		/// <summary>
		/// Returns number of points minus 1.
		/// </summary>
		public int Length => IntVector2.ManhattanDistance(new IntVector2(0, 0), From - To);

		/// <summary>
		/// Construct an orthogonal line from given endpoints.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <exception cref="ArgumentException">Thrown when given points do not form an orthogonal line.</exception>
		public OrthogonalLine(IntVector2 from, IntVector2 to)
		{
			if (from.X != to.X && from.Y != to.Y)
			{
				throw new ArgumentException("The line is not orthogonal");
			}

			From = from;
			To = to;
			degeneratedDirection = Direction.Undefined;
		}

		/// <summary>
		/// Construct an orthogonal line from given endpoints and a direction.
		/// </summary>
		/// <remarks>
		/// Direction is used only when the line is degenerated - that means
		/// when both endpoints are in fact the same point. It is useful in situations
		/// where line's direction is an important information.
		/// </remarks>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="direction"></param>
		/// <exception cref="ArgumentException">Thrown when given points do not form an orthogonal line.</exception>
		public OrthogonalLine(IntVector2 from, IntVector2 to, Direction direction)
		{
			if (from.X != to.X && from.Y != to.Y)
			{
				throw new ArgumentException("The line is not orthogonal");
			}

			if (from != to && direction != GetDirection(from, to))
			{
				throw new InvalidOperationException("Given direction is wrong");
			}

			From = from;
			To = to;
			degeneratedDirection = direction;
		}

		/// <summary>
		/// Returns a direction of the line.
		/// </summary>
		/// <remarks>
		/// If the line is degenerated, returns the direction that was set
		/// in constructor (or Undefined if none was set).
		/// </remarks>
		/// <returns></returns>
		[Pure]
		public Direction GetDirection()
		{
			if (From == To)
			{
				return degeneratedDirection;
			}

			return GetDirection(From, To);
		}

		/// <summary>
		/// Gets a direction of an orthogonal lined formed by given points.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <exception cref="ArgumentException">Thrown when given points do not form an orthogonal line</exception>
		/// <returns></returns>
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

			if (from.Y == to.Y)
			{
				return from.X > to.X ? Direction.Left : Direction.Right;
			}

			throw new ArgumentException("Given points do not form an orthogonal line");
		}

		/// <summary>
		/// Rotate the line.
		/// </summary>
		/// <remarks>
		/// Positive degrees mean clockwise rotation.
		/// </remarks>
		/// <param name="degrees"></param>
		/// <param name="checkDirection">Throws if set to true and the rotated line has an undefined direction.</param>
		/// <exception cref="ArgumentException">Thrown when degress are not a multiple of 90.</exception>
		/// <returns></returns>
		[Pure]
		public OrthogonalLine Rotate(int degrees, bool checkDirection = true)
		{
			if (degrees % 90 != 0)
				throw new ArgumentException("Degress must be a multiple of 90.", nameof(degrees));

			if (checkDirection)
			{
				return new OrthogonalLine(From.RotateAroundCenter(degrees), To.RotateAroundCenter(degrees), RotateDirection(GetDirection(), degrees));
			}

			return new OrthogonalLine(From.RotateAroundCenter(degrees), To.RotateAroundCenter(degrees));
		}

		/// <summary>
		/// Returns a line where From and To are switched.
		/// </summary>
		/// <returns></returns>
		public OrthogonalLine SwitchOrientation()
		{
			return new OrthogonalLine(To, From, GetOppositeDirection(GetDirection()));
		}

		/// <summary>
		/// Shrinks the line.
		/// </summary>
		/// <remarks>
		/// Specified number of points is removed from respective sides of the line.
		/// </remarks>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public OrthogonalLine Shrink(int from, int to)
		{
			if (Length - from - to < 0)
				throw new ArgumentException("There must be at least one point left after shrinking.");

			var rotation = ComputeRotation();
			var rotated = Rotate(rotation);

			var movedFrom = new IntVector2(rotated.From.X + from, rotated.From.Y);
			var movedTo = new IntVector2(rotated.To.X - to, rotated.To.Y);

			return new OrthogonalLine(movedFrom, movedTo, RotateDirection(GetDirection(), rotation)).Rotate(-rotation);
		}

		/// <summary>
		/// Shrinks the line by the same amount on both sides.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public OrthogonalLine Shrink(int length)
		{
			return Shrink(length, length);
		}

		/// <summary>
		/// Computes how the line must be rotated around the center to have "Right" direction.
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
		/// Gets opposite direction.
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

				case Direction.Undefined:
					return Direction.Undefined;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets all points of the line. Both "From" and "To" are inclusive.
		/// The direction is from "From" to "To";
		/// </summary>
		/// <returns></returns>
		public List<IntVector2> GetPoints()
		{
			var points = new List<IntVector2>();

			switch (GetDirection())
			{
				case var _ when(From == To):
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
			var index = OrderedDirections.FindIndex(x => x == direction);
			var newIndex = (index + shift).Mod(4);

			return OrderedDirections[newIndex];
		}

		/// <summary>
		/// Returns a line that has the same endpoints and From is smaller than To.
		/// </summary>
		/// <returns></returns>
		public OrthogonalLine GetNormalized()
		{
			return From < To ? new OrthogonalLine(From, To) : new OrthogonalLine(To, From);
		}

		/// <summary>
		/// Gets nth point on the line. (Counted from From)
		/// </summary>
		/// <param name="n"></param>
		/// 
		/// <returns></returns>
		[Pure]
		public IntVector2 GetNthPoint(int n)
		{
			if (n > Length)
				throw new ArgumentException("n is greater than the length of the line.", nameof(n));

			var direction = GetDirection();

			switch (direction)
			{
				case Direction.Top:
					return From + new IntVector2(0, n);
				case Direction.Right:
					return From + new IntVector2(n, 0);
				case Direction.Bottom:
					return From - new IntVector2(0, n);
				case Direction.Left:
					return From - new IntVector2(n, 0);
				case Direction.Undefined:
				{
					if (n > 0)
						throw new ArgumentException();

					return From;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Checks if the orthogonal line contains a given point.
		/// </summary>
		/// <remarks>
		/// Index is 0 for From and Count + 1 for To.
		/// </remarks>
		/// <param name="point"></param>
		/// <returns>Index of a given point on the line or -1.</returns>
		[Pure]
		public int Contains(IntVector2 point)
		{
			var direction = GetDirection();

			switch (direction)
			{
				case Direction.Top:
				{
					if (point.X == From.X && point.Y <= To.Y && point.Y >= From.Y)
					{
						return point.Y - From.Y;
					}
					break;
				}
				case Direction.Right:
				{
					if (point.Y == From.Y && point.X <= To.X && point.X >= From.X)
					{
						return point.X - From.X;
					}
					break;
				}
				case Direction.Bottom:
				{
					if (point.X == From.X && point.Y >= To.Y && point.Y <= From.Y)
					{
						return From.Y - point.Y;
					}
					break;
				}
				case Direction.Left:
				{
					if (point.Y == From.Y && point.X >= To.X && point.X <= From.X)
					{
						return From.X - point.X;
					}
					break;
				}
				case Direction.Undefined:
				{
					if (point == From)
					{
						return 0;
					}

					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}

			return -1;
		}

		/// <summary>
		/// Gets a direction vector of the line. 
		/// </summary>
		/// <remarks>
		/// That is a vector that satisfies that From + Length * direction_vector = To.
		/// </remarks>
		/// <returns></returns>
		[Pure]
		public IntVector2 GetDirectionVector()
		{
			switch (GetDirection())
			{
				case Direction.Top:
					return new IntVector2(0, 1);
				case Direction.Right:
					return new IntVector2(1, 0);
				case Direction.Bottom:
					return new IntVector2(0, -1);
				case Direction.Left:
					return new IntVector2(-1, 0);
				case Direction.Undefined:
					throw new InvalidOperationException("Degenerated lines without a direction set do not have a direction vector.");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#region Operators

		/// <summary>
		/// Adds given IntVector2 to both endpoints of a given orthogonal line.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static OrthogonalLine operator +(OrthogonalLine line, IntVector2 point)
		{
			return new OrthogonalLine(line.From + point, line.To + point, line.GetDirection());
		}

		/// <summary>
		/// Adds given IntVector2 to both endpoints of a given orthogonal line.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static OrthogonalLine operator +(IntVector2 point, OrthogonalLine line)
		{
			return line + point;
		}

		#endregion

		/// <inheritdoc />
		public bool Equals(OrthogonalLine other)
		{
			return From.Equals(other.From) && To.Equals(other.To);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			return obj is OrthogonalLine line && Equals(line);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				return (From.GetHashCode() * 397) ^ To.GetHashCode();
			}
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"IntLine: {From.ToStringShort()} -> {To.ToStringShort()} ({GetDirection()})";
		}

		/// <summary>
		/// Enum that holds a direction of an orthogonal line.
		/// </summary>
		public enum Direction
		{
			Top, Right, Bottom, Left, Undefined
		}
	}
}
