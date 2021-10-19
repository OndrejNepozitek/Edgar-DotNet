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
    public class DoorSocketTests
    {
        [TestFixture]
        public class DirectedGraphTests
        {
            [Test]
            public void DifferentInstancesOfTheSameSocket()
            {
                var roomTemplate1 = new RoomTemplateGrid2D(PolygonGrid2D.GetSquare(5), new ManualDoorModeGrid2D(
                    new List<DoorGrid2D>()
                    {
                        new DoorGrid2D(new Vector2Int(0, 2), new Vector2Int(0, 3), socket: new Socket1()),
                        new DoorGrid2D(new Vector2Int(5, 2), new Vector2Int(5, 3), socket: new Socket1()),
                        new DoorGrid2D(new Vector2Int(2, 0), new Vector2Int(3, 0), socket: new Socket1()),
                        new DoorGrid2D(new Vector2Int(2, 5), new Vector2Int(3, 5), socket: new Socket1()),
                    }));

                var levelDescription = new LevelDescriptionGrid2D<int>();

                levelDescription.AddRoom(0,
                    new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() {roomTemplate1}));
                levelDescription.AddRoom(1,
                    new RoomDescriptionGrid2D(false, new List<RoomTemplateGrid2D>() {roomTemplate1}));
                levelDescription.AddConnection(0, 1);

                var generator = new GraphBasedGeneratorGrid2D<int>(levelDescription, new GraphBasedGeneratorConfiguration<int>()
                {
                    EarlyStopIfTimeExceeded = TimeSpan.FromSeconds(1),
                });
                generator.InjectRandomGenerator(new Random(0));

                var layout = generator.GenerateLayout();

                Assert.That(layout, Is.Not.Null);
            }

            /// <summary>
            /// A socket that is compatible with any instance of itself.
            /// </summary>
            private class Socket1 : IDoorSocket
            {
                public bool IsCompatibleWith(IDoorSocket otherSocket)
                {
                    return otherSocket is Socket1;
                }
            }
        }
    }
}