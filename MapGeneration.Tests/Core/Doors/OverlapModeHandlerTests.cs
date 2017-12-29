namespace MapGeneration.Tests.Core.Doors
{
	using System.Collections.Generic;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using MapGeneration.Core.Doors.DoorHandlers;
	using MapGeneration.Core.Doors.DoorModes;
	using NUnit.Framework;

	[TestFixture]
	public class OverlapModeHandlerTests
	{
		private OverlapModeHandler overlapModeHandler;

		[SetUp]
		public void SetUp()
		{
			overlapModeHandler = new OverlapModeHandler();
		}

		[Test]
		public void Rectangle_NoOverlap()
		{
			var polygon = GridPolygon.GetRectangle(3, 5);
			var mode = new OverlapMode(1, 0);
			var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 4)),
				new OrthogonalLine(new IntVector2(0, 5), new IntVector2(2, 5)),
				new OrthogonalLine(new IntVector2(3, 5), new IntVector2(3, 1)),
				new OrthogonalLine(new IntVector2(3, 0), new IntVector2(1, 0))
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_OneOverlap()
		{
			var polygon = GridPolygon.GetRectangle(3, 5);
			var mode = new OverlapMode(1, 1);
			var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new IntVector2(0, 1), new IntVector2(0, 3)),
				new OrthogonalLine(new IntVector2(1, 5), new IntVector2(1, 5)),
				new OrthogonalLine(new IntVector2(3, 4), new IntVector2(3, 2)),
				new OrthogonalLine(new IntVector2(2, 0), new IntVector2(2, 0))
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_TwoOverlap()
		{
			var polygon = GridPolygon.GetRectangle(3, 5);
			var mode = new OverlapMode(1, 2);
			var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new IntVector2(0, 2), new IntVector2(0, 2)),
				new OrthogonalLine(new IntVector2(3, 3), new IntVector2(3, 3)),
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}

		[Test]
		public void Rectangle_LengthTwo()
		{
			var polygon = GridPolygon.GetRectangle(3, 5);
			var mode = new OverlapMode(2, 0);
			var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);
			var expectedPositions = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 3)),
				new OrthogonalLine(new IntVector2(0, 5), new IntVector2(1, 5)),
				new OrthogonalLine(new IntVector2(3, 5), new IntVector2(3, 2)),
				new OrthogonalLine(new IntVector2(3, 0), new IntVector2(2, 0))
			};

			Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
		}
	}
}