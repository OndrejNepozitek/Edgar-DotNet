using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace GeneralAlgorithms.Tests.DataStructures.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
    using NUnit.Framework;

	[TestFixture]
	public class OrthogonalLineTests
	{
		[Test]
		public void GetPoints_Top_ReturnsPoints()
		{
			var line = new OrthogonalLineGrid2D(new Vector2Int(2, 2), new Vector2Int(2, 4));
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(2, 2),
				new Vector2Int(2, 3),
				new Vector2Int(2, 4),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetPoints_Bottom_ReturnsPoints()
		{
			var line = new OrthogonalLineGrid2D(new Vector2Int(2, 4), new Vector2Int(2, 2));
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(2, 4),
				new Vector2Int(2, 3),
				new Vector2Int(2, 2),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}


		[Test]
		public void GetPoints_Right_ReturnsPoints()
		{
			var line = new OrthogonalLineGrid2D(new Vector2Int(5, 3), new Vector2Int(8, 3));
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(5, 3),
				new Vector2Int(6, 3),
				new Vector2Int(7, 3),
				new Vector2Int(8, 3),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetPoints_Left_ReturnsPoints()
		{
			var line = new OrthogonalLineGrid2D(new Vector2Int(8, 3), new Vector2Int(5, 3));
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(8, 3),
				new Vector2Int(7, 3),
				new Vector2Int(6, 3),
				new Vector2Int(5, 3),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetDirection_ReturnsDirection()
		{
			var top = new OrthogonalLineGrid2D(new Vector2Int(2, 2), new Vector2Int(2, 4));
			var bottom = new OrthogonalLineGrid2D(new Vector2Int(2, 4), new Vector2Int(2, 2));
			var right = new OrthogonalLineGrid2D(new Vector2Int(5, 3), new Vector2Int(8, 3));
			var left = new OrthogonalLineGrid2D(new Vector2Int(8, 3), new Vector2Int(5, 3));

			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Top, top.GetDirection());
			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Bottom, bottom.GetDirection());
			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Right, right.GetDirection());
			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Left, left.GetDirection());
		}

		[Test]
		public void Shrink_Valid_ReturnsShrinked()
		{
			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(5, 0));
				var expected = new OrthogonalLineGrid2D(new Vector2Int(1, 0), new Vector2Int(3, 0));
				var shrinked = line.Shrink(1, 2);

				Assert.AreEqual(expected, shrinked);
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 6));
				var expected = new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 5));
				var shrinked = line.Shrink(2, 1);

				Assert.AreEqual(expected, shrinked);
			}
		}

		[Test]
		public void Shrink_Invalid_Throws()
		{
			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(5, 0));
				Assert.Throws<ArgumentException>(() => line.Shrink(3));
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(-6, 0));
				Assert.Throws<ArgumentException>(() => line.Shrink(4, 3));
			}
		}

		[Test]
		public void Rotate_ReturnsRotated()
		{
			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(5, 0));
				var expected = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, -5));
				Assert.AreEqual(expected, line.Rotate(90));
				Assert.AreEqual(expected, line.Rotate(-270));
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(-2, -2), new Vector2Int(-2, 5));
				var expected = new OrthogonalLineGrid2D(new Vector2Int(2, 2), new Vector2Int(2, -5));
				Assert.AreEqual(expected, line.Rotate(180));
				Assert.AreEqual(expected, line.Rotate(-180));
			}
		}

		[Test]
		public void Rotate_InvalidDegrees_Throws()
		{
			var line = new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(5, 0));
			Assert.Throws<ArgumentException>(() => line.Rotate(1));
			Assert.Throws<ArgumentException>(() => line.Rotate(15));
			Assert.Throws<ArgumentException>(() => line.Rotate(-181));
		}

		[Test]
		public void RotateDirection_ReturnsRotated()
		{
			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Bottom, OrthogonalLineGrid2D.RotateDirection(OrthogonalLineGrid2D.Direction.Right, 90));
			Assert.AreEqual(OrthogonalLineGrid2D.Direction.Top, OrthogonalLineGrid2D.RotateDirection(OrthogonalLineGrid2D.Direction.Bottom, -180));
		}

		[Test]
		public void Contains_Inside_ReturnsIndex()
		{
			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(4, 2), new Vector2Int(10, 2));
				var point = new Vector2Int(7, 2);

				// TODO: why is it on the polygon?
				foreach (var rotation in PolygonGrid2D.PossibleRotations)
				{
					var rotatedLine = line.Rotate(rotation);
					var rotatedPoint = point.RotateAroundCenter(rotation);

					var actualIndex = rotatedLine.Contains(rotatedPoint);

					Assert.AreEqual(3, actualIndex);
				}
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(4, 2), new Vector2Int(10, 2));
				var point = new Vector2Int(4, 2);

				// TODO: why is it on the polygon?
				foreach (var rotation in PolygonGrid2D.PossibleRotations)
				{
					var rotatedLine = line.Rotate(rotation);
					var rotatedPoint = point.RotateAroundCenter(rotation);

					var actualIndex = rotatedLine.Contains(rotatedPoint);

					Assert.AreEqual(0, actualIndex);
				}
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(4, 2), new Vector2Int(10, 2));
				var point = new Vector2Int(10, 2);

				// TODO: why is it on the polygon?
				foreach (var rotation in PolygonGrid2D.PossibleRotations)
				{
					var rotatedLine = line.Rotate(rotation);
					var rotatedPoint = point.RotateAroundCenter(rotation);

					var actualIndex = rotatedLine.Contains(rotatedPoint);

					Assert.AreEqual(6, actualIndex);
				}
			}
		}

		[Test]
		public void Contains_Outside_ReturnsMinusOne()
		{
			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(4, 2), new Vector2Int(10, 2));
				var point = new Vector2Int(3, 2);

				// TODO: why is it on the polygon?
				foreach (var rotation in PolygonGrid2D.PossibleRotations)
				{
					var rotatedLine = line.Rotate(rotation);
					var rotatedPoint = point.RotateAroundCenter(rotation);

					var actualIndex = rotatedLine.Contains(rotatedPoint);

					Assert.AreEqual(-1, actualIndex);
				}
			}

			{
				var line = new OrthogonalLineGrid2D(new Vector2Int(4, 2), new Vector2Int(10, 2));
				var point = new Vector2Int(12, 2);

				// TODO: why is it on the polygon?
				foreach (var rotation in PolygonGrid2D.PossibleRotations)
				{
					var rotatedLine = line.Rotate(rotation);
					var rotatedPoint = point.RotateAroundCenter(rotation);

					var actualIndex = rotatedLine.Contains(rotatedPoint);

					Assert.AreEqual(-1, actualIndex);
				}
			}
		}
	}
}