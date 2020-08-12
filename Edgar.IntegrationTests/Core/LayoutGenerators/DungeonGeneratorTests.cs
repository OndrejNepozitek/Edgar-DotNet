using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Doors.ManualMode;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Core.LayoutGenerators
{
    [TestFixture]
    public class DungeonGeneratorTests
    {
        [Test]
        public void SimpleMapDescriptionTest()
        {
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 0), TransformationHelper.GetAllTransformations().ToList());
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorMode(1, 0), TransformationHelper.GetAllTransformations().ToList());

            var roomDescription1 = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1 });
            var roomDescription2 = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate2 });

            var mapDescription = new MapDescription<int>();
            mapDescription.AddRoom(0, roomDescription1);
            mapDescription.AddRoom(1, roomDescription2);
            mapDescription.AddConnection(0, 1);

            var dungeonGenerator = new DungeonGenerator<int>(mapDescription);
            dungeonGenerator.InjectRandomGenerator(new Random(0));

            var layout = dungeonGenerator.GenerateLayout();

            Assert.That(layout, Is.Not.Null);
            Assert.That(layout.Rooms.Count(), Is.EqualTo(2));
        }

        [Test, Timeout(10000)]
        public void EarlyStoppingWhenTimeExceededTest()
        {
            var mapDescription = GetImpossibleMapDescription();
            var dungeonGenerator = new DungeonGenerator<int>(mapDescription, new DungeonGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(3)
            });
            dungeonGenerator.InjectRandomGenerator(new Random(0));

            var layout = dungeonGenerator.GenerateLayout();
        }

        [Test, Timeout(10000)]
        public void EarlyStoppingWhenIterationsExceededTest()
        {
            var mapDescription = GetImpossibleMapDescription();
            var dungeonGenerator = new DungeonGenerator<int>(mapDescription, new DungeonGeneratorConfiguration<int>()
            {
                EarlyStopIfIterationsExceeded = 5000,
            });
            dungeonGenerator.InjectRandomGenerator(new Random(0));

            var layout = dungeonGenerator.GenerateLayout();
        }

        /// <summary>
        /// Gets a room description that cannot be draw because every room template has only a single door position
        /// but there is a room that should be connected to two other rooms.
        /// </summary>
        /// <returns></returns>
        public static MapDescription<int> GetImpossibleMapDescription()
        {
            var roomShape = PolygonGrid2D.GetSquare(3);
            var doorPositions = new List<OrthogonalLine>()
            {
                new OrthogonalLine(new Vector2Int(0, 0), new Vector2Int(0, 1)),
            };

            var roomTemplate = new RoomTemplate(roomShape, new ManualDoorMode(doorPositions), TransformationHelper.GetAllTransformations().ToList());
            var roomDescription = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate });

            var mapDescription = new MapDescription<int>();
            mapDescription.AddRoom(0, roomDescription);
            mapDescription.AddRoom(1, roomDescription);
            mapDescription.AddRoom(2, roomDescription);
            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(0, 2);

            return mapDescription;
        }
    }
}