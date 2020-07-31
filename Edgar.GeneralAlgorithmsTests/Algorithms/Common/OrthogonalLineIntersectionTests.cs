using System;

namespace GeneralAlgorithms.Tests.Algorithms.Common
{
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Common;
	using GeneralAlgorithms.DataStructures.Common;
	using NUnit.Framework;

	[TestFixture]
	public class OrthogonalLineIntersectionTests
	{
		private OrthogonalLineIntersection orthogonalLineIntersection;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			orthogonalLineIntersection = new OrthogonalLineIntersection();
		}

		[Test]
		public void TryGetIntersection_HorizontalLines()
		{
			{
				// No intersection - different Y
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(5, 2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// No intersection - same Y
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(6, 1), new Vector2Int(8, 1));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// Intersection is one point
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(5, 1), new Vector2Int(10, 1));
				var expected = new OrthogonalLine(new Vector2Int(5, 1), new Vector2Int(5, 1));

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.AreEqual(expected, intersection1.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));
				Assert.AreEqual(expected, intersection2.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.AreEqual(expected, intersection3.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
				Assert.AreEqual(expected, intersection4.GetNormalized());
			}

			{
				// Intersection is a line
				var line1 = new OrthogonalLine(new Vector2Int(3, 2), new Vector2Int(10, 2));
				var line2 = new OrthogonalLine(new Vector2Int(7, 2), new Vector2Int(13, 2));
				var expected = new OrthogonalLine(new Vector2Int(7, 2), new Vector2Int(10, 2)).GetNormalized();

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.AreEqual(expected, intersection1.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));
				Assert.AreEqual(expected, intersection2.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.AreEqual(expected, intersection3.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
				Assert.AreEqual(expected, intersection4.GetNormalized());
			}
		}

		[Test]
		public void TryGetIntersection_VerticalLines()
		{
			{
				// No intersection - different Y
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1)).Rotate(90);
				var line2 = new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(5, 2)).Rotate(90);

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// No intersection - same Y
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1)).Rotate(90);
				var line2 = new OrthogonalLine(new Vector2Int(6, 1), new Vector2Int(8, 1)).Rotate(90);

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// Intersection is one point
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1)).Rotate(90);
				var line2 = new OrthogonalLine(new Vector2Int(5, 1), new Vector2Int(10, 1)).Rotate(90);
				var expected = new OrthogonalLine(new Vector2Int(5, 1), new Vector2Int(5, 1), OrthogonalLine.Direction.Bottom).Rotate(90);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.AreEqual(expected, intersection1);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));
				Assert.AreEqual(expected, intersection2);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.AreEqual(expected, intersection3);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
				Assert.AreEqual(expected, intersection4);
			}

			{
				// Intersection is a line
				var line1 = new OrthogonalLine(new Vector2Int(3, 2), new Vector2Int(10, 2)).Rotate(90);
				var line2 = new OrthogonalLine(new Vector2Int(7, 2), new Vector2Int(13, 2)).Rotate(90);
				var expected = new OrthogonalLine(new Vector2Int(7, 2), new Vector2Int(10, 2)).Rotate(90).GetNormalized();

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.AreEqual(expected, intersection1.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));
				Assert.AreEqual(expected, intersection2.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.AreEqual(expected, intersection3.GetNormalized());

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
				Assert.AreEqual(expected, intersection4.GetNormalized());
			}
		}

		[Test]
		public void TryGetIntersection_HorizontalAndVertical()
		{
			{
				// No intersection - one above the other
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(3, 2), new Vector2Int(3, 7));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// No intersection - one next to the other
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(6, 2), new Vector2Int(6, 7));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));

				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.IsFalse(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
			}

			{
				// Intersection is one point
				var line1 = new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(5, 1));
				var line2 = new OrthogonalLine(new Vector2Int(3, -2), new Vector2Int(3, 5));
				var expected = new OrthogonalLine(new Vector2Int(3, 1), new Vector2Int(3, 1));

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1, line2, out var intersection1));
				Assert.AreEqual(expected, intersection1);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line1.SwitchOrientation(), line2.SwitchOrientation(), out var intersection2));
				Assert.AreEqual(expected, intersection2);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2, line1, out var intersection3));
				Assert.AreEqual(expected, intersection3);

				Assert.IsTrue(orthogonalLineIntersection.TryGetIntersection(line2.SwitchOrientation(), line1.SwitchOrientation(), out var intersection4));
				Assert.AreEqual(expected, intersection4);
			}
		}

		[Test]
		public void GetIntersections()
		{
			var lines1 = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new Vector2Int(2, 1), new Vector2Int(2, 7)),
				new OrthogonalLine(new Vector2Int(3, 4), new Vector2Int(8, 4)),
				new OrthogonalLine(new Vector2Int(6, 5), new Vector2Int(6, 8))
			};

			var lines2 = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new Vector2Int(1, 6), new Vector2Int(7, 6)),
				new OrthogonalLine(new Vector2Int(4, 4), new Vector2Int(7, 4)),
				new OrthogonalLine(new Vector2Int(8, 2), new Vector2Int(8, 6))
			};

			var expected = new List<OrthogonalLine>()
			{
				new OrthogonalLine(new Vector2Int(2, 6), new Vector2Int(2, 6)),
				new OrthogonalLine(new Vector2Int(4, 4), new Vector2Int(7, 4)),
				new OrthogonalLine(new Vector2Int(6, 6), new Vector2Int(6, 6)),
				new OrthogonalLine(new Vector2Int(8, 4), new Vector2Int(8, 4)),
			};

			var intersection = orthogonalLineIntersection.GetIntersections(lines1, lines2);

			Assert.IsTrue(expected.SequenceEqualWithoutOrder(intersection.Select(x => x.GetNormalized())));
		}

		[Test]
        public void PartitionByIntersection_NotIntersection_Throws()
        {
			var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
			var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(11, 0), new Vector2Int(15, 0))
			};

            Assert.Throws<ArgumentException>(() => orthogonalLineIntersection.PartitionByIntersection(line, intersection));
        }

        [Test]
        public void PartitionByIntersection_PerpendicularIntersection_Throws()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(5, -1), new Vector2Int(5, 1))
            };

            Assert.Throws<ArgumentException>(() => orthogonalLineIntersection.PartitionByIntersection(line, intersection));
        }

        [Test]
        public void PartitionByIntersection_OverlappingIntersections_Throws()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(4, 0), new Vector2Int(6, 0)),
                new OrthogonalLine(new Vector2Int(5, 0), new Vector2Int(7, 0)),
            };

            Assert.Throws<ArgumentException>(() => orthogonalLineIntersection.PartitionByIntersection(line, intersection));
        }

		[Test]
        public void PartitionByIntersection_OnePointIntersection_ReturnsTwoLines()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(5, 0), new Vector2Int(5, 0))
            };
			var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(4, 0)),
                new OrthogonalLine(new Vector2Int(6, 0), new Vector2Int(10, 0)),
            };

			var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

        [Test]
        public void PartitionByIntersection_TwoPointsIntersection_ReturnsThreeLines()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(5, 0), new Vector2Int(5, 0)),
                new OrthogonalLine(new Vector2Int(7, 0), new Vector2Int(7, 0)),
            };
            var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(4, 0)),
                new OrthogonalLine(new Vector2Int(6, 0), new Vector2Int(6, 0)),
                new OrthogonalLine(new Vector2Int(8, 0), new Vector2Int(10, 0)),
            };

            var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

        [Test]
        public void PartitionByIntersection_TwoNeighboringPointsIntersection_ReturnsTwoLines()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(5, 0), new Vector2Int(5, 0)),
                new OrthogonalLine(new Vector2Int(6, 0), new Vector2Int(6, 0)),
            };
            var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(4, 0)),
                new OrthogonalLine(new Vector2Int(7, 0), new Vector2Int(10, 0)),
            };

            var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

		[Test]
        public void PartitionByIntersection_TwoPointsReversedIntersection_ReturnsThreeLines()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(7, 0), new Vector2Int(7, 0)),
				new OrthogonalLine(new Vector2Int(5, 0), new Vector2Int(5, 0)),
            };
            var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(4, 0)),
                new OrthogonalLine(new Vector2Int(6, 0), new Vector2Int(6, 0)),
                new OrthogonalLine(new Vector2Int(8, 0), new Vector2Int(10, 0)),
            };

            var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

        [Test]
        public void PartitionByIntersection_TwoEndpointsIntersection_ReturnsOneLine()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, 0)),
                new OrthogonalLine(new Vector2Int(10, 0), new Vector2Int(10, 0)),
            };
            var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(9, 0)),
            };

            var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

		[Test]
        public void PartitionByIntersection_OneLineIntersection_ReturnsTwoLines()
        {
            var line = new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0));
            var intersection = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(4, 0), new Vector2Int(6, 0))
            };
            var expectedPartitions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(3, 0)),
                new OrthogonalLine(new Vector2Int(7, 0), new Vector2Int(10, 0)),
            };

            var partitions = orthogonalLineIntersection.PartitionByIntersection(line, intersection);

            Assert.That(partitions, Is.EquivalentTo(expectedPartitions));
        }

		[Test]
        public void RemoveIntersections_OneLine_ReturnsOneLine()
        {
            var lines = new List<OrthogonalLine>()
            {
				new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0))
            };

            var linesWithoutIntersections = orthogonalLineIntersection.RemoveIntersections(lines);

			Assert.That(linesWithoutIntersections, Is.EquivalentTo(lines));
        }

        [Test]
        public void RemoveIntersections_MultipleLines()
        {
            var lines = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(10, 0)),
                new OrthogonalLine(new Vector2Int(0, 5), new Vector2Int(10, 5)),
                new OrthogonalLine(new Vector2Int(2, -5), new Vector2Int(2, 10)),
                new OrthogonalLine(new Vector2Int(7, -5), new Vector2Int(7, 10)),
            };

            var linesWithoutIntersections = orthogonalLineIntersection.RemoveIntersections(lines);
            var points = linesWithoutIntersections.SelectMany(x => x.GetPoints());
            var expectedPoints = lines.SelectMany(x => x.GetPoints()).Distinct();

            Assert.That(points, Is.EquivalentTo(expectedPoints));
        }
	}
}