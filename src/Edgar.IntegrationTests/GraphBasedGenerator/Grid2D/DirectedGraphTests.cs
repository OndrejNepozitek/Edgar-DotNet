using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using NUnit.Framework;

namespace Edgar.IntegrationTests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class DirectedGraphTests
    {
        [Test]
        public void BasicDoorTypeTest()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine, Is.EqualTo(new OrthogonalLineGrid2D(new Vector2Int(5, 3), new Vector2Int(5, 2))));
            }
        }

        [Test]
        public void BasicDoorTypeTestOpposite()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
            }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(1, 0);

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine, Is.EqualTo(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3))));
            }
        }

        [Test]
        public void BasicCorridorDoorTypeTest()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            }));

            var corridorRoomTemplate = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(3, 1), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 1), type: DoorType.Exit),
            }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(true, new List<RoomTemplateGrid2D>() { corridorRoomTemplate }));
            levelDescription.AddConnection(0, 2);
            levelDescription.AddConnection(2, 1);

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine, Is.EqualTo(new OrthogonalLineGrid2D(new Vector2Int(5, 3), new Vector2Int(5, 2))));
            }
        }

        [Test]
        public void BasicCorridorDoorTypeTestOpposite()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
            }));

            var corridorRoomTemplate = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(3, 1), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1), type: DoorType.Entrance),
                new DoorGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 1), type: DoorType.Exit),
            }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(true, new List<RoomTemplateGrid2D>() { corridorRoomTemplate }));
            levelDescription.AddConnection(2, 0);
            levelDescription.AddConnection(1, 2);

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine, Is.EqualTo(new OrthogonalLineGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3))));
            }
        }
    }
}