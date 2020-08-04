namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using NUnit.Framework;

	[TestFixture]
	internal class GridPolygonOverlapTests
	{
		private PolygonOverlap polygonOverlap;

		[SetUp]
		public void SetUp()
		{
			polygonOverlap = new PolygonOverlap();
		}

		[Test]
		public void DoOverlap_IdenticalRectangles_ReturnsTrue()
		{
			{
				// Identical squares
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(2, 2));
				Assert.AreEqual(true, polygonOverlap.DoOverlap(r1, r1));
			}

			{
				// Identical rectangles
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(4, 2));
				Assert.AreEqual(true, polygonOverlap.DoOverlap(r1, r1));
			}
		}

		[Test]
		public void DoOverlap_OverlappingRectangles_ReturnsTrue()
		{
			{
				// Overlapping squares
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(2, 2));
				var r2 = r1 + new IntVector2(1, 0);
				Assert.AreEqual(true, polygonOverlap.DoOverlap(r1, r2));
			}

			{
				// Overlapping rectangles
				var r1 = new GridRectangle(new IntVector2(4, 5), new IntVector2(10, 7));
				var r2 = new GridRectangle(new IntVector2(5, 6), new IntVector2(12, 10));
				Assert.AreEqual(true, polygonOverlap.DoOverlap(r1, r2));
			}

			{
				// A rectangle inside another rectangle
				var r1 = new GridRectangle(new IntVector2(1, 1), new IntVector2(5, 6));
				var r2 = new GridRectangle(new IntVector2(2, 2), new IntVector2(4, 5));
				Assert.AreEqual(true, polygonOverlap.DoOverlap(r1, r2));
			}
		}

		[Test]
		public void DoOverlap_NonOverlappingRectangles_ReturnsFalse()
		{
			{
				// Non-overlapping squares
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(2, 2));
				var r2 = r1 + new IntVector2(6, 0);
				Assert.AreEqual(false, polygonOverlap.DoOverlap(r1, r2));
			}

			{
				// Non-overlapping rectangles
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(6, 3));
				var r2 = new GridRectangle(new IntVector2(7, 4), new IntVector2(10, 6));
				Assert.AreEqual(false, polygonOverlap.DoOverlap(r1, r2));
			}
		}

		[Test]
		public void DoOverlap_TouchingRectangles_ReturnsFalse()
		{
			{
				// Side-touching squares
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(2, 2));
				var r2 = r1 + new IntVector2(0, 2);
				Assert.AreEqual(false, polygonOverlap.DoOverlap(r1, r2));
			}

			{
				// Vertex-touching rectangles
				var r1 = new GridRectangle(new IntVector2(0, 0), new IntVector2(6, 3));
				var r2 = new GridRectangle(new IntVector2(6, 3), new IntVector2(10, 6));
				Assert.AreEqual(false, polygonOverlap.DoOverlap(r1, r2));
			}
		}

		[Test]
		public void DoOverlap_OverlappingPolygons_ReturnsTrue()
		{
			{
				var p1 = GetLShape();
				var p2 = GridPolygon.GetSquare(3);

				Assert.IsTrue(polygonOverlap.DoOverlap(p1, new IntVector2(0, 0), p2, new IntVector2(3, 0)));
			}

			{
				var p1 = GetPlusShape();
				var p2 = GridPolygon.GetRectangle(2, 3);

				Assert.IsTrue(polygonOverlap.DoOverlap(p1, new IntVector2(0, 0), p2, new IntVector2(3, 4)));
			}
		}

		[Test]
		public void DoOverlap_NonOverlappingPolygons_ReturnsFalse()
		{
			{
				var p1 = GetLShape().Rotate(90);
				var p2 = GridPolygon.GetSquare(3);

				Assert.IsFalse(polygonOverlap.DoOverlap(p1, new IntVector2(0, 0), p2, new IntVector2(0, 0)));
			}

			{
				var p1 = GetPlusShape();
				var p2 = GridPolygon.GetRectangle(2, 3);

				Assert.IsFalse(polygonOverlap.DoOverlap(p1, new IntVector2(0, 0), p2, new IntVector2(4, 4)));
			}
		}

		[Test]
		public void OverlapArea_NonTouching_ReturnsZero()
		{
			var r1 = GridPolygon.GetSquare(6);
			var r2 = GridPolygon.GetRectangle(2, 8);

			Assert.AreEqual(0, polygonOverlap.OverlapArea(r1, new IntVector2(0,0), r2, new IntVector2(7, 2)));
		}

		[Test]
		public void OverlapArea_TwoSquares()
		{
			var r1 = GridPolygon.GetSquare(6);
			var r2 = GridPolygon.GetSquare(3);

			Assert.AreEqual(6, polygonOverlap.OverlapArea(r1, new IntVector2(0, 0), r2, new IntVector2(2, -1)));
		}

		[Test]
		public void OverlapArea_TwoRectangles()
		{
			var r1 = GridPolygon.GetRectangle(4, 6);
			var r2 = GridPolygon.GetRectangle(5, 3);

			Assert.AreEqual(9, polygonOverlap.OverlapArea(r1, new IntVector2(0, 0), r2, new IntVector2(1, 2)));
		}

		[Test]
		public void OverlapArea_PlusShapeAndSquare()
		{
			var p1 = GetPlusShape();
			var p2 = GridPolygon.GetSquare(3);

			foreach (var degrees in GridPolygon.PossibleRotations)
			{
				Assert.AreEqual(5, polygonOverlap.OverlapArea(p1.Rotate(degrees), new IntVector2(0, 0), p2.Rotate(degrees), new IntVector2(3, 3).RotateAroundCenter(degrees)));
			}
		}

		[Test]
		public void DoTouch_TwoSquares()
		{
			var r1 = GridPolygon.GetSquare(6);
			var r2 = GridPolygon.GetSquare(3);

			Assert.AreEqual(true, polygonOverlap.DoTouch(r1, new IntVector2(0, 0), r2, new IntVector2(6, 0)));
			Assert.AreEqual(true, polygonOverlap.DoTouch(r1, new IntVector2(0, 0), r2, new IntVector2(6, -3)));
			Assert.AreEqual(true, polygonOverlap.DoTouch(r1, new IntVector2(0, 0), r2, new IntVector2(6, -2)));
		}

		[Test]
		public void DoTouch_TwoPolygons()
		{
			var p1 = GetLShape();
			var p2 = GridPolygon.GetSquare(3);

			foreach (var degrees in GridPolygon.PossibleRotations)
			{
				var pr1 = p1.Rotate(degrees);
				var pr2 = p2.Rotate(degrees);

				var pos1 = new IntVector2(0, 0).RotateAroundCenter(degrees);
				var pos2 = new IntVector2(4, 3).RotateAroundCenter(degrees);

				Assert.AreEqual(true, polygonOverlap.DoTouch(pr1, pos1, pr2, pos2));
				Assert.AreEqual(true, polygonOverlap.DoTouch(pr1, pos1, pr2, pos2, 2));
				Assert.AreEqual(false, polygonOverlap.DoTouch(pr1, pos1, pr2, pos2, 3));
			}
		}

        [Test]
        public void DoHaveMinimumDistance_TwoSquares()
        {
            var r1 = GridPolygon.GetSquare(6);
            var r2 = GridPolygon.GetSquare(3);

			Assert.IsTrue(polygonOverlap.DoHaveMinimumDistance(r1, new IntVector2(0, 0), r2, new IntVector2(10, 10), 4));
			Assert.IsFalse(polygonOverlap.DoHaveMinimumDistance(r1, new IntVector2(0, 0), r2, new IntVector2(10, 10), 5));
			Assert.IsFalse(polygonOverlap.DoHaveMinimumDistance(r1, new IntVector2(0, 0), r2, new IntVector2(3, 3), 0));
        }

        [Test]
        public void DoHaveMinimumDistance_TwoPolygons()
        {
            var p1 = GetLShape();
            var p2 = GridPolygon.GetSquare(3);

            foreach (var degrees in GridPolygon.PossibleRotations)
            {
                var pr1 = p1.Rotate(degrees);
                var pr2 = p2.Rotate(degrees);

                var pos1 = new IntVector2(0, 0).RotateAroundCenter(degrees);
                var pos2 = new IntVector2(6, 6).RotateAroundCenter(degrees);

                Assert.IsTrue(polygonOverlap.DoHaveMinimumDistance(pr1, pos1, pr2, pos2, 3));
                Assert.IsFalse(polygonOverlap.DoHaveMinimumDistance(pr1, pos1, pr2, pos2, 4));
            }
        }

		[Test]
		public void OverlapAlongLine_Rectangles_NonOverlapping()
		{
			var p1 = GridPolygon.GetSquare(5);
			var p2 = GridPolygon.GetRectangle(2, 3) + new IntVector2(10, 10);
			var line = new OrthogonalLine(new IntVector2(0, 0), new IntVector2(10, 0));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void OverlapAlongLine_Rectangles_OverlapEnd()
		{
			var p1 = GridPolygon.GetSquare(5);
			var p2 = GridPolygon.GetRectangle(2, 3) + new IntVector2(0, 8);
			var line = new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 10));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(0, 4), true),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_Rectangles_OverlapStart()
		{
			var p1 = GridPolygon.GetSquare(5);
			var p2 = GridPolygon.GetRectangle(2, 3);
			var line = new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 10));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(0, 0), true),
				Tuple.Create(new IntVector2(0, 3), false),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_Rectangles_OverlapStart2()
		{
			var p1 = GridPolygon.GetSquare(5);
			var p2 = GridPolygon.GetRectangle(2, 3) + new IntVector2(0, -3);
			var line = new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 10));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);

			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void OverlapAlongLine_SquareAndL()
		{
			var p1 = GridPolygon.GetSquare(6);
			var p2 = GetLShape();
			var line = new OrthogonalLine(new IntVector2(-2, 3), new IntVector2(5, 3));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(-2, 3), true),
				Tuple.Create(new IntVector2(3, 3), false),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_SquareAndL2()
		{
			var p1 = GridPolygon.GetSquare(6);
			var p2 = GetLShape();
			var line = new OrthogonalLine(new IntVector2(3, 5), new IntVector2(3, -2));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(3, 2), true),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_SquareAndL3()
		{
			var p1 = GridPolygon.GetSquare(6);
			var p2 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 6)
				.AddPoint(6, 6)
				.AddPoint(6, 3)
				.AddPoint(3, 3)
				.AddPoint(3, 0)
				.Build();
			var line = new OrthogonalLine(new IntVector2(3, 2), new IntVector2(3, -5));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(3, 2), true),
				Tuple.Create(new IntVector2(3, -3), false),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_LAndL()
		{
			var p1 = GetLShape();
			var p2 = GetLShape();
			var line = new OrthogonalLine(new IntVector2(-3, -5), new IntVector2(-3, 2));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(-3, -2), true),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_LAndL2()
		{
			var p1 = GetLShape();
			var p2 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 9)
				.AddPoint(3, 9)
				.AddPoint(3, 3)
				.AddPoint(6, 3)
				.AddPoint(6, 0)
				.Build();
			var line = new OrthogonalLine(new IntVector2(3, 8), new IntVector2(3, -2));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(3, 2), true),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_LAndL3()
		{
			var p1 = GetLShape();
			var p2 = GetLShape();
			var line = new OrthogonalLine(new IntVector2(3, 5), new IntVector2(3, -2));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(3, 2), true),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		[Test]
		public void OverlapAlongLine_ComplexCase()
		{
			var p1 = GetPlusShape();
			var p2 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 8)
				.AddPoint(8, 8)
				.AddPoint(8, 2)
				.AddPoint(6, 2)
				.AddPoint(6, 6)
				.AddPoint(2, 6)
				.AddPoint(2, 0)
				.Build();

			var line = new OrthogonalLine(new IntVector2(0, -2), new IntVector2(15, -2));

			var result = polygonOverlap.OverlapAlongLine(p1, p2, line);
			var expected = new List<Tuple<IntVector2, bool>>()
			{
				Tuple.Create(new IntVector2(0, -2), true),
				Tuple.Create(new IntVector2(2, -2), false),
				Tuple.Create(new IntVector2(3, -2), true),
				Tuple.Create(new IntVector2(6, -2), false),
			};

			Assert.IsTrue(expected.SequenceEqual(result));
		}

		private static GridPolygon GetPlusShape()
		{
			return new GridPolygonBuilder()
				.AddPoint(0, 2)
				.AddPoint(0, 4)
				.AddPoint(2, 4)
				.AddPoint(2, 6)
				.AddPoint(4, 6)
				.AddPoint(4, 4)
				.AddPoint(6, 4)
				.AddPoint(6, 2)
				.AddPoint(4, 2)
				.AddPoint(4, 0)
				.AddPoint(2, 0)
				.AddPoint(2, 2)
				.Build();
		}

		private static GridPolygon GetLShape()
		{
			return new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 6)
				.AddPoint(3, 6)
				.AddPoint(3, 3)
				.AddPoint(6, 3)
				.AddPoint(6, 0)
				.Build();
		}
	}
}
