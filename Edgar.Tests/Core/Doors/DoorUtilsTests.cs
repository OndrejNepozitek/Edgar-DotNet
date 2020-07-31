using MapGeneration.Core.Doors.Interfaces;

namespace MapGeneration.Tests.Core.Doors
{
	using System.Collections.Generic;
	using GeneralAlgorithms.DataStructures.Common;
    using MapGeneration.Core.Doors;
	using NUnit.Framework;

	[TestFixture]
	public class DoorUtilsTests
	{
		[Test]
		public void MergeDoorLines_CorrectlyMerges()
		{
			var doorLines = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(2, 0)), 2),
				new DoorLine(new OrthogonalLine(new Vector2Int(3, 0), new Vector2Int(5, 0)), 2),
				new DoorLine(new OrthogonalLine(new Vector2Int(-3, 0), new Vector2Int(0, 0)), 1),
				new DoorLine(new OrthogonalLine(new Vector2Int(-3, 0), new Vector2Int(0, 0)), 2),
				new DoorLine(new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, 3)), 2),
				new DoorLine(new OrthogonalLine(new Vector2Int(0, -2), new Vector2Int(0, -1)), 2),
			};

			var expectedDoorLines = new List<DoorLine>()
			{
				new DoorLine(new OrthogonalLine(new Vector2Int(-3, 0), new Vector2Int(0, 0)), 1),
				new DoorLine(new OrthogonalLine(new Vector2Int(-3, 0), new Vector2Int(5, 0)), 2),
				new DoorLine(new OrthogonalLine(new Vector2Int(0, -2), new Vector2Int(0, 3)), 2),
			};

			var mergedDoorLines = DoorUtils.MergeDoorLines(doorLines);

			Assert.That(mergedDoorLines, Is.EquivalentTo(expectedDoorLines));
		}
	}
}