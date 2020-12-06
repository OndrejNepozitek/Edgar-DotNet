using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Grid2D;
using NUnit.Framework;

namespace Edgar.IntegrationTests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class BasicLayoutConverterGrid2DTests
    {
        [Test]
        public void DoorsHaveCorrectOrientation()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(3), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(3, 1), new Vector2Int(3, 2))
            }));

            var roomTemplate2 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(3), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 1), new Vector2Int(0, 2))
            }));

            var roomTemplate3 = new RoomTemplateGrid2D(PolygonGrid2D.GetRectangle(3, 1), new ManualDoorModeGrid2D(new List<DoorGrid2D>()
            {
                new DoorGrid2D(new Vector2Int(0, 0), new Vector2Int(0, 1)),
                new DoorGrid2D(new Vector2Int(3, 0), new Vector2Int(3, 1))
            }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(true, new List<RoomTemplateGrid2D>() { roomTemplate3 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate2 }));
            
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0 - basic room

                var room = layout.Rooms.Single(x => x.Room == 0);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine.GetDirection(), Is.EqualTo(OrthogonalLineGrid2D.Direction.Bottom));
            }

            {
                // Room 2 - basic room

                var room = layout.Rooms.Single(x => x.Room == 2);
                var door = room.Doors.Single();

                Assert.That(door.DoorLine.GetDirection(), Is.EqualTo(OrthogonalLineGrid2D.Direction.Top));
            }

            {
                // Room 1 - corridor room

                var room = layout.Rooms.Single(x => x.Room == 1);
                var doorTo0 = room.Doors.Single(x => x.ToRoom == 0);
                var doorTo2 = room.Doors.Single(x => x.ToRoom == 2);

                Assert.That(doorTo0.DoorLine.GetDirection(), Is.EqualTo(OrthogonalLineGrid2D.Direction.Top));
                Assert.That(doorTo2.DoorLine.GetDirection(), Is.EqualTo(OrthogonalLineGrid2D.Direction.Bottom));
            }
        }
    }
}