namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using GeneralAlgorithms.Algorithms.Polygons;
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
			var square = GridPolygonUtils.GetSquare(3);
			var rectangle = GridPolygonUtils.GetRectangle(2, 4);

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
	}
}
