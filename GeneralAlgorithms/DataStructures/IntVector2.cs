namespace GeneralAlgorithms.DataStructures
{
	using System;
	using System.Collections.Generic;

	public struct IntVector2
	{
		public readonly int X;

		public readonly int Y;

		public IntVector2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return $"IntVector2 ({X}, {Y})";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is IntVector2))
				return false;

			var vector = (IntVector2)obj;
			return vector.X == X && vector.Y == Y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 17;
				hash = hash * 23 + X.GetHashCode();
				hash = hash * 23 + Y.GetHashCode();
				return hash;
			}
		}

		public List<IntVector2> GetAdjacentTiles()
		{
			var positions = new List<IntVector2>();

			positions.Add(new IntVector2(X + 1, Y));
			positions.Add(new IntVector2(X - 1, Y));
			positions.Add(new IntVector2(X, Y + 1));
			positions.Add(new IntVector2(X, Y - 1));

			return positions;
		}

		public List<IntVector2> GetAdjacentTilesAndDiagonal()
		{
			var positions = GetAdjacentTiles();

			positions.Add(new IntVector2(X + 1, Y + 1));
			positions.Add(new IntVector2(X - 1, Y - 1));
			positions.Add(new IntVector2(X - 1, Y + 1));
			positions.Add(new IntVector2(X + 1, Y - 1));

			return positions;
		}

		public static IntVector2 GetGridDirection(int x, int y)
		{
			if (x != 0)
				y = 0;

			if (y != 0)
				x = 0;

			return new IntVector2(x, y);
		}

		public static int ManhattanDistance(IntVector2 a, IntVector2 b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		public static double EuclideanDistance(IntVector2 a, IntVector2 b)
		{
			return Math.Sqrt((int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
		}

		public static int MaxDistance(IntVector2 a, IntVector2 b)
		{
			return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
		}

		public List<IntVector2> GetRadius(int radius, Func<IntVector2, IntVector2, int> metric, bool includeInside)
		{
			var positions = new List<IntVector2>();

			for (var i = X - radius; i <= X + radius; i++)
			{
				for (var j = Y - radius; j <= Y + radius; j++)
				{
					var pos = new IntVector2(i, j);

					if (includeInside)
					{
						if (metric(this, pos) <= radius)
						{
							positions.Add(pos);
						}
					}
					else
					{
						if (metric(this, pos) == radius)
						{
							positions.Add(pos);
						}
					}
				}
			}

			return positions;
		}

		#region Operators

		public static IntVector2 operator +(IntVector2 a, IntVector2 b)
		{
			return new IntVector2(a.X + b.X, a.Y + b.Y);
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b)
		{
			return new IntVector2(a.X - b.X, a.Y - b.Y);
		}

		public static IntVector2 operator *(int a, IntVector2 b)
		{
			return new IntVector2(a * b.X, a * b.Y);
		}

		public static bool operator ==(IntVector2 a, IntVector2 b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(IntVector2 a, IntVector2 b)
		{

			return !Equals(a, b);
		}
		#endregion
	}
}
