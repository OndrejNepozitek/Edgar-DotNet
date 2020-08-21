using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.ConfigurationSpaces;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
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
            var roomShape = PolygonGrid2D.GetSquare(10);
            var doorsMode = new SimpleDoorMode(1, 0);
            var transformations = new List<TransformationGrid2D>() { TransformationGrid2D.Identity };

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
            var roomShapeNormalized = PolygonGrid2D.GetSquare(10);
            var roomShape = roomShapeNormalized + new Vector2Int(5, 5);

            var doorsMode = new SimpleDoorMode(1, 0);
            var transformations = new List<TransformationGrid2D>() { TransformationGrid2D.Identity };

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
            var roomShape = PolygonGrid2D.GetSquare(10);
            var doorsMode = new SimpleDoorMode(1, 0);
            var transformations = ((TransformationGrid2D[]) Enum.GetValues(typeof(TransformationGrid2D))).ToList();

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
            var roomShape = PolygonGrid2D.GetSquare(10);
            var doorsMode = new ManualDoorMode(new List<OrthogonalLine>() { new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0))});
            var transformations = new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90, TransformationGrid2D.Rotate180, TransformationGrid2D.Rotate270 };

            var expectedDoorPositions = new Dictionary<TransformationGrid2D, DoorLine>()
            {
                { TransformationGrid2D.Identity, new DoorLine(new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(1, 0)), 1) },
                { TransformationGrid2D.Rotate90, new DoorLine(new OrthogonalLine(new Vector2Int(0, 9), new Vector2Int(0, 9)), 1) },
                { TransformationGrid2D.Rotate180, new DoorLine(new OrthogonalLine(new Vector2Int(9, 10), new Vector2Int(9, 10)), 1) },
                { TransformationGrid2D.Rotate270, new DoorLine(new OrthogonalLine(new Vector2Int(10, 1), new Vector2Int(10, 1)), 1) },
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
            var roomShape = PolygonGrid2D.GetSquare(10);
            var doorsMode = new ManualDoorMode(new List<OrthogonalLine>() { new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)) });
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var expectedDoorPositions = new Dictionary<TransformationGrid2D, DoorLine>()
            {
                { TransformationGrid2D.Identity, new DoorLine(new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(1, 0)), 1) },
                { TransformationGrid2D.Rotate90, new DoorLine(new OrthogonalLine(new Vector2Int(0, 9), new Vector2Int(0, 9)), 1) },
                { TransformationGrid2D.Rotate180, new DoorLine(new OrthogonalLine(new Vector2Int(9, 10), new Vector2Int(9, 10)), 1) },
                { TransformationGrid2D.Rotate270, new DoorLine(new OrthogonalLine(new Vector2Int(10, 1), new Vector2Int(10, 1)), 1) },

                { TransformationGrid2D.MirrorY, new DoorLine(new OrthogonalLine(new Vector2Int(10, 0), new Vector2Int(10, 0)), 1) },
                { TransformationGrid2D.MirrorX, new DoorLine(new OrthogonalLine(new Vector2Int(0, 10), new Vector2Int(0, 10)), 1) },
                { TransformationGrid2D.Diagonal13, new DoorLine(new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, 0)), 1) },
                { TransformationGrid2D.Diagonal24, new DoorLine(new OrthogonalLine(new Vector2Int(10, 10), new Vector2Int(10, 10)), 1) },
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
            var roomShape = PolygonGrid2D.GetRectangle(5, 10);

            var doorsMode = new SimpleDoorMode(1, 0);
            var transformations = new List<TransformationGrid2D>() { TransformationGrid2D.Identity, TransformationGrid2D.Rotate90, TransformationGrid2D.Rotate180, TransformationGrid2D.Rotate270 };

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

            Assert.That(tallRectangle.RoomShape, Is.EqualTo(PolygonGrid2D.GetRectangle(5, 10)));
            Assert.That(wideRectangle.RoomShape, Is.EqualTo(PolygonGrid2D.GetRectangle(10, 5)));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomVerticalCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new SimpleDoorMode(1, 0);

            var corridor = PolygonGrid2D.GetRectangle(1, 2);
            var corridorDoorsMode = new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(0, 0)),
                new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(1, 2)),
            });

            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-4, 7), new Vector2Int(4, 7)),
                new OrthogonalLine(new Vector2Int(-4, -7), new Vector2Int(4, -7)),
            };

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridor, corridorDoorsMode);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedLines.SelectMany(x => x.GetPoints())));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomHorizontalCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new SimpleDoorMode(1, 0);

            var corridor = PolygonGrid2D.GetRectangle(2, 1);
            var corridorDoorsMode = new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 1), new Vector2Int(0, 0)),
                new OrthogonalLine(new Vector2Int(2, 0), new Vector2Int(2, 1)),
            });

            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, -4), new Vector2Int(-7, 4)),
                new OrthogonalLine(new Vector2Int(7, -4), new Vector2Int(7, 4)),
            };

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridor, corridorDoorsMode);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedLines.SelectMany(x => x.GetPoints())));
        }

        [Test]
        [Ignore("The configuration spaces generator currently does not check whether the corridor does not overlap do moving polygon")]
        public void GetConfigurationSpaceOverCorridor_RoomShapesThatCannotBeCorrectlyConnected()
        {
            var roomShape1 = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode1 = new SimpleDoorMode(1, 0);

            var roomShape2 = new PolygonGrid2DBuilder()
                .AddPoint(0, 1)
                .AddPoint(0, 2)
                .AddPoint(2, 2)
                .AddPoint(2, 0)
                .AddPoint(1, 0)
                .AddPoint(1, 1)
                .Build();
            var roomDoorsMode2 = new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(1, 1), new Vector2Int(0, 1)),
            });

            var corridor = PolygonGrid2D.GetSquare(2);
            var corridorDoorsMode = new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)),
                new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(1, 2)),
            });

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape2, roomDoorsMode2, roomShape1,
                roomDoorsMode1, corridor, corridorDoorsMode);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.Empty);
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomHorizontalVerticalCorridors()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var basicRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(5), new SimpleDoorMode(1, 0), transformations);
            var basicRoomTemplateInstance = generator.GetRoomTemplateInstances(basicRoomTemplate).First();

            var corridorRoomTemplate = new RoomTemplate(PolygonGrid2D.GetRectangle(2, 1), new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 1), new Vector2Int(0, 0)),
                new OrthogonalLine(new Vector2Int(2, 0), new Vector2Int(2, 1)),
            }), transformations);
            var corridorRoomTemplateInstances = generator.GetRoomTemplateInstances(corridorRoomTemplate);
            
            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, -4), new Vector2Int(-7, 4)),
                new OrthogonalLine(new Vector2Int(7, -4), new Vector2Int(7, 4)),
                new OrthogonalLine(new Vector2Int(-4, 7), new Vector2Int(4, 7)),
                new OrthogonalLine(new Vector2Int(-4, -7), new Vector2Int(4, -7)),
            };

            var configurationSpace = generator.GetConfigurationSpaceOverCorridors(basicRoomTemplateInstance,
                basicRoomTemplateInstance, corridorRoomTemplateInstances);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedLines.SelectMany(x => x.GetPoints())));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomSquareCorridorLengthZero()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var basicRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(5), new SimpleDoorMode(0, 0), transformations);
            var basicRoomTemplateInstance = generator.GetRoomTemplateInstances(basicRoomTemplate).First();

            var corridorRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(2), new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(1, 0)),
                new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(1, 2)),
            }), transformations);
            var corridorRoomTemplateInstances = generator.GetRoomTemplateInstances(corridorRoomTemplate);
            
            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, -5), new Vector2Int(-7, 5)),
                new OrthogonalLine(new Vector2Int(7, -5), new Vector2Int(7, 5)),
                new OrthogonalLine(new Vector2Int(-5, 7), new Vector2Int(5, 7)),
                new OrthogonalLine(new Vector2Int(-5, -7), new Vector2Int(5, -7)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridors(basicRoomTemplateInstance,
                basicRoomTemplateInstance, corridorRoomTemplateInstances);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_DegeneratedLines()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var basicRoomTemplate = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 4), new SimpleDoorMode(0, 2), new List<TransformationGrid2D>() { TransformationGrid2D.Identity });
            var basicRoomTemplateInstance = generator.GetRoomTemplateInstances(basicRoomTemplate).First();

            var corridorRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(2), new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(1, 0)),
                new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(1, 2)),
            }), new List<TransformationGrid2D>() { TransformationGrid2D.Rotate90 });
            var corridorRoomTemplateInstances = generator.GetRoomTemplateInstances(corridorRoomTemplate);
            
            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, 0), new Vector2Int(-7, 0)),
                new OrthogonalLine(new Vector2Int(7, 0), new Vector2Int(7, 0)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridors(basicRoomTemplateInstance,
                basicRoomTemplateInstance, corridorRoomTemplateInstances);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomSquareCorridor()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var basicRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(5), new SimpleDoorMode(1, 0), transformations);
            var basicRoomTemplateInstance = generator.GetRoomTemplateInstances(basicRoomTemplate).First();

            var corridorRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(2), new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)),
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(2, 0)),
                new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(1, 2)),
                new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(2, 2)),
            }), transformations);
            var corridorRoomTemplateInstances = generator.GetRoomTemplateInstances(corridorRoomTemplate);
            
            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, -5), new Vector2Int(-7, 5)),
                new OrthogonalLine(new Vector2Int(7, -5), new Vector2Int(7, 5)),
                new OrthogonalLine(new Vector2Int(-5, 7), new Vector2Int(5, 7)),
                new OrthogonalLine(new Vector2Int(-5, -7), new Vector2Int(5, -7)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridors(basicRoomTemplateInstance,
                basicRoomTemplateInstance, corridorRoomTemplateInstances);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomSquareCorridorDifferentDoorLengths()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();

            var basicRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(5), new SimpleDoorMode(1, 0), transformations);
            var basicRoomTemplateInstance = generator.GetRoomTemplateInstances(basicRoomTemplate).First();

            var corridorRoomTemplate = new RoomTemplate(PolygonGrid2D.GetSquare(2), new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(1, 0)),
                new OrthogonalLine(new Vector2Int(1, 0), new Vector2Int(2, 0)),
                new OrthogonalLine(new Vector2Int(0, 2), new Vector2Int(1, 2)),
                new OrthogonalLine(new Vector2Int(1, 2), new Vector2Int(2, 2)),
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, 2)),
                new OrthogonalLine(new Vector2Int(2, 0), new Vector2Int(2, 2)),
            }), transformations);
            var corridorRoomTemplateInstances = generator.GetRoomTemplateInstances(corridorRoomTemplate);
            
            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-7, -5), new Vector2Int(-7, 5)),
                new OrthogonalLine(new Vector2Int(7, -5), new Vector2Int(7, 5)),
                new OrthogonalLine(new Vector2Int(-5, 7), new Vector2Int(5, 7)),
                new OrthogonalLine(new Vector2Int(-5, -7), new Vector2Int(5, -7)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridors(basicRoomTemplateInstance,
                basicRoomTemplateInstance, corridorRoomTemplateInstances);

            Assert.That(configurationSpace.Lines.SelectMany(x => x.GetPoints()), Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetConfigurationSpaceOverCorridor_SquareRoomLShapedCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new SimpleDoorMode(1, 0);

            var corridor = new PolygonGrid2DBuilder()
                .AddPoint(0, 1)
                .AddPoint(0, 2)
                .AddPoint(2, 2)
                .AddPoint(2, 0)
                .AddPoint(1, 0)
                .AddPoint(1, 1)
                .Build();

            var corridorDoorsMode = new ManualDoorMode(new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 1), new Vector2Int(0, 2)),
                new OrthogonalLine(new Vector2Int(2, 0), new Vector2Int(1, 0)),
            });

            var expectedLines = new List<OrthogonalLine>() {
                new OrthogonalLine(new Vector2Int(-6, 2), new Vector2Int(-6, 6)), // Left side
                new OrthogonalLine(new Vector2Int(-5, 2), new Vector2Int(-5, 6)),
                new OrthogonalLine(new Vector2Int(-6, 6), new Vector2Int(-2, 6)), // Top side
                new OrthogonalLine(new Vector2Int(-6, 5), new Vector2Int(-2, 5)),
                new OrthogonalLine(new Vector2Int(2, -6), new Vector2Int(6, -6)), // Bottom side
                new OrthogonalLine(new Vector2Int(2, -5), new Vector2Int(6, -5)),
                new OrthogonalLine(new Vector2Int(5, -2), new Vector2Int(5, -6)), // Right side
                new OrthogonalLine(new Vector2Int(6, -2), new Vector2Int(6, -6)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridor, corridorDoorsMode);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.EquivalentTo(expectedPoints));
        }
    }
}