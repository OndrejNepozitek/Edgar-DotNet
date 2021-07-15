using System;
using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Exceptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using NUnit.Framework;

namespace Edgar.Tests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class ManualDoorModeGrid2DTests
    {
        [Test]
        public void FromDoorLines_EquivalentToSimpleMode()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var simpleMode = new SimpleDoorModeGrid2D(2, 2);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 6)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(10, 2), new Vector2Int(10, 6)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(2, 0), new Vector2Int(6, 0)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(2, 10), new Vector2Int(6, 10)), 2, null, DoorType.Undirected),
            });

            var expected = simpleMode.GetDoors(roomShape);
            var actual = manualMode.GetDoors(roomShape);

            Assert.IsTrue(actual.SequenceEqualWithoutOrder(expected));
        }

        [Test]
        public void FromDoorLines_BackwardCompatible()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var simpleMode = new SimpleDoorModeGrid2D(2, 2);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 6)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(10, 2), new Vector2Int(10, 6)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(2, 0), new Vector2Int(6, 0)), 2, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(2, 10), new Vector2Int(6, 10)), 2, null, DoorType.Undirected),
            });

            var expected = simpleMode.GetDoors(roomShape);
            var actual = manualMode.GetDoors(roomShape);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void FromDoorLines_CornerDoorLine_ReturnsTwoDoorsLines()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0)), 0, null, DoorType.Undirected),
            });

            var expected = new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0),
                        OrthogonalLineGrid2D.Direction.Top), 0, null, DoorType.Undirected),
                new DoorLineGrid2D(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0),
                        OrthogonalLineGrid2D.Direction.Left), 0, null, DoorType.Undirected),
            };
            var actual = manualMode.GetDoors(roomShape);

            Assert.IsTrue(actual.SequenceEqualWithoutOrder(expected));
        }

        [Test]
        public void FromDoorLines_OverlappingDoorLines_Throws()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 2)), 1, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 4)), 1, null, DoorType.Undirected),
            });

            Assert.Throws<DuplicateDoorPositionException>(() => manualMode.GetDoors(roomShape));
        }

        [Test]
        public void FromDoorLines_OverlappingDoorLines2_Throws()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 2)), 1, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 1), new Vector2Int(0, 2)), 1, null, DoorType.Undirected),
            });

            Assert.Throws<DuplicateDoorPositionException>(() => manualMode.GetDoors(roomShape));
        }

        [Test]
        public void FromDoorLines_DuplicateDoorLines_Throws()
        {
            var roomShape = PolygonGrid2D.GetSquare(10);
            var manualMode = new ManualDoorModeGrid2D(new List<DoorLineGrid2D>()
            {
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 4)), 1, null, DoorType.Undirected),
                new DoorLineGrid2D(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 4)), 1, null, DoorType.Undirected),
            });

            Assert.Throws<DuplicateDoorPositionException>(() => manualMode.GetDoors(roomShape));
        }
    }
}