using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace GeneralAlgorithms.Tests.Algorithms.Polygons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var polygon = new PolygonGrid2DBuilder()
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

            var partitions = partitioner.GetPartitions(polygon);
            var expected = new List<List<RectangleGrid2D>>()
            {
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(2, 0), new Vector2Int(4, 2)),
                    new RectangleGrid2D(new Vector2Int(0, 2), new Vector2Int(6, 4)),
                    new RectangleGrid2D(new Vector2Int(2, 4), new Vector2Int(4, 6)),
                },
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(0, 2), new Vector2Int(2, 4)),
                    new RectangleGrid2D(new Vector2Int(2, 0), new Vector2Int(4, 6)),
                    new RectangleGrid2D(new Vector2Int(4, 2), new Vector2Int(6, 4)),
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

            foreach (var p in polygon.GetAllRotations().Select(x => utils.NormalizePolygon(x)))
            {
                var rotated = partitioner.GetPartitions(p);
                Assert.AreEqual(expected[0].Count, rotated.Count);
            }
        }

        [Test]
        public void GetPolygons_LShape()
        {
            var polygon = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 6)
                .AddPoint(3, 6)
                .AddPoint(3, 3)
                .AddPoint(7, 3)
                .AddPoint(7, 0)
                .Build();

            var partitions = partitioner.GetPartitions(polygon);
            var expected = new List<List<RectangleGrid2D>>()
            {
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(0, 3), new Vector2Int(3, 6)),
                    new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(7, 3)),
                },
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(3, 6)),
                    new RectangleGrid2D(new Vector2Int(3, 0), new Vector2Int(7, 3)),
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

            foreach (var p in polygon.GetAllRotations().Select(x => utils.NormalizePolygon(x)))
            {
                var rotated = partitioner.GetPartitions(p);
                Assert.AreEqual(expected[0].Count, rotated.Count);
            }
        }

        [Test]
        public void GetPolygons_AnotherShape()
        {
            var polygon = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 3)
                .AddPoint(-1, 3)
                .AddPoint(-1, 5)
                .AddPoint(5, 5)
                .AddPoint(5, 3)
                .AddPoint(4, 3)
                .AddPoint(4, 0)
                .AddPoint(5, 0)
                .AddPoint(5, -2)
                .AddPoint(-1, -2)
                .AddPoint(-1, 0)
                .Build();

            var partitions = partitioner.GetPartitions(polygon);
            var expected = new List<List<RectangleGrid2D>>()
            {
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(-1, -2), new Vector2Int(5, 0)),
                    new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(4, 3)),
                    new RectangleGrid2D(new Vector2Int(-1, 3), new Vector2Int(5, 5)),
                },
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

            foreach (var p in polygon.GetAllRotations().Select(x => utils.NormalizePolygon(x)))
            {
                var rotated = partitioner.GetPartitions(p);
                Assert.AreEqual(expected[0].Count, rotated.Count);
            }
        }

        [Test]
        public void GetPolygons_ComplexShape()
        {
            var polygon = new PolygonGrid2DBuilder()
                .AddPoint(2, 0)
                .AddPoint(2, 1)
                .AddPoint(1, 1)
                .AddPoint(1, 2)
                .AddPoint(0, 2)
                .AddPoint(0, 7)
                .AddPoint(1, 7)
                .AddPoint(1, 8)
                .AddPoint(2, 8)
                .AddPoint(2, 9)
                .AddPoint(7, 9)
                .AddPoint(7, 8)
                .AddPoint(8, 8)
                .AddPoint(8, 7)
                .AddPoint(9, 7)
                .AddPoint(9, 2)
                .AddPoint(8, 2)
                .AddPoint(8, 1)
                .AddPoint(7, 1)
                .AddPoint(7, 0)
                .Build();

            var partitions = partitioner.GetPartitions(polygon);
            var expected = new List<List<RectangleGrid2D>>()
            {
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(2, 0), new Vector2Int(7, 1)),
                    new RectangleGrid2D(new Vector2Int(1, 1), new Vector2Int(8, 2)),
                    new RectangleGrid2D(new Vector2Int(0, 2), new Vector2Int(9, 7)),
                    new RectangleGrid2D(new Vector2Int(1, 7), new Vector2Int(8, 8)),
                    new RectangleGrid2D(new Vector2Int(2, 8), new Vector2Int(7, 9)),
                },
                new List<RectangleGrid2D>()
                {
                    new RectangleGrid2D(new Vector2Int(0, 2), new Vector2Int(1, 7)),
                    new RectangleGrid2D(new Vector2Int(1, 1), new Vector2Int(2, 8)),
                    new RectangleGrid2D(new Vector2Int(2, 0), new Vector2Int(7, 9)),
                    new RectangleGrid2D(new Vector2Int(7, 1), new Vector2Int(8, 8)),
                    new RectangleGrid2D(new Vector2Int(8, 2), new Vector2Int(9, 7)),
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

            foreach (var p in polygon.GetAllRotations().Select(x => utils.NormalizePolygon(x)))
            {
                var rotated = partitioner.GetPartitions(p);
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