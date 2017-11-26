namespace MapGeneration.Tests.Grid
{
	using System.Collections.Generic;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Grid;
	using NUnit.Framework;

	[TestFixture]
	public class ConfigurationSpacesTests
	{
		private ConfigurationSpacesGenerator generator;

		[SetUp]
		public void SetUp()
		{
			generator = new ConfigurationSpacesGenerator();
		}

		[Test]
		public void GetMaximumIntersection_ThreeSquares()
		{
			#region MyRegion

			//	     |
			//	     |
			//	     ===
			//	     = =
			//	  XXX===XXX
			//	  X X   X X
			//	__XXX___XXX__
			//	     |
			//	     |
			//	     |			

			#endregion

			var square = GridPolygon.GetSquare(3);
			var configurationSpaces = generator.Generate(new List<GridPolygon>() {square});

			Assert.AreEqual(1, configurationSpaces.GetPolygons().Count);

			var c1 = new Configuration(square, new IntVector2(-3, 0));
			var c2 = new Configuration(square, new IntVector2(0, 0));
			var c3 = new Configuration(square, new IntVector2(3, 0));

			var intersection = configurationSpaces.GetMaximumIntersection(new List<Configuration>() {c1, c3}, c2);
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(0, -2),
				new IntVector2(0, -1),
				new IntVector2(0, 0),
				new IntVector2(0, 1),
				new IntVector2(0, 2),
			};

			Assert.AreEqual(expectedPoints.Count, intersection.Count);

			foreach (var point in expectedPoints)
			{
				Assert.IsTrue(intersection.Contains(point));
			}
		}

		[Test]
		public void GetMaximumIntersection_FourSquares()
		{
			// One square is far from the others
			var square = GridPolygon.GetSquare(3);
			var configurationSpaces = generator.Generate(new List<GridPolygon>() { square });

			Assert.AreEqual(1, configurationSpaces.GetPolygons().Count);

			var c1 = new Configuration(square, new IntVector2(-3, 0));
			var c2 = new Configuration(square, new IntVector2(3, 0));
			var c3 = new Configuration(square, new IntVector2(10, 0));
			var c4 = new Configuration(square, new IntVector2(0, 0));

			var intersection = configurationSpaces.GetMaximumIntersection(new List<Configuration>() { c1, c2, c3 }, c4);
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(0, -2),
				new IntVector2(0, -1),
				new IntVector2(0, 0),
				new IntVector2(0, 1),
				new IntVector2(0, 2),
			};

			Assert.AreEqual(expectedPoints.Count, intersection.Count);

			foreach (var point in expectedPoints)
			{
				Assert.IsTrue(intersection.Contains(point));
			}
		}

		[Test]
		public void GetMaximumIntersection_ThreeRectangle()
		{
			#region Sketch

			//	| XXX
			//	| X X
			//	| X X
			//	| X X
			//	| XXX
			//	| =====
			//	| =   =
			//	| =   =
			//	| =====
			//	XXXX
			//	XXXX________

			#endregion

			var r1 = GridPolygon.GetRectangle(4, 2);
			var r2 = GridPolygon.GetRectangle(3, 5);
			var r3 = GridPolygon.GetRectangle(5, 4);

			var configurationSpaces = generator.Generate(new List<GridPolygon>() { r1, r2, r3 });

			Assert.AreEqual(6, configurationSpaces.GetPolygons().Count);

			var c1 = new Configuration(r1, new IntVector2(-1, 0));
			var c2 = new Configuration(r2, new IntVector2(3, 6));
			var c3 = new Configuration(r3, new IntVector2(0, 0));

			var intersection = configurationSpaces.GetMaximumIntersection(new List<Configuration>() { c1, c2 }, c3);
			var expectedPoints = new IntLine(new IntVector2(-1, 2), new IntVector2(2, 2)).GetPoints();

			Assert.AreEqual(expectedPoints.Count, intersection.Count);

			foreach (var point in expectedPoints)
			{
				Assert.IsTrue(intersection.Contains(point));
			}
		}
	}
}