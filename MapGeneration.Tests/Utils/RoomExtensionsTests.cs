using System.Collections.Generic;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Core.MapLayouts;
using MapGeneration.Utils;
using NUnit.Framework;

namespace MapGeneration.Tests.Utils
{
    [TestFixture]
    public class RoomExtensionsTests
    {
        [Test]
        public void TransformPointToNewPosition_Identity()
        {
            var room = GetRoom(new IntVector2(0, 0), new IntVector2(0, 0), Transformation.Identity, null);

            var point = new IntVector2(5, 5);
            var transformedPoint = room.TransformPointToNewPosition(point);

            Assert.That(transformedPoint, Is.EqualTo(new IntVector2(5, 5)));
        }

        [Test]
        public void TransformPointToNewPosition_Rotate90()
        {
            var room = GetRoom(new IntVector2(0, 0), new IntVector2(0, 0), Transformation.Rotate90, null);

            var point = new IntVector2(10, 5);
            var transformedPoint = room.TransformPointToNewPosition(point);

            Assert.That(transformedPoint, Is.EqualTo(new IntVector2(5, 0)));
        }

        [Test]
        public void TransformPointToNewPosition_Identity_WithPosition()
        {
            var room = GetRoom(new IntVector2(10, 10), new IntVector2(0, 0), Transformation.Identity, null);

            var point = new IntVector2(5, 5);
            var transformedPoint = room.TransformPointToNewPosition(point);

            Assert.That(transformedPoint, Is.EqualTo(new IntVector2(15, 15)));
        }

        [Test]
        public void TransformPointToNewPosition_Rotate180_WithPosition()
        {
            var room = GetRoom(new IntVector2(10, 10), new IntVector2(0, 0), Transformation.Rotate180, null);

            var point = new IntVector2(10, 5);
            var transformedPoint = room.TransformPointToNewPosition(point);

            Assert.That(transformedPoint, Is.EqualTo(new IntVector2(10, 15)));
        }

        [Test]
        public void TransformPointToNewPosition_Rotate180_WithPosition_WithOrigin()
        {
            var room = GetRoom(new IntVector2(10, 10), new IntVector2(7, 10), Transformation.Rotate180, null);

            var point = new IntVector2(10, 5);
            var transformedPoint = room.TransformPointToNewPosition(point);

            Assert.That(transformedPoint, Is.EqualTo(new IntVector2(17, 25)));
        }

        private Room<int> GetRoom(IntVector2 position, IntVector2 roomShapeOrigin, Transformation transformation, List<Transformation> transformations)
        {
            return new Room<int>(0,
                GridPolygon.GetSquare(10),
                position,
                false,
                new RoomDescription(GridPolygon.GetSquare(10) + roomShapeOrigin, new OverlapMode(1, 0)),
                transformation,
                transformations);
        }
    }
}