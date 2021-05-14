using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.GraphBasedGenerator.Common;
using Edgar.GraphBasedGenerator.Common.Doors;
using Edgar.GraphBasedGenerator.Grid2D;
using NUnit.Framework;

namespace Edgar.IntegrationTests.GraphBasedGenerator.Grid2D
{
    [TestFixture]
    public class FixedConfigurationConstraintTests
    {

        [Test]
        public void FixedPosition_Path_OneRoom_StartOfChain()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 0,
                    Position = new Vector2Int(20, 20),
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 0

                var room = layout.Rooms.Single(x => x.Room == 0);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Path_OneRoom_InsideOfChain()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 1,
                    Position = new Vector2Int(20, 20),
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 1

                var room = layout.Rooms.Single(x => x.Room == 1);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }

        [Test]
        public void FixedPosition_Cycle_OneRoom()
        {
            var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new SimpleDoorModeGrid2D(1, 0));

            var levelDescription = new LevelDescriptionGrid2D<int>();

            levelDescription.AddRoom(0, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(1, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(2, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddRoom(3, new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() { roomTemplate1 }));
            levelDescription.AddConnection(0, 1);
            levelDescription.AddConnection(1, 2);
            levelDescription.AddConnection(2, 3);
            levelDescription.AddConnection(3, 0);

            levelDescription.Constraints = new List<IGeneratorConstraintGrid2D<int>>()
            {
                new FixedConfigurationConstraint<int>()
                {
                    Room = 1,
                    Position = new Vector2Int(20, 20),
                }
            };

            var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
            {
                EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(2),
            });
            generator.InjectRandomGenerator(new Random(0));

            var layout = generator.GenerateLayout();

            Assert.That(layout, Is.Not.Null, "Layout not generated");

            {
                // Room 1

                var room = layout.Rooms.Single(x => x.Room == 1);

                Assert.That(room.Position, Is.EqualTo(new Vector2Int(20, 20)));
            }
        }
    }
}