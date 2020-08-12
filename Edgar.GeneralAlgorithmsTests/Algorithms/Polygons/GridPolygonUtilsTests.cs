using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;

namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using System.Collections.Generic;
	using System.Linq;
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
			var expectedPoints = new List<Vector2Int>()
			{
				new Vector2Int(0, 3),
				new Vector2Int(0, 5),
				new Vector2Int(5, 5),
				new Vector2Int(5, 0),
				new Vector2Int(2, 0),
				new Vector2Int(2, 3),
			};

			Assert.IsTrue(expectedPoints.SequenceEqual(squarePoints));
		}
	}
}
