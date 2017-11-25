namespace GeneralAlgorithms.Tests.DataStructures.Polygons
{
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
		public void Equals_WhenNotEqual_ReturnsTrue()
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

				Assert.IsTrue(p1.Equals(p2));
			}
		}

		[Test]
		public void Scale_ReturnsScaled()
		{
			{
				var p = GridPolygonUtils.GetSquare(3);
				var factor = new IntVector2(2, 3);

				var scaled = p.Scale(factor);
				var expected = GridPolygonUtils.GetRectangle(6, 9);

				Assert.AreEqual(expected, scaled);
			}

			{
				var p = GridPolygonUtils.GetRectangle(2, 4);
				var factor = new IntVector2(-3, 5);

				var scaled = p.Scale(factor);
				var expected = GridPolygonUtils.GetRectangle(-6, 20);

				Assert.AreEqual(expected, scaled);
			}
		}

		[Test]
		public void Plus_ReturnsTranslated()
		{
			{
				var p = GridPolygonUtils.GetSquare(3);
				var amount = new IntVector2(2, 3);

				var translated = p + amount;
				var expected = new GridPolygon(p.GetPoints().Select(x => x + amount));

				Assert.AreEqual(expected, translated);
			}

			{
				var p = GridPolygonUtils.GetRectangle(2, 4);
				var amount = new IntVector2(-3, 5);

				var translated = p + amount;
				var expected = new GridPolygon(p.GetPoints().Select(x => x + amount));

				Assert.AreEqual(expected, translated);
			}
		}

		[Test]
		public void BoudingRectangle_ReturnsBoundingBox()
		{
			{
				var p = GridPolygonUtils.GetRectangle(2, 4);

				var boundingRectangle = p.BoundingRectangle;
				var expected = new GridRectangle(new IntVector2(0, 0), new IntVector2(2, 4));

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
				var expected = new GridRectangle(new IntVector2(0, 0), new IntVector2(7, 6));

				Assert.AreEqual(expected, boundingRectangle);
			}
		}
	}
}