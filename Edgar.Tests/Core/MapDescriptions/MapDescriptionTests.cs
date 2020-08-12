using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
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
            var roomDescription = new BasicRoomDescription(GetRoomTemplates());

            mapDescription.AddRoom(0, roomDescription);

            Assert.Throws<ArgumentException>(() => mapDescription.AddRoom(0, roomDescription));
        }

        [Test]
        public void AddRoom_DuplicatePassage_Throws()
        {
            var mapDescription = new MapDescription<int>();
            var roomDescription = new BasicRoomDescription(GetRoomTemplates());

            mapDescription.AddRoom(0, roomDescription);
            mapDescription.AddRoom(1, roomDescription);

            mapDescription.AddConnection(0, 1);

            Assert.Throws<ArgumentException>(() => mapDescription.AddConnection(0, 1));
        }

        [Test]
        public void GetGraph_WhenTwoNeighboringCorridors_Throws()
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomDescription = new BasicRoomDescription(GetRoomTemplates());
            var corridorRoomDescription = new CorridorRoomDescription(GetRoomTemplates());

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

            var basicRoomDescription = new BasicRoomDescription(GetRoomTemplates());
            var corridorRoomDescription = new CorridorRoomDescription(GetRoomTemplates());

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
            var basicRoomDescription = new BasicRoomDescription(GetRoomTemplates());

            mapDescription.AddRoom(0, basicRoomDescription);
            mapDescription.AddRoom(1, basicRoomDescription);
            mapDescription.AddRoom(2, basicRoomDescription);
            mapDescription.AddRoom(3, basicRoomDescription);

            mapDescription.AddConnection(0, 1);
            mapDescription.AddConnection(0, 2);
            mapDescription.AddConnection(0, 3);

            var graph = mapDescription.GetGraph();
            var stageOneGraph = mapDescription.GetStageOneGraph();

            Assert.That(graph.VerticesCount, Is.EqualTo(4));
            Assert.That(graph.Edges.Count(), Is.EqualTo(3));

            Assert.That(stageOneGraph.VerticesCount, Is.EqualTo(4));
            Assert.That(stageOneGraph.Edges.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetRoomDescription_ReturnsRoomDescription()
        {
            var mapDescription = new MapDescription<int>();

            var basicRoomDescription1 = new BasicRoomDescription(GetRoomTemplates());
            var basicRoomDescription2 = new BasicRoomDescription(GetRoomTemplates());

            mapDescription.AddRoom(0, basicRoomDescription1);
            mapDescription.AddRoom(1, basicRoomDescription2);

            Assert.That(mapDescription.GetRoomDescription(0), Is.EqualTo(basicRoomDescription1));
            Assert.That(mapDescription.GetRoomDescription(1), Is.EqualTo(basicRoomDescription2));
        }

        private static List<RoomTemplate> GetRoomTemplates()
        {
            return new List<RoomTemplate>()
            {
                new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 1)),
            };
        }
    }
}