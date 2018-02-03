namespace MapGeneration.Tests.Core.ConfigurationSpaces
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core;
	using MapGeneration.Core.ConfigurationSpaces;
	using MapGeneration.Core.Doors;
	using MapGeneration.Core.Doors.DoorModes;
	using NUnit.Framework;

	[TestFixture]
	public class CSGeneratorTests
	{
		private ConfigurationSpacesGenerator generator;

		[SetUp]
		public void SetUp()
		{
			generator = new ConfigurationSpacesGenerator(new PolygonOverlap(), DoorHandler.DefaultHandler, new OrthogonalLineIntersection(), new GridPolygonUtils());
		}

		[Test]
		public void GetConfigurationSpace_Squares()
		{
			var p1 = GridPolygon.GetSquare(3);
			var p2 = GridPolygon.GetSquare(5);

			var configurationSpace = generator.GetConfigurationSpace(p1, new OverlapMode(1, 0),  p2, new OverlapMode(1, 0));
			var expectedPoints = new List<IntVector2>();
			var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

			{
				// Top side of fixed
				var points = new OrthogonalLine(new IntVector2(-2, 5), new IntVector2(4, 5)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Bottom side of fixed
				var points = new OrthogonalLine(new IntVector2(-2, -3), new IntVector2(4, -3)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Right side of fixed
				var points = new OrthogonalLine(new IntVector2(5, 4), new IntVector2(5, -2)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Left side of fixed
				var points = new OrthogonalLine(new IntVector2(-3, -2), new IntVector2(-3, 4)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
		}

		[Test]
		public void GetConfigurationSpace_MovedSquares()
		{
			// Check if the generator works even when the polygons are not normalized
			Assert.IsTrue(false);
		}

		[Test]
		public void GetConfigurationSpace_OverlapOne()
		{
			var p1 = GridPolygon.GetSquare(3);
			var p2 = GridPolygon.GetSquare(5);

			var configurationSpace = generator.GetConfigurationSpace(p1, new OverlapMode(1, 1), p2, new OverlapMode(1, 1));
			var expectedPoints = new List<IntVector2>();
			var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

			{
				// Top side of fixed
				var points = new OrthogonalLine(new IntVector2(0, 5), new IntVector2(2, 5)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Bottom side of fixed
				var points = new OrthogonalLine(new IntVector2(0, -3), new IntVector2(2, -3)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Right side of fixed
				var points = new OrthogonalLine(new IntVector2(5, 2), new IntVector2(5, 0)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			{
				// Left side of fixed
				var points = new OrthogonalLine(new IntVector2(-3, 0), new IntVector2(-3, 2)).GetPoints();
				expectedPoints.AddRange(points);
				Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
			}

			Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
		}

		[Test]
		public void GetConfigurationSpace_DifferentLengths()
		{
			{
				var p1 = GridPolygon.GetSquare(3);
				var p2 = GridPolygon.GetSquare(5);

				var configurationSpace = generator.GetConfigurationSpace(p1, new OverlapMode(1, 0), p2, new OverlapMode(2, 0));
				var expectedPoints = new List<IntVector2>();
				var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

				{
					// Top side of fixed
					var points = new OrthogonalLine(new IntVector2(-1, 5), new IntVector2(3, 5)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Bottom side of fixed
					var points = new OrthogonalLine(new IntVector2(-1, -3), new IntVector2(3, -3)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Right side of fixed
					var points = new OrthogonalLine(new IntVector2(5, 3), new IntVector2(5, -1)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Left side of fixed
					var points = new OrthogonalLine(new IntVector2(-3, -1), new IntVector2(-3, 3)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
			}

			{
				var p1 = GridPolygon.GetSquare(3);
				var p2 = GridPolygon.GetSquare(5);

				var configurationSpace = generator.GetConfigurationSpace(p1, new OverlapMode(2, 0), p2, new OverlapMode(1, 0));
				var expectedPoints = new List<IntVector2>();
				var actualPoints = configurationSpace.Lines.Select(x => x.GetPoints()).SelectMany(x => x).ToList();

				{
					// Top side of fixed
					var points = new OrthogonalLine(new IntVector2(-1, 5), new IntVector2(3, 5)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Bottom side of fixed
					var points = new OrthogonalLine(new IntVector2(-1, -3), new IntVector2(3, -3)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Right side of fixed
					var points = new OrthogonalLine(new IntVector2(5, 3), new IntVector2(5, -1)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				{
					// Left side of fixed
					var points = new OrthogonalLine(new IntVector2(-3, -1), new IntVector2(-3, 3)).GetPoints();
					expectedPoints.AddRange(points);
					Assert.AreEqual(points.Count, actualPoints.Intersect(points).Count());
				}

				Assert.AreEqual(expectedPoints.Distinct().Count(), actualPoints.Count);
			}
		}

		[Test]
		public void Generate_BasicTest()
		{
			// This test cannot check if the generated configuration spaces are valid
			var mapDescription = new MapDescription<int>();
			var squareRoom = new RoomDescription(GridPolygon.GetSquare(3), new OverlapMode(1, 0));
			var rectangleRoom = new RoomDescription(GridPolygon.GetRectangle(4, 5), new OverlapMode(1, 1));

			mapDescription.AddRoomShapes(squareRoom, rotate: true);
			mapDescription.AddRoomShapes(rectangleRoom, rotate: true, probability: 0.5d);

			mapDescription.AddRoom(0);
			mapDescription.AddRoom(1);
			mapDescription.AddPassage(0, 1);

			mapDescription.AddRoomShapes(1, rectangleRoom, false);

			var configurationSpaces = generator.Generate(mapDescription);

			Assert.AreEqual(3, configurationSpaces.GetShapesForNode(0).Count);
			Assert.AreEqual(1, configurationSpaces.GetShapesForNode(1).Count);
		}
	}
}