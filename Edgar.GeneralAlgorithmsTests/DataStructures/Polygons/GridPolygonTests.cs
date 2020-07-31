namespace GeneralAlgorithms.Tests.DataStructures.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using NUnit.Framework;

	[TestFixture]
	public class GridPolygonTests
	{
		[Test]
		public void Equals_WhenEqual_ReturnsTrue()
		{
			{
				var p1 = new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 3)
					.AddPoint(3, 3)
					.AddPoint(3, 0)
					.Build();

				var p2 = new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 3)
					.AddPoint(3, 3)
					.AddPoint(3, 0)
					.Build();

				Assert.IsTrue(p1.Equals(p2));
			}
		}

		[Test]
		public void Equals_WhenNotEqual_ReturnsFalse()
		{
			{
				var p1 = new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 3)
					.AddPoint(3, 3)
					.AddPoint(3, 0)
					.Build();

				var p2 = new GridPolygonBuilder()
					.AddPoint(3, 0)
					.AddPoint(0, 0)
					.AddPoint(0, 3)
					.AddPoint(3, 3)
					.Build();

				Assert.IsFalse(p1.Equals(p2));
			}
		}

		[Test]
		public void Scale_ReturnsScaled()
		{
			{
				var p = GridPolygon.GetSquare(3);
				var factor = new Vector2Int(2, 3);

				var scaled = p.Scale(factor);
				var expected = GridPolygon.GetRectangle(6, 9);

				Assert.AreEqual(expected, scaled);
			}

			{
				/*var p = GridPolygon.GetRectangle(2, 4);
				var factor = new IntVector2(-3, 5);

				var scaled = p.Scale(factor);
				var expected = GridPolygon.GetRectangle(-6, 20);

				Assert.AreEqual(expected, scaled);*/
			}
		}

		[Test]
		public void Plus_ReturnsTranslated()
		{
			{
				var p = GridPolygon.GetSquare(3);
				var amount = new Vector2Int(2, 3);

				var translated = p + amount;
				var expected = new GridPolygon(p.GetPoints().Select(x => x + amount));

				Assert.AreEqual(expected, translated);
			}

			{
				var p = GridPolygon.GetRectangle(2, 4);
				var amount = new Vector2Int(-3, 5);

				var translated = p + amount;
				var expected = new GridPolygon(p.GetPoints().Select(x => x + amount));

				Assert.AreEqual(expected, translated);
			}
		}

		[Test]
		public void BoudingRectangle_ReturnsBoundingBox()
		{
			{
				var p = GridPolygon.GetRectangle(2, 4);

				var boundingRectangle = p.BoundingRectangle;
				var expected = new GridRectangle(new Vector2Int(0, 0), new Vector2Int(2, 4));

				Assert.AreEqual(expected, boundingRectangle);
			}

			{
				var p = new GridPolygonBuilder()
					.AddPoint(0, 0)
					.AddPoint(0, 6)
					.AddPoint(3, 6)
					.AddPoint(3, 3)
					.AddPoint(7, 3)
					.AddPoint(7, 0)
					.Build();

				var boundingRectangle = p.BoundingRectangle;
				var expected = new GridRectangle(new Vector2Int(0, 0), new Vector2Int(7, 6));

				Assert.AreEqual(expected, boundingRectangle);
			}
		}

		[Test]
		public void Rotate_Square_ReturnsRotated()
		{
			var square = GridPolygon.GetSquare(4);
			var rotatedSquare = square.Rotate(180);
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(0, 0),
				new Vector2Int(0, -4),
				new Vector2Int(-4, -4),
				new Vector2Int(-4, 0),
			};

			Assert.IsTrue(expectedPoints.SequenceEqual(rotatedSquare.GetPoints()));
		}

		[Test]
		public void Rotate_Rectangle_ReturnsRotated()
		{
			var polygon = GridPolygon.GetRectangle(2, 5);
			var rotatedPolygon = polygon.Rotate(270);
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(0, 0),
				new Vector2Int(-5, 0),
				new Vector2Int(-5, 2),
				new Vector2Int(0, 2),
			};

			Assert.IsTrue(expectedPoints.SequenceEqual(rotatedPolygon.GetPoints()));
		}

		[Test]
		public void Constructor_ValidPolygons()
		{
			var square = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 3)
				.AddPoint(3, 3)
				.AddPoint(3, 0);

			var lPolygon = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 6)
				.AddPoint(3, 6)
				.AddPoint(3, 3)
				.AddPoint(6, 3)
				.AddPoint(6, 0);
				
			Assert.DoesNotThrow(() => square.Build());
			Assert.DoesNotThrow(() => lPolygon.Build());
		}

		[Test]
		public void Constructor_TooFewPoints_Throws()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5);

			var polygon2 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5)
				.AddPoint(5, 5);

			Assert.Throws<ArgumentException>(() => polygon1.Build());
			Assert.Throws<ArgumentException>(() => polygon2.Build());
		}

		[Test]
		public void Constructor_EdgesNotOrthogonal_Trows()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5)
				.AddPoint(3, 4)
				.AddPoint(4, 0);

			Assert.Throws<ArgumentException>(() => polygon1.Build());
		}

		[Test]
		public void Constructor_OverlappingEdges_Throws()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(3, 0)
				.AddPoint(3, 2)
				.AddPoint(0, 2)
				.AddPoint(0, 4);

			Assert.Throws<ArgumentException>(() => polygon1.Build());
		}

		[Test]
		public void Constructor_CounterClockwise_Throws()
		{
			{
				// Square
				var p = new GridPolygonBuilder()
					.AddPoint(3, 0)
					.AddPoint(3, 3)
					.AddPoint(0, 3)
					.AddPoint(0, 0);

				Assert.Throws<ArgumentException>(() => p.Build());
			}

			{
				// L Shape
				var p = new GridPolygonBuilder()
					.AddPoint(6, 0)
					.AddPoint(6, 3)
					.AddPoint(3, 3)
					.AddPoint(3, 6)
					.AddPoint(0, 6)
					.AddPoint(0, 0);

				Assert.Throws<ArgumentException>(() => p.Build());
			}
		}

		[Test]
		public void Transform_ReturnsTransformed()
		{
			var polygon = GridPolygon.GetRectangle(2, 1);

			{
				var transformed = polygon.Transform(Transformation.Identity);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(0, 1),
					new Vector2Int(2, 1),
					new Vector2Int(2, 0)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.Rotate90);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(1, 0),
					new Vector2Int(1, -2),
					new Vector2Int(0, -2)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.Rotate180);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(0, -1),
					new Vector2Int(-2, -1),
					new Vector2Int(-2, 0)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.Rotate270);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(-1, 0),
					new Vector2Int(-1, 2),
					new Vector2Int(0, 2)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.MirrorX);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(2, 0),
					new Vector2Int(2, -1),
					new Vector2Int(0, -1)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.MirrorY);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(-2, 0),
					new Vector2Int(-2, 1),
					new Vector2Int(0, 1)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.Diagonal13);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(0, 2),
					new Vector2Int(1, 2),
					new Vector2Int(1, 0)
				}));
			}

			{
				var transformed = polygon.Transform(Transformation.Diagonal24);
				Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
				{
					new Vector2Int(0, 0),
					new Vector2Int(0, -2),
					new Vector2Int(-1, -2),
					new Vector2Int(-1, 0)
				}));
			}
		}

		[Test]
		public void GetAllTransformations_ReturnsCorrectCount()
		{
			var square = GridPolygon.GetSquare(1);
			var rectangle = GridPolygon.GetRectangle(1, 2);

			var transformedSquares = square.GetAllTransformations();
			var transformedRectangles = rectangle.GetAllTransformations();

			Assert.That(transformedSquares.Count(), Is.EqualTo(8));
			Assert.That(transformedRectangles.Count(), Is.EqualTo(8));
		}
	}
}