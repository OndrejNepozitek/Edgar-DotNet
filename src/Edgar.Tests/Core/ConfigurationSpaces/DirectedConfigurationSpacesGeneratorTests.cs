using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using Edgar.GraphBasedGenerator.Grid2D.Internal;
using Edgar.Legacy.Core.Doors;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Polygons;
using NUnit.Framework;

namespace Edgar.Tests.Core.ConfigurationSpaces
{
    public class DirectedConfigurationSpacesGeneratorTests
    {
        private DirectedConfigurationSpacesGenerator generator;

        [SetUp]
        public void SetUp()
        {
            generator = new DirectedConfigurationSpacesGenerator(
                new PolygonOverlap(),
                new OrthogonalLineIntersection());
        }

        [Test]
        public void GetDirectedConfigurationSpace_SquareRoom_FromFixedToMoving()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), type: DoorType.Undirected),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            });

            var expectedLines = new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(5, 0), new Vector2Int(5, 0)),
                new OrthogonalLineGrid2D(new Vector2Int(0, 5), new Vector2Int(0, 5)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpace(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, ConfigurationSpaceDirection.FromFixedToMoving);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetDirectedConfigurationSpace_SquareRoom_FromMovingToFixed()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), type: DoorType.Undirected),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            });

            var expectedLines = new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(-5, 0), new Vector2Int(-5, 0)),
                new OrthogonalLineGrid2D(new Vector2Int(0, -5), new Vector2Int(0, -5)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpace(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, ConfigurationSpaceDirection.FromMovingToFixed);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.EquivalentTo(expectedPoints));
        }

        [Test]
        public void GetDirectedConfigurationSpaceOverCorridor_FromFixedToMoving_OneDirectionCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), type: DoorType.Undirected),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            });

            var corridorShape = PolygonGrid2D.GetRectangle(3, 1);
            var corridorDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 1), type: DoorType.Exit),
            });

            var expectedLines = new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(8, 0), new Vector2Int(8, 0)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridorShape, corridorDoorsMode, ConfigurationSpaceDirection.FromFixedToMoving);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.EquivalentTo(expectedPoints));
        }


        [Test]
        public void GetDirectedConfigurationSpaceOverCorridor_FromFixedToMoving_WrongDirectionCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), type: DoorType.Undirected),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            });

            var corridorShape = PolygonGrid2D.GetRectangle(3, 1);
            var corridorDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 1), type: DoorType.Entrance),
            });

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridorShape, corridorDoorsMode, ConfigurationSpaceDirection.FromFixedToMoving);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.Empty);
        }

        [Test]
        public void GetDirectedConfigurationSpaceOverCorridor_Vertical_FromFixedToMoving_AnyDirectionCorridor()
        {
            var roomShape = PolygonGrid2D.GetSquare(5);
            var roomDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), type: DoorType.Undirected), // Top
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Undirected), // Bottom
            });

            var corridorShape = PolygonGrid2D.GetRectangle(1, 3);
            var corridorDoorsMode = new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(1, 0), type: DoorType.Undirected),
                new DoorGrid2D(new Vector2Int(0, 3), new Vector2Int(1, 3), type: DoorType.Undirected),
            });

            var expectedLines = new List<OrthogonalLineGrid2D>()
            {
                new OrthogonalLineGrid2D(new Vector2Int(0, 8), new Vector2Int(0, 8)),
                new OrthogonalLineGrid2D(new Vector2Int(0, -8), new Vector2Int(0, -8)),
            };

            var expectedPoints = expectedLines
                .SelectMany(x => x.GetPoints())
                .Distinct()
                .ToList();

            var configurationSpace = generator.GetConfigurationSpaceOverCorridor(roomShape, roomDoorsMode, roomShape,
                roomDoorsMode, corridorShape, corridorDoorsMode, ConfigurationSpaceDirection.FromFixedToMoving);

            var configurationSpacePoints = configurationSpace
                .Lines
                .SelectMany(x => x.GetPoints())
                .ToList();

            Assert.That(configurationSpacePoints, Is.EquivalentTo(expectedPoints));
        }
    }
}