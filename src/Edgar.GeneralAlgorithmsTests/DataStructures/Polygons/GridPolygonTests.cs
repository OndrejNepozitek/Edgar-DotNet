﻿using Edgar.Geometry;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace GeneralAlgorithms.Tests.DataStructures.Polygons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class GridPolygonTests
    {
        [Test]
        public void Equals_WhenEqual_ReturnsTrue()
        {
            {
                var p1 = new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 3)
                    .AddPoint(3, 3)
                    .AddPoint(3, 0)
                    .Build();

                var p2 = new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 3)
                    .AddPoint(3, 3)
                    .AddPoint(3, 0)
                    .Build();

                Assert.IsTrue(p1.Equals(p2));
            }
        }

        [Test]
        public void Equals_WhenNotEqual_ReturnsFalse()
        {
            {
                var p1 = new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 3)
                    .AddPoint(3, 3)
                    .AddPoint(3, 0)
                    .Build();

                var p2 = new PolygonGrid2DBuilder()
                    .AddPoint(3, 0)
                    .AddPoint(0, 0)
                    .AddPoint(0, 3)
                    .AddPoint(3, 3)
                    .Build();

                Assert.IsFalse(p1.Equals(p2));
            }
        }

        [Test]
        public void Scale_ReturnsScaled()
        {
            {
                var p = PolygonGrid2D.GetSquare(3);
                var factor = new Vector2Int(2, 3);

                var scaled = p.Scale(factor);
                var expected = PolygonGrid2D.GetRectangle(6, 9);

                Assert.AreEqual(expected, scaled);
            }

            {
                /*var p = GridPolygon.GetRectangle(2, 4);
                var factor = new IntVector2(-3, 5);

                var scaled = p.Scale(factor);
                var expected = GridPolygon.GetRectangle(-6, 20);

                Assert.AreEqual(expected, scaled);*/
            }
        }

        [Test]
        public void Plus_ReturnsTranslated()
        {
            {
                var p = PolygonGrid2D.GetSquare(3);
                var amount = new Vector2Int(2, 3);

                var translated = p + amount;
                var expected = new PolygonGrid2D(p.GetPoints().Select(x => x + amount));

                Assert.AreEqual(expected, translated);
            }

            {
                var p = PolygonGrid2D.GetRectangle(2, 4);
                var amount = new Vector2Int(-3, 5);

                var translated = p + amount;
                var expected = new PolygonGrid2D(p.GetPoints().Select(x => x + amount));

                Assert.AreEqual(expected, translated);
            }
        }

        [Test]
        public void BoudingRectangle_ReturnsBoundingBox()
        {
            {
                var p = PolygonGrid2D.GetRectangle(2, 4);

                var boundingRectangle = p.BoundingRectangle;
                var expected = new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(2, 4));

                Assert.AreEqual(expected, boundingRectangle);
            }

            {
                var p = new PolygonGrid2DBuilder()
                    .AddPoint(0, 0)
                    .AddPoint(0, 6)
                    .AddPoint(3, 6)
                    .AddPoint(3, 3)
                    .AddPoint(7, 3)
                    .AddPoint(7, 0)
                    .Build();

                var boundingRectangle = p.BoundingRectangle;
                var expected = new RectangleGrid2D(new Vector2Int(0, 0), new Vector2Int(7, 6));

                Assert.AreEqual(expected, boundingRectangle);
            }
        }

        [Test]
        public void Rotate_Square_ReturnsRotated()
        {
            var square = PolygonGrid2D.GetSquare(4);
            var rotatedSquare = square.Rotate(180);
            var expectedPoints = new List<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, -4),
                new Vector2Int(-4, -4),
                new Vector2Int(-4, 0),
            };

            Assert.IsTrue(expectedPoints.SequenceEqual(rotatedSquare.GetPoints()));
        }

        [Test]
        public void Rotate_Rectangle_ReturnsRotated()
        {
            var polygon = PolygonGrid2D.GetRectangle(2, 5);
            var rotatedPolygon = polygon.Rotate(270);
            var expectedPoints = new List<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(-5, 0),
                new Vector2Int(-5, 2),
                new Vector2Int(0, 2),
            };

            Assert.IsTrue(expectedPoints.SequenceEqual(rotatedPolygon.GetPoints()));
        }

        [Test]
        public void Constructor_ValidPolygons()
        {
            var square = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 3)
                .AddPoint(3, 3)
                .AddPoint(3, 0);

            var lPolygon = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 6)
                .AddPoint(3, 6)
                .AddPoint(3, 3)
                .AddPoint(6, 3)
                .AddPoint(6, 0);

            Assert.DoesNotThrow(() => square.Build());
            Assert.DoesNotThrow(() => lPolygon.Build());
        }

        [Test]
        public void Constructor_TooFewPoints_Throws()
        {
            var polygon1 = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 5);

            var polygon2 = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 5)
                .AddPoint(5, 5);

            Assert.Throws<ArgumentException>(() => polygon1.Build());
            Assert.Throws<ArgumentException>(() => polygon2.Build());
        }

        [Test]
        public void Constructor_EdgesNotOrthogonal_Trows()
        {
            var polygon1 = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(0, 5)
                .AddPoint(3, 4)
                .AddPoint(4, 0);

            Assert.Throws<ArgumentException>(() => polygon1.Build());
        }

        [Test]
        public void Constructor_OverlappingEdges_Throws()
        {
            var polygon1 = new PolygonGrid2DBuilder()
                .AddPoint(0, 0)
                .AddPoint(3, 0)
                .AddPoint(3, 2)
                .AddPoint(0, 2)
                .AddPoint(0, 4);

            Assert.Throws<ArgumentException>(() => polygon1.Build());
        }

        [Test]
        public void Constructor_CounterClockwise_Throws()
        {
            {
                // Square
                var p = new PolygonGrid2DBuilder()
                    .AddPoint(3, 0)
                    .AddPoint(3, 3)
                    .AddPoint(0, 3)
                    .AddPoint(0, 0);

                Assert.Throws<ArgumentException>(() => p.Build());
            }

            {
                // L Shape
                var p = new PolygonGrid2DBuilder()
                    .AddPoint(6, 0)
                    .AddPoint(6, 3)
                    .AddPoint(3, 3)
                    .AddPoint(3, 6)
                    .AddPoint(0, 6)
                    .AddPoint(0, 0);

                Assert.Throws<ArgumentException>(() => p.Build());
            }
        }

        [Test]
        public void Transform_ReturnsTransformed()
        {
            var polygon = PolygonGrid2D.GetRectangle(2, 1);

            {
                var transformed = polygon.Transform(TransformationGrid2D.Identity);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 0)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.Rotate90);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, -2),
                    new Vector2Int(0, -2)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.Rotate180);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(-2, -1),
                    new Vector2Int(-2, 0)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.Rotate270);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(-1, 2),
                    new Vector2Int(0, 2)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.MirrorX);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(2, -1),
                    new Vector2Int(0, -1)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.MirrorY);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-2, 0),
                    new Vector2Int(-2, 1),
                    new Vector2Int(0, 1)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.Diagonal13);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(1, 0)
                }));
            }

            {
                var transformed = polygon.Transform(TransformationGrid2D.Diagonal24);
                Assert.That(transformed.GetPoints(), Is.EquivalentTo(new List<Vector2Int>()
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, -2),
                    new Vector2Int(-1, -2),
                    new Vector2Int(-1, 0)
                }));
            }
        }

        [Test]
        public void GetAllTransformations_ReturnsCorrectCount()
        {
            var square = PolygonGrid2D.GetSquare(1);
            var rectangle = PolygonGrid2D.GetRectangle(1, 2);

            var transformedSquares = square.GetAllTransformations();
            var transformedRectangles = rectangle.GetAllTransformations();

            Assert.That(transformedSquares.Count(), Is.EqualTo(8));
            Assert.That(transformedRectangles.Count(), Is.EqualTo(8));
        }
    }
}