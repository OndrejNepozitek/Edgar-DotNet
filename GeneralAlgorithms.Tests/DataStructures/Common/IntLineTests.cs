namespace GeneralAlgorithms.Tests.DataStructures.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.DataStructures.Common;
	using NUnit.Framework;

	[TestFixture]
	public class IntLineTests
	{
		[Test]
		public void GetPoints_Top_ReturnsPoints()
		{
			var line = new IntLine(new IntVector2(2, 2), new IntVector2(2, 4));
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(2, 2),
				new IntVector2(2, 3),
				new IntVector2(2, 4),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetPoints_Bottom_ReturnsPoints()
		{
			var line = new IntLine(new IntVector2(2, 4), new IntVector2(2, 2));
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(2, 4),
				new IntVector2(2, 3),
				new IntVector2(2, 2),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}


		[Test]
		public void GetPoints_Right_ReturnsPoints()
		{
			var line = new IntLine(new IntVector2(5, 3), new IntVector2(8, 3));
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(5, 3),
				new IntVector2(6, 3),
				new IntVector2(7, 3),
				new IntVector2(8, 3),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetPoints_Left_ReturnsPoints()
		{
			var line = new IntLine(new IntVector2(8, 3), new IntVector2(5, 3));
			var expectedPoints = new List<IntVector2>()
			{
				new IntVector2(8, 3),
				new IntVector2(7, 3),
				new IntVector2(6, 3),
				new IntVector2(5, 3),
			};

			Assert.IsTrue(line.GetPoints().SequenceEqual(expectedPoints));
		}

		[Test]
		public void GetDirection_ReturnsDirection()
		{
			var top = new IntLine(new IntVector2(2, 2), new IntVector2(2, 4));
			var bottom = new IntLine(new IntVector2(2, 4), new IntVector2(2, 2));
			var right = new IntLine(new IntVector2(5, 3), new IntVector2(8, 3));
			var left = new IntLine(new IntVector2(8, 3), new IntVector2(5, 3));

			Assert.AreEqual(IntLine.Direction.Top, top.GetDirection());
			Assert.AreEqual(IntLine.Direction.Bottom, bottom.GetDirection());
			Assert.AreEqual(IntLine.Direction.Right, right.GetDirection());
			Assert.AreEqual(IntLine.Direction.Left, left.GetDirection());
		}

		[Test]
		public void Shrink_Valid_ReturnsShrinked()
		{
			{
				var line = new IntLine(new IntVector2(0, 0), new IntVector2(5, 0));
				var expected = new IntLine(new IntVector2(1, 0), new IntVector2(3, 0));
				var shrinked = line.Shrink(1, 2);

				Assert.AreEqual(expected, shrinked);
			}

			{
				var line = new IntLine(new IntVector2(0, 0), new IntVector2(0, 6));
				var expected = new IntLine(new IntVector2(0, 2), new IntVector2(0, 5));
				var shrinked = line.Shrink(2, 1);

				Assert.AreEqual(expected, shrinked);
			}
		}

		[Test]
		public void Shrink_Invalid_Throws()
		{
			{
				var line = new IntLine(new IntVector2(0, 0), new IntVector2(5, 0));
				Assert.Throws<InvalidOperationException>(() => line.Shrink(3));
			}

			{
				var line = new IntLine(new IntVector2(0, 0), new IntVector2(-6, 0));
				Assert.Throws<InvalidOperationException>(() => line.Shrink(4, 2));
			}
		}
	}
}