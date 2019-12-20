using System;
using System.Linq;
using MapGeneration.Core.MapDescriptions;
using NUnit.Framework;

namespace MapGeneration.Tests.Core.MapDescriptions
{
    [TestFixture]
    public class MapDescriptionTests
    {
        [Test]
        public void AddRoom_DuplicateRoom_Throws()
        {
            var mapDescription = new MapDescription<int>();
            var roomDescription = new BasicRoomDescription();

            mapDescription.AddRoom(0, roomDescription);

            Assert.Throws<ArgumentException>(() => mapDescription.AddRoom(0, roomDescription));
        }

        [Test]
        public void AddRoom_DuplicatePassage_Throws()
        {
            var mapDescription = new MapDescription<int>();
            var roomDescription = new BasicRoomDescription();

            mapDescription.AddRoom(0, roomDescription);
            mapDescription.AddRoom(1, roomDescription);

            mapDescription.AddConnection(0, 1);

            Assert.Throws<ArgumentException>(() => mapDescription.AddConnection(0, 1));
        }

        [Test]
        public void GetGraph_WhenTwoNeighboringCorridors_Throws()
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomDescription = new BasicRoomDescription();
            var corridorRoomDescription = new CorridorRoomDescription();

            mapDescription.AddRoom(0, basicRoomDescription);
            mapDescription.AddRoom(1, corridorRoomDescription);
            mapDescription.AddRoom(2, corridorRoomDescription);
            mapDescription.AddRoom(3, basicRoomDescription);

            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(1, 2);
            mapDescription.AddConnection(2, 3);

            Assert.Throws<ArgumentException>(() => mapDescription.GetGraph());
            Assert.Throws<ArgumentException>(() => mapDescription.GetStageOneGraph());
        }

        [Test]
        public void GetGraph_WhenCorridorTooManyNeighbors_Throws()
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomDescription = new BasicRoomDescription();
            var corridorRoomDescription = new CorridorRoomDescription();

            mapDescription.AddRoom(0, corridorRoomDescription);
            mapDescription.AddRoom(1, basicRoomDescription);
            mapDescription.AddRoom(2, basicRoomDescription);
            mapDescription.AddRoom(3, basicRoomDescription);

            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(0, 2);
            mapDescription.AddConnection(0, 3);

            Assert.Throws<ArgumentException>(() => mapDescription.GetGraph());
            Assert.Throws<ArgumentException>(() => mapDescription.GetStageOneGraph());
        }

        [Test]
        public void GetGraph_WhenOnlyBasicRooms_ReturnsGraph()
        {
            var mapDescription = new MapDescription<int>();
            var basicRoomDescription = new BasicRoomDescription();

            mapDescription.AddRoom(0, basicRoomDescription);
            mapDescription.AddRoom(1, basicRoomDescription);
            mapDescription.AddRoom(2, basicRoomDescription);
            mapDescription.AddRoom(3, basicRoomDescription);

            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(0, 2);
            mapDescription.AddConnection(0, 3);

            var graph = mapDescription.GetGraph();

            Assert.That(graph.VerticesCount, Is.EqualTo(4));
            Assert.That(graph.Edges.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetRoomDescription_ReturnsRoomDescription()
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomDescription1 = new BasicRoomDescription();
            var basicRoomDescription2 = new BasicRoomDescription();

            mapDescription.AddRoom(0, basicRoomDescription1);
            mapDescription.AddRoom(1, basicRoomDescription2);

            Assert.That(mapDescription.GetRoomDescription(0), Is.EqualTo(basicRoomDescription1));
            Assert.That(mapDescription.GetRoomDescription(1), Is.EqualTo(basicRoomDescription2));
        }
    }
}