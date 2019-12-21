using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.Algorithms.Polygons;
using GeneralAlgorithms.DataStructures.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.ConfigurationSpaces;
using MapGeneration.Core.Doors;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.Doors;
using NUnit.Framework;

namespace MapGeneration.Tests.Core.ConfigurationSpaces
{
    [TestFixture]
    public class ConfigurationSpacesGeneratorTests
    {
        private ConfigurationSpacesGenerator generator;

        [SetUp]
        public void SetUp()
        {
            generator = new ConfigurationSpacesGenerator(
                new PolygonOverlap(),
                DoorHandler.DefaultHandler,
                new OrthogonalLineIntersection(),
                new GridPolygonUtils());
        }

        [Test]
        public void GetRoomTemplateInstances_SquareIdentity_ReturnsOneInstance()
        {
            var roomShape = GridPolygon.GetSquare(10);
            var doorsMode = new OverlapMode(1, 0);
            var transformations = new List<Transformation>() { Transformation.Identity };

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(1));
            Assert.That(instances[0].RoomTemplate, Is.EqualTo(roomTemplate));
            Assert.That(instances[0].RoomShape, Is.EqualTo(roomShape));
            Assert.That(instances[0].Transformations, Is.EquivalentTo(transformations));
        }

        [Test]
        public void GetRoomTemplateInstances_SquareNotNormalized_ReturnsNormalizedInstance()
        {
            var roomShapeNormalized = GridPolygon.GetSquare(10);
            var roomShape = roomShapeNormalized + new IntVector2(5, 5);

            var doorsMode = new OverlapMode(1, 0);
            var transformations = new List<Transformation>() { Transformation.Identity };

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(1));
            Assert.That(instances[0].RoomTemplate, Is.EqualTo(roomTemplate));
            Assert.That(instances[0].RoomShape, Is.EqualTo(roomShapeNormalized));
            Assert.That(instances[0].Transformations, Is.EquivalentTo(transformations));
        }

        [Test]
        public void GetRoomTemplateInstances_SquareAllTransformations_ReturnsOneInstance()
        {
            var roomShape = GridPolygon.GetSquare(10);
            var doorsMode = new OverlapMode(1, 0);
            var transformations = ((Transformation[]) Enum.GetValues(typeof(Transformation))).ToList();

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(1));
            Assert.That(instances[0].RoomTemplate, Is.EqualTo(roomTemplate));
            Assert.That(instances[0].RoomShape, Is.EqualTo(roomShape));
            Assert.That(instances[0].Transformations, Is.EquivalentTo(transformations));
        }

        [Test]
        public void GetRoomTemplateInstances_SquareAllRotationsOneDoor_ReturnsFourInstance()
        {
            var roomShape = GridPolygon.GetSquare(10);
            var doorsMode = new SpecificPositionsMode(new List<OrthogonalLine>() { new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0))});
            var transformations = new List<Transformation>() { Transformation.Identity, Transformation.Rotate90, Transformation.Rotate180, Transformation.Rotate270 };

            var expectedDoorPositions = new Dictionary<Transformation, IDoorLine>()
            {
                { Transformation.Identity, new DoorLine(new OrthogonalLine(new IntVector2(1, 0), new IntVector2(1, 0)), 1) },
                { Transformation.Rotate90, new DoorLine(new OrthogonalLine(new IntVector2(0, 9), new IntVector2(0, 9)), 1) },
                { Transformation.Rotate180, new DoorLine(new OrthogonalLine(new IntVector2(9, 10), new IntVector2(9, 10)), 1) },
                { Transformation.Rotate270, new DoorLine(new OrthogonalLine(new IntVector2(10, 1), new IntVector2(10, 1)), 1) },
            };

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(4));

            foreach (var instance in instances)
            {
                Assert.That(instance.RoomShape, Is.EqualTo(roomShape));
                Assert.That(instance.Transformations.Count, Is.EqualTo(1));

                var transformation = instance.Transformations[0];

                Assert.That(instance.DoorLines.Count, Is.EqualTo(1));
                Assert.That(instance.DoorLines[0].Length, Is.EqualTo(expectedDoorPositions[transformation].Length));
                Assert.That(instance.DoorLines[0].Line, Is.EqualTo(expectedDoorPositions[transformation].Line));
            }
        }

        [Test]
        public void GetRoomTemplateInstances_SquareAllTransformationsOneDoor_ReturnsFourInstance()
        {
            var roomShape = GridPolygon.GetSquare(10);
            var doorsMode = new SpecificPositionsMode(new List<OrthogonalLine>() { new OrthogonalLine(new IntVector2(0, 0), new IntVector2(1, 0)) });
            var transformations = TransformationHelper.GetAllTransformations().ToList();

            var expectedDoorPositions = new Dictionary<Transformation, IDoorLine>()
            {
                { Transformation.Identity, new DoorLine(new OrthogonalLine(new IntVector2(1, 0), new IntVector2(1, 0)), 1) },
                { Transformation.Rotate90, new DoorLine(new OrthogonalLine(new IntVector2(0, 9), new IntVector2(0, 9)), 1) },
                { Transformation.Rotate180, new DoorLine(new OrthogonalLine(new IntVector2(9, 10), new IntVector2(9, 10)), 1) },
                { Transformation.Rotate270, new DoorLine(new OrthogonalLine(new IntVector2(10, 1), new IntVector2(10, 1)), 1) },

                { Transformation.MirrorY, new DoorLine(new OrthogonalLine(new IntVector2(10, 0), new IntVector2(10, 0)), 1) },
                { Transformation.MirrorX, new DoorLine(new OrthogonalLine(new IntVector2(0, 10), new IntVector2(0, 10)), 1) },
                { Transformation.Diagonal13, new DoorLine(new OrthogonalLine(new IntVector2(0, 0), new IntVector2(0, 0)), 1) },
                { Transformation.Diagonal24, new DoorLine(new OrthogonalLine(new IntVector2(10, 10), new IntVector2(10, 10)), 1) },
            };

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(8));

            foreach (var instance in instances)
            {
                Assert.That(instance.RoomShape, Is.EqualTo(roomShape));
                Assert.That(instance.Transformations.Count, Is.EqualTo(1));

                var transformation = instance.Transformations[0];

                Assert.That(instance.DoorLines.Count, Is.EqualTo(1));
                Assert.That(instance.DoorLines[0].Length, Is.EqualTo(expectedDoorPositions[transformation].Length));
                Assert.That(instance.DoorLines[0].Line, Is.EqualTo(expectedDoorPositions[transformation].Line));
            }
        }

        [Test]
        public void GetRoomTemplateInstances_RectangleAllRotations_ReturnsTwoInstances()
        {
            var roomShape = GridPolygon.GetRectangle(5, 10);

            var doorsMode = new OverlapMode(1, 0);
            var transformations = new List<Transformation>() { Transformation.Identity, Transformation.Rotate90, Transformation.Rotate180, Transformation.Rotate270 };

            var roomTemplate = new RoomTemplate(roomShape, doorsMode, transformations);
            var instances = generator.GetRoomTemplateInstances(roomTemplate);

            Assert.That(instances.Count, Is.EqualTo(2));

            var wideRectangle = instances[0];
            var tallRectangle = instances[1];

            if (wideRectangle.RoomShape.Equals(roomShape))
            {
                wideRectangle = instances[1];
                tallRectangle = instances[0];
            }

            Assert.That(tallRectangle.RoomShape, Is.EqualTo(GridPolygon.GetRectangle(5, 10)));
            Assert.That(wideRectangle.RoomShape, Is.EqualTo(GridPolygon.GetRectangle(10, 5)));
        }
    }
}