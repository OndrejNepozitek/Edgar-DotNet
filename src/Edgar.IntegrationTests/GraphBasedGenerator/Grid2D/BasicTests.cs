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
    public class BasicTests
    {
        [Test]
        public void NoRoomTests()
        {
            var levelDescription = new LevelDescriptionGrid2D<int>();

            Assert.Throws<InvalidOperationException>(() => new GraphBasedGeneratorGrid2D<int>(levelDescription));
        }

        [Test]
        public void SingleRoomTest()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(
                new List<DoorGrid2D>()
                {
                    new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), type: DoorType.Entrance),
                    new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), type: DoorType.Exit),
                    new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), type: DoorType.Entrance),
                }));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0,
                new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription);
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            {
                // Room 0
                var room = layout.Rooms.Single(x => x.Room == 0);
                Assert.That(room.Position, Is.EqualTo(new Vector2Int(0, 0)));
            }
        }
    }
}