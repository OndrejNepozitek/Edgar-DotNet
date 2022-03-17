using System.Collections.Generic;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Core.MapDescriptions
{
    [TestFixture]
    public class MapDescriptionMappingTests
    {
        [Test]
        public void BasicTest()
        {
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 0));
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorMode(1, 0));

            var roomDescription1 = new BasicRoomDescription(new List<RoomTemplate>() {roomTemplate1});
            var roomDescription2 = new BasicRoomDescription(new List<RoomTemplate>() {roomTemplate2});

            var mapDescription = new MapDescription<string>();
            mapDescription.AddRoom("0", roomDescription1);
            mapDescription.AddRoom("1", roomDescription2);
            mapDescription.AddConnection("0", "1");

            var mapDescriptionMapping = new MapDescriptionMapping<string>(mapDescription);
            var mapping = mapDescriptionMapping.GetMapping();

            Assert.That(mapDescriptionMapping.GetRoomDescription(mapping["0"]), Is.EqualTo(roomDescription1));
            Assert.That(mapDescriptionMapping.GetRoomDescription(mapping["1"]), Is.EqualTo(roomDescription2));
            Assert.That(mapDescriptionMapping.GetGraph().VerticesCount, Is.EqualTo(2));
            Assert.That(mapDescriptionMapping.GetGraph().HasEdge(mapping["0"], mapping["1"]), Is.True);
        }

        [Test]
        public void BasicCorridorsTest()
        {
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 0));
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorMode(1, 0));

            var roomDescription1 = new BasicRoomDescription(new List<RoomTemplate>() {roomTemplate1});
            var roomDescription2 = new BasicRoomDescription(new List<RoomTemplate>() {roomTemplate2});
            var corridorDescription = new CorridorRoomDescription(new List<RoomTemplate>() {roomTemplate2});

            var mapDescription = new MapDescription<string>();
            mapDescription.AddRoom("0", roomDescription1);
            mapDescription.AddRoom("1", corridorDescription);
            mapDescription.AddRoom("2", roomDescription2);
            mapDescription.AddConnection("0", "1");
            mapDescription.AddConnection("1", "2");

            var mapDescriptionMapping = new MapDescriptionMapping<string>(mapDescription);
            var mapping = mapDescriptionMapping.GetMapping();

            Assert.That(mapDescriptionMapping.GetRoomDescription(mapping["0"]), Is.EqualTo(roomDescription1));
            Assert.That(mapDescriptionMapping.GetRoomDescription(mapping["1"]), Is.EqualTo(corridorDescription));
            Assert.That(mapDescriptionMapping.GetRoomDescription(mapping["2"]), Is.EqualTo(roomDescription2));

            Assert.That(mapDescriptionMapping.GetGraph().VerticesCount, Is.EqualTo(3));
            Assert.That(mapDescriptionMapping.GetStageOneGraph().VerticesCount, Is.EqualTo(2));
            Assert.That(mapDescriptionMapping.GetGraph().HasEdge(mapping["0"], mapping["1"]), Is.True);
            Assert.That(mapDescriptionMapping.GetGraph().HasEdge(mapping["1"], mapping["2"]), Is.True);
            Assert.That(mapDescriptionMapping.GetStageOneGraph().HasEdge(mapping["0"], mapping["2"]), Is.True);
        }
    }
}