using System;
using System.Collections.Generic;
using System.Linq;
using GeneralAlgorithms.Algorithms.Common;
using GeneralAlgorithms.DataStructures.Polygons;
using MapGeneration.Core.Doors.DoorModes;
using MapGeneration.Core.LayoutGenerators.DungeonGenerator;
using MapGeneration.Core.MapDescriptions;
using MapGeneration.Interfaces.Core.MapDescriptions;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Core.LayoutGenerators
{
    [TestFixture]
    public class DungeonGeneratorTests
    {
        [Test]
        public void SimpleMapDescription()
        {
            var roomTemplate1 = new RoomTemplate(GridPolygon.GetSquare(10), new OverlapMode(1, 0), TransformationHelper.GetAllTransformations().ToList());
            var roomTemplate2 = new RoomTemplate(GridPolygon.GetRectangle(5, 10), new OverlapMode(1, 0), TransformationHelper.GetAllTransformations().ToList());

            var roomDescription1 = new BasicRoomDescription(new List<IRoomTemplate>() { roomTemplate1 });
            var roomDescription2 = new BasicRoomDescription(new List<IRoomTemplate>() { roomTemplate2 });

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
    }
}