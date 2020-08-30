namespace MapGeneration.Tests.Grid
{
	using System.Collections.Generic;
	using System.Linq;
    using NUnit.Framework;

	[TestFixture]
	public class ConfigurationSpaceGeneratorTests
	{
		/*private ConfigSpacesGenerator generator;

		[SetUp]
		public void SetUp()
		{
			generator = new ConfigSpacesGenerator();
		}

		[Test]
		public void GetConfigurationSpace_Squares()
		{
			var p1 = GridPolygon.GetSquare(3);
			var p2 = GridPolygon.GetSquare(5);

			var configurationSpace = generator.GetConfigurationSpace(p1, p2);
			var expectedPoints = new List<IntVector2>();

			{
				// Top side of fixed
				var points = new IntLine(new IntVector2(-2, 5), new IntVector2(4, 5)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Bottom side of fixed
				var points = new IntLine(new IntVector2(-2, -3), new IntVector2(4, -3)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Right side of fixed
				var points = new IntLine(new IntVector2(5, 4), new IntVector2(5, -2)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Left side of fixed
				var points = new IntLine(new IntVector2(-3, -2), new IntVector2(-3, 4)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			Assert.AreEqual(expectedPoints.Distinct().Count(), configurationSpace.Points.Count);
		}

		[Test]
		public void GetConfigurationSpace_MarginOne()
		{
			var p1 = GridPolygon.GetSquare(3);
			var p2 = GridPolygon.GetSquare(5);

			var configurationSpace = generator.GetConfigurationSpace(p1, p2, 1);
			var expectedPoints = new List<IntVector2>();

			{
				// Top side of fixed
				var points = new IntLine(new IntVector2(0, 5), new IntVector2(2, 5)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Bottom side of fixed
				var points = new IntLine(new IntVector2(0, -3), new IntVector2(2, -3)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Right side of fixed
				var points = new IntLine(new IntVector2(5, 2), new IntVector2(5, 0)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			{
				// Left side of fixed
				var points = new IntLine(new IntVector2(-3, 0), new IntVector2(-3, 2)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, configurationSpace.Points.Intersect(points).Count());
			}

			Assert.AreEqual(expectedPoints.Distinct().Count(), configurationSpace.Points.Count);
		}*/
	}
}