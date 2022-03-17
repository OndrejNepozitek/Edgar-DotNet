using Edgar.Geometry;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;

namespace MapGeneration.Tests.Core.Doors
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class SpecificPositionsModeHandlerTests
    {
        private ManualDoorModeHandler overlapModeHandler;

        [SetUp]
        public void SetUp()
        {
            overlapModeHandler = new ManualDoorModeHandler();
        }

        [Test]
        public void Rectangle_LengthZeroCorners()
        {
            var polygon = PolygonGrid2D.GetRectangle(3, 5);
            var mode = new ManualDoorMode(new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0)),
                new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(0, 5)),
                new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 5)),
                new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 0)),
            });
            var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);

            var expectedPositions = new List<DoorLine>()
            {
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0),
                        OrthogonalLineGrid2D.Direction.Left), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 0),
                        OrthogonalLineGrid2D.Direction.Top), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(0, 5),
                        OrthogonalLineGrid2D.Direction.Top), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(0, 5),
                        OrthogonalLineGrid2D.Direction.Right), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 5),
                        OrthogonalLineGrid2D.Direction.Right), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(3, 5), new Vector2Int(3, 5),
                        OrthogonalLineGrid2D.Direction.Bottom), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 0),
                        OrthogonalLineGrid2D.Direction.Bottom), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 0),
                        OrthogonalLineGrid2D.Direction.Left), 0),
            };

            Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
        }

        [Test]
        public void Rectangle_LengthZeroInside()
        {
            var polygon = PolygonGrid2D.GetRectangle(3, 5);
            var mode = new ManualDoorMode(new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(0, 1), new Vector2Int(0, 1)),
                new OrthogonalLineGrid2D(new Vector2Int(1, 5), new Vector2Int(1, 5)),
                new OrthogonalLineGrid2D(new Vector2Int(3, 4), new Vector2Int(3, 4)),
                new OrthogonalLineGrid2D(new Vector2Int(2, 0), new Vector2Int(2, 0)),
            });
            var doorPositions = overlapModeHandler.GetDoorPositions(polygon, mode);

            var expectedPositions = new List<DoorLine>()
            {
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(0, 1), new Vector2Int(0, 1),
                        OrthogonalLineGrid2D.Direction.Top), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(1, 5), new Vector2Int(1, 5),
                        OrthogonalLineGrid2D.Direction.Right), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(3, 4), new Vector2Int(3, 4),
                        OrthogonalLineGrid2D.Direction.Bottom), 0),
                new DoorLine(
                    new OrthogonalLineGrid2D(new Vector2Int(2, 0), new Vector2Int(2, 0),
                        OrthogonalLineGrid2D.Direction.Left), 0),
            };

            Assert.IsTrue(doorPositions.SequenceEqualWithoutOrder(expectedPositions));
        }
    }
}