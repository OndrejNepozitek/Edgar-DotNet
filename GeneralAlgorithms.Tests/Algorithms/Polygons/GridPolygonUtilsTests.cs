namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using NUnit.Framework;

	[TestFixture]
	internal class GridPolygonUtilsTests
	{
		private GridPolygonUtils utils;

		[SetUp]
		public void SetUp()
		{
			utils = new GridPolygonUtils();
		}

		[Test]
		public void CheckIntegrity_ValidPolygons_ReturnsTrue()
		{
			var square = GridPolygon.GetSquare(3);
			var rectangle = GridPolygon.GetRectangle(2, 4);

			var lPolygon = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(6, 0)
				.AddPoint(6, 3)
				.AddPoint(3, 3)
				.AddPoint(3, 6)
				.AddPoint(0, 6)
				.Build();

			Assert.IsTrue(utils.CheckIntegrity(square));
			Assert.IsTrue(utils.CheckIntegrity(rectangle));
			Assert.IsTrue(utils.CheckIntegrity(lPolygon));
		}

		[Test]
		public void CheckIntegrity_TooFewPoints_ReturnsFalse()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5)
				.Build();

			var polygon2 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5)
				.AddPoint(5, 5)
				.Build();

			Assert.IsFalse(utils.CheckIntegrity(polygon1));
			Assert.IsFalse(utils.CheckIntegrity(polygon2));
		}

		[Test]
		public void CheckIntegrity_EdgesNotOrthogonal_ReturnsFalse()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 5)
				.AddPoint(3, 4)
				.AddPoint(4, 0)
				.Build();

			Assert.IsFalse(utils.CheckIntegrity(polygon1));
		}

		[Test]
		public void CheckIntegrity_OverlappingEdges_ReturnsFalse()
		{
			var polygon1 = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(3, 0)
				.AddPoint(3, 2)
				.AddPoint(0, 2)
				.AddPoint(0, 4)
				.Build();

			Assert.IsFalse(utils.CheckIntegrity(polygon1));
		}

		[Test]
		public void NormalizePolygon_Square_ReturnsNormalizedPolygon()
		{
			var square = new GridPolygonBuilder()
				.AddPoint(-2, -2)
				.AddPoint(2, -2)
				.AddPoint(2, 2)
				.AddPoint(-2, 2)
				.Build();

			var normalizedSquare = utils.NormalizePolygon(square);
			var squarePoints = normalizedSquare.GetPoints();
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(0, 0),
				new IntVector2(0, 4),
				new IntVector2(4, 4),
				new IntVector2(4, 0),
			};

			Assert.IsTrue(expectedPoints.SequenceEqual(squarePoints));
		}

		[Test]
		public void NormalizePolygon_Polygon_ReturnsNormalizedPolygon()
		{
			var polygon = new GridPolygonBuilder()
				.AddPoint(3, 0)
				.AddPoint(-2, 0)
				.AddPoint(-2, -2)
				.AddPoint(0, -2)
				.AddPoint(0, -5)
				.AddPoint(3, -5)
				.Build();

			var normalized = utils.NormalizePolygon(polygon);
			var squarePoints = normalized.GetPoints();
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(0, 3),
				new IntVector2(0, 5),
				new IntVector2(5, 5),
				new IntVector2(5, 0),
				new IntVector2(2, 0),
				new IntVector2(2, 3),
			};

			Assert.IsTrue(expectedPoints.SequenceEqual(squarePoints));
		}

		[Test]
		public void DecomposeIntoRectangle_Polygon_ReturnsDecomposition()
		{
			/*var polygon = new GridPolygonBuilder()
				.AddPoint(0, 3)
				.AddPoint(0, 5)
				.AddPoint(5, 5)
				.AddPoint(5, 0)
				.AddPoint(2, 0)
				.AddPoint(2, 3)
				.Build();

			var expectedRectangles = new List<GridRectangle>
			{
				new GridRectangle(new IntVector2(0, 3), new IntVector2(5, 5)),
				new GridRectangle(new IntVector2(0, 3), new IntVector2(5, 5)),
				new GridRectangle(new IntVector2(0, 3), new IntVector2(5, 5)),
				new GridRectangle(new IntVector2(0, 3), new IntVector2(5, 5)),
				new GridRectangle(new IntVector2(0, 3), new IntVector2(5, 5)),
			};

			var rectangles = utils.DecomposeIntoRectangles(polygon);*/
		}
	}
}
