namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
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
		public void NormalizePolygon_Polygon_ReturnsNormalizedPolygon()
		{
			var polygon = new GridPolygonBuilder()
				.AddPoint(0, 5)
				.AddPoint(5, 5)
				.AddPoint(5, 0)
				.AddPoint(2, 0)
				.AddPoint(2, 3)
				.AddPoint(0, 3)
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
	}
}
