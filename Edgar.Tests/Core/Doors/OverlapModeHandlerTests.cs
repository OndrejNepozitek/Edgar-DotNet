using Edgar.Geometry;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Tests.Core.Doors
{
	using System.Collections.Generic;
    using NUnit.Framework;

	[TestFixture]
	public class OverlapModeHandlerTests
	{
		private SimpleModeHandler simpleModeHandler;

		[SetUp]
		public void SetUp()
		{
			simpleModeHandler = new SimpleModeHandler();
		}

		[Test]
		public void Rectangle_NoOverlap()
		{
			var polygon = PolygonGrid2D.GetRectangle(3, 5);
			var mode = new SimpleDoorMode(1, 0);
			var doorPositions = simpleModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 4)), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(2, 5)), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 1)), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(1, 0)), 1)
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_OneOverlap()
		{
			var polygon = PolygonGrid2D.GetRectangle(3, 5);
			var mode = new SimpleDoorMode(1, 1);
			var doorPositions = simpleModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 1), new Vector2Int(0, 3)), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(1, 5), new Vector2Int(1, 5), OrthogonalLineGrid2D.Direction.Right), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 4), new Vector2Int(3, 2)), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(2, 0), new Vector2Int(2, 0), OrthogonalLineGrid2D.Direction.Left), 1),
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_TwoOverlap()
		{
			var polygon = PolygonGrid2D.GetRectangle(3, 5);
			var mode = new SimpleDoorMode(1, 2);
			var doorPositions = simpleModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 2), OrthogonalLineGrid2D.Direction.Top), 1),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 3), new Vector2Int(3, 3), OrthogonalLineGrid2D.Direction.Bottom), 1),
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_LengthTwo()
		{
			var polygon = PolygonGrid2D.GetRectangle(3, 5);
			var mode = new SimpleDoorMode(2, 0);
			var doorPositions = simpleModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 3)), 2),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(1, 5)), 2),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 2)), 2),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(2, 0)), 2),
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_LengthZero()
		{
			var polygon = PolygonGrid2D.GetRectangle(3, 5);
			var mode = new SimpleDoorMode(0, 0);
			var doorPositions = simpleModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 5)), 0),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(3, 5)), 0),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 0)), 0),
				new DoorLine(new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(0, 0)), 0),
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}
	}
}