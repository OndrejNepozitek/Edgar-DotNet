namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using GeneralAlgorithms.Algorithms.Polygons;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using NUnit.Framework;

	[TestFixture]
	public class GridPolygonPartitioningTests
	{
		private GridPolygonPartitioning partitioner;
		private GridPolygonUtils utils;

		[SetUp]
		public void SetUp()
		{
			partitioner = new GridPolygonPartitioning();
			utils = new GridPolygonUtils();
		}

		[Test]
		public void GetPolygons_PlusShape()
		{
			var polygon = new GridPolygonBuilder()
				.AddPoint(0, 2)
				.AddPoint(0, 4)
				.AddPoint(2, 4)
				.AddPoint(2, 6)
				.AddPoint(4, 6)
				.AddPoint(4, 4)
				.AddPoint(6, 4)
				.AddPoint(6, 2)
				.AddPoint(4, 2)
				.AddPoint(4, 0)
				.AddPoint(2, 0)
				.AddPoint(2, 2)
				.Build();

			var partitions = partitioner.GetRectangles(polygon);
			var expected = new List<List<GridRectangle>>()
			{
				new List<GridRectangle>()
				{
					new GridRectangle(new IntVector2(2, 0), new IntVector2(4, 2)),
					new GridRectangle(new IntVector2(0, 2), new IntVector2(6, 4)),
					new GridRectangle(new IntVector2(2, 4), new IntVector2(4, 6)),
				},
				new List<GridRectangle>()
				{
					new GridRectangle(new IntVector2(0, 2), new IntVector2(2, 4)),
					new GridRectangle(new IntVector2(2, 0), new IntVector2(4, 6)),
					new GridRectangle(new IntVector2(4, 2), new IntVector2(6, 4)),
				}
			};

			var matched = false;

			foreach (var rectangles in expected)
			{
				foreach (var r in rectangles)
				{
					if (!partitions.Contains(r))
					{
						break;
					}
				}

				matched = true;
			}

			Assert.AreEqual(expected[0].Count, partitions.Count);
			Assert.AreEqual(true, matched);

			foreach (var p in utils.GetAllRotations(polygon).Select(x => utils.NormalizePolygon(x)))
			{
				var rotated = partitioner.GetRectangles(p);
				Assert.AreEqual(expected[0].Count, rotated.Count);
			}
		}

		[Test]
		public void GetPolygons_LShape()
		{
			var polygon = new GridPolygonBuilder()
				.AddPoint(0, 0)
				.AddPoint(0, 6)
				.AddPoint(3, 6)
				.AddPoint(3, 3)
				.AddPoint(7, 3)
				.AddPoint(7, 0)
				.Build();

			var partitions = partitioner.GetRectangles(polygon);
			var expected = new List<List<GridRectangle>>()
			{
				new List<GridRectangle>()
				{
					new GridRectangle(new IntVector2(0, 3), new IntVector2(3, 6)),
					new GridRectangle(new IntVector2(0, 0), new IntVector2(7, 3)),
				},
				new List<GridRectangle>()
				{
					new GridRectangle(new IntVector2(0, 0), new IntVector2(3, 6)),
					new GridRectangle(new IntVector2(3, 0), new IntVector2(7, 3)),
				}
			};

			var matched = false;

			foreach (var rectangles in expected)
			{
				foreach (var r in rectangles)
				{
					if (!partitions.Contains(r))
					{
						break;
					}
				}

				matched = true;
			}

			Assert.AreEqual(expected[0].Count, partitions.Count);
			Assert.AreEqual(true, matched);

			foreach (var p in utils.GetAllRotations(polygon).Select(x => utils.NormalizePolygon(x)))
			{
				var rotated = partitioner.GetRectangles(p);
				Assert.AreEqual(expected[0].Count, rotated.Count);
			}
		}

		[Test]
		public void VertexCover_Basic()
		{
			{
				var cover = partitioner.BipartiteVertexCover(2, 3, new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(0, 2),
					new Tuple<int, int>(0, 3),
					new Tuple<int, int>(1, 3),
					new Tuple<int, int>(1, 4),
				});

				Assert.AreEqual(2, cover.Item1.Count);
				Assert.AreEqual(0, cover.Item2.Count);
			}

			{
				var cover = partitioner.BipartiteVertexCover(4, 2, new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(0, 4),
					new Tuple<int, int>(1, 4),
					new Tuple<int, int>(1, 5),
					new Tuple<int, int>(2, 5),
					new Tuple<int, int>(3, 5),
				});

				Assert.AreEqual(0, cover.Item1.Count);
				Assert.AreEqual(2, cover.Item2.Count);
			}
		}

		[Test]
		public void IndependentSet_Basic()
		{
			{
				var cover = partitioner.BipartiteIndependentSet(2, 3, new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(0, 2),
					new Tuple<int, int>(0, 3),
					new Tuple<int, int>(1, 3),
					new Tuple<int, int>(1, 4),
				});

				Assert.AreEqual(3, cover.Count);
			}

			{
				var cover = partitioner.BipartiteIndependentSet(4, 2, new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(0, 4),
					new Tuple<int, int>(1, 4),
					new Tuple<int, int>(1, 5),
					new Tuple<int, int>(2, 5),
					new Tuple<int, int>(3, 5),
				});

				Assert.AreEqual(4, cover.Count);
			}
		}
	}
}